using System;
using System.ComponentModel;
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
                    throw new Win32Exception(Marshal.GetLastWin32Error());

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
                    throw new Win32Exception(Marshal.GetLastWin32Error());

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
    }
}