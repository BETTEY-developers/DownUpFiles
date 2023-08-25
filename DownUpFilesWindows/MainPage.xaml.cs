using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace DownUpFilesWindows
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void NavigationGV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridViewItem gvi = (sender as GridView).SelectedItem as GridViewItem;
            (Window.Current.Content as Frame).Navigate(Assembly.GetExecutingAssembly().GetType("DownUpFilesWindows." + (gvi.Tag as string)));
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    Windows.UI.Core.AppViewBackButtonVisibility.Visible;
            base.OnNavigatingFrom(e);
        }
    }
}
