using DownUpFilesWindows.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Composition;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace DownUpFilesWindows
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UploadFilesPage : Page
    {
        public class FileIndex
        {
            public int Version { get; set; }
            public string Time { get; set; }
            public List<FileItem> FileItems { get; set; }
        }

        public bool IsActivting { get; set; }

        public ObservableCollection<FileItem> Items { get; set; }

        public static string __RemoveButtonText { get; set; } = "删除文件";

        public UploadFilesPage()
        {
            this.InitializeComponent();
            Items = new ObservableCollection<FileItem>();
            FilesItemsSource.Source = Items;
#if DEBUG
            AddItems();
#endif
        }

#if DEBUG
        private void AddItems()
        {
            FileItem fi = new FileItem
            {
                Path = "114514",
                Name = "上北泽",
                IsDirectory = false,
                Size = "1919810KB"
            };
            Items.Add(fi);
            Items.Add(fi);

        }
#endif

        public static T FindChildrenParent<T>(DependencyObject obj) where T : class
        {
            if (obj == null) return null;

            if (obj is T listView)
            {
                return listView;
            }
            else
            {
                int count = VisualTreeHelper.GetChildrenCount(obj);
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(obj, i);
                    var listViewChild = FindChildrenParent<T>(child);
                    if (listViewChild != null)
                        return listViewChild;
                }
            }

            return null;
        }

        private void RemoveFileItem(object sender, RoutedEventArgs e)
        {
            var f = FilesItems;
            var parentgrid = VisualTreeHelper.GetParent((((sender as FrameworkElement).Parent as FrameworkElement).Parent as FrameworkElement).Parent as Grid) as ListViewItemPresenter;
            Items.Remove(parentgrid.Content as FileItem);
        }

        private string FormatSize(ulong size)
        {
            string result = "";
            if(size >= 1024 * 1024 * 1024)
            {
                result += Math.Ceiling((decimal)(size / (1024 * 1024 * 1024))).ToString() + " GiB";
            }
            else if(size >= 1024 * 1024)
            {
                result += Math.Ceiling((decimal)(size / (1024 * 1024))).ToString() + " MiB";
            }
            else if (size >= 1024)
            {
                result += Math.Ceiling((decimal)(size / 1024)).ToString() + " KiB";
            }
            else
            {
                result += size.ToString() + " Byte";
            }
            return result;
        }
        
        private async void AddFiles_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            fileOpenPicker.FileTypeFilter.Add("*");
            var f = await fileOpenPicker.PickMultipleFilesAsync();
            var filesobj = f.ToList();

            foreach( var file in filesobj )
            {
                FileItem fileItem = new FileItem
                {
                    Path = file.Path.Trim()
                };
                var prop = await file.GetBasicPropertiesAsync();
                fileItem.Size = FormatSize(prop.Size);
                fileItem.IsDirectory = false;
                fileItem.Name = file.Name.Trim();
                fileItem.RawSize = (int)(prop.Size % int.MaxValue);
                if(!Contains(fileItem, Items))
                {
                    Items.Add(fileItem);
                }
                else
                {
                    var md = new MessageDialog("已经添加过该文件！","错误");
                    md.Options = MessageDialogOptions.AcceptUserInputAfterDelay;
                    md.Commands.Add(new UICommand("确认"));
                    md.DefaultCommandIndex = 0;
                    md.CancelCommandIndex = 0;
                    await md.ShowAsync();
                }
            }
        }

        private bool Contains<T>(T EqualItem, IList<T> Collection)
        {
            foreach(var item in Collection)
            {
                if (item.Equals(EqualItem))
                {
                    return true;
                }
            }
            return false;
        }

        private async void UploadFileList_Click(object sender, RoutedEventArgs e)
        {
            string message = "";

            if (Items.Count == 0)
            {
                message = "列表为空";
            }
            else
            {
                FileIndex findex = new FileIndex()
                {
                    Version = 1,
                    Time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm"),
                    FileItems = Items.ToList()
                };
                
                void Create(StreamedFileDataRequest streamedFileData)
                {
                    JsonSerializer.Serialize(streamedFileData.AsStreamForWrite(), Items, typeof(IEnumerable<FileItem>));
                }

                StorageFile.CreateStreamedFileAsync("DownUpFilesWindows.FilesList.json", Create, null);
                message = "列表文件已经生成！";
            }

            MessageDialog md = new MessageDialog(message);
            md.Title = "";

            md.Commands.Add(new UICommand("确定"));
            md.CancelCommandIndex = 0;
            md.DefaultCommandIndex = 0;

            await md.ShowAsync();
        }

        enum ActionStatus
        {
            OK,
            NO,
            Error
        }

        private ActionStatus Open()
        {
            Thread.Sleep(1000);
            return ActionStatus.OK;
        }
        private ActionStatus Close()
        {
            Thread.Sleep(1000);
            return ActionStatus.OK;
        }

        private async Task OpenToggle()
        {
            var sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.HorizontalAlignment = HorizontalAlignment.Stretch;
            sp.Children.Add(new TextBlock() { Text = "Opening..." });
            sp.Children.Add(new ProgressRing() { Margin = new Thickness(10,0,0,0), IsActive = true });
            ServerActive.OnContent = sp;
            this.IsEnabled = false;

            var status = await Task.Run(Open);

            switch (status)
            {
                case ActionStatus.OK:
                    ServerActive.OnContent = new TextBlock() { Text = "Opened" };
                    IsEnabled = true;
                    break;
            }

            await Task.Delay(500);

            var openstring = new TextBlock() { Text = "Open" };

            // 创建一个Storyboard动画
            Storyboard storyboard = new Storyboard();

            // 创建一个DoubleAnimation
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.Duration = TimeSpan.FromSeconds(0.5);
            opacityAnimation.To = 0;
            opacityAnimation.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 4.5 };

            // 将动画添加到Storyboard中
            storyboard.Children.Add(opacityAnimation);

            // 设置TargetName和TargetProperty
            Storyboard.SetTarget(opacityAnimation, (DependencyObject)ServerActive.OnContent);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");

            // 激活动画
            storyboard.Begin();

            await Task.Delay(500);

            storyboard = new Storyboard();

            ServerActive.OnContent = openstring;
            (ServerActive.OnContent as TextBlock).Opacity = 0;
            opacityAnimation = new DoubleAnimation();
            opacityAnimation.Duration = TimeSpan.FromSeconds(0.5);
            opacityAnimation.From = 0;
            opacityAnimation.To = 1;
            opacityAnimation.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 4.5 };
            storyboard.Children.Add(opacityAnimation);
            Storyboard.SetTarget(opacityAnimation, (DependencyObject)ServerActive.OnContent);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
            storyboard.Begin();
            await Task.Delay(500);

        }
        private async Task CloseToggle()
        {
            var sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.HorizontalAlignment = HorizontalAlignment.Stretch;
            sp.Children.Add(new TextBlock() { Text = "Closing..." });
            sp.Children.Add(new ProgressRing() { Margin = new Thickness(10, 0, 0, 0), IsActive = true });
            ServerActive.OffContent = sp;
            this.IsEnabled = false;

            var status = await Task.Run(Close);

            switch (status)
            {
                case ActionStatus.OK:
                    ServerActive.OffContent = new TextBlock() { Text = "Closed" };
                    IsEnabled = true;
                    break;
            }
            await Task.Delay(500);

            var endstring = new TextBlock() { Text = "Close" };

            // 创建一个Storyboard动画
            Storyboard storyboard = new Storyboard();

            // 创建一个DoubleAnimation
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.Duration = TimeSpan.FromSeconds(0.5);
            opacityAnimation.To = 0;
            opacityAnimation.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 4.5 };

            // 将动画添加到Storyboard中
            storyboard.Children.Add(opacityAnimation);

            // 设置TargetName和TargetProperty
            Storyboard.SetTarget(opacityAnimation, (DependencyObject)ServerActive.OffContent);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");

            // 激活动画
            storyboard.Begin();

            await Task.Delay(500);

            storyboard = new Storyboard();

            ServerActive.OffContent = endstring;
            (ServerActive.OffContent as TextBlock).Opacity = 0;
            opacityAnimation = new DoubleAnimation();
            opacityAnimation.Duration = TimeSpan.FromSeconds(0.5);
            opacityAnimation.From = 0;
            opacityAnimation.To = 1;
            opacityAnimation.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 4.5 };
            storyboard.Children.Add(opacityAnimation);
            Storyboard.SetTarget(opacityAnimation, (DependencyObject)ServerActive.OffContent);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
            storyboard.Begin();
            await Task.Delay(500);
        }
        private void ServerActive_Toggled(object sender, RoutedEventArgs e)
        {
            if (ServerActive.IsOn)
            {
                OpenToggle();
            }
            else
            {
                CloseToggle();
            }
        }

        
    }
}
