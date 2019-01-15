﻿using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Hardcodet.Wpf.TaskbarNotification;

namespace InvvardDev.ErgodoxLayoutDisplay.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}
