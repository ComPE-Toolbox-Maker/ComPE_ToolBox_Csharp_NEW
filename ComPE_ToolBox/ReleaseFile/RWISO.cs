using DiscUtils.Udf;

namespace ComPE_ToolBox
{
    public class RWISO
    {
        public static void ExtractISO(string ISOPath,string TargetPath,string RootPath)
        {
            if(!Path.Exists(TargetPath))
                Directory.CreateDirectory(TargetPath);
            using (FileStream fs = File.Open(ISOPath,FileMode.Open))
            {
                UdfReader reader = new(new DiscUtils.Streams.SubStream(fs,0,fs.Length));
                ExtractFiles(reader,ISOPath, TargetPath, RootPath,TargetPath);
            }
        }
        public static void ExtractFiles(UdfReader reader,string ISOPath, string TargetPath, string RootPath,string TargetRootPath)
        {
            foreach (string a in reader.GetFiles(RootPath))
            {
                Stream s = reader.OpenFile(a,FileMode.Open);
                FileStream fs = File.Create(TargetRootPath + a);
                s.CopyTo(fs);
                s.Close();
                fs.Close();
            }
            foreach (string a in reader.GetDirectories(RootPath))
            {
                string nextTargetPath = TargetRootPath+ a;
                Directory.CreateDirectory(nextTargetPath);
                ExtractFiles(reader,ISOPath, nextTargetPath, a,TargetRootPath);
            }
          
        }
    }
}
