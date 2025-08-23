using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
namespace ComPE_ToolBox
{

	public class ReleaseBins
	{
		public static string? TempPath;
		private static void CreateTempPath(string name)
		{
			if (TempPath != null)
			{
				return;
			}
			string sysTemp = Path.GetTempPath();
			Directory.CreateDirectory(Path.Combine(sysTemp,name));
			TempPath = Path.Combine(sysTemp,name);
        }
		public static bool ReleaseFile(string FileName) {
			CreateTempPath("ComPE");
            Assembly assm = Assembly.GetExecutingAssembly();
            Stream istr = assm.GetManifestResourceStream("ComPE_ToolBox.DependExecutable."+FileName);
			FileStream fs = File.Create(Path.Combine(TempPath, FileName));
            istr.CopyTo(fs);
			fs.Close();
			istr.Close();
            return true;
		}
		public static void RemoveTempPath() {
			try
			{
				Directory.Delete(TempPath, true);
			}catch {/*无视*/}
		}
        public static void RemoveFile(string FileName)
        {
			try
			{
				File.Delete(Path.Combine(TempPath, FileName));
			}
			catch {/*无视*/}
        }
    }
}
