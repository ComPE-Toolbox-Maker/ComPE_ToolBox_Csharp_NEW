namespace ComPE_ToolBox
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            AntdUI.Tabs.StyleCard styleCard1 = new AntdUI.Tabs.StyleCard();
            pageHeader1 = new AntdUI.PageHeader();
            image3d1 = new AntdUI.Image3D();
            label1 = new AntdUI.Label();
            label2 = new AntdUI.Label();
            tabs1 = new AntdUI.Tabs();
            tabPage1 = new AntdUI.TabPage();
            button2 = new AntdUI.Button();
            label6 = new AntdUI.Label();
            label5 = new AntdUI.Label();
            inputNumber1 = new AntdUI.InputNumber();
            label4 = new AntdUI.Label();
            input1 = new AntdUI.Input();
            label3 = new AntdUI.Label();
            tabPage2 = new AntdUI.TabPage();
            button8 = new AntdUI.Button();
            label14 = new AntdUI.Label();
            label12 = new AntdUI.Label();
            select4 = new AntdUI.Select();
            label10 = new AntdUI.Label();
            select3 = new AntdUI.Select();
            label9 = new AntdUI.Label();
            select2 = new AntdUI.Select();
            select1 = new AntdUI.Select();
            label7 = new AntdUI.Label();
            label11 = new AntdUI.Label();
            button7 = new AntdUI.Button();
            label8 = new AntdUI.Label();
            tabPage3 = new AntdUI.TabPage();
            button10 = new AntdUI.Button();
            button9 = new AntdUI.Button();
            label15 = new AntdUI.Label();
            label16 = new AntdUI.Label();
            input2 = new AntdUI.Input();
            label18 = new AntdUI.Label();
            tabPage4 = new AntdUI.TabPage();
            select5 = new AntdUI.Select();
            label19 = new AntdUI.Label();
            button11 = new AntdUI.Button();
            label17 = new AntdUI.Label();
            select8 = new AntdUI.Select();
            label22 = new AntdUI.Label();
            button12 = new AntdUI.Button();
            label23 = new AntdUI.Label();
            divider1 = new AntdUI.Divider();
            divider2 = new AntdUI.Divider();
            flowLayoutPanel1 = new FlowLayoutPanel();
            button1 = new AntdUI.Button();
            button3 = new AntdUI.Button();
            button4 = new AntdUI.Button();
            button5 = new AntdUI.Button();
            button6 = new AntdUI.Button();
            progress1 = new AntdUI.Progress();
            saveFileDialog1 = new SaveFileDialog();
            label13 = new AntdUI.Label();
            tabs1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage4.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // pageHeader1
            // 
            pageHeader1.BackColor = SystemColors.MenuHighlight;
            pageHeader1.ColorScheme = AntdUI.TAMode.Dark;
            pageHeader1.Font = new Font("微软雅黑", 7.5F, FontStyle.Regular, GraphicsUnit.Point, 134);
            pageHeader1.ForeColor = SystemColors.HighlightText;
            pageHeader1.Location = new Point(0, 0);
            pageHeader1.Margin = new Padding(5, 4, 5, 4);
            pageHeader1.MaximizeBox = false;
            pageHeader1.Mode = AntdUI.TAMode.Dark;
            pageHeader1.Name = "pageHeader1";
            pageHeader1.ShowButton = true;
            pageHeader1.ShowIcon = true;
            pageHeader1.Size = new Size(1006, 56);
            pageHeader1.TabIndex = 0;
            pageHeader1.Text = "ComPE 维护系统";
            pageHeader1.UseTitleFont = true;
            pageHeader1.Click += pageHeader1_Click;
            // 
            // image3d1
            // 
            image3d1.Image = (Image)resources.GetObject("image3d1.Image");
            image3d1.ImageFit = AntdUI.TFit.Contain;
            image3d1.Location = new Point(19, 72);
            image3d1.Margin = new Padding(5, 4, 5, 4);
            image3d1.Name = "image3d1";
            image3d1.Size = new Size(341, 76);
            image3d1.TabIndex = 1;
            image3d1.Text = "image3d1";
            // 
            // label1
            // 
            label1.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            label1.ForeColor = Color.DarkSlateBlue;
            label1.Location = new Point(391, 88);
            label1.Margin = new Padding(5, 4, 5, 4);
            label1.Name = "label1";
            label1.Size = new Size(614, 32);
            label1.TabIndex = 2;
            label1.Text = "Copyright © 2025 ComPE-纯净安全且简洁的WindowsPE维护系统";
            // 
            // label2
            // 
            label2.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label2.ForeColor = Color.DarkSlateBlue;
            label2.Location = new Point(13, 148);
            label2.Margin = new Padding(5, 4, 5, 4);
            label2.Name = "label2";
            label2.Size = new Size(369, 32);
            label2.TabIndex = 4;
            label2.Text = "纯净安全且简洁的Windows PE维护系统";
            // 
            // tabs1
            // 
            tabs1.Controls.Add(tabPage1);
            tabs1.Controls.Add(tabPage2);
            tabs1.Controls.Add(tabPage3);
            tabs1.Controls.Add(tabPage4);
            tabs1.Location = new Point(391, 133);
            tabs1.Margin = new Padding(5, 4, 5, 4);
            tabs1.Name = "tabs1";
            tabs1.Pages.Add(tabPage1);
            tabs1.Pages.Add(tabPage2);
            tabs1.Pages.Add(tabPage3);
            tabs1.Pages.Add(tabPage4);
            tabs1.SelectedIndex = 1;
            tabs1.Size = new Size(596, 528);
            tabs1.Style = styleCard1;
            tabs1.TabIndex = 5;
            tabs1.TabMenuVisible = false;
            tabs1.Text = "tabs1";
            tabs1.Type = AntdUI.TabType.Card;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(button2);
            tabPage1.Controls.Add(label6);
            tabPage1.Controls.Add(label5);
            tabPage1.Controls.Add(inputNumber1);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(input1);
            tabPage1.Controls.Add(label3);
            tabPage1.Location = new Point(-586, -520);
            tabPage1.Margin = new Padding(5, 4, 5, 4);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new Size(586, 520);
            tabPage1.TabIndex = 11;
            tabPage1.Text = "tabPage1";
            tabPage1.Click += tabPage1_Click;
            // 
            // button2
            // 
            button2.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            button2.IconRatio = 1F;
            button2.IconSvg = resources.GetString("button2.IconSvg");
            button2.Location = new Point(185, 407);
            button2.Margin = new Padding(5, 4, 5, 4);
            button2.Name = "button2";
            button2.Size = new Size(215, 79);
            button2.TabIndex = 18;
            button2.Text = "开始保存";
            button2.Type = AntdUI.TTypeMini.Info;
            button2.Click += button2_Click;
            // 
            // label6
            // 
            label6.Location = new Point(112, 301);
            label6.Margin = new Padding(5, 4, 5, 4);
            label6.Name = "label6";
            label6.Size = new Size(396, 52);
            label6.TabIndex = 17;
            label6.Text = "注意：该操作涉及操作系统引导修改\r\n可能会引起防病毒软件的警告，请谨慎判断。";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.Location = new Point(112, 56);
            label5.Margin = new Padding(5, 4, 5, 4);
            label5.Name = "label5";
            label5.Size = new Size(396, 52);
            label5.TabIndex = 16;
            label5.Text = "本界面可以帮助您将ComPE安装到本地系统\r\n以便于启动时直接选择启动ComPE";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // inputNumber1
            // 
            inputNumber1.HandDragFolder = false;
            inputNumber1.InterceptArrowKeys = false;
            inputNumber1.Location = new Point(140, 198);
            inputNumber1.Margin = new Padding(5, 4, 5, 4);
            inputNumber1.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            inputNumber1.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            inputNumber1.Name = "inputNumber1";
            inputNumber1.Size = new Size(368, 42);
            inputNumber1.TabIndex = 15;
            inputNumber1.Text = "5";
            inputNumber1.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // label4
            // 
            label4.Location = new Point(44, 198);
            label4.Margin = new Padding(5, 4, 5, 4);
            label4.Name = "label4";
            label4.Size = new Size(132, 42);
            label4.TabIndex = 14;
            label4.Text = "等待时间：";
            // 
            // input1
            // 
            input1.Location = new Point(140, 147);
            input1.Margin = new Padding(5, 4, 5, 4);
            input1.Name = "input1";
            input1.Size = new Size(363, 42);
            input1.TabIndex = 13;
            input1.Text = "ComPE 维护系统";
            input1.TextChanged += input1_TextChanged;
            // 
            // label3
            // 
            label3.Location = new Point(24, 147);
            label3.Margin = new Padding(5, 4, 5, 4);
            label3.Name = "label3";
            label3.Size = new Size(132, 42);
            label3.TabIndex = 12;
            label3.Text = "启动项名称：";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(button8);
            tabPage2.Controls.Add(label14);
            tabPage2.Controls.Add(label12);
            tabPage2.Controls.Add(select4);
            tabPage2.Controls.Add(label10);
            tabPage2.Controls.Add(select3);
            tabPage2.Controls.Add(label9);
            tabPage2.Controls.Add(select2);
            tabPage2.Controls.Add(select1);
            tabPage2.Controls.Add(label7);
            tabPage2.Controls.Add(label11);
            tabPage2.Controls.Add(button7);
            tabPage2.Controls.Add(label8);
            tabPage2.Location = new Point(5, 4);
            tabPage2.Margin = new Padding(5, 4, 5, 4);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(586, 520);
            tabPage2.TabIndex = 12;
            tabPage2.Text = "tabPage2";
            // 
            // button8
            // 
            button8.Location = new Point(424, 104);
            button8.Margin = new Padding(5, 4, 5, 4);
            button8.Name = "button8";
            button8.Size = new Size(77, 41);
            button8.TabIndex = 39;
            button8.Text = "刷新";
            button8.Type = AntdUI.TTypeMini.Info;
            button8.Click += button8_Click;
            // 
            // label14
            // 
            label14.Location = new Point(82, 294);
            label14.Margin = new Padding(5, 4, 5, 4);
            label14.Name = "label14";
            label14.Size = new Size(442, 104);
            label14.TabIndex = 38;
            label14.Text = "注意：请勿在操作执行期间移除可移动磁盘设备\r\n否则可能造成数据损失！\r\n该操作涉及磁盘引导扇区读写，可能引起反病毒软件的识别（包括对bootsect.exe的），请谨慎判断。\r\n";
            label14.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            label12.Location = new Point(-5, 253);
            label12.Margin = new Padding(5, 4, 5, 4);
            label12.Name = "label12";
            label12.Size = new Size(218, 32);
            label12.TabIndex = 37;
            label12.Text = "引导分区大小(MB)：";
            label12.TextAlign = ContentAlignment.MiddleRight;
            // 
            // select4
            // 
            select4.HandDragFolder = false;
            select4.Items.AddRange(new object[] { "1024", "2048", "4096" });
            select4.Location = new Point(204, 246);
            select4.Margin = new Padding(5, 4, 5, 4);
            select4.Name = "select4";
            select4.SelectedIndex = 1;
            select4.SelectedValue = "2048";
            select4.Size = new Size(297, 40);
            select4.TabIndex = 36;
            select4.Text = "2048";
            // 
            // label10
            // 
            label10.Location = new Point(39, 205);
            label10.Margin = new Padding(5, 4, 5, 4);
            label10.Name = "label10";
            label10.Size = new Size(174, 32);
            label10.TabIndex = 33;
            label10.Text = "分区方式：";
            label10.TextAlign = ContentAlignment.MiddleRight;
            // 
            // select3
            // 
            select3.HandDragFolder = false;
            select3.Items.AddRange(new object[] { "双分区（BOOT+DATA）", "单分区（BOOT=DATA）" });
            select3.List = true;
            select3.Location = new Point(204, 198);
            select3.Margin = new Padding(5, 4, 5, 4);
            select3.Name = "select3";
            select3.SelectedIndex = 0;
            select3.SelectedValue = "双分区（BOOT+DATA）";
            select3.Size = new Size(297, 42);
            select3.TabIndex = 32;
            select3.Text = "双分区（BOOT+DATA）";
            select3.SelectedIndexChanged += select3_SelectedIndexChanged;
            // 
            // label9
            // 
            label9.Location = new Point(39, 157);
            label9.Margin = new Padding(5, 4, 5, 4);
            label9.Name = "label9";
            label9.Size = new Size(174, 32);
            label9.TabIndex = 31;
            label9.Text = "数据区分区格式：";
            label9.TextAlign = ContentAlignment.MiddleRight;
            // 
            // select2
            // 
            select2.HandDragFolder = false;
            select2.Items.AddRange(new object[] { "exFAT", "NTFS" });
            select2.List = true;
            select2.Location = new Point(204, 154);
            select2.Margin = new Padding(5, 4, 5, 4);
            select2.Name = "select2";
            select2.SelectedIndex = 0;
            select2.SelectedValue = "exFAT";
            select2.Size = new Size(297, 38);
            select2.TabIndex = 30;
            select2.Text = "exFAT";
            // 
            // select1
            // 
            select1.List = true;
            select1.Location = new Point(204, 107);
            select1.Margin = new Padding(5, 4, 5, 4);
            select1.Name = "select1";
            select1.Size = new Size(223, 38);
            select1.TabIndex = 29;
            // 
            // label7
            // 
            label7.Location = new Point(115, 112);
            label7.Margin = new Padding(5, 4, 5, 4);
            label7.Name = "label7";
            label7.Size = new Size(107, 32);
            label7.TabIndex = 28;
            label7.Text = "安装磁盘：";
            // 
            // label11
            // 
            label11.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Underline, GraphicsUnit.Point, 134);
            label11.Location = new Point(424, 491);
            label11.Margin = new Padding(5, 4, 5, 4);
            label11.Name = "label11";
            label11.Size = new Size(157, 32);
            label11.TabIndex = 26;
            label11.Text = "U盘启动按键查询";
            label11.TextAlign = ContentAlignment.MiddleRight;
            label11.Click += label11_Click;
            // 
            // button7
            // 
            button7.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            button7.IconRatio = 1F;
            button7.IconSvg = resources.GetString("button7.IconSvg");
            button7.Location = new Point(185, 407);
            button7.Margin = new Padding(5, 4, 5, 4);
            button7.Name = "button7";
            button7.Size = new Size(215, 79);
            button7.TabIndex = 25;
            button7.Text = "开始安装";
            button7.Type = AntdUI.TTypeMini.Info;
            button7.Click += button7_Click;
            // 
            // label8
            // 
            label8.Location = new Point(82, 44);
            label8.Margin = new Padding(5, 4, 5, 4);
            label8.Name = "label8";
            label8.Size = new Size(453, 52);
            label8.TabIndex = 23;
            label8.Text = "本界面可以帮助您将ComPE安装到可移动磁盘设备\r\n以便于在不同的环境下去使用";
            label8.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(button10);
            tabPage3.Controls.Add(button9);
            tabPage3.Controls.Add(label15);
            tabPage3.Controls.Add(label16);
            tabPage3.Controls.Add(input2);
            tabPage3.Controls.Add(label18);
            tabPage3.Location = new Point(-586, -520);
            tabPage3.Margin = new Padding(5, 4, 5, 4);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(586, 520);
            tabPage3.TabIndex = 13;
            tabPage3.Text = "tabPage3";
            // 
            // button10
            // 
            button10.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            button10.IconRatio = 0.6F;
            button10.IconSvg = resources.GetString("button10.IconSvg");
            button10.Location = new Point(462, 181);
            button10.Margin = new Padding(5, 4, 5, 4);
            button10.Name = "button10";
            button10.Size = new Size(46, 40);
            button10.TabIndex = 26;
            button10.Type = AntdUI.TTypeMini.Info;
            button10.Click += button10_Click;
            // 
            // button9
            // 
            button9.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            button9.IconRatio = 1F;
            button9.IconSvg = resources.GetString("button9.IconSvg");
            button9.Location = new Point(185, 407);
            button9.Margin = new Padding(5, 4, 5, 4);
            button9.Name = "button9";
            button9.Size = new Size(215, 79);
            button9.TabIndex = 25;
            button9.Text = "开始保存";
            button9.Type = AntdUI.TTypeMini.Info;
            button9.Click += button9_Click;
            // 
            // label15
            // 
            label15.Location = new Point(112, 301);
            label15.Margin = new Padding(5, 4, 5, 4);
            label15.Name = "label15";
            label15.Size = new Size(396, 52);
            label15.TabIndex = 24;
            label15.Text = "提醒：尽量不要保存在不便于查找的地方，\r\n避免给您带来不便。";
            label15.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            label16.Location = new Point(112, 66);
            label16.Margin = new Padding(5, 4, 5, 4);
            label16.Name = "label16";
            label16.Size = new Size(396, 52);
            label16.TabIndex = 23;
            label16.Text = "本界面可以帮助您将ComPE的光盘镜像文件\r\n分离到指定目录";
            label16.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // input2
            // 
            input2.Location = new Point(140, 178);
            input2.Margin = new Padding(5, 4, 5, 4);
            input2.Name = "input2";
            input2.Size = new Size(328, 42);
            input2.TabIndex = 20;
            // 
            // label18
            // 
            label18.Location = new Point(52, 178);
            label18.Margin = new Padding(5, 4, 5, 4);
            label18.Name = "label18";
            label18.Size = new Size(132, 42);
            label18.TabIndex = 19;
            label18.Text = "保存位置：";
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(select5);
            tabPage4.Controls.Add(label19);
            tabPage4.Controls.Add(button11);
            tabPage4.Controls.Add(label17);
            tabPage4.Controls.Add(select8);
            tabPage4.Controls.Add(label22);
            tabPage4.Controls.Add(button12);
            tabPage4.Controls.Add(label23);
            tabPage4.Location = new Point(-586, -520);
            tabPage4.Margin = new Padding(5, 4, 5, 4);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(586, 520);
            tabPage4.TabIndex = 14;
            tabPage4.Text = "tabPage4";
            // 
            // select5
            // 
            select5.Items.AddRange(new object[] { "Legacy BIOS", "UEFI x32", "UEFI x64" });
            select5.List = true;
            select5.Location = new Point(204, 219);
            select5.Margin = new Padding(5, 4, 5, 4);
            select5.Name = "select5";
            select5.SelectedIndex = 0;
            select5.SelectedValue = "Legacy BIOS";
            select5.Size = new Size(223, 38);
            select5.TabIndex = 53;
            select5.Text = "Legacy BIOS";
            // 
            // label19
            // 
            label19.Location = new Point(39, 217);
            label19.Margin = new Padding(5, 4, 5, 4);
            label19.Name = "label19";
            label19.Size = new Size(190, 32);
            label19.TabIndex = 52;
            label19.Text = "模拟启动固件类型：";
            // 
            // button11
            // 
            button11.Location = new Point(424, 169);
            button11.Margin = new Padding(5, 4, 5, 4);
            button11.Name = "button11";
            button11.Size = new Size(77, 41);
            button11.TabIndex = 51;
            button11.Text = "刷新";
            button11.Type = AntdUI.TTypeMini.Info;
            button11.Click += button11_Click;
            // 
            // label17
            // 
            label17.Location = new Point(68, 319);
            label17.Margin = new Padding(5, 4, 5, 4);
            label17.Name = "label17";
            label17.Size = new Size(459, 52);
            label17.TabIndex = 50;
            label17.Text = "提示：若弹出有关”内存不足无法创建RamDisk”的内容\r\n属正常现象，建议在实际环境下测试";
            label17.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // select8
            // 
            select8.List = true;
            select8.Location = new Point(204, 172);
            select8.Margin = new Padding(5, 4, 5, 4);
            select8.Name = "select8";
            select8.Size = new Size(223, 38);
            select8.TabIndex = 43;
            // 
            // label22
            // 
            label22.Location = new Point(115, 176);
            label22.Margin = new Padding(5, 4, 5, 4);
            label22.Name = "label22";
            label22.Size = new Size(107, 32);
            label22.TabIndex = 42;
            label22.Text = "安装磁盘：";
            // 
            // button12
            // 
            button12.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            button12.IconRatio = 1F;
            button12.IconSvg = resources.GetString("button12.IconSvg");
            button12.Location = new Point(189, 404);
            button12.Margin = new Padding(5, 4, 5, 4);
            button12.Name = "button12";
            button12.Size = new Size(215, 79);
            button12.TabIndex = 41;
            button12.Text = "启动虚拟机";
            button12.Type = AntdUI.TTypeMini.Info;
            button12.Click += button12_Click;
            // 
            // label23
            // 
            label23.Location = new Point(74, 56);
            label23.Margin = new Padding(5, 4, 5, 4);
            label23.Name = "label23";
            label23.Size = new Size(453, 73);
            label23.TabIndex = 40;
            label23.Text = "本界面可用于在当前环境下测试“移动安装”启动盘\r\n是否正常制作\r\n（测试环境为QEMU虚拟机）";
            label23.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // divider1
            // 
            divider1.BackColor = Color.Transparent;
            divider1.ColorSplit = SystemColors.ActiveBorder;
            divider1.Location = new Point(369, 128);
            divider1.Margin = new Padding(5, 4, 5, 4);
            divider1.Name = "divider1";
            divider1.Size = new Size(41, 524);
            divider1.TabIndex = 4;
            divider1.Text = "";
            divider1.Vertical = true;
            // 
            // divider2
            // 
            divider2.BackColor = Color.Transparent;
            divider2.ColorSplit = SystemColors.AppWorkspace;
            divider2.Location = new Point(401, 116);
            divider2.Margin = new Padding(5, 4, 5, 4);
            divider2.Name = "divider2";
            divider2.Size = new Size(586, 23);
            divider2.TabIndex = 5;
            divider2.Text = "";
            divider2.Thickness = 1F;
            divider2.Click += divider2_Click;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(button1);
            flowLayoutPanel1.Controls.Add(button3);
            flowLayoutPanel1.Controls.Add(button4);
            flowLayoutPanel1.Controls.Add(button5);
            flowLayoutPanel1.Controls.Add(button6);
            flowLayoutPanel1.Location = new Point(19, 189);
            flowLayoutPanel1.Margin = new Padding(5, 4, 5, 4);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(341, 467);
            flowLayoutPanel1.TabIndex = 6;
            // 
            // button1
            // 
            button1.BackColor = Color.DodgerBlue;
            button1.Font = new Font("等线", 15.75F, FontStyle.Bold | FontStyle.Italic);
            button1.ForeColor = Color.Black;
            button1.IconRatio = 0.8F;
            button1.IconSvg = resources.GetString("button1.IconSvg");
            button1.Location = new Point(5, 4);
            button1.Margin = new Padding(5, 4, 5, 4);
            button1.Name = "button1";
            button1.Size = new Size(336, 85);
            button1.TabIndex = 0;
            button1.Text = "本地安装";
            button1.Type = AntdUI.TTypeMini.Primary;
            button1.Click += button1_Click;
            // 
            // button3
            // 
            button3.BackColor = SystemColors.InactiveCaption;
            button3.Font = new Font("等线", 15.75F, FontStyle.Bold | FontStyle.Italic);
            button3.ForeColor = Color.Black;
            button3.IconRatio = 0.8F;
            button3.IconSvg = resources.GetString("button3.IconSvg");
            button3.Location = new Point(5, 97);
            button3.Margin = new Padding(5, 4, 5, 4);
            button3.Name = "button3";
            button3.Size = new Size(336, 85);
            button3.TabIndex = 5;
            button3.Text = "移动安装";
            button3.Type = AntdUI.TTypeMini.Primary;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.BackColor = SystemColors.InactiveCaption;
            button4.Font = new Font("等线", 15.75F, FontStyle.Bold | FontStyle.Italic);
            button4.ForeColor = Color.Black;
            button4.IconRatio = 0.8F;
            button4.IconSvg = resources.GetString("button4.IconSvg");
            button4.Location = new Point(5, 190);
            button4.Margin = new Padding(5, 4, 5, 4);
            button4.Name = "button4";
            button4.Size = new Size(336, 85);
            button4.TabIndex = 2;
            button4.Text = "镜像保存";
            button4.Type = AntdUI.TTypeMini.Primary;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.BackColor = SystemColors.InactiveCaption;
            button5.Font = new Font("等线", 15.75F, FontStyle.Bold | FontStyle.Italic);
            button5.ForeColor = Color.Black;
            button5.IconRatio = 0.8F;
            button5.IconSvg = resources.GetString("button5.IconSvg");
            button5.Location = new Point(5, 283);
            button5.Margin = new Padding(5, 4, 5, 4);
            button5.Name = "button5";
            button5.Size = new Size(336, 85);
            button5.TabIndex = 3;
            button5.Text = "启动测试";
            button5.Type = AntdUI.TTypeMini.Primary;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.BackColor = SystemColors.InactiveCaption;
            button6.Font = new Font("等线", 15.75F, FontStyle.Bold | FontStyle.Italic);
            button6.ForeColor = Color.Black;
            button6.IconRatio = 0.8F;
            button6.IconSvg = resources.GetString("button6.IconSvg");
            button6.Location = new Point(5, 376);
            button6.Margin = new Padding(5, 4, 5, 4);
            button6.Name = "button6";
            button6.Size = new Size(336, 85);
            button6.TabIndex = 4;
            button6.Text = "重启系统";
            button6.Type = AntdUI.TTypeMini.Primary;
            button6.Click += button6_Click;
            // 
            // progress1
            // 
            progress1.ContainerControl = this;
            progress1.HandDragFolder = false;
            progress1.Location = new Point(0, 659);
            progress1.Margin = new Padding(5, 4, 5, 4);
            progress1.Name = "progress1";
            progress1.ShowInTaskbar = true;
            progress1.ShowTextDot = 1;
            progress1.Size = new Size(1006, 17);
            progress1.State = AntdUI.TType.Success;
            progress1.Steps = 100;
            progress1.TabIndex = 7;
            progress1.Text = "";
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileName = "ComPE_Release.iso";
            saveFileDialog1.Filter = "光盘镜像文件 (*.iso)|*.iso";
            // 
            // label13
            // 
            label13.Location = new Point(740, 628);
            label13.Margin = new Padding(5, 4, 5, 4);
            label13.Name = "label13";
            label13.Size = new Size(248, 28);
            label13.TabIndex = 18;
            label13.Text = "启动固件类型：Legacy";
            label13.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1006, 678);
            ControlBox = false;
            Controls.Add(label13);
            Controls.Add(progress1);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(divider2);
            Controls.Add(divider1);
            Controls.Add(tabs1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(image3d1);
            Controls.Add(pageHeader1);
            Dark = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(6, 6, 6, 6);
            Mode = AntdUI.TAMode.Dark;
            Name = "Form1";
            Resizable = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ComPE 维护系统";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            tabs1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tabPage4.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.PageHeader pageHeader1;
        private AntdUI.Image3D image3d1;
        private AntdUI.Label label1;
        private AntdUI.Label label2;
        private AntdUI.Tabs tabs1;
        private AntdUI.Divider divider1;
        private AntdUI.Divider divider2;
        private AntdUI.TabPage tabPage1;
        private AntdUI.TabPage tabPage2;
        private AntdUI.TabPage tabPage3;
        private AntdUI.TabPage tabPage4;
        private AntdUI.Input input1;
        private AntdUI.Label label3;
        private FlowLayoutPanel flowLayoutPanel1;
        private AntdUI.Button button1;
        private AntdUI.Button button4;
        private AntdUI.Button button5;
        private AntdUI.Button button6;
        private AntdUI.Button button3;
        private AntdUI.Label label6;
        private AntdUI.Label label5;
        private AntdUI.Button button2;
        private AntdUI.Progress progress1;
        private AntdUI.Label label11;
        private AntdUI.Button button7;
        private AntdUI.Label label8;
        private AntdUI.Label label7;
        private AntdUI.Select select1;
        private AntdUI.Select select2;
        private AntdUI.Label label9;
        private AntdUI.Select select3;
        private AntdUI.Label label12;
        private AntdUI.Select select4;
        private AntdUI.Label label10;
        private AntdUI.Label label14;
        private AntdUI.Button button8;
        private AntdUI.Button button9;
        private AntdUI.Label label15;
        private AntdUI.Label label16;
        private AntdUI.Input input2;
        private AntdUI.Label label18;
        private AntdUI.InputNumber inputNumber1;
        private AntdUI.Label label4;
        private AntdUI.Button button10;
        private SaveFileDialog saveFileDialog1;
        private AntdUI.Label label13;
        private AntdUI.Button button11;
        private AntdUI.Label label17;
        private AntdUI.Select select8;
        private AntdUI.Label label22;
        private AntdUI.Button button12;
        private AntdUI.Label label23;
        private AntdUI.Select select5;
        private AntdUI.Label label19;
    }
}
