using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Windows;
using Plugin;
using System.Threading;
using System.Security.Permissions;
using System.AddIn.Pipeline;
using System.Threading.Tasks;

namespace MultiProcessClient
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<TabItemView> _tabItems;
        /// <summary>
        /// tab集合
        /// </summary>
        public ObservableCollection<TabItemView> TabItems
        {
            get { return _tabItems; }
            set
            {
                _tabItems = value;
                RaisePropertyChanged("TabItems");
            }
        }

        private TabItemView _selectedItem;
        /// <summary>
        /// 选中的tab
        /// </summary>
        public TabItemView SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }

        /// <summary>
        /// 添加tab
        /// </summary>
        public RelayCommand AddTabCommand { get; set; }

        /// <summary>
        /// 关闭tab
        /// </summary>
        public RelayCommand<TabItemView> CloseTabCommand { get; set; }

        string _processName = string.Empty;
        private IpcChannel _channelHost;
        private EventWaitHandle _readyEvent;

        public MainViewModel()
        {
            _processName = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "Plugin.exe");
            _channelHost = new IpcChannel();
            ChannelServices.RegisterChannel(_channelHost, false);

            AddTabCommand = new RelayCommand(AddTab);
            CloseTabCommand = new RelayCommand<TabItemView>(CloseTab);

            TabItems = new ObservableCollection<TabItemView>();
        }

        /// <summary>
        /// 添加tab
        /// </summary>
        private void AddTab()
        {
            //WellKnownClientTypeEntry remoteType = new WellKnownClientTypeEntry(typeof(ItemControl), string.Format("ipc://{0}ServerChannel/RemoteObject", name));
            //RemotingConfiguration.RegisterWellKnownClientType(remoteType);
            //ItemControl tab = new ItemControl();
            ProcessStartInfo startProcess = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = _processName,
            };
            AddTabItem(startProcess);

        }

        /// <summary>
        /// 添加子界面和进程
        /// </summary>
        /// <param name="startInfo"></param>
        private void AddTabItem(ProcessStartInfo startInfo)
        {
            string name = Guid.NewGuid().ToString();
            Task.Factory.StartNew(() =>
            {
                _readyEvent = new EventWaitHandle(false, EventResetMode.ManualReset, name + ".Ready");
                startInfo.Arguments = name;
                var process = Process.Start(startInfo);
                return process;
            }).ContinueWith((result) =>
            {
                if (!_readyEvent.WaitOne(2000))
                {
                    MessageBox.Show("子进程UI创建超时");
                }
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    RemotePlugin tab = (RemotePlugin)Activator.GetObject(typeof(RemotePlugin), string.Format("ipc://{0}ServerChannel/RemoteObject", name));
                    if (tab == null) return;
                    FrameworkElement remoteView = FrameworkElementAdapters.ContractToViewAdapter(tab.Contract);
                    TabItemView item = new TabItemView()
                    {
                        Title = "进程",
                        Contract = tab.Contract,
                        Process = result.Result as Process,
                    };
                    item.Error += ItemError;
                    TabItems.Add(item);
                    SelectedItem = item;
                }));
            });
        }

        /// <summary>
        /// 子进程出现异常回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemError(object sender, EventArgs e)
        {
            var item = sender as TabItemView;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                TabItems.Remove(item);
            }));
            AddTabItem(item.Process.StartInfo);
        }

        /// <summary>
        /// 关闭tab
        /// </summary>
        private void CloseTab(TabItemView view)
        {
            TabItems.Remove(view);
            view.Dispose();
        }

        /// <summary>
        /// 关闭所有子进程界面
        /// </summary>
        public void CloseAllTabs()
        {
            if (TabItems.Count() > 0)
            {
                foreach (var item in TabItems)
                {
                    item.Dispose();
                }
            }
        }
    }
}
