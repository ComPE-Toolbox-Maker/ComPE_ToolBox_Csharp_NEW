using AntdUI;
using System.ComponentModel;
using System.Diagnostics;
using Message = System.Windows.Forms.Message;


namespace ComPE_ToolBox
{
    public partial class Form1 : AntdUI.BorderlessForm
    {
        const Int32 WM_DEVICECHANGE = 0x0219;
        const Int32 DBT_DEVICEARRIVAL = 0x8000;
        const Int32 DBT_DEVICEREMOVECOMPLETE = 0x8004;
        bool canClose = true;
        public Form1()
        {
            InitializeComponent();
        }

        private void pageHeader1_Click(object sender, EventArgs e)
        {

        }



        private void divider2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.BackColor = Color.DodgerBlue;
            button3.BackColor = SystemColors.InactiveCaption;
            button4.BackColor = SystemColors.InactiveCaption;
            button5.BackColor = SystemColors.InactiveCaption;
            tabs1.SelectedIndex = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabs1.SelectedIndex = 1;
            button3.BackColor = Color.DodgerBlue;
            button1.BackColor = SystemColors.InactiveCaption;
            button4.BackColor = SystemColors.InactiveCaption;
            button5.BackColor = SystemColors.InactiveCaption;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabs1.SelectedIndex = 2;
            button4.BackColor = Color.DodgerBlue;
            button1.BackColor = SystemColors.InactiveCaption;
            button3.BackColor = SystemColors.InactiveCaption;
            button5.BackColor = SystemColors.InactiveCaption;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tabs1.SelectedIndex = 3;
            button5.BackColor = Color.DodgerBlue;
            button1.BackColor = SystemColors.InactiveCaption;
            button3.BackColor = SystemColors.InactiveCaption;
            button4.BackColor = SystemColors.InactiveCaption;
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialog = AntdUI.Modal.open(new Modal.Config(this, "执行前警告：", "本操作将修改操作系统启动的引导配置。\n正常无风险，但可能被防病毒软件误识别，请谨慎判断！\n继续请选择“确认”", TType.Warn)
            {
                MaskClosable = false
            });
            if (dialog != DialogResult.OK)
            {
                return;
            }
            button2.Loading = true;
            string errorDescription = "";
            int result = LocalPE.InstallPE(input1.Text, int.Parse(inputNumber1.Text), progress1, out errorDescription);
            button2.Loading = false;
            if (result != 0)
            {
                AntdUI.Modal.open(new Modal.Config(this, "执行失败：", "安装过程中发生错误，错误描述：" + errorDescription, TType.Error)
                {
                    CancelText = null,
                    MaskClosable = false
                });
                ExitThis(result);
            }
            AntdUI.Modal.open(new Modal.Config(this, "执行成功：", "本地PE已安装完成！\n您现在可以在下次启动时选择ComPE进入应急环境了。\n按下“确认”退出程序。", TType.Success)
            {
                CancelText = null,
                MaskClosable = false
            });
            ExitThis(0);
        }

        private void label11_Click(object sender, EventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = Path.Combine(ReleaseBins.TempPath, "USB_BOOT_MENU_SEARCH.EXE");
            process.Start();
            Hide();
            process.WaitForExit();
            Show();
        }

        public void GetPhysicalDriveNames()
        {
            if (progress1.State != AntdUI.TType.Success)
            {
                return;
            }
            select1.Items.Clear();
            select1.Enabled = false;
            for (int i = 0; i < 26; i++)
            {
                string tempModel = DiskInfo.DiskInfo.GetDiskProductModel(i);
                if (tempModel == null)
                {
                    ;
                    continue;
                }
                if (DiskInfo.DiskInfo.GetPhysicalDiskNumberFromDriveLetter(Environment.GetFolderPath(Environment.SpecialFolder.Windows).First()) == i)
                {
                    continue;
                }
                List<string> list = new List<string>();
                for (char j = 'A'; j <= 'Z'; j++)
                {
                    try
                    {
                        string temp = DiskUtility.GetPhysicalDrivePath(j).Replace("\\\\.\\PhysicalDrive", "");
                        if (temp == i.ToString())
                        {
                            list.Add(j.ToString() + ":");
                        }


                    }
                    catch { }
                }
                if(list.Count==0)
                {
                    continue;
                }
                Debug.WriteLine(i.ToString() + ":" + tempModel + "|[" + list.Aggregate("", (current, s) => current + s + ";").TrimEnd(';') + "]");
                select1.Items.Add(i.ToString() + ":" + tempModel + "|[" + list.Aggregate("", (current, s) => current + s + ";").TrimEnd(';') + "]");
            }
            select1.Enabled = true;
            if (select1.Items.Count > 0)
            {
                select1.SelectedIndex = 0;
                button7.Enabled = true;
                return;
            }
            select1.Text = "无可用移动磁盘";
            button7.Enabled = false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            GetPhysicalDriveNames();
            label13.Text = "当前固件类型：" + FirmwareDetector.GetUEFIorLegacy();
            input2.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ComPE_Release.iso");
            ReleaseBins.ReleaseFile("USB_Boot_Menu_Search.exe");
            //ReleaseBins.ReleaseFile("fat32format.exe");
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            GetPhysicalDriveNames();
        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DEVICECHANGE:
                    {
                        switch (m.WParam)
                        {
                            case DBT_DEVICEARRIVAL:
                                {
                                    GetPhysicalDriveNames();
                                    break;
                                }
                            case DBT_DEVICEREMOVECOMPLETE:
                                {
                                    GetPhysicalDriveNames();
                                    break;
                                }
                        }
                        break;
                    }

            }
            base.WndProc(ref m);
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            DialogResult dialog = AntdUI.Modal.open(new Modal.Config(this, "执行前警告：", "本操作将清空选择的磁盘下的数据，请注意备份！\n继续请选择“确认”", TType.Warn)
            {
                MaskClosable = false
            });
            if (dialog != DialogResult.OK)
            {
                return;
            }
            string ErrorDescription = "";
            button7.Loading = true;
            tabPage2.Enabled = false;
            flowLayoutPanel1.Enabled = false;
            try
            {
                await Task.Run(() =>
                {
                    int result = USBPE.InstallPE(progress1, int.Parse(select1.Text.First().ToString()), int.Parse(select4.Text), select3.SelectedIndex == 0, (select1.Text.Split("|")[1].Replace("[", "").Replace("]", "").Split(",")[0][0]), out ErrorDescription, select2.Text.ToUpper());
                    if (result != 0)
                    {
                        AntdUI.Modal.open(new Modal.Config(this, "执行失败：", "安装过程中发生错误，错误描述：" + ErrorDescription, TType.Error)
                        {
                            CancelText = null,
                            MaskClosable = false
                        });
                        ExitThis(result);

                    }
                });

            }
            catch (Exception ex)
            {
                AntdUI.Modal.open(new Modal.Config(this, "执行失败：", "安装过程中发生异常，异常信息：" + ex.Message, TType.Error)
                {
                    CancelText = null,
                    MaskClosable = false
                });
                ExitThis(-1);
                return;
            }

            button7.Loading = false;
            tabPage2.Enabled = true;
            flowLayoutPanel1.Enabled = true;
            progress1.Value = 0;
            progress1.State = AntdUI.TType.Success;
            GetPhysicalDriveNames();
            AntdUI.Modal.open(new Modal.Config(this, "执行成功：", "USB启动盘已制作完成！\n您现在可以将其作为应急启动盘使用了。\n按下“确认”退出程序。", TType.Success)
            {
                CancelText = null,
                MaskClosable = false
            });
            ReleaseBins.RemoveTempPath();
            ExitThis(0);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            DialogResult dialog = saveFileDialog1.ShowDialog(this);
            if (dialog != DialogResult.OK)
            { return; }
            input2.Text = saveFileDialog1.FileName;
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            DialogResult dialog = AntdUI.Modal.open(new Modal.Config(this, "执行前询问：", "请再次确认目录是否正确。\n继续请选择“确认”", TType.Info)
            {
                MaskClosable = false
            });
            if (dialog != DialogResult.OK)
            {
                return;
            }
            if (input2.Text.Length <= 0)
            {
                AntdUI.Modal.open(new Modal.Config(this, "执行失败：", "您未选择欲保存的目录，请选择后重试。", TType.Error)
                {
                    CancelText = null,
                    MaskClosable = false
                });
                button9.Loading = true;
                tabPage3.Enabled = false;
                flowLayoutPanel1.Enabled = false;
                return;
            }
            button9.Loading = true;
            tabPage3.Enabled = false;
            flowLayoutPanel1.Enabled = false;
            try
            {
                await Task.Run(() =>
                {
                    ReleaseBins.ReleaseFile("ComPE.iso");
                    File.Move(Path.Combine(ReleaseBins.TempPath, "ComPE.iso"), input2.Text, true);
                });
            }
            catch (Exception ex)
            {
                ReleaseBins.RemoveFile("ComPE.iso");
                AntdUI.Modal.open(new Modal.Config(this, "执行失败：", "保存文件的时候发生异常。\n异常信息：" + ex.Message, TType.Error)
                {
                    CancelText = null,
                    MaskClosable = false
                });
                button9.Loading = false;
                tabPage3.Enabled = true;
                flowLayoutPanel1.Enabled = true;
                return;
            }
            AntdUI.Modal.open(new Modal.Config(this, "成功：", "文件保存成功！", TType.Success)
            {
                MaskClosable = false
            });
            button9.Loading = false;
            tabPage3.Enabled = true;
            flowLayoutPanel1.Enabled = true;
            return;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!canClose)
            {
                e.Cancel = true;
                return;
            }
            DialogResult dialog = AntdUI.Modal.open(new Modal.Config(this, "询问：", "您确定要退出程序吗？", TType.Info)
            {
                MaskClosable = false
            });
            if (dialog != DialogResult.OK)
            {
                e.Cancel = true;
            }
            ExitThis(0);
        }

        private void input1_TextChanged(object sender, EventArgs e)
        {

        }
        public void ExitThis(int code)//带着清理资源的过程退出程序
        {
            Debug.Write("程序清理");
            canClose = true;
            ReleaseBins.RemoveTempPath();
            Debug.Write("程序清理完成");
            Environment.Exit(code);
        }
    }
}
