using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace ComPE_ToolBox
{
    // 错误代码定义
    public enum Fat32Error
    {
        Success = 0,
        InvalidParam = 1,
        OpenDevice = 2,
        IoctlFailed = 3,
        AllocFailed = 4,
        FormatFailed = 5,
        TooSmall = 6,
        TooLarge = 7,
        ClusterCount = 8,
        UserCancelled = 9,
        SeekFailed = 10,
        WriteFailed = 11,
        LockFailed = 12,
        DismountFailed = 13,
        UnlockFailed = 14,
        PartitionInfoFailed = 15
    }

    // 格式化参数结构
    public struct FormatParams
    {
        public int SectorsPerCluster;        // 0 for default or 1,2,4,8,16,32 or 64
        public bool MakeProtectedAutorun;
        public bool AllYes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string VolumeLabel;          // +1 for null terminator
    }

    public static class Fat32Formatter
    {
        private const int ALIGNING_SIZE = 1024 * 1024;

        // 获取错误信息
        public static string GetFat32ErrorMessage(Fat32Error errorCode)
        {
            switch (errorCode)
            {
                case Fat32Error.Success: return "Success";
                case Fat32Error.InvalidParam: return "Invalid parameter";
                case Fat32Error.OpenDevice: return "Failed to open device";
                case Fat32Error.IoctlFailed: return "IOCTL operation failed";
                case Fat32Error.AllocFailed: return "Memory allocation failed";
                case Fat32Error.FormatFailed: return "Format failed";
                case Fat32Error.TooSmall: return "Drive is too small for FAT32";
                case Fat32Error.TooLarge: return "Drive is too large for FAT32";
                case Fat32Error.ClusterCount: return "Invalid cluster count";
                case Fat32Error.UserCancelled: return "User cancelled the operation";
                case Fat32Error.SeekFailed: return "Seek operation failed";
                case Fat32Error.WriteFailed: return "Write operation failed";
                case Fat32Error.LockFailed: return "Failed to lock volume";
                case Fat32Error.DismountFailed: return "Failed to dismount volume";
                case Fat32Error.UnlockFailed: return "Failed to unlock volume";
                case Fat32Error.PartitionInfoFailed: return "Failed to get partition info";
                default: return "Unknown error";
            }
        }

        // 报告错误
        private static Fat32Error ReportError(string errorMessage, Fat32Error errorCode)
        {
            int error = Marshal.GetLastWin32Error();

            if (error != 0 && error != 0)
            {
                string systemError = new Win32Exception(error).Message;
                Console.Error.WriteLine($"Error: {errorMessage}\nSystem Error ({error}): {systemError}");
            }
            else
            {
                Console.Error.WriteLine($"Error: {errorMessage}");
            }

            return errorCode;
        }

        // 获取卷ID
        public static uint GetVolumeId()
        {
            DateTime now = DateTime.Now;

            ushort lo = (ushort)(now.Day + (now.Month << 8));
            ushort tmp = (ushort)((now.Millisecond / 10) + (now.Second << 8));
            lo += tmp;

            ushort hi = (ushort)(now.Minute + (now.Hour << 8));
            hi += (ushort)now.Year;

            return (uint)(lo + (hi << 16));
        }

        // 获取FAT大小（扇区数）
        public static uint GetFatSizeSectors(uint dskSize, uint reservedSecCnt, uint secPerClus, uint numFATs, uint bytesPerSect)
        {
            const ulong fatElementSize = 4;

            ulong numerator = fatElementSize * (dskSize - reservedSecCnt);
            ulong denominator = (1UL * secPerClus * bytesPerSect) + (fatElementSize * numFATs);
            ulong fatSz = numerator / denominator;

            // 向上取整
            fatSz += 1;

            uint alignSectorCount = ALIGNING_SIZE / bytesPerSect;
            fatSz = ((fatSz * numFATs + reservedSecCnt + alignSectorCount - 1) / alignSectorCount * alignSectorCount - reservedSecCnt) / numFATs;

            return (uint)fatSz;
        }

        // 获取每簇扇区数
        public static byte GetSectorsPerCluster(long diskSizeBytes, uint bytesPerSect)
        {
            long diskSizeMB = diskSizeBytes / (1024 * 1024);

            // 大于32,768 MB - 32 KB
            if (diskSizeMB > 32768)
                return GetSpc(32, bytesPerSect);

            // 16,384 MB 到 32,767 MB - 16 KB
            if (diskSizeMB > 16384)
                return GetSpc(16, bytesPerSect);

            // 8,192 MB 到 16,383 MB - 8 KB
            if (diskSizeMB > 8192)
                return GetSpc(8, bytesPerSect);

            // 256 MB 到 8,191 MB - 4 KB
            if (diskSizeMB > 256)
                return GetSpc(4, bytesPerSect);

            // 128 MB 到 256 MB - 2 KB
            if (diskSizeMB > 128)
                return GetSpc(2, bytesPerSect);

            // 64 MB 到 128 MB - 1 KB
            if (diskSizeMB > 64)
                return GetSpc(1, bytesPerSect);

            // 32 MB 到 64 MB - 0.5 KB
            return 1;
        }

        private static byte GetSpc(uint clusterSizeKB, uint bytesPerSect)
        {
            uint spc = (clusterSizeKB * 1024) / bytesPerSect;
            return (byte)spc;
        }

        // 主要格式化函数
        public static Fat32Error FormatFat32Drive(string drive, FormatParams parameters)
        {
            if (string.IsNullOrEmpty(drive) || parameters.VolumeLabel == null)
            {
                return Fat32Error.InvalidParam;
            }

            string devicePath = "";
            string volumePath = "";

            // 处理不同的驱动器路径格式
            if (char.IsLetter(drive[0]) && drive.Length >= 2 && drive[1] == ':')
            {
                // 格式: "C:"
                devicePath = $@"\\.\{drive[0]}:";
                volumePath = $@"{drive[0]}:\";
            }
            else if (drive.StartsWith(@"\\.\"))
            {
                // 格式: "\\.\C:"
                devicePath = drive;
                volumePath = drive + @"\";
            }
            else if (drive.StartsWith(@"\\?\Volume{"))
            {
                // 格式: "\\?\Volume{GUID}"
                devicePath = drive;
                volumePath = drive + @"\";
            }
            else
            {
                return Fat32Error.InvalidParam;
            }

            // 调用格式化函数
            Fat32Error result = FormatVolume(devicePath, parameters);

            // 如果格式化成功，设置卷标
            if (result == Fat32Error.Success && !string.IsNullOrEmpty(parameters.VolumeLabel))
            {
                if (!SetVolumeLabel(volumePath, parameters.VolumeLabel))
                {
                    // 注意：设置卷标失败不是致命错误，我们可以返回成功但记录警告
                    Console.Error.WriteLine($"Warning: Failed to set volume label (Error: {Marshal.GetLastWin32Error()})");
                }
            }

            return result;
        }

        // 打印使用说明
        public static void PrintUsage()
        {
            Console.WriteLine(
                "Usage Fat32Format [-cN] [-lLABEL] [-p] [-y] { C: | \\\\.\\C: | \\\\?\\Volume{GUID} }\n" +
                "Erase all data on specified volume, format it for FAT32\n" +
                "\n" +
                "    -c  Specify a cluster size by sector count.\n" +
                "        Accepts 1, 2, 4, 8, 16, 32, 64, 128\n" +
                "        EXAMPLE: Fat32Format -c4 X:  - use 4 sectors per cluster\n" +
                "    -l  Specify volume label.\n" +
                "        If exceeds 11-bytes, truncate label.\n" +
                "    -p  Make immutable AUTORUN.INF on root directory.\n" +
                "        This file cannot do anything on Windows.\n" +
                "\n" +
                "Modified Version see https://github.com/0xbadfca11/fat32format \n" +
                "\n" +
                "Original Version 1.07, see http://www.ridgecrop.demon.co.uk/fat32format.htm \n" +
                "This software is covered by the GPL \n" +
                "Use with care - Ridgecrop are not liable for data lost using this tool"
            );
        }

        #region 内部实现
        // 定义Windows API常量
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_WRITE = 0x00000002;
        private const uint OPEN_EXISTING = 3;
        private const uint FILE_FLAG_NO_BUFFERING = 0x20000000;
        private const uint FSCTL_ALLOW_EXTENDED_DASD_IO = 0x00090083;
        private const uint FSCTL_LOCK_VOLUME = 0x00090018;
        private const uint FSCTL_UNLOCK_VOLUME = 0x0009001C;
        private const uint FSCTL_DISMOUNT_VOLUME = 0x00090020;
        private const uint IOCTL_DISK_GET_DRIVE_GEOMETRY = 0x00070000;
        private const uint IOCTL_DISK_GET_PARTITION_INFO = 0x00074004;
        private const uint IOCTL_DISK_GET_PARTITION_INFO_EX = 0x00070048;
        private const uint IOCTL_DISK_SET_PARTITION_INFO = 0x0007C004;
        private const uint IOCTL_STORAGE_QUERY_PROPERTY = 0x002D1400;

        private const uint PARTITION_FAT32_XINT13 = 0x0C;

        // 结构体定义
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FAT_BOOTSECTOR32
        {
            // 公共字段
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] sJmpBoot;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public char[] sOEMName;
            public ushort wBytsPerSec;
            public byte bSecPerClus;
            public ushort wRsvdSecCnt;
            public byte bNumFATs;
            public ushort wRootEntCnt;
            public ushort wTotSec16;
            public byte bMedia;
            public ushort wFATSz16;
            public ushort wSecPerTrk;
            public ushort wNumHeads;
            public uint dHiddSec;
            public uint dTotSec32;
            // FAT32专用字段
            public uint dFATSz32;
            public ushort wExtFlags;
            public ushort wFSVer;
            public uint dRootClus;
            public ushort wFSInfo;
            public ushort wBkBootSec;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] Reserved;
            public byte bDrvNum;
            public byte Reserved1;
            public byte bBootSig;
            public uint dBS_VolID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public byte[] sVolLab;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] sBS_FilSysType;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FAT_FSINFO
        {
            public uint dLeadSig;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 480)]
            public byte[] sReserved1;
            public uint dStrucSig;
            public uint dFree_Count;
            public uint dNxt_Free;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] sReserved2;
            public uint dTrailSig;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FAT_DIRECTORY
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public byte[] DIR_Name;
            public byte DIR_Attr;
            public byte DIR_NTRes;
            public byte DIR_CrtTimeTenth;
            public ushort DIR_CrtTime;
            public ushort DIR_CrtDate;
            public ushort DIR_LstAccDate;
            public ushort DIR_FstClusHI;
            public ushort DIR_WrtTime;
            public ushort DIR_WrtDate;
            public ushort DIR_FstClusLO;
            public uint DIR_FileSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISK_GEOMETRY
        {
            public long Cylinders;
            public int MediaType;
            public int TracksPerCylinder;
            public int SectorsPerTrack;
            public int BytesPerSector;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PARTITION_INFORMATION
        {
            public long StartingOffset;
            public long PartitionLength;
            public uint HiddenSectors;
            public uint PartitionNumber;
            public byte PartitionType;
            public byte BootIndicator;
            public byte RecognizedPartition;
            public byte RewritePartition;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PARTITION_INFORMATION_EX
        {
            public int PartitionStyle;
            public long StartingOffset;
            public long PartitionLength;
            public uint PartitionNumber;
            public bool RewritePartition;
            public Guid PartitionType;
            public Guid PartitionId;
            public bool RecognizedPartition;
            public bool BootIndicator;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SET_PARTITION_INFORMATION
        {
            public byte PartitionType;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct STORAGE_PROPERTY_QUERY
        {
            public int PropertyId;
            public int QueryType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] AdditionalParameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR
        {
            public uint Version;
            public uint Size;
            public uint BytesPerCacheLine;
            public uint BytesOffsetForCacheAlignment;
            public uint BytesPerLogicalSector;
            public uint BytesPerPhysicalSector;
            public uint BytesOffsetForSectorAlignment;
        }

        // Windows API函数声明
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern SafeFileHandle CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            int nInBufferSize,
            IntPtr lpOutBuffer,
            int nOutBufferSize,
            out int lpBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            ref SET_PARTITION_INFORMATION lpInBuffer,
            int nInBufferSize,
            IntPtr lpOutBuffer,
            int nOutBufferSize,
            out int lpBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            ref STORAGE_PROPERTY_QUERY lpInBuffer,
            int nInBufferSize,
            ref STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR lpOutBuffer,
            int nOutBufferSize,
            out int lpBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetFilePointerEx(
            SafeFileHandle hFile,
            long liDistanceToMove,
            out long lpNewFilePointer,
            uint dwMoveMethod);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(
            SafeFileHandle hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadFile(
            SafeFileHandle hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAlloc(
            IntPtr lpAddress,
            uint dwSize,
            uint flAllocationType,
            uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualFree(
            IntPtr lpAddress,
            uint dwSize,
            uint dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetVolumeLabel(
            string lpRootPathName,
            string lpVolumeName);

        [DllImport("kernel32.dll")]
        private static extern void QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("kernel32.dll")]
        private static extern void QueryPerformanceFrequency(out long lpFrequency);

        [DllImport("kernel32.dll")]
        private static extern bool IsDebuggerPresent();

        // 内部辅助方法
        private static Fat32Error SeekToSect(SafeFileHandle hDevice, uint sector, uint bytesPerSect)
        {
            long offset = sector * (long)bytesPerSect;

            if (!SetFilePointerEx(hDevice, offset, out _, 0))
            {
                return ReportError("Failed to seek", Fat32Error.SeekFailed);
            }

            return Fat32Error.Success;
        }

        private static Fat32Error WriteSect(SafeFileHandle hDevice, uint sector, uint bytesPerSector, IntPtr data, uint numSects)
        {
            Fat32Error result = SeekToSect(hDevice, sector, bytesPerSector);
            if (result != Fat32Error.Success)
            {
                return result;
            }

            if (!WriteFile(hDevice, data, numSects * bytesPerSector, out _, IntPtr.Zero))
            {
                return ReportError("Failed to write", Fat32Error.WriteFailed);
            }

            return Fat32Error.Success;
        }

        private static Fat32Error ZeroSectors(SafeFileHandle hDevice, uint sector, uint bytesPerSect, uint numSects)
        {
            // 临时跳过清零功能以测试快速格式化
            // return Fat32Error.Success;

            uint qBytesTotal = numSects * bytesPerSect;
            uint burstSize = 1024 * 1024 / bytesPerSect;

            // 分配清零缓冲区
            IntPtr pZeroSect = VirtualAlloc(IntPtr.Zero, bytesPerSect * burstSize, 0x1000 | 0x2000, 0x04);
            if (pZeroSect == IntPtr.Zero)
            {
                return ReportError("Failed to allocate zero buffer", Fat32Error.AllocFailed);
            }

            try
            {
                Fat32Error result = SeekToSect(hDevice, sector, bytesPerSect);
                if (result != Fat32Error.Success)
                {
                    return result;
                }

                QueryPerformanceFrequency(out long frequency);
                QueryPerformanceCounter(out long start);

                while (numSects > 0)
                {
                    uint writeSize = Math.Min(numSects, burstSize);

                    if (!WriteFile(hDevice, pZeroSect, writeSize * bytesPerSect, out _, IntPtr.Zero))
                    {
                        return ReportError("Failed to write", Fat32Error.WriteFailed);
                    }

                    numSects -= writeSize;
                }

                QueryPerformanceCounter(out long end);
                long ticks = end - start;
                double fTime = (double)ticks / frequency;
                double fBytesTotal = qBytesTotal;

                Console.WriteLine($"Wrote {qBytesTotal} bytes in {fTime:F2} seconds, {fBytesTotal / (fTime * 1024.0 * 1024.0):F2} Megabytes/sec");

                return Fat32Error.Success;
            }
            finally
            {
                VirtualFree(pZeroSect, 0, 0x8000);
            }
        }

        private static Fat32Error FormatVolume(string volume, FormatParams parameters)
        {
            const ushort reservedSectCount = 32;
            const byte numFATs = 2;

            // 资源指针，用于清理
            IntPtr pFAT32BootSect = IntPtr.Zero;
            IntPtr pFAT32FsInfo = IntPtr.Zero;
            IntPtr pFirstSectOfFat = IntPtr.Zero;
            IntPtr pFAT32Directory = IntPtr.Zero;
            SafeFileHandle hDevice = null;

            try
            {
                if (!IsDebuggerPresent() && !parameters.AllYes)
                {
                    Console.Write($"Warning ALL data on drive '{volume}' will be lost irretrievably, are you sure\n(y/n) :");
                    if (char.ToUpper(Console.ReadKey().KeyChar) != 'Y')
                    {
                        return Fat32Error.UserCancelled;
                    }
                    Console.WriteLine();
                }

                // 打开设备
                hDevice = CreateFile(
                    volume,
                    GENERIC_READ | GENERIC_WRITE,
                    FILE_SHARE_READ | FILE_SHARE_WRITE,
                    IntPtr.Zero,
                    OPEN_EXISTING,
                    FILE_FLAG_NO_BUFFERING,
                    IntPtr.Zero);

                if (hDevice.IsInvalid)
                {
                    return ReportError("Failed to open device - close any files before formatting and make sure you have Admin rights when using fat32format\n Are you SURE you're formatting the RIGHT DRIVE!!!",
                        Fat32Error.OpenDevice);
                }

                // 允许扩展DASD I/O
                if (!DeviceIoControl(hDevice, FSCTL_ALLOW_EXTENDED_DASD_IO, IntPtr.Zero, 0, IntPtr.Zero, 0, out _, IntPtr.Zero))
                {
                    Console.WriteLine("Warning: Failed to allow extended DASD on device");
                }

                // 锁定卷
                if (!DeviceIoControl(hDevice, FSCTL_LOCK_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, out _, IntPtr.Zero))
                {
                    return ReportError("Failed to lock device", Fat32Error.LockFailed);
                }

                // 获取设备几何信息
                DISK_GEOMETRY dgDrive = new DISK_GEOMETRY();
                if (!DeviceIoControl(hDevice, IOCTL_DISK_GET_DRIVE_GEOMETRY, IntPtr.Zero, 0,
                    Marshal.AllocHGlobal(Marshal.SizeOf(dgDrive)), Marshal.SizeOf(dgDrive), out _, IntPtr.Zero))
                {
                    return ReportError("Failed to get device geometry", Fat32Error.IoctlFailed);
                }

                // 获取分区信息
                PARTITION_INFORMATION piDrive = new PARTITION_INFORMATION();
                bool bGPTMode = false;

                if (!DeviceIoControl(hDevice, IOCTL_DISK_GET_PARTITION_INFO, IntPtr.Zero, 0,
                    Marshal.AllocHGlobal(Marshal.SizeOf(piDrive)), Marshal.SizeOf(piDrive), out _, IntPtr.Zero))
                {
                    Console.WriteLine("IOCTL_DISK_GET_PARTITION_INFO failed, trying IOCTL_DISK_GET_PARTITION_INFO_EX");

                    PARTITION_INFORMATION_EX xpiDrive = new PARTITION_INFORMATION_EX();
                    if (!DeviceIoControl(hDevice, IOCTL_DISK_GET_PARTITION_INFO_EX, IntPtr.Zero, 0,
                        Marshal.AllocHGlobal(Marshal.SizeOf(xpiDrive)), Marshal.SizeOf(xpiDrive), out _, IntPtr.Zero))
                    {
                        return ReportError("Failed to get partition info (both regular and _ex)",
                            Fat32Error.PartitionInfoFailed);
                    }

                    piDrive.StartingOffset = xpiDrive.StartingOffset;
                    piDrive.PartitionLength = xpiDrive.PartitionLength;
                    piDrive.HiddenSectors = (uint)(xpiDrive.StartingOffset / dgDrive.BytesPerSector);

                    bGPTMode = xpiDrive.PartitionStyle != 0; // 0 = PARTITION_STYLE_MBR
                    Console.WriteLine($"IOCTL_DISK_GET_PARTITION_INFO_EX ok, GPTMode={bGPTMode}");
                }

                uint bytesPerSect = (uint)dgDrive.BytesPerSector;

                // 检查磁盘大小
                ulong qTotalSectors = (ulong)(piDrive.PartitionLength / dgDrive.BytesPerSector);

                // 低端限制 - 65536个扇区
                if (qTotalSectors < 65536)
                {
                    return ReportError("This drive is too small for FAT32 - there must be at least 64K clusters\n",
                        Fat32Error.TooSmall);
                }

                if (qTotalSectors >= 0xffffffff)
                {
                    return ReportError("This drive is too big for FAT32 - max 2TB supported\n",
                        Fat32Error.TooLarge);
                }

                // 检查Windows Vista或更高版本的对齐属性
                if (Environment.OSVersion.Version.Major >= 6) // Windows Vista or later
                {
                    STORAGE_PROPERTY_QUERY query = new STORAGE_PROPERTY_QUERY
                    {
                        PropertyId = 0, // StorageAccessAlignmentProperty
                        QueryType = 0   // PropertyStandardQuery
                    };

                    STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR alignment = new STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR();

                    if (DeviceIoControl(hDevice, IOCTL_STORAGE_QUERY_PROPERTY, ref query, Marshal.SizeOf(query),
                        ref alignment, Marshal.SizeOf(alignment), out _, IntPtr.Zero))
                    {
                        if (alignment.BytesOffsetForSectorAlignment != 0)
                            Console.WriteLine("Warning This disk has 'alignment offset'");

                        if (piDrive.StartingOffset > 0 && piDrive.StartingOffset % alignment.BytesPerPhysicalSector != 0)
                            Console.WriteLine("Warning This partition isn't aligned");
                    }
                }

                // 分配内存
                pFAT32BootSect = VirtualAlloc(IntPtr.Zero, bytesPerSect, 0x1000 | 0x2000, 0x04);
                pFAT32FsInfo = VirtualAlloc(IntPtr.Zero, bytesPerSect, 0x1000 | 0x2000, 0x04);
                pFirstSectOfFat = VirtualAlloc(IntPtr.Zero, bytesPerSect, 0x1000 | 0x2000, 0x04);
                pFAT32Directory = VirtualAlloc(IntPtr.Zero, bytesPerSect, 0x1000 | 0x2000, 0x04);

                if (pFAT32BootSect == IntPtr.Zero || pFAT32FsInfo == IntPtr.Zero ||
                    pFirstSectOfFat == IntPtr.Zero || pFAT32Directory == IntPtr.Zero)
                {
                    return ReportError("Failed to allocate memory", Fat32Error.AllocFailed);
                }

                // 填充引导扇区
                FAT_BOOTSECTOR32 bootSect = new FAT_BOOTSECTOR32
                {
                    sJmpBoot = new byte[] { 0xEB, 0x58, 0x90 },
                    sOEMName = new char[] { 'M', 'S', 'W', 'I', 'N', '4', '.', '1' },
                    wBytsPerSec = (ushort)bytesPerSect
                };

                byte sectorsPerCluster;
                if (parameters.SectorsPerCluster != 0)
                {
                    sectorsPerCluster = (byte)parameters.SectorsPerCluster;
                }
                else
                {
                    sectorsPerCluster = GetSectorsPerCluster(piDrive.PartitionLength, bytesPerSect);
                }

                bootSect.bSecPerClus = sectorsPerCluster;
                bootSect.wRsvdSecCnt = reservedSectCount;
                bootSect.bNumFATs = numFATs;
                bootSect.wRootEntCnt = 0;
                bootSect.wTotSec16 = 0;
                bootSect.bMedia = 0xF8;
                bootSect.wFATSz16 = 0;
                bootSect.wSecPerTrk = (ushort)dgDrive.SectorsPerTrack;
                bootSect.wNumHeads = (ushort)dgDrive.TracksPerCylinder;
                bootSect.dHiddSec = piDrive.HiddenSectors;
                uint totalSectors = (uint)(piDrive.PartitionLength / dgDrive.BytesPerSector);
                bootSect.dTotSec32 = totalSectors;

                uint fatSize = GetFatSizeSectors(bootSect.dTotSec32, bootSect.wRsvdSecCnt,
                    bootSect.bSecPerClus, bootSect.bNumFATs, bytesPerSect);

                bootSect.dFATSz32 = fatSize;
                bootSect.wExtFlags = 0;
                bootSect.wFSVer = 0;
                bootSect.dRootClus = 2;
                bootSect.wFSInfo = 1;
                bootSect.wBkBootSec = 6;
                bootSect.bDrvNum = 0x80;
                bootSect.Reserved1 = 0;
                bootSect.bBootSig = 0x29;
                bootSect.dBS_VolID = GetVolumeId();

                // 使用传入的卷标
                string volumeLabel = parameters.VolumeLabel;
                if (string.IsNullOrEmpty(volumeLabel))
                {
                    volumeLabel = "NO NAME    ";
                }

                // 确保卷标不超过11个字符
                if (volumeLabel.Length > 11)
                {
                    volumeLabel = volumeLabel.Substring(0, 11);
                }

                // 填充空格到11个字符
                volumeLabel = volumeLabel.PadRight(11, ' ');

                bootSect.sVolLab = Encoding.ASCII.GetBytes(volumeLabel);
                bootSect.sBS_FilSysType = Encoding.ASCII.GetBytes("FAT32   ");

                // 设置引导扇区签名
                Marshal.WriteByte(pFAT32BootSect, 510, 0x55);
                Marshal.WriteByte(pFAT32BootSect, 511, 0xAA);

                if (bytesPerSect != 512)
                {
                    Marshal.WriteByte(pFAT32BootSect, (int)bytesPerSect - 2, 0x55);
                    Marshal.WriteByte(pFAT32BootSect, (int)bytesPerSect - 1, 0xAA);
                }

                // 填充FSInfo扇区
                FAT_FSINFO fsInfo = new FAT_FSINFO
                {
                    dLeadSig = 0x41615252,
                    dStrucSig = 0x61417272,
                    dFree_Count = 0xFFFFFFFF,
                    dNxt_Free = 0xFFFFFFFF,
                    dTrailSig = 0xAA550000
                };

                // 填充第一个FAT扇区
                Marshal.WriteInt32(pFirstSectOfFat, 0, 0x0FFFFFF8);  // 保留簇1
                Marshal.WriteInt32(pFirstSectOfFat, 4, 0x0FFFFFFF);  // 保留簇2 EOC
                Marshal.WriteInt32(pFirstSectOfFat, 8, 0x0FFFFFFF);  // 根目录结束簇链

                uint userAreaSize = totalSectors - reservedSectCount - numFATs * fatSize;
                ulong clusterCount = userAreaSize / sectorsPerCluster;

                // 检查簇数是否超过2^28
                if (clusterCount > 0x0FFFFFF4)
                {
                    return ReportError("This drive has more than 2^28 clusters, try to specify a larger cluster size\n",
                        Fat32Error.ClusterCount);
                }

                // 检查簇数是否少于64K
                if (clusterCount < 65536)
                {
                    return ReportError("FAT32 must have at least 65536 clusters, try to specify a smaller cluster size\n",
                        Fat32Error.ClusterCount);
                }

                // 检查FAT是否足够大
                ulong fatNeeded = clusterCount * 4;
                fatNeeded = (fatNeeded + bytesPerSect - 1) / bytesPerSect;
                if (fatNeeded > fatSize)
                {
                    return ReportError("This drive is too big for this version of fat32format, check for an upgrade\n",
                        Fat32Error.FormatFailed);
                }

                // 打印信息
                Console.WriteLine($"Size : {piDrive.PartitionLength / (1024.0f * 1024.0f * 1024.0f):F2}GB {totalSectors} sectors");
                Console.WriteLine($"{bytesPerSect} Bytes Per Sector, Cluster size {sectorsPerCluster * bytesPerSect} bytes");
                Console.WriteLine($"Volume ID is {bootSect.dBS_VolID >> 16:X04}:{bootSect.dBS_VolID & 0xffff:X04}");
                Console.WriteLine($"{reservedSectCount} Reserved Sectors, {fatSize} Sectors per FAT, {numFATs} fats");
                Console.WriteLine($"{clusterCount} Total clusters");

                // 修复FSInfo扇区
                fsInfo.dFree_Count = (userAreaSize / sectorsPerCluster) - 1;
                fsInfo.dNxt_Free = 3; // 簇0-1保留，我们使用簇2作为根目录

                Console.WriteLine($"{fsInfo.dFree_Count} Free Clusters");
                Console.WriteLine($"Formatting drive {volume}...");

                // 清零保留扇区+FAT大小*FAT数+每簇扇区数
                uint systemAreaSize = (bootSect.wRsvdSecCnt + (bootSect.bNumFATs * fatSize) + sectorsPerCluster);
                Console.WriteLine($"Clearing out {systemAreaSize} sectors for Reserved sectors, fats and root directory...");

                Fat32Error result = ZeroSectors(hDevice, 0, bytesPerSect, systemAreaSize);
                if (result != Fat32Error.Success)
                {
                    return result;
                }

                Console.WriteLine("Initialising reserved sectors and FATs...");

                // 写入引导扇区和FSInfo两次，一次在0位置，一次在备份引导扇区位置
                for (int i = 0; i < 2; i++)
                {
                    uint sectorStart = (i == 0) ? 0 : (uint)bootSect.wBkBootSec;

                    result = WriteSect(hDevice, sectorStart, bytesPerSect, pFAT32BootSect, 1);
                    if (result != Fat32Error.Success)
                    {
                        return result;
                    }

                    result = WriteSect(hDevice, sectorStart + 1, bytesPerSect, pFAT32FsInfo, 1);
                    if (result != Fat32Error.Success)
                    {
                        return result;
                    }
                }

                // 在正确的位置写入第一个FAT扇区
                for (int i = 0; i < bootSect.bNumFATs; i++)
                {
                    uint sectorStart = bootSect.wRsvdSecCnt + (uint)(i * fatSize);

                    result = WriteSect(hDevice, sectorStart, bytesPerSect, pFirstSectOfFat, 1);
                    if (result != Fat32Error.Success)
                    {
                        return result;
                    }
                }

                if (parameters.MakeProtectedAutorun)
                {
                    FAT_DIRECTORY directory = new FAT_DIRECTORY
                    {
                        DIR_Name = Encoding.ASCII.GetBytes("AUTORUN INF"),
                        DIR_Attr = 0x06 | 0x02 | 0x04 // FILE_ATTRIBUTE_DEVICE | FILE_ATTRIBUTE_HIDDEN | FILE_ATTRIBUTE_SYSTEM
                    };

                    Marshal.StructureToPtr(directory, pFAT32Directory, false);

                    result = WriteSect(hDevice, reservedSectCount + numFATs * fatSize, bytesPerSect, pFAT32Directory, 1);
                    if (result != Fat32Error.Success)
                    {
                        return result;
                    }
                }

                // 如果是GPT磁盘，不修改分区类型
                if (!bGPTMode)
                {
                    SET_PARTITION_INFORMATION spiDrive = new SET_PARTITION_INFORMATION
                    {
                        PartitionType = PARTITION_FAT32_XINT13
                    };

                    if (!DeviceIoControl(hDevice, IOCTL_DISK_SET_PARTITION_INFO, ref spiDrive, Marshal.SizeOf(spiDrive),
                        IntPtr.Zero, 0, out _, IntPtr.Zero))
                    {
                        // 如果驱动器是超级软盘（即没有分区表），这可能会失败
                        // 仅当确实有分区表需要设置时才报告错误
                        if (piDrive.HiddenSectors != 0)
                        {
                            return ReportError("Failed to set partition info", Fat32Error.IoctlFailed);
                        }
                    }
                }

                // 卸载卷
                if (!DeviceIoControl(hDevice, FSCTL_DISMOUNT_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, out _, IntPtr.Zero))
                {
                    return ReportError("Failed to dismount device", Fat32Error.DismountFailed);
                }

                // 解锁卷
                if (!DeviceIoControl(hDevice, FSCTL_UNLOCK_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, out _, IntPtr.Zero))
                {
                    return ReportError("Failed to unlock device", Fat32Error.UnlockFailed);
                }

                Console.WriteLine("Done");
                return Fat32Error.Success;
            }
            finally
            {
                // 清理资源
                if (pFAT32BootSect != IntPtr.Zero) VirtualFree(pFAT32BootSect, 0, 0x8000);
                if (pFAT32FsInfo != IntPtr.Zero) VirtualFree(pFAT32FsInfo, 0, 0x8000);
                if (pFirstSectOfFat != IntPtr.Zero) VirtualFree(pFirstSectOfFat, 0, 0x8000);
                if (pFAT32Directory != IntPtr.Zero) VirtualFree(pFAT32Directory, 0, 0x8000);

                if (hDevice != null && !hDevice.IsInvalid)
                {
                    try
                    {
                        // 尝试解锁和关闭句柄
                        DeviceIoControl(hDevice, FSCTL_UNLOCK_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, out _, IntPtr.Zero);
                        hDevice.Close();
                    }
                    catch
                    {
                        // 忽略清理错误
                    }
                }
            }
        }
        #endregion
    }
}