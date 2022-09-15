
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Plugin
{
    public class PluginViewModel : ViewModelBase
    {
        private string _content;
        /// <summary>
        /// TabItem内容
        /// </summary>
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                RaisePropertyChanged("Content");
            }
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        public RelayCommand AddContent { get; set; }
        /// <summary>
        /// 抛出异常
        /// </summary>
        public RelayCommand ThrowException { get; set; }

        public PluginViewModel()
        {
            AddContent = new RelayCommand(Add);
            ThrowException = new RelayCommand(Throw);
        }

        /// <summary>
        /// 添加
        /// </summary>
        private void Add()
        {
            Task.Factory.StartNew(() =>
            {
                ThreadDispatcher.Dispatcher.Invoke(() =>
                {
                    TestWindowView win = new TestWindowView();
                    win.Show();
                });
                for (int i = 0; ; i++)
                {
                    Thread.Sleep(1000);
                    Content = "测试多进程Mvvm功能：" + i;
                }
            });
        }

        /// <summary>
        /// 抛出异常
        /// </summary>
        private void Throw()
        {
            throw new Exception("出错啦！！！");
        }
    }
}
