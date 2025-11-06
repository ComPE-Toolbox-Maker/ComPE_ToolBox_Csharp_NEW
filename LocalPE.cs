using System;

namespace ComPE_ToolBox
{
	public class LocalPE
	{
		public static int InstallPE(string BootMenuText,int CountTime,AntdUI.Progress hwnd, out string ErrorDescription)
		{
			ErrorDescription = "本地安装功能尚未实现。";
			return -1; // 返回错误代码，表示功能未实现
        }
    }
}