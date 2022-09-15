using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Plugin
{
    /// <summary>
    /// 主线程调度器
    /// </summary>
    public class ThreadDispatcher
    {
        private static Dispatcher _currentDispatcher;
        /// <summary>
        /// 当前主线程
        /// </summary>
        public static Dispatcher Dispatcher
        {
            get { return _currentDispatcher; }
            set { _currentDispatcher = value; }
        }

        static ThreadDispatcher()
        {
            _currentDispatcher = Dispatcher.CurrentDispatcher;
        }

    }
}
