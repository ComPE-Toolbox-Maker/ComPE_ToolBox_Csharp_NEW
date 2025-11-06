using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscUtils;
using DiscUtils.Fat;
using DiscUtils.Partitions;
using DiscUtils.Internal;
using DiskInfo;

namespace ComPE_ToolBox
{
    public class USBPE
    {
        private static string originalNoDriveTypeAutoRunValue = null;
        private const string NoDriveTypeAutoRunKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
        private const string NoDriveTypeAutoRunName = "NoDriveTypeAutoRun";

        // 禁用自动播放
        public static void DisableAutoPlay()
        {
            try
            {
                // 获取当前值并保存
                originalNoDriveTypeAutoRunValue = Microsoft.Win32.Registry.GetValue(NoDriveTypeAutoRunKey, NoDriveTypeAutoRunName, null)?.ToString();

                // 设置新值: 0xFF 表示禁用所有类型的驱动器的自动播放
                Microsoft.Win32.Registry.SetValue(NoDriveTypeAutoRunKey, NoDriveTypeAutoRunName, 0xFF, Microsoft.Win32.RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"禁用自动播放时出错: {ex.Message}");
            }
        }

        // 恢复自动播放设置
        public static void RestoreAutoPlay()
        {
            try
            {
                if (originalNoDriveTypeAutoRunValue != null)
                {
                    // 恢复原始值
                    Microsoft.Win32.Registry.SetValue(NoDriveTypeAutoRunKey, NoDriveTypeAutoRunName, Convert.ToInt32(originalNoDriveTypeAutoRunValue), Microsoft.Win32.RegistryValueKind.DWord);
                }
                else
                {
                    // 如果原先没有该值，则删除它
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", true)?.DeleteValue(NoDriveTypeAutoRunName, false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"恢复自动播放时出错: {ex.Message}");
            }
        }
        public static int InstallPE(AntdUI.Progress ProgressHwnd,int Device,int EFISize,bool isDouble,char PartitionLetter,out string ErrorDescription,string FormatType)
        {
            ProgressHwnd.State = AntdUI.TType.None;
            DisableAutoPlay();
            Debug.WriteLine($"InstallPE(Device={Device},EFISize={EFISize},isDouble={isDouble},PartitionLetter={PartitionLetter})");
            
            ReleaseBins.ReleaseFile("ComPE.iso");
            ProgressHwnd.Value = 1F / 4F;
            int result = CreateEFIPartition(Device, EFISize,PartitionLetter,out ErrorDescription);
            if(result != 0)
            {
                ErrorDescription = "创建EFI分区失败，错误代码："+ result.ToString();
                ProgressHwnd.State = AntdUI.TType.Error;
                ProgressHwnd.Value = 1;
                RestoreAutoPlay();
                return result;
            }
 
            ProgressHwnd.Value = 2F / 4F;
            RWISO.ExtractISO(Path.Combine(ReleaseBins.TempPath, "ComPE.iso"), PartitionLetter + ":", "");
            if (isDouble)
            {
                CreateDataPartition(Device, PartitionLetter, FormatType);
            }
            ErrorDescription = "未发生执行错误现象。";

            File.Delete(Path.Combine(ReleaseBins.TempPath, "ComPE.iso"));
            File.Delete(Path.Combine(ReleaseBins.TempPath, "diskpart.txt"));
            ProgressHwnd.Value = 3F / 4F;
            RestoreAutoPlay();
            ProgressHwnd.Value = 4F / 4F;
            return 0;
        }
        private static int CreateEFIPartition(int Device,int EFISize,char PartitionLetter,out string ErrorDesc)
        {
            string Script = "select disk "+Device.ToString()+"\n" +
                "clean\n" +
                "create par pri size="+EFISize.ToString()+"\n" +
                "assign letter="+PartitionLetter+"\n" +
                "active";

            using (FileStream fs = File.Create(Path.Combine(ReleaseBins.TempPath, "diskpart.txt")))
            {
                fs.Write(Encoding.UTF8.GetBytes(Script));
            }
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "diskpart",
                    Arguments = $"/s \"{Path.Combine(ReleaseBins.TempPath, "diskpart.txt")}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true, // 不显示窗口
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            process.Start();
            process.WaitForExit();
            if(process.ExitCode!=0)
            {
                ErrorDesc = process.StandardError.ReadToEnd();
                return process.ExitCode;
            }
            process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(ReleaseBins.TempPath, "fat32format.exe"),
                    Arguments = $"{PartitionLetter}: -y",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true, // 不显示窗口
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                ErrorDesc = process.StandardError.ReadToEnd();
                return process.ExitCode;
            }
            ErrorDesc = "";
            return 0;
        }
        private static int CreateDataPartition(int Device, char PartitionLetter,string formatType)
        {
            string Script = "select disk " + Device.ToString() + "\n" +
                "select partition 1\n" +
                "remove letter="+PartitionLetter+"\n" +
                "create par pri\n" +
                "format fs=" +formatType+" quick\n"+
                "assign letter=" + PartitionLetter;
            using (FileStream fs = File.Create(Path.Combine(ReleaseBins.TempPath, "diskpart.txt")))
            {
                fs.Write(Encoding.UTF8.GetBytes(Script));
            }
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "diskpart",
                    Arguments = $"/s \"{Path.Combine(ReleaseBins.TempPath, "diskpart.txt")}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true, // 不显示窗口
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                string errorOutput = process.StandardError.ReadToEnd();
                Debug.WriteLine("DiskPart Error: " + errorOutput);
                return process.ExitCode;
            }
            return 0;
        }
    }
}
