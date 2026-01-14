using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ComPE_ToolBox
{


    public class FirmwareDetector
    {
        // 固件类型枚举
        public enum FIRMWARE_TYPE
        {
            FirmwareTypeUnknown,
            FirmwareTypeBios,
            FirmwareTypeUefi,
            FirmwareTypeMax
        }

        // 导入Windows API
        [DllImport("kernel32.dll")]
        private static extern bool GetFirmwareType(ref FIRMWARE_TYPE firmwareType);

        public static string GetUEFIorLegacy()
        {
            try
            {
                FIRMWARE_TYPE firmwareType = FIRMWARE_TYPE.FirmwareTypeUnknown;

                if (GetFirmwareType(ref firmwareType))
                {
                    switch (firmwareType)
                    {
                        case FIRMWARE_TYPE.FirmwareTypeBios:
                            return "Legacy";
                        case FIRMWARE_TYPE.FirmwareTypeUefi:
                            return "UEFI";
                        default:
                            return "Unknown";
                    }
                }
                return "Unknown";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"检测固件类型时出错: {ex.Message}");
                return "Unknown";
            }
        }
    }
}
