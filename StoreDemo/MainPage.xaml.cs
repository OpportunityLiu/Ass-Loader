using System;
using System.IO;
using System.Threading.Tasks;
using AssLoader;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace StoreDemo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".ass");
        }

        private FileOpenPicker picker;

        private async void open_Click(object sender, RoutedEventArgs e)
        {
            var file = await picker.PickSingleFileAsync();
            if(file == null)
                return;
            bool catched = false;
            try
            {
                await LoadFile(file);
            }
            catch(FormatException)
            {
                catched = true;
            }
            catch(ArgumentException)
            {
                catched = true;
            }
            if(catched)
                await new MessageDialog("Wrong file format.", "Error").ShowAsync();
        }

        StorageFile file;

        private async void close_Click(object sender, RoutedEventArgs e)
        {
            await CloseFile(true);
        }

        public async Task CloseFile(bool save)
        {
            if(save)
            {
                using(var s = await file.OpenStreamForWriteAsync())
                using(var writer = new StreamWriter(s))
                    ((Subtitle<AssScriptInfo>)this.DataContext).Serialize(writer);
            }
            this.DataContext = null;
            file = null;
            this.bclose.IsEnabled = false;
        }

        public async Task LoadFile(StorageFile file)
        {
            if(this.DataContext != null)
                await CloseFile(false);
            if(file == null)
                throw new ArgumentNullException("file");
            using(var s = await file.OpenStreamForReadAsync())
            using(var reader = new StreamReader(s))
                this.DataContext = Subtitle.Parse<AssScriptInfo>(reader);
            this.file = file;
            this.bclose.IsEnabled = true;
        }
    }
}
