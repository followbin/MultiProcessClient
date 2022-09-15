using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Plugin
{
    public class RemotePlugin : MarshalByRefObject, IDisposable
    {
        public RemotePlugin()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// 注册信道
        /// </summary>
        /// <param name="name"></param>
        public void RegisterChannel(string name)
        {
            Console.WriteLine("注册信道:" + name);  
            IpcChannel serverChannel = new IpcChannel(name + "ServerChannel");
            ChannelServices.RegisterChannel(serverChannel, false);
            //RemotingConfiguration.RegisterWellKnownServiceType(typeof(ItemControl), "RemoteObject", WellKnownObjectMode.Singleton);
            RemotingServices.Marshal(this, "RemoteObject", typeof(RemotePlugin));
            Console.WriteLine("开始生成UI");
            ThreadDispatcher.Dispatcher.Invoke(() =>
            {
                var local = FrameworkElementAdapters.ViewToContractAdapter(new PluginView());
                Contract = new NativeHandleContractInsulator(local);
                Console.WriteLine("生成UI结束:" + Contract.GetHandle());
            });
        }

        public INativeHandleContract Contract { get; private set; }

        public override object InitializeLifetimeService()
        {
            return null; // 永存
        }

        private  void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// 释放控件
        /// </summary>
        public void Dispose()
        {
            if (ThreadDispatcher.Dispatcher != null)
            {
                ThreadDispatcher.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
            }
            else
            {
                Environment.Exit(0);
            }
        }
        
    }
}
