using System;
using System.Diagnostics;

namespace ComPE_ToolBox
{
	public class LocalPE
	{
		public static int InstallPE(string BootMenuText,int CountTime,AntdUI.Progress hwnd, out string ErrorDescription)
		{
            int result = CreateBootMenuEntry(BootMenuText,CountTime,out ErrorDescription);
            if (result != 0)
            {
                return result;
            }
            result = CopyFilesToLocalPEPartition($"{Environment.GetFolderPath(Environment.SpecialFolder.System).First()}:\\ComPE");
            if (result != 0)
            {
                return result;
            }
            //ErrorDescription = "本地安装功能尚未实现。";
            return 0;
        }
		private static int CreateBootMenuEntry(string BootMenuText, int CountTime, out string ErrorDescription)
		{
            string InstallDisk =Environment.GetFolderPath(Environment.SpecialFolder.Windows).First().ToString();
            string[] cmdLines = 
{
    $@"bcdedit /create {{40d2b668-d94d-7ee2-0c07-7a796bb7a1d1}} /d ""{BootMenuText}"" /device",
    $@"bcdedit /create  {{beaab3b9-80c3-13a8-6cf2-f5cc87f4006e}} /d ""{BootMenuText}"" /application osloader",
    $@"bcdedit /set {{40d2b668-d94d-7ee2-0c07-7a796bb7a1d1}} ramdisksdidevice partition={InstallDisk}:",
    $@"bcdedit /set {{40d2b668-d94d-7ee2-0c07-7a796bb7a1d1}} ramdisksdipath  \ComPE\boot\boot.sdi",
    $@"bcdedit /set  {{beaab3b9-80c3-13a8-6cf2-f5cc87f4006e}} device ramdisk=[{InstallDisk}:]\ComPE\sources\boot.wim,{{40d2b668-d94d-7ee2-0c07-7a796bb7a1d1}}",
    $@"bcdedit /set  {{beaab3b9-80c3-13a8-6cf2-f5cc87f4006e}} path \windows\system32\boot\winload.exe",
    $@"bcdedit /set  {{beaab3b9-80c3-13a8-6cf2-f5cc87f4006e}} locale zh-CN",
    $@"bcdedit /set  {{beaab3b9-80c3-13a8-6cf2-f5cc87f4006e}} inherit {{bootloadersettings}}",
    $@"bcdedit /set  {{beaab3b9-80c3-13a8-6cf2-f5cc87f4006e}} osdevice ramdisk=[{InstallDisk}:]\ComPE\sources\boot.wim,{{40d2b668-d94d-7ee2-0c07-7a796bb7a1d1}}",
    $@"bcdedit /set  {{beaab3b9-80c3-13a8-6cf2-f5cc87f4006e}} systemroot \windows",
    $@"bcdedit /set  {{beaab3b9-80c3-13a8-6cf2-f5cc87f4006e}} detecthal Yes",
    $@"bcdedit /set  {{beaab3b9-80c3-13a8-6cf2-f5cc87f4006e}} winpe Yes",
    $@"bcdedit /displayorder {{beaab3b9-80c3-13a8-6cf2-f5cc87f4006e}} /addlast",
    $@"bcdedit /timeout {CountTime}"
};

            // 根据启动模式添加不同的路径设置
            if (FirmwareDetector.GetUEFIorLegacy() == "UEFI")
            {
                // 如果需要动态添加，可以使用List<string>
                var cmdList = cmdLines.ToList();
                cmdList.Add($@"bcdedit /set  {{beaab3b9-80c3-13a8-6cf2-f5cc87f4006e}} path \windows\system32\boot\winload.efi");
                cmdLines = cmdList.ToArray();
            }
            else
            {
                var cmdList = cmdLines.ToList();
                cmdList.Add($@"bcdedit /set  {{beaab3b9-80c3-13a8-6cf2-f5cc87f4006e}} path \windows\system32\boot\winload.exe");
                cmdLines = cmdList.ToArray();
            }
            bcdBackup.BackupBCDFile(out ErrorDescription);
            bcdBackup.InitBCD();
            Debug.WriteLine("Creating boot menu entry with the following commands:");
            foreach (var cmd in cmdLines)
            {
                Debug.WriteLine(cmd);
                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c " + cmd)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (Process proc = Process.Start(psi))
                {
                    proc.WaitForExit();
                    string output = proc.StandardOutput.ReadToEnd();
                    string error = output+proc.StandardError.ReadToEnd();
                    Debug.WriteLine($"Output: {output}");
                    if (proc.ExitCode != 0)
                    {
                        ErrorDescription = error;
                        Debug.WriteLine($"Error: {error}");
                        bcdBackup.RestoreBCDFile(out _);
                        return proc.ExitCode;
                    }
                }
            }
            ErrorDescription = "";
			return 0;
        }
        private static int CopyFilesToLocalPEPartition(string InstallPath)
        {
            ReleaseBins.ReleaseFile("ComPE.iso");
            RWISO.ExtractISO(Path.Combine(ReleaseBins.TempPath, "ComPE.iso"), InstallPath, "");
            return 0;
        }
    }
}