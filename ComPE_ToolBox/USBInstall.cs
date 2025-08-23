using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComPE_ToolBox
{
    public class USBPE
    {
        public static int InstallPE(AntdUI.Progress ProgressHwnd,int Device,int EFISize,bool isDouble,char PartitionLetter)
        {
            
            ReleaseBins.ReleaseFile("ComPE.iso");
            CreateEFIPartition(Device, EFISize,PartitionLetter);
            RWISO.ExtractISO(Path.Combine(ReleaseBins.TempPath, "ComPE.iso"), PartitionLetter + ":\\ComPE", "");
            return 0;
        }
        private static int CreateEFIPartition(int Device,int EFISize,char PartitionLetter)
        {
            string Script = "select disk "+Device.ToString()+"\n" +
                "clear\n" +
                "create par pri size="+EFISize.ToString()+"\n" +
                "assign letter="+PartitionLetter+"\n" +
                "active";
            return 0;
        }
        private static int CreateDataPartition(int Device, int EFISize, char PartitionLetter)
        {
            string Script = "select disk " + Device.ToString() + "\n" +
                "select partition 1\n" +
                "remove letter="+PartitionLetter+"\n" +
                "create par pri\n" +
                "assign letter=" + PartitionLetter;
            return 0;
        }
    }
}
