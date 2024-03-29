﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Plugin
{
    /// <summary>
    /// TabItemControl.xaml 的交互逻辑
    /// </summary>
    public partial class PluginView : UserControl
    {
        public PluginView()
        {
            InitializeComponent();
            ProcessID.Text = "当前进程：" + Process.GetCurrentProcess().Id.ToString();
            this.DataContext = new PluginViewModel();
        }
    }
}
