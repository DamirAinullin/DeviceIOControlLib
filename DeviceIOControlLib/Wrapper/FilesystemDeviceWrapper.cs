using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using DeviceIOControlLib.Objects.Enums;
using DeviceIOControlLib.Objects.FileSystem;
using DeviceIOControlLib.Utilities;
using Microsoft.Win32.SafeHandles;

namespace DeviceIOControlLib.Wrapper
{
    public class FilesystemDeviceWrapper : DeviceIoWrapperBase
    {
        public FilesystemDeviceWrapper(SafeFileHandle handle, bool ownsHandle = false)
            : base(handle, ownsHandle)
        {
        }

        //FsctlAllowExtendedDasdIo
        //FsctlCreateOrGetObjectId
        //FsctlCreateUsnJournal
        //FsctlDeleteObjectId
        //FsctlDeleteReparsePoint
        //FsctlDeleteUsnJournal
        //FsctlDismountVolume
        //FsctlDumpPropertyData
        //FsctlEnableUpgrade
        //FsctlEncryptionFsctlIo

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa364563(v=vs.85).aspx"/></summary>
        public IUSN_RECORD[] FileSystemEnumUsnData()
        {
            ulong nextUsn = 0;
            const int chunkSize = 1 * 1024 * 1024;      // 1 MB chunks

            IntPtr dataPtr = IntPtr.Zero;

            List<IUSN_RECORD> res = new List<IUSN_RECORD>();

            try
            {
                dataPtr = Marshal.AllocHGlobal(chunkSize);

                do
                {
                    MFT_ENUM_DATA_V0 input = new MFT_ENUM_DATA_V0();
                    input.StartFileReferenceNumber = nextUsn;
                    input.LowUsn = long.MinValue;
                    input.HighUsn = long.MaxValue;

                    int errorCode;
                    byte[] data = DeviceIoControlHelper.InvokeIoControl(Handle, IOControlCode.FsctlEnumUsnData, chunkSize, input, out errorCode);

                    if (errorCode != 0)
                        // Exit when theres no more to do
                        break;

                    nextUsn = BitConverter.ToUInt64(data, 0);

                    int dataOffset = 8;

                    while (dataOffset < data.Length)
                    {
                        // Get record length
                        int length = BitConverter.ToInt32(data, dataOffset);
                        ushort majorVersion = BitConverter.ToUInt16(data, dataOffset + sizeof(int));

                        if (length <= 0)
                            // No more records
                            break;

                        // Copy out record subset
                        byte[] bytes = new byte[length];
                        Array.Copy(data, dataOffset, bytes, 0, length);

                        Marshal.Copy(bytes, 0, dataPtr, bytes.Length);

                        switch (majorVersion)
                        {
                            case 2:
                                USN_RECORD_V2 recordv2 = (USN_RECORD_V2)Marshal.PtrToStructure(dataPtr, typeof(USN_RECORD_V2));

                                // Parse string manually, as we cannot rely on the string to be null-terminated.
                                recordv2.FileName = Encoding.Unicode.GetString(bytes, recordv2.FileNameOffset, recordv2.FileNameLength);

                                res.Add(recordv2);

                                break;
                            case 3:
                                USN_RECORD_V3 recordv3 = (USN_RECORD_V3)Marshal.PtrToStructure(dataPtr, typeof(USN_RECORD_V3));

                                // Parse string manually, as we cannot rely on the string to be null-terminated.
                                recordv3.FileName = Encoding.Unicode.GetString(bytes, recordv3.FileNameOffset, recordv3.FileNameLength);

                                res.Add(recordv3);

                                break;
                            default:
                                // Ignore
                                Debugger.Break();
                                break;
                        }

                        // Move to next record
                        dataOffset += length;
                    }

                    // Fetch next chunk
                } while (true);
            }
            finally
            {
                if (dataPtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(dataPtr);
            }

            return res.ToArray();
        }

        //FsctlExtendVolume

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa364565(v=vs.85).aspx"/></summary>
        public FileSystemStats[] FileSystemGetStatistics()
        {
            byte[] data = DeviceIoControlHelper.InvokeIoControlUnknownSize(Handle, IOControlCode.FsctlFileSystemGetStatistics, 512);
            IntPtr dataPtr = IntPtr.Zero;

            FileSystemStats[] res;

            try
            {
                dataPtr = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, dataPtr, data.Length);

                IntPtr currentDataPtr = dataPtr;

                FILESYSTEM_STATISTICS firstStats = (FILESYSTEM_STATISTICS)Marshal.PtrToStructure(currentDataPtr, typeof(FILESYSTEM_STATISTICS));

                int fsStatsSize = Marshal.SizeOf(typeof(FILESYSTEM_STATISTICS));

                int elementSize = (int)firstStats.SizeOfCompleteStructure;
                int procCount = data.Length / elementSize;

                res = new FileSystemStats[procCount];

                for (int i = 0; i < procCount; i++)
                {
                    res[i] = new FileSystemStats();
                    res[i].Stats = (FILESYSTEM_STATISTICS)Marshal.PtrToStructure(currentDataPtr, typeof(FILESYSTEM_STATISTICS));

                    switch (res[i].Stats.FileSystemType)
                    {
                        case FILESYSTEM_STATISTICS_TYPE.FILESYSTEM_STATISTICS_TYPE_NTFS:
                            NTFS_STATISTICS ntfsStats = (NTFS_STATISTICS)Marshal.PtrToStructure(currentDataPtr + fsStatsSize, typeof(NTFS_STATISTICS));

                            res[i].FSStats = ntfsStats;

                            break;
                        case FILESYSTEM_STATISTICS_TYPE.FILESYSTEM_STATISTICS_TYPE_FAT:
                            FAT_STATISTICS fatStats = (FAT_STATISTICS)Marshal.PtrToStructure(currentDataPtr + fsStatsSize, typeof(FAT_STATISTICS));

                            res[i].FSStats = fatStats;
                            break;
                        case FILESYSTEM_STATISTICS_TYPE.FILESYSTEM_STATISTICS_TYPE_EXFAT:
                            EXFAT_STATISTICS exFatStats = (EXFAT_STATISTICS)Marshal.PtrToStructure(currentDataPtr + fsStatsSize, typeof(EXFAT_STATISTICS));

                            res[i].FSStats = exFatStats;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    currentDataPtr += elementSize;
                }
            }
            finally
            {
                if (dataPtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(dataPtr);
            }

            return res;
        }

        //FsctlFindFilesBySid

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa364567(v=vs.85).aspx"/></summary>
        public COMPRESSION_FORMAT FileSystemGetCompression()
        {
            return (COMPRESSION_FORMAT)DeviceIoControlHelper.InvokeIoControl<ushort>(Handle, IOControlCode.FsctlGetCompression);
        }

        //FsctlGetHfsInformation

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/aa364568(v=vs.85).aspx"/></summary>
        public NTFS_FILE_RECORD_OUTPUT_BUFFER FileSystemGetNtfsFileRecord(ulong fileId)
        {
            NTFS_FILE_RECORD_INPUT_BUFFER input = new NTFS_FILE_RECORD_INPUT_BUFFER();
            input.FileReferenceNumber = fileId;

            byte[] data = DeviceIoControlHelper.InvokeIoControlUnknownSize(Handle, IOControlCode.FsctlGetNtfsFileRecord, input, 1024);    // NTFS File records are in 1K chunks

            NTFS_FILE_RECORD_OUTPUT_BUFFER res = new NTFS_FILE_RECORD_OUTPUT_BUFFER();
            res.FileReferenceNumber = BitConverter.ToUInt64(data, 0);
            res.FileRecordLength = BitConverter.ToUInt32(data, 8);

            res.FileRecordBuffer = new byte[res.FileRecordLength];
            Array.Copy(data, 8 + 4, res.FileRecordBuffer, 0, res.FileRecordLength);

            return res;
        }

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa364569(v=vs.85).aspx"/></summary>
        public NTFS_VOLUME_DATA_BUFFER FileSystemGetNtfsVolumeData()
        {
            return DeviceIoControlHelper.InvokeIoControl<NTFS_VOLUME_DATA_BUFFER>(Handle, IOControlCode.FsctlGetNtfsVolumeData);
        }

        //FsctlGetObjectId
        //FsctlGetReparsePoint

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/dd405526(v=vs.85).aspx"/></summary>
        public RETRIEVAL_POINTER_BASE FileSystemGetRetrievalPointerBase()
        {
            return DeviceIoControlHelper.InvokeIoControl<RETRIEVAL_POINTER_BASE>(Handle, IOControlCode.FsctlGetRetrievalPointerBase);
        }

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/dd405526(v=vs.85).aspx"/></summary>
        /// <remarks>
        ///     Does not correcly handle all cases of this method!. Especially regarding compressed/encrypted and/or sparse files in NTFS.
        ///     Consider yourself warned.
        /// </remarks>
        public FileExtentInfo[] FileSystemGetRetrievalPointers()
        {
            STARTING_VCN_INPUT_BUFFER input = new STARTING_VCN_INPUT_BUFFER();
            input.StartingVcn = 0;

            List<FileExtentInfo> extents = new List<FileExtentInfo>();

            uint chunkSize = 1024;
            uint singleExtentSize = (uint)Marshal.SizeOf(typeof(RETRIEVAL_POINTERS_EXTENT));

            int lastError;
            IntPtr dataPtr = IntPtr.Zero;

            try
            {
                do
                {
                    byte[] data = DeviceIoControlHelper.InvokeIoControl(Handle, IOControlCode.FsctlGetRetrievalPointers, chunkSize, input, out lastError);

                    RETRIEVAL_POINTERS_BUFFER output = new RETRIEVAL_POINTERS_BUFFER();
                    output.ExtentCount = BitConverter.ToUInt32(data, 0);
                    output.StartingVcn = BitConverter.ToUInt64(data, sizeof(ulong));

                    output.Extents = new RETRIEVAL_POINTERS_EXTENT[output.ExtentCount];

                    // Parse extents
                    int extentsSize = (int)(output.ExtentCount * singleExtentSize);
                    dataPtr = Marshal.AllocHGlobal(extentsSize);

                    Marshal.Copy(data, sizeof(ulong) + sizeof(ulong), dataPtr, extentsSize);

                    for (ulong i = 0; i < output.ExtentCount; i++)
                    {
                        IntPtr currentPtr = dataPtr + (int)(singleExtentSize * i);
                        output.Extents[i] = (RETRIEVAL_POINTERS_EXTENT)Marshal.PtrToStructure(currentPtr, typeof(RETRIEVAL_POINTERS_EXTENT));
                    }

                    // Make extents more readable
                    for (ulong i = 0; i < output.ExtentCount; i++)
                    {
                        ulong startVcn = i == 0
                            ? output.StartingVcn
                            : output.Extents[i - 1].NextVcn;

                        ulong size = output.Extents[i].NextVcn - startVcn;

                        FileExtentInfo extent = new FileExtentInfo();
                        extent.Size = size;
                        extent.Vcn = startVcn;
                        extent.Lcn = output.Extents[i].Lcn;

                        extents.Add(extent);
                    }

                    if (lastError == 38)
                        // End of file reached
                        break;

                    // Prep the start point for the next call
                    input.StartingVcn = output.Extents[output.ExtentCount - 1].NextVcn;
                } while (lastError == 234);
            }
            finally
            {
                if (dataPtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(dataPtr);
            }

            return extents.ToArray();
        }

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa364573(v=vs.85).aspx"/></summary>
        public VOLUME_BITMAP_BUFFER FileSystemGetVolumeBitmap(ulong startingLcn = 0)
        {
            STARTING_LCN_INPUT_BUFFER startingLcnStruct = new STARTING_LCN_INPUT_BUFFER();
            startingLcnStruct.StartingLcn = startingLcn;

            // Fetch 128 bytes (this includes the size parameter of the VOLUME_BITMAP_BUFFER structure.
            int lastError;
            byte[] data = DeviceIoControlHelper.InvokeIoControl(Handle, IOControlCode.FsctlGetVolumeBitmap, 128, startingLcnStruct, out lastError);

            // Is there more data? (Most often there is).
            if (lastError == 234)
            {
                // Parse length attribute (2'nd 64-bit attribute)
                uint newLength = (uint)BitConverter.ToUInt64(data, 8);

                // Length: Clusters / 8 + 2x 64 bit numbers
                newLength = (uint)Math.Ceiling(newLength / 8d) + 2 * 8;

                data = DeviceIoControlHelper.InvokeIoControl(Handle, IOControlCode.FsctlGetVolumeBitmap, newLength, startingLcnStruct, out lastError);
            }

            // Ensure the last call to InvokeIoControl succeeded.
            if (lastError != 0)
                throw new Win32Exception("Couldn't invoke FileSystemGetVolumeBitmap. LastError: " + Utils.GetWin32ErrorMessage(lastError));

            // Build the VOLUME_BITMAP_BUFFER structure.
            VOLUME_BITMAP_BUFFER res = new VOLUME_BITMAP_BUFFER();

            res.StartingLcn = BitConverter.ToUInt64(data, 0);
            res.BitmapSize = BitConverter.ToUInt64(data, sizeof(UInt64));

            res.Buffer = new BitArray((int)res.BitmapSize);

            for (int i = 0; i < res.Buffer.Length; i++)
            {
                int dataByteIndex = sizeof(UInt64) * 2 + i / 8;
                byte dataByte = data[dataByteIndex];

                int byteIdx = i % 8;

                res.Buffer[i] = ((dataByte >> byteIdx) & 1) == 1;
            }

            return res;
        }

        //FsctlHsmData
        //FsctlHsmMsg
        //FsctlInvalidateVolumes
        //FsctlIsPathnameValid
        //FsctlIsVolumeDirty
        //FsctlIsVolumeMounted
        //FsctlLockVolume
        //FsctlMarkAsSystemHive
        //FsctlMarkHandle
        //FsctlMarkVolumeDirty

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/aa364577(v=vs.85).aspx"/></summary>
        public void FileSystemMoveFile(IntPtr fileHandle, ulong startingVcn, ulong startingLcn, uint clusterCount)
        {
            MOVE_FILE_DATA input = new MOVE_FILE_DATA();
            input.FileHandle = fileHandle;
            input.StartingVcn = startingVcn;
            input.StartingLcn = startingLcn;
            input.ClusterCount = clusterCount;

            DeviceIoControlHelper.InvokeIoControl(Handle, IOControlCode.FsctlMoveFile, input);
        }

        //FsctlNssControl
        //FsctlNssRcontrol
        //FsctlOpBatchAckClosePending
        //FsctlOplockBreakAckNo2
        //FsctlOplockBreakAcknowledge
        //FsctlOplockBreakNotify

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa364582(v=vs.85).aspx"/></summary>
        public FILE_ALLOCATED_RANGE_BUFFER[] FileSystemQueryAllocatedRanges(long offset, long length)
        {
            FILE_ALLOCATED_RANGE_BUFFER input = new FILE_ALLOCATED_RANGE_BUFFER();
            input.FileOffset = offset;
            input.Length = offset + length;

            byte[] res = DeviceIoControlHelper.InvokeIoControlUnknownSize(Handle, IOControlCode.FsctlQueryAllocatedRanges, input, 512);

            int singleSize = Marshal.SizeOf(typeof(FILE_ALLOCATED_RANGE_BUFFER));
            List<FILE_ALLOCATED_RANGE_BUFFER> ranges = new List<FILE_ALLOCATED_RANGE_BUFFER>();

            for (int i = 0; i < res.Length; i += singleSize)
            {
                FILE_ALLOCATED_RANGE_BUFFER single = new FILE_ALLOCATED_RANGE_BUFFER();
                single.FileOffset = BitConverter.ToInt64(res, i);
                single.Length = BitConverter.ToInt64(res, i + 8);

                ranges.Add(single);
            }

            return ranges.ToArray();
        }

        //FsctlQueryFatBpb
        //FsctlQueryRetrievalPointers
        //FsctlQueryOnDiskVolumeInfo

        /// <summary><see cref="https://msdn.microsoft.com/en-us/library/windows/desktop/aa364583(v=vs.85).aspx"/></summary>
        public USN_JOURNAL_DATA_V0 FileSystemQueryUsnJournal()
        {
            USN_JOURNAL_DATA_V0 res = DeviceIoControlHelper.InvokeIoControl<USN_JOURNAL_DATA_V0>(Handle, IOControlCode.FsctlQueryUsnJournal);

            return res;
        }

        //FsctlReadFileUsnData
        //FsctlReadPropertyData
        //FsctlReadRawEncrypted
        //FsctlReadUsnJournal
        //FsctlRecallFile
        //FsctlRequestBatchOplock
        //FsctlRequestFilterOplock
        //FsctlRequestOplockLevel1
        //FsctlRequestOplockLevel2
        //FsctlSecurityIdCheck

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa364592(v=vs.85).aspx"/></summary>
        public void FileSystemSetCompression(COMPRESSION_FORMAT level)
        {
            DeviceIoControlHelper.InvokeIoControl(Handle, IOControlCode.FsctlSetCompression, (ushort)level);
        }

        //FsctlSetEncryption
        //FsctlSetObjectId
        //FsctlSetObjectIdExtended
        //FsctlSetReparsePoint

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa364596(v=vs.85).aspx"/></summary>
        public void FileSystemSetSparseFile(bool setSparse)
        {
            FILE_SET_SPARSE_BUFFER input = new FILE_SET_SPARSE_BUFFER();
            input.SetSparse = setSparse;

            DeviceIoControlHelper.InvokeIoControl(Handle, IOControlCode.FsctlSetSparse, input);
        }

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa364597(v=vs.85).aspx"/></summary>
        public void FileSystemSetZeroData(long fileOffset, long length)
        {
            FILE_ZERO_DATA_INFORMATION input = new FILE_ZERO_DATA_INFORMATION();
            input.FileOffset = fileOffset;
            input.BeyondFinalZero = fileOffset + length;

            DeviceIoControlHelper.InvokeIoControl(Handle, IOControlCode.FsctlSetZeroData, input);
        }

        //FsctlSisCopyFile
        //FsctlSisLinkFiles
        //FsctlUnlockVolume
        //FsctlWritePropertyData
        //FsctlWriteRawEncrypted
        //FsctlWriteUsnCloseRecord
    }
}