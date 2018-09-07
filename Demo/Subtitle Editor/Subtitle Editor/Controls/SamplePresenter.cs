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
                this.SizeChanged += new SizeChangedEventHandler(this.SamplePresenter_SizeChanged);
            };
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.BackgroundImage = (ImageBrush)this.GetTemplateChild("BackgroundImage");
            if(this.BackgroundImage != null)
                this.BackgroundImage.Stretch = Windows.UI.Xaml.Media.Stretch.None;
        }

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(SamplePresenter), new PropertyMetadata(null, new PropertyChangedCallback(OnImageSourceUriChanged)));

        public string ImageSource
        {
            get => (string)this.GetValue(ImageSourceProperty);
            set => this.SetValue(ImageSourceProperty, value);
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
            await this.UpdateBackground(false);
        }

        private async Task UpdateImageSource()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(this.ImageSource));
            using(IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                PixelDataProvider framePixelProvider = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, new BitmapTransform(), ExifOrientationMode.RespectExifOrientation, ColorManagementMode.DoNotColorManage);

                this._sourcePixelWidth = (int)decoder.PixelWidth;
                this._sourcePixelHeight = (int)decoder.PixelHeight;
                this._sourceFramePixels = framePixelProvider.DetachPixelData();
            }
        }

        private WriteableBitmap bitmap;

        private async Task UpdateBackground(bool forcedRedraw)
        {
            if(this.BackgroundImage == null)
                return;
            var width = Convert.ToInt32(Math.Ceiling(this.ActualWidth / this._sourcePixelWidth));
            var height = Convert.ToInt32(Math.Ceiling(this.ActualHeight / this._sourcePixelHeight));
            if(width <= 0 || height <= 0)
            {
                this.BackgroundImage.ImageSource = null;
                return;
            }
            var drawHeight = height * this._sourcePixelHeight;
            var drawWidth = width * this._sourcePixelWidth;
            if(!forcedRedraw && this.bitmap != null && this.bitmap.PixelHeight >= drawHeight && this.bitmap.PixelWidth >= drawWidth)
                return;
            if(this._sourceFramePixels == null)
                await this.UpdateImageSource();
            this.bitmap = new WriteableBitmap(drawWidth, drawHeight);
            using(var targetStream = this.bitmap.PixelBuffer.AsStream())
            {
                int currentSourceY = 0;
                for(int targetY = 0; targetY < drawHeight; targetY++)
                {
                    var offset = currentSourceY * this._sourcePixelWidth * 4;
                    for(int targetX = 0; targetX < width; targetX++)
                    {
                        targetStream.Write(this._sourceFramePixels, offset, this._sourcePixelWidth * 4);
                    }
                    currentSourceY = (currentSourceY + 1) % this._sourcePixelHeight;
                }
            }
            this.BackgroundImage.ImageSource = this.bitmap;
        }
    }
}