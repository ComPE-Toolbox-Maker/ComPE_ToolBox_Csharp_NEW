using AntdUI;
namespace ComPE_ToolBox
{

    internal static class Program
    {
        static Mutex mutex = new Mutex(true, "{AE86470F-17B5-3F04-64CB-D948FCE8F9BE}");

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                DialogResult dialog = AntdUI.Modal.open(new Modal.Config(null, "提示：", "程序已在运行，为避免资源冲突，请勿重复运行。", TType.Error)
                {
                    CancelText = null,
                    MaskClosable = false
                });
                return;
            }
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            try
            {
                // 主程序逻辑
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1()); // WinForms 示例
            }
            finally
            {
                mutex.ReleaseMutex(); // 释放 Mutex
            }
        }
    }
}