using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;


namespace SubtitleEditor.Controls
{
    public sealed class SamplePresenter : ContentControl
    {
        public SamplePresenter()
        {
            this.DefaultStyleKey = typeof(SamplePresenter);
            this.Loaded += (o, e) =>
            {   // listen for size changes after control is loaded
                this.SizeChanged += new SizeChangedEventHandler(SamplePresenter_SizeChanged);
            };
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.BackgroundImage = (ImageBrush)GetTemplateChild("BackgroundImage");
            if(BackgroundImage != null)
                this.BackgroundImage.Stretch = Windows.UI.Xaml.Media.Stretch.None;
        }

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(SamplePresenter), new PropertyMetadata(null, new PropertyChangedCallback(OnImageSourceUriChanged)));

        public string ImageSource
        {
            get
            {
                return (string)GetValue(ImageSourceProperty);
            }
            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }

        private static async void OnImageSourceUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(DesignMode.DesignModeEnabled)
                return;
            var sender = ((SamplePresenter)d);
            await sender.UpdateImageSource();
            await sender.UpdateBackground(true);
        }

        private int _sourcePixelWidth;
        private int _sourcePixelHeight;
        private byte[] _sourceFramePixels;
        private ImageBrush BackgroundImage;

        private async void SamplePresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            await UpdateBackground(false);
        }

        private async Task UpdateImageSource()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(ImageSource));
            using(IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                PixelDataProvider framePixelProvider = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, new BitmapTransform(), ExifOrientationMode.RespectExifOrientation, ColorManagementMode.DoNotColorManage);

                _sourcePixelWidth = (int)decoder.PixelWidth;
                _sourcePixelHeight = (int)decoder.PixelHeight;
                _sourceFramePixels = framePixelProvider.DetachPixelData();
            }
        }

        private WriteableBitmap bitmap;

        private async Task UpdateBackground(bool forcedRedraw)
        {
            if(BackgroundImage == null)
                return;
            var width = Convert.ToInt32(Math.Ceiling(this.ActualWidth / _sourcePixelWidth));
            var height = Convert.ToInt32(Math.Ceiling(this.ActualHeight / _sourcePixelHeight));
            if(width <= 0 || height <= 0)
            {
                BackgroundImage.ImageSource = null;
                return;
            }
            var drawHeight = height * _sourcePixelHeight;
            var drawWidth = width * _sourcePixelWidth;
            if(!forcedRedraw && bitmap != null && bitmap.PixelHeight >= drawHeight && bitmap.PixelWidth >= drawWidth)
                return;
            if(_sourceFramePixels == null)
                await UpdateImageSource();
            bitmap = new WriteableBitmap(drawWidth, drawHeight);
            using(var targetStream = bitmap.PixelBuffer.AsStream())
            {
                int currentSourceY = 0;
                for(int targetY = 0; targetY < drawHeight; targetY++)
                {
                    var offset = currentSourceY * _sourcePixelWidth * 4;
                    for(int targetX = 0; targetX < width; targetX++)
                    {
                        targetStream.Write(_sourceFramePixels, offset, _sourcePixelWidth * 4);
                    }
                    currentSourceY = (currentSourceY + 1) % _sourcePixelHeight;
                }
            }
            BackgroundImage.ImageSource = bitmap;
        }
    }
}