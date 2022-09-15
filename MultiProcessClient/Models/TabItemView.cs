using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultiProcessClient
{
    public class TabItemView : IDisposable
    {
        /// <summary>
        /// View所在进程关闭时触发
        /// </summary>
        public event EventHandler Error;

        /// <summary>
        /// tab标题
        /// </summary>
        public string Title { get; set; }

        private INativeHandleContract _contract;
        /// <summary>
        /// 子进程窗口句柄
        /// </summary>
        public INativeHandleContract Contract
        {
            get { return _contract; }
            set
            {
                _contract = value;
                if (_contract != null)
                {
                    View = FrameworkElementAdapters.ContractToViewAdapter(value);
                }
            }
        }

        /// <summary>
        /// tab内容视图
        /// </summary>
        public FrameworkElement View { get; set; }

        private Process _process;
        /// <summary>
        /// 当前进程
        /// </summary>
        public Process Process
        {
            get { return _process; }
            set
            {
                _process = value;
                _process.Exited += ProcessExited;
                _process.EnableRaisingEvents = true;
            }
        }

        /// <summary>
        /// 进程退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessExited(object sender, EventArgs e)
        {
            Error(this, e);
        }

        /// <summary>
        /// 释放tab，并关闭进程
        /// </summary>
        public void Dispose()
        {
            try
            {
                var view = View as IDisposable;
                if (view != null)
                {
                    view.Dispose();
                }
                if (Process != null)
                {
                    if (!Process.HasExited)
                    {
                        Process.EnableRaisingEvents = false;
                        Process.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("关闭tab出现异常：" + ex.Message);
            }
        }
    }
}
