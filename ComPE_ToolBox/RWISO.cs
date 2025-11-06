using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscUtils;
using DiscUtils.Iso9660;

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
                CDReader reader = new(fs,false);
                ExtractFiles(reader,ISOPath, TargetPath, RootPath,TargetPath);
            }
        }
        public static void ExtractFiles(CDReader reader,string ISOPath, string TargetPath, string RootPath,string TargetRootPath)
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
