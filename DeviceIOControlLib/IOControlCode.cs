using System;
using System.IO;

namespace DeviceIOControlLib
{
    /// <summary>
    /// IO Control Codes
    /// Useful links:
    ///     http://www.ioctls.net/
    ///     http://msdn.microsoft.com/en-us/library/windows/hardware/ff543023(v=vs.85).aspx
    ///     http://sourceforge.net/apps/trac/mingw-w64/browser/experimental/headers_additions_test/include/winioctl.h
    /// </summary>
    [Flags]
    public enum IOControlCode : uint
    {
        // STORAGE
        StorageCheckVerify = (IOFileDevice.MassStorage << 16) | (0x0200 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        StorageCheckVerify2 = (IOFileDevice.MassStorage << 16) | (0x0200 << 2) | IOMethod.Buffered | (0 << 14),
        StorageMediaRemoval = (IOFileDevice.MassStorage << 16) | (0x0201 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        StorageEjectMedia = (IOFileDevice.MassStorage << 16) | (0x0202 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        StorageLoadMedia = (IOFileDevice.MassStorage << 16) | (0x0203 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        StorageLoadMedia2 = (IOFileDevice.MassStorage << 16) | (0x0203 << 2) | IOMethod.Buffered | (0 << 14),
        StorageReserve = (IOFileDevice.MassStorage << 16) | (0x0204 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        StorageRelease = (IOFileDevice.MassStorage << 16) | (0x0205 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        StorageFindNewDevices = (IOFileDevice.MassStorage << 16) | (0x0206 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        StorageEjectionControl = (IOFileDevice.MassStorage << 16) | (0x0250 << 2) | IOMethod.Buffered | (0 << 14),
        StorageMcnControl = (IOFileDevice.MassStorage << 16) | (0x0251 << 2) | IOMethod.Buffered | (0 << 14),
        StorageGetMediaTypes = (IOFileDevice.MassStorage << 16) | (0x0300 << 2) | IOMethod.Buffered | (0 << 14),
        StorageGetMediaTypesEx = (IOFileDevice.MassStorage << 16) | (0x0301 << 2) | IOMethod.Buffered | (0 << 14),
        StorageResetBus = (IOFileDevice.MassStorage << 16) | (0x0400 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        StorageResetDevice = (IOFileDevice.MassStorage << 16) | (0x0401 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        StorageGetDeviceNumber = (IOFileDevice.MassStorage << 16) | (0x0420 << 2) | IOMethod.Buffered | (0 << 14),
        StoragePredictFailure = (IOFileDevice.MassStorage << 16) | (0x0440 << 2) | IOMethod.Buffered | (0 << 14),
        StorageObsoleteResetBus = (IOFileDevice.MassStorage << 16) | (0x0400 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        StorageObsoleteResetDevice = (IOFileDevice.MassStorage << 16) | (0x0401 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        // DISK
        DiskGetDriveGeometry = (IOFileDevice.Disk << 16) | (0x0000 << 2) | IOMethod.Buffered | (0 << 14),
        DiskGetDriveGeometryEx = (IOFileDevice.Disk << 16) | (0x0028 << 2) | IOMethod.Buffered | (0 << 14),
        DiskGetPartitionInfo = (IOFileDevice.Disk << 16) | (0x0001 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        DiskGetPartitionInfoEx = (IOFileDevice.Disk << 16) | (0x0012 << 2) | IOMethod.Buffered | (0 << 14),
        DiskSetPartitionInfo = (IOFileDevice.Disk << 16) | (0x0002 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskSetPartitionInfoEx = (IOFileDevice.Disk << 16) | (0x0013 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskGetDriveLayout = (IOFileDevice.Disk << 16) | (0x0003 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        DiskGetDriveLayoutEx = (IOFileDevice.Disk << 16) | (0x0014 << 2) | IOMethod.Buffered | (0 << 14),
        DiskSetDriveLayout = (IOFileDevice.Disk << 16) | (0x0004 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskSetDriveLayoutEx = (IOFileDevice.Disk << 16) | (0x0015 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskVerify = (IOFileDevice.Disk << 16) | (0x0005 << 2) | IOMethod.Buffered | (0 << 14),
        DiskFormatTracks = (IOFileDevice.Disk << 16) | (0x0006 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskReassignBlocks = (IOFileDevice.Disk << 16) | (0x0007 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskPerformance = (IOFileDevice.Disk << 16) | (0x0008 << 2) | IOMethod.Buffered | (0 << 14),
        DiskIsWritable = (IOFileDevice.Disk << 16) | (0x0009 << 2) | IOMethod.Buffered | (0 << 14),
        DiskLogging = (IOFileDevice.Disk << 16) | (0x000a << 2) | IOMethod.Buffered | (0 << 14),
        DiskFormatTracksEx = (IOFileDevice.Disk << 16) | (0x000b << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskHistogramStructure = (IOFileDevice.Disk << 16) | (0x000c << 2) | IOMethod.Buffered | (0 << 14),
        DiskHistogramData = (IOFileDevice.Disk << 16) | (0x000d << 2) | IOMethod.Buffered | (0 << 14),
        DiskHistogramReset = (IOFileDevice.Disk << 16) | (0x000e << 2) | IOMethod.Buffered | (0 << 14),
        DiskRequestStructure = (IOFileDevice.Disk << 16) | (0x000f << 2) | IOMethod.Buffered | (0 << 14),
        DiskRequestData = (IOFileDevice.Disk << 16) | (0x0010 << 2) | IOMethod.Buffered | (0 << 14),
        DiskControllerNumber = (IOFileDevice.Disk << 16) | (0x0011 << 2) | IOMethod.Buffered | (0 << 14),
        DiskSmartGetVersion = (IOFileDevice.Disk << 16) | (0x0020 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        DiskSmartSendDriveCommand = (IOFileDevice.Disk << 16) | (0x0021 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskSmartRcvDriveData = (IOFileDevice.Disk << 16) | (0x0022 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskUpdateDriveSize = (IOFileDevice.Disk << 16) | (0x0032 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskGrowPartition = (IOFileDevice.Disk << 16) | (0x0034 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskGetCacheInformation = (IOFileDevice.Disk << 16) | (0x0035 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        DiskSetCacheInformation = (IOFileDevice.Disk << 16) | (0x0036 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskDeleteDriveLayout = (IOFileDevice.Disk << 16) | (0x0040 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskFormatDrive = (IOFileDevice.Disk << 16) | (0x00f3 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskSenseDevice = (IOFileDevice.Disk << 16) | (0x00f8 << 2) | IOMethod.Buffered | (0 << 14),
        DiskCheckVerify = (IOFileDevice.Disk << 16) | (0x0200 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        DiskMediaRemoval = (IOFileDevice.Disk << 16) | (0x0201 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        DiskEjectMedia = (IOFileDevice.Disk << 16) | (0x0202 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        DiskLoadMedia = (IOFileDevice.Disk << 16) | (0x0203 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        DiskReserve = (IOFileDevice.Disk << 16) | (0x0204 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        DiskRelease = (IOFileDevice.Disk << 16) | (0x0205 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        DiskFindNewDevices = (IOFileDevice.Disk << 16) | (0x0206 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        DiskCreateDisk = (IOFileDevice.Disk << 16) | (0x0016 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        DiskGetLengthInfo = (IOFileDevice.Disk << 16) | (0x0017 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        DiskGetDiskAttributes = (IOFileDevice.Disk << 16) | (0x003c << 2) | IOMethod.Buffered | (0 << 14),
        DiskSetDiskAttributes = (IOFileDevice.Disk << 16) | (0x003d << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        // CHANGER
        ChangerGetParameters = (IOFileDevice.Changer << 16) | (0x0000 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        ChangerGetStatus = (IOFileDevice.Changer << 16) | (0x0001 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        ChangerGetProductData = (IOFileDevice.Changer << 16) | (0x0002 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        ChangerSetAccess = (IOFileDevice.Changer << 16) | (0x0004 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        ChangerGetElementStatus = (IOFileDevice.Changer << 16) | (0x0005 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        ChangerInitializeElementStatus = (IOFileDevice.Changer << 16) | (0x0006 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        ChangerSetPosition = (IOFileDevice.Changer << 16) | (0x0007 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        ChangerExchangeMedium = (IOFileDevice.Changer << 16) | (0x0008 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        ChangerMoveMedium = (IOFileDevice.Changer << 16) | (0x0009 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        ChangerReinitializeTarget = (IOFileDevice.Changer << 16) | (0x000A << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        ChangerQueryVolumeTags = (IOFileDevice.Changer << 16) | (0x000B << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        // FILESYSTEM
        FsctlRequestOplockLevel1 = (IOFileDevice.FileSystem << 16) | (0 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlRequestOplockLevel2 = (IOFileDevice.FileSystem << 16) | (1 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlRequestBatchOplock = (IOFileDevice.FileSystem << 16) | (2 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlOplockBreakAcknowledge = (IOFileDevice.FileSystem << 16) | (3 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlOpBatchAckClosePending = (IOFileDevice.FileSystem << 16) | (4 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlOplockBreakNotify = (IOFileDevice.FileSystem << 16) | (5 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlLockVolume = (IOFileDevice.FileSystem << 16) | (6 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlUnlockVolume = (IOFileDevice.FileSystem << 16) | (7 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlDismountVolume = (IOFileDevice.FileSystem << 16) | (8 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlIsVolumeMounted = (IOFileDevice.FileSystem << 16) | (10 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlIsPathnameValid = (IOFileDevice.FileSystem << 16) | (11 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlMarkVolumeDirty = (IOFileDevice.FileSystem << 16) | (12 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlQueryRetrievalPointers = (IOFileDevice.FileSystem << 16) | (14 << 2) | IOMethod.Neither | (0 << 14),
        FsctlGetCompression = (IOFileDevice.FileSystem << 16) | (15 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlSetCompression = (IOFileDevice.FileSystem << 16) | (16 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        FsctlMarkAsSystemHive = (IOFileDevice.FileSystem << 16) | (19 << 2) | IOMethod.Neither | (0 << 14),
        FsctlOplockBreakAckNo2 = (IOFileDevice.FileSystem << 16) | (20 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlInvalidateVolumes = (IOFileDevice.FileSystem << 16) | (21 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlQueryFatBpb = (IOFileDevice.FileSystem << 16) | (22 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlRequestFilterOplock = (IOFileDevice.FileSystem << 16) | (23 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlFileSystemGetStatistics = (IOFileDevice.FileSystem << 16) | (24 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlGetNtfsVolumeData = (IOFileDevice.FileSystem << 16) | (25 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlGetNtfsFileRecord = (IOFileDevice.FileSystem << 16) | (26 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlGetVolumeBitmap = (IOFileDevice.FileSystem << 16) | (27 << 2) | IOMethod.Neither | (0 << 14),
        FsctlGetRetrievalPointers = (IOFileDevice.FileSystem << 16) | (28 << 2) | IOMethod.Neither | (0 << 14),
        FsctlGetRetrievalPointerBase = (IOFileDevice.FileSystem << 16) | (141 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlMoveFile = (IOFileDevice.FileSystem << 16) | (29 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlIsVolumeDirty = (IOFileDevice.FileSystem << 16) | (30 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlGetHfsInformation = (IOFileDevice.FileSystem << 16) | (31 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlAllowExtendedDasdIo = (IOFileDevice.FileSystem << 16) | (32 << 2) | IOMethod.Neither | (0 << 14),
        FsctlReadPropertyData = (IOFileDevice.FileSystem << 16) | (33 << 2) | IOMethod.Neither | (0 << 14),
        FsctlWritePropertyData = (IOFileDevice.FileSystem << 16) | (34 << 2) | IOMethod.Neither | (0 << 14),
        FsctlFindFilesBySid = (IOFileDevice.FileSystem << 16) | (35 << 2) | IOMethod.Neither | (0 << 14),
        FsctlDumpPropertyData = (IOFileDevice.FileSystem << 16) | (37 << 2) | IOMethod.Neither | (0 << 14),
        FsctlSetObjectId = (IOFileDevice.FileSystem << 16) | (38 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlGetObjectId = (IOFileDevice.FileSystem << 16) | (39 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlDeleteObjectId = (IOFileDevice.FileSystem << 16) | (40 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlSetReparsePoint = (IOFileDevice.FileSystem << 16) | (41 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlGetReparsePoint = (IOFileDevice.FileSystem << 16) | (42 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlDeleteReparsePoint = (IOFileDevice.FileSystem << 16) | (43 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlEnumUsnData = (IOFileDevice.FileSystem << 16) | (44 << 2) | IOMethod.Neither | (0 << 14),
        FsctlSecurityIdCheck = (IOFileDevice.FileSystem << 16) | (45 << 2) | IOMethod.Neither | (FileAccess.Read << 14),
        FsctlReadUsnJournal = (IOFileDevice.FileSystem << 16) | (46 << 2) | IOMethod.Neither | (0 << 14),
        FsctlSetObjectIdExtended = (IOFileDevice.FileSystem << 16) | (47 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlCreateOrGetObjectId = (IOFileDevice.FileSystem << 16) | (48 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlSetSparse = (IOFileDevice.FileSystem << 16) | (49 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlSetZeroData = (IOFileDevice.FileSystem << 16) | (50 << 2) | IOMethod.Buffered | (FileAccess.Write << 14),
        FsctlQueryAllocatedRanges = (IOFileDevice.FileSystem << 16) | (51 << 2) | IOMethod.Neither | (FileAccess.Read << 14),
        FsctlEnableUpgrade = (IOFileDevice.FileSystem << 16) | (52 << 2) | IOMethod.Buffered | (FileAccess.Write << 14),
        FsctlSetEncryption = (IOFileDevice.FileSystem << 16) | (53 << 2) | IOMethod.Neither | (0 << 14),
        FsctlEncryptionFsctlIo = (IOFileDevice.FileSystem << 16) | (54 << 2) | IOMethod.Neither | (0 << 14),
        FsctlWriteRawEncrypted = (IOFileDevice.FileSystem << 16) | (55 << 2) | IOMethod.Neither | (0 << 14),
        FsctlReadRawEncrypted = (IOFileDevice.FileSystem << 16) | (56 << 2) | IOMethod.Neither | (0 << 14),
        FsctlCreateUsnJournal = (IOFileDevice.FileSystem << 16) | (57 << 2) | IOMethod.Neither | (0 << 14),
        FsctlReadFileUsnData = (IOFileDevice.FileSystem << 16) | (58 << 2) | IOMethod.Neither | (0 << 14),
        FsctlWriteUsnCloseRecord = (IOFileDevice.FileSystem << 16) | (59 << 2) | IOMethod.Neither | (0 << 14),
        FsctlExtendVolume = (IOFileDevice.FileSystem << 16) | (60 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlQueryUsnJournal = (IOFileDevice.FileSystem << 16) | (61 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlDeleteUsnJournal = (IOFileDevice.FileSystem << 16) | (62 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlMarkHandle = (IOFileDevice.FileSystem << 16) | (63 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlSisCopyFile = (IOFileDevice.FileSystem << 16) | (64 << 2) | IOMethod.Buffered | (0 << 14),
        FsctlSisLinkFiles = (IOFileDevice.FileSystem << 16) | (65 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        FsctlHsmMsg = (IOFileDevice.FileSystem << 16) | (66 << 2) | IOMethod.Buffered | (FileAccess.ReadWrite << 14),
        FsctlNssControl = (IOFileDevice.FileSystem << 16) | (67 << 2) | IOMethod.Buffered | (FileAccess.Write << 14),
        FsctlHsmData = (IOFileDevice.FileSystem << 16) | (68 << 2) | IOMethod.Neither | (FileAccess.ReadWrite << 14),
        FsctlRecallFile = (IOFileDevice.FileSystem << 16) | (69 << 2) | IOMethod.Neither | (0 << 14),
        FsctlNssRcontrol = (IOFileDevice.FileSystem << 16) | (70 << 2) | IOMethod.Buffered | (FileAccess.Read << 14),
        // VIDEO
        VideoQuerySupportedBrightness = (IOFileDevice.Video << 16) | (0x0125 << 2) | IOMethod.Buffered | (0 << 14),
        VideoQueryDisplayBrightness = (IOFileDevice.Video << 16) | (0x0126 << 2) | IOMethod.Buffered | (0 << 14),
        VideoSetDisplayBrightness = (IOFileDevice.Video << 16) | (0x0127 << 2) | IOMethod.Buffered | (0 << 14)
    }
}