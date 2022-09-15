using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Plugin
{
    class Program
    {
        private static string _name;

        [STAThread]
        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        static void Main(string[] args)
        {
            try
            {
                if (args.Count() > 0)
                {
                    _name = args[0];
                    Console.WriteLine("进程Name：" + _name);
                    Console.WriteLine("进程ID：" + Process.GetCurrentProcess().Id);
                    var appDomain = AppDomain.CreateDomain("TabDomain");
                    var boot = (BootStrapper)appDomain.CreateInstanceFromAndUnwrap(typeof(BootStrapper).Assembly.Location, typeof(BootStrapper).FullName);
                    boot.Run(_name);
                    SendReadyToHost();
                }
                Dispatcher.Run();
            }
            catch (Exception)
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        /// <summary>
        /// 向主进程发送“已准备好”信号，线程同步
        /// </summary>
        private static void SendReadyToHost()
        {
            var eventName = _name + ".Ready";
            var readyEvent = EventWaitHandle.OpenExisting(eventName);
            readyEvent.Set();
        }
    }
}
