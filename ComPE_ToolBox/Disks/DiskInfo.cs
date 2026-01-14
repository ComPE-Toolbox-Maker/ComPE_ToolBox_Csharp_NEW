
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace DiskInfo
{
    public class DiskInfo
    {
        private const uint IOCTL_STORAGE_QUERY_PROPERTY = 0x002D1400;

        public static string GetDiskProductModel(int driveNumber)
        {
            string path = $@"\\.\PhysicalDrive{driveNumber}";
            using (SafeFileHandle handle = CreateFile(path, FileAccess.ReadWrite, FileShare.Read | FileShare.Write, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero))
            {
                if (handle.IsInvalid)
                    return null;

                var query = new STORAGE_PROPERTY_QUERY
                {
                    PropertyId = STORAGE_PROPERTY_ID.StorageDeviceProperty,
                    QueryType = STORAGE_QUERY_TYPE.PropertyStandardQuery
                };

                int querySize = Marshal.SizeOf(query);
                IntPtr queryPtr = Marshal.AllocHGlobal(querySize);
                Marshal.StructureToPtr(query, queryPtr, false);

                byte[] outBuffer = new byte[1024];
                uint bytesReturned = 0;

                bool success = DeviceIoControl(
                    handle,
                    IOCTL_STORAGE_QUERY_PROPERTY,
                    queryPtr,
                    querySize,
                    outBuffer,
                    (uint)outBuffer.Length,
                    out bytesReturned,
                    IntPtr.Zero);

                Marshal.FreeHGlobal(queryPtr);

                if (!success)
                    return null;

                // 解析设备描述符
                unsafe
                {
                    fixed (byte* ptr = outBuffer)
                    {
                        var descriptor = (STORAGE_DEVICE_DESCRIPTOR*)ptr;
                        if (descriptor->ProductIdOffset != 0)
                        {
                            byte* productIdPtr = ptr + descriptor->ProductIdOffset;
                            return new string((sbyte*)productIdPtr).Trim();
                        }
                    }
                }
                return null;
            }
        }
        private const uint IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS = 0x00560000;

        public static int GetPhysicalDiskNumberFromDriveLetter(char driveLetter)
        {
            string volumePath = $@"\\.\{driveLetter}:"; // 例如 \\.\C:
            using (SafeFileHandle volumeHandle = CreateFile(
                volumePath,
                FileAccess.ReadWrite,
                FileShare.Read | FileShare.Write,
                IntPtr.Zero,
                FileMode.Open,
                0,
                IntPtr.Zero))
            {
                if (volumeHandle.IsInvalid)
                    return -1;

                // 发送控制码获取磁盘扩展信息
                byte[] outBuffer = new byte[4096];
                uint bytesReturned = 0;

                bool success = DeviceIoControl(
                    volumeHandle,
                    IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS,
                    IntPtr.Zero,
                    0,
                    outBuffer,
                    (uint)outBuffer.Length,
                    out bytesReturned,
                    IntPtr.Zero);

                if (!success)
                    return -1;

                // 解析返回的 DISK_EXTENT 结构
                unsafe
                {
                    fixed (byte* ptr = outBuffer)
                    {
                        VOLUME_DISK_EXTENTS* extents = (VOLUME_DISK_EXTENTS*)ptr;
                        if (extents->NumberOfDiskExtents < 1)
                            throw new InvalidOperationException("未找到关联的物理磁盘");

                        // 计算第一个 DISK_EXTENT 的地址
                        DISK_EXTENT* firstExtent = &extents->Extents;

                        // 访问第一个元素
                        return (int)firstExtent->DiskNumber;
                    }
                }
            }
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern SafeFileHandle CreateFile(
            string lpFileName,
            FileAccess dwDesiredAccess,
            FileShare dwShareMode,
            IntPtr lpSecurityAttributes,
            FileMode dwCreationDisposition,
            int dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            int nInBufferSize,
            byte[] lpOutBuffer,
            uint nOutBufferSize,
            out uint lpBytesReturned,
            IntPtr lpOverlapped);

        private enum STORAGE_PROPERTY_ID
        {
            StorageDeviceProperty = 0
        }

        private enum STORAGE_QUERY_TYPE
        {
            PropertyStandardQuery = 0
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct STORAGE_PROPERTY_QUERY
        {
            public STORAGE_PROPERTY_ID PropertyId;
            public STORAGE_QUERY_TYPE QueryType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] AdditionalParameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct STORAGE_DEVICE_DESCRIPTOR
        {
            public uint Version;
            public uint Size;
            public byte DeviceType;
            public byte DeviceTypeModifier;
            [MarshalAs(UnmanagedType.U1)]
            public bool RemovableMedia;
            [MarshalAs(UnmanagedType.U1)]
            public bool CommandQueueing;
            public uint VendorIdOffset;
            public uint ProductIdOffset;
            public uint ProductRevisionOffset;
            public uint SerialNumberOffset;
            public STORAGE_BUS_TYPE BusType;
            public uint RawPropertiesLength;
            public IntPtr RawDeviceProperties;
        }

        private enum STORAGE_BUS_TYPE
        {
            BusTypeUnknown = 0x00,
            BusTypeNvme = 0x11,
            // 其他总线类型省略
        }
        [StructLayout(LayoutKind.Sequential)]
    private struct VOLUME_DISK_EXTENTS
    {
        public uint NumberOfDiskExtents;
        public DISK_EXTENT Extents; // 实际可能是多个，此处简化为第一个
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DISK_EXTENT
    {
        public uint DiskNumber;
        public long StartingOffset;
        public long ExtentLength;
    }
    }
}