using ComPE_ToolBox;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ComPE_ToolBox
{
    public class NativeMethods
    {

        static NativeMethods()
        {
            string dllpath = Path.Combine(ReleaseBins.TempPath, "fat32format.dll");
            Debug.WriteLine("Fat32 DLL Path: " + dllpath);
            SetDllDirectory(ReleaseBins.TempPath);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        [DllImport("fat32format.dll", SetLastError = true)]
        public static extern int mainFormat(string PartitionLetter, string Lable);

        [DllImport("fat32format.dll", SetLastError = true)]
        public static extern IntPtr GetFat32ErrorMessage(int errorCode);
    }
    public class Fat32Formatter
    {
        public static int FormatFat32(string PartitionLetter, string Label, out string ErrorDescription)
        {
            Debug.WriteLine($"Formatting partition {PartitionLetter} with label '{Label}' as FAT32.");
            int result = NativeMethods.mainFormat(PartitionLetter, Label);
            if (result != 0)
            {
                IntPtr errorMsgPtr = NativeMethods.GetFat32ErrorMessage(result);
                ErrorDescription = Marshal.PtrToStringAnsi(errorMsgPtr) ?? "未知错误";
                Debug.WriteLine(Marshal.PtrToStringAnsi(errorMsgPtr));
                return result;
            }
            ErrorDescription = "格式化成功";
            return 0;
        }
        public static bool initFatFormat()
        {
            return ReleaseBins.ReleaseFile("fat32format.dll");
        }

        public static void FreeFatFormat()
        {
            File.Delete(Path.Combine(ReleaseBins.TempPath, "fat32format.dll"));
        }
    }
}
