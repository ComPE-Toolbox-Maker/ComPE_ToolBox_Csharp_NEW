using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComPE_ToolBox
{
    public class bcdBackup
    {
        public static int BackupBCDFile(out string ErrorDescription) //备份BCD文件
        {
            //ErrorDescription = "本功能尚未实现。";
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", $"/c BCDEDIT /export \"{ReleaseBins.TempPath}\" BCDBACKUP")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process proc = new Process
            {
                StartInfo = psi
            };
            proc.Start();
            proc.WaitForExit();
            ErrorDescription = proc.StandardError.ReadToEnd();
            return proc.ExitCode;
        }
        public static int RestoreBCDFile(out string ErrorDescription)//恢复BCD文件
        {
            //ErrorDescription = "本功能尚未实现。";
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", $"/c BCDEDIT /import \"{ReleaseBins.TempPath}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process proc = new Process
            {
                StartInfo = psi
            };
            proc.Start();
            proc.WaitForExit();
            ErrorDescription = proc.StandardError.ReadToEnd();
            return proc.ExitCode;
        }
        public static int InitBCD() {
            string[] cmds = { $@"BCDEDIT /delete {{beaab3b9-80c3-13a8-6cf2-f5cc87f4006e}}",
            $@"BCDEDIT /delete {{40d2b668-d94d-7ee2-0c07-7a796bb7a1d1}}"};
            foreach (string cmd in cmds) {
                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", $"/c {cmd}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process proc = new Process
                {
                    StartInfo = psi
                };
                proc.Start();
                proc.WaitForExit();

            }
            return 0;

        }
    }
}
