using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ComPE_ToolBox
{
    /// <summary>
    /// 磁盘工具类 - 提供磁盘相关操作功能
    /// </summary>
    public static class DiskUtility
    {
        private const uint ERROR_MORE_DATA = 234;
        private const uint IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS = 0x560000;
        
        /// <summary>
        /// 获取逻辑驱动器对应的物理磁盘路径
        /// </summary>
        /// <param name="driveLetter">盘符字母（A-Z）</param>
        /// <returns>物理磁盘路径（如 \\.\PhysicalDrive0）</returns>
        /// <exception cref="ArgumentException">无效盘符</exception>
        /// <exception cref="Win32Exception">Windows API错误</exception>
        public static string GetPhysicalDrivePath(char driveLetter)
        {
            char upperLetter = char.ToUpper(driveLetter);
            if (upperLetter < 'A' || upperLetter > 'Z')
                throw new ArgumentException("盘符必须是A-Z之间的字母", nameof(driveLetter));

            string volumePath = $@"\\.\{upperLetter}:";
            
            using (SafeFileHandle volumeHandle = CreateFile(
                volumePath,
                FileAccess.Read,
                FileShare.Read | FileShare.Write,
                IntPtr.Zero,
                FileMode.Open,
                FileAttributes.Normal,
                IntPtr.Zero))
            {
                if (volumeHandle.IsInvalid)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), $"无法访问卷 '{upperLetter}:'");

                // 首次尝试获取磁盘扩展信息
                if (TryGetDiskExtents(volumeHandle, out VOLUME_DISK_EXTENTS diskExtents))
                    return FormatDrivePath(diskExtents);

                // 处理需要更大缓冲区的情况
                return HandleLargeBufferCase(volumeHandle);
            }
        }

        private static bool TryGetDiskExtents(SafeFileHandle handle, out VOLUME_DISK_EXTENTS diskExtents)
        {
            diskExtents = default;
            int bytesReturned;
            return DeviceIoControl(
                handle,
                IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS,
                IntPtr.Zero,
                0,
                out diskExtents,
                Marshal.SizeOf<VOLUME_DISK_EXTENTS>(),
                out bytesReturned,
                IntPtr.Zero);
        }

        private static string HandleLargeBufferCase(SafeFileHandle handle)
        {
            int error = Marshal.GetLastWin32Error();
            if (error != ERROR_MORE_DATA)
                throw new Win32Exception(error, "获取磁盘扩展信息失败");

            // 获取所需缓冲区大小
            int bytesReturned;
            VOLUME_DISK_EXTENTS_HEADER header;
            if (!DeviceIoControl(
                handle,
                IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS,
                IntPtr.Zero,
                0,
                out header,
                Marshal.SizeOf<VOLUME_DISK_EXTENTS_HEADER>(),
                out bytesReturned,
                IntPtr.Zero))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "获取磁盘扩展头信息失败");

            // 分配动态缓冲区
            int bufferSize = Marshal.SizeOf<VOLUME_DISK_EXTENTS_HEADER>() + 
                           Marshal.SizeOf<DISK_EXTENT>() * (int)header.NumberOfDiskExtents;
            
            IntPtr buffer = Marshal.AllocHGlobal(bufferSize);
            try
            {
                if (!DeviceIoControl(
                    handle,
                    IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS,
                    IntPtr.Zero,
                    0,
                    buffer,
                    bufferSize,
                    out bytesReturned,
                    IntPtr.Zero))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "获取完整磁盘扩展信息失败");

                var diskExtents = Marshal.PtrToStructure<VOLUME_DISK_EXTENTS>(buffer);
                return FormatDrivePath(diskExtents);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private static string? FormatDrivePath(VOLUME_DISK_EXTENTS diskExtents) //物理磁盘路径字符串格式化（不是格盘）
        {
            return diskExtents.NumberOfDiskExtents == 0 ? 
                null : 
                $@"\\.\PhysicalDrive{diskExtents.Extents[0].DiskNumber}";
        }
        public static List<char> GetAvailableDriveLetters()
        {
            // 获取已使用的盘符
            var usedDrives = DriveInfo.GetDrives()
                .Select(drive => drive.Name[0])
                .Distinct()
                .ToList();

            // 生成C到Z的所有可能盘符
            var allLetters = Enumerable.Range('C', 24)  // 'C' ASCII 67, 24个字母到Z
                .Select(c => (char)c)
                .ToList();

            // 返回未使用的盘符列表
            return allLetters.Except(usedDrives).ToList();
        }

        #region Win32 API 定义

        [StructLayout(LayoutKind.Sequential)]
        private struct DISK_EXTENT
        {
            public uint DiskNumber;
            public long StartingOffset;
            public long ExtentLength;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct VOLUME_DISK_EXTENTS_HEADER
        {
            public uint NumberOfDiskExtents;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct VOLUME_DISK_EXTENTS
        {
            public uint NumberOfDiskExtents;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public DISK_EXTENT[] Extents;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern SafeFileHandle CreateFile(
            string lpFileName,
            [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
            IntPtr lpSecurityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            int nInBufferSize,
            out VOLUME_DISK_EXTENTS lpOutBuffer,
            int nOutBufferSize,
            out int lpBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            int nInBufferSize,
            out VOLUME_DISK_EXTENTS_HEADER lpOutBuffer,
            int nOutBufferSize,
            out int lpBytesReturned,
            IntPtr lpOverlapped);

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

        #endregion
    }
}