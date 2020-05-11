using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Advertisments
{
    /// <summary>
    /// Interaction logic for WindowVideo.xaml
    /// </summary>
    public partial class WindowVideo : Window
    {
        List<string> images = new List<string>();
        WriteableBitmap inverted;
        BitmapSource original;

        int imageIndex = 0;
        int progressValue = 0;
        int max = 50;

        DoubleAnimation FadingIn, FadingOut;
        bool fading = false;
        int fadingInt = 0;

        string bannerAd = "";

        public WindowVideo()
        {
            InitializeComponent();
        }

        List<Grid> grids = new List<Grid>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            getPrices();
            rotate10Days.CenterX = textAyam10DaysPrice.ActualWidth / 2;
            rotate10Days.CenterY = textAyam10DaysPrice.ActualHeight / 2;

            //grids.Add(grid1);
            //grids.Add(grid2);
            //grids.Add(grid3);
            //grids.Add(grid4);
            //grids.Add(grid5);
            //images = Directory.GetFiles("Ads", "*.png");


            //MediaTimeline mt = new MediaTimeline(new Uri(path, UriKind.RelativeOrAbsolute));

            //MediaClock mc = mt.CreateClock();
            //mediaElement.Source = new Uri(path, UriKind.RelativeOrAbsolute);
            //mediaElement.Volume = 0.1;
            //mediaElement.LoadedBehavior = MediaState.Manual;
            //mediaElement.Play();
            //mediaElement.Clock.CurrentTimeInvalidated += Clock_CurrentTimeInvalidated;
            //mediaElement.Clock = mc;
            //mc.CurrentTimeInvalidated += Clock_CurrentTimeInvalidated;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += Timer_Tick;
            timer.Start();

            DispatcherTimer timerOneCard = new DispatcherTimer();
            timerOneCard.Interval = new TimeSpan(0, 0, 0, 10, 0);
            timerOneCard.Tick += TimerOneCard_Tick; ;
            timerOneCard.Start();

            DoubleAnimation da = new DoubleAnimation();
            da.By = 50;
            da.Duration = new Duration(TimeSpan.FromSeconds(1.5));
            da.AutoReverse = true;
            da.RepeatBehavior = RepeatBehavior.Forever;
            textBannerPrice.BeginAnimation(Button.FontSizeProperty, da);

            DoubleAnimation da2 = new DoubleAnimation();
            da.By = 70;
            da.Duration = new Duration(TimeSpan.FromSeconds(1.5));
            da.AutoReverse = true;
            da.BeginTime = TimeSpan.FromSeconds(1.5);
            da.RepeatBehavior = RepeatBehavior.Forever;
            imageBannerLogo.BeginAnimation(Image.HeightProperty, da);

            FadingIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(2.5));
            FadingOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1.5));

        }

        private void TimerOneCard_Tick(object sender, EventArgs e)
        {
            if (!fading)
            {
                switch (fadingInt)
                {
                    case 0:
                        OneCardImage.Source = new BitmapImage(new Uri("touchSmall.jpg", UriKind.Relative));
                        OneCardTextInfo.Text = "تاتش صغير";
                        OneCardTextPrice.Text = textTouchSmallPrice.Text;
                        break;
                    case 1:
                        OneCardImage.Source = new BitmapImage(new Uri("touchBig.jpg", UriKind.Relative));
                        OneCardTextInfo.Text = "تاتش كبير";
                        OneCardTextPrice.Text = textTouchBigPrice.Text;
                        break;
                    case 2:
                        OneCardImage.Source = new BitmapImage(new Uri("start.jpg", UriKind.Relative));
                        OneCardTextInfo.Text = "ســــتارت";
                        OneCardTextPrice.Text = textStartPrice.Text;
                        break;
                    case 3:
                        OneCardImage.Source = new BitmapImage(new Uri("alfaSmall.jpg", UriKind.Relative));
                        OneCardTextInfo.Text = "ألفا صغير";
                        OneCardTextPrice.Text = textAlfaSmallPrice.Text;
                        break;
                    case 4:
                        OneCardImage.Source = new BitmapImage(new Uri("alfaBig.jpg", UriKind.Relative));
                        OneCardTextInfo.Text = "ألفا كبير";
                        OneCardTextPrice.Text = textAlfaBigPrice.Text;
                        break;
                    case 5:
                        OneCardImage.Source = new BitmapImage(new Uri("start.jpg", UriKind.Relative));
                        OneCardTextInfo.Text = "ســــتارت";
                        OneCardTextPrice.Text = textStartPrice.Text;
                        break;
                }
                GridOneCard.BeginAnimation(FrameworkElement.OpacityProperty, FadingIn);
                fading = true;
            }
            else
            {
                GridOneCard.BeginAnimation(FrameworkElement.OpacityProperty, FadingOut);
                fading = false;
                if (++fadingInt == 6) fadingInt = 0;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (video.Visibility == Visibility.Visible)
            {
                if (video.Position > new TimeSpan(0, 1, 11))
                {
                    video.Visibility = Visibility.Collapsed;
                    video.Stop();
                    progressValue = 0;
                }
                return;
            }

            if (progressValue == 0)
            {
                hideAllTexts();
                getPrices();

                images = Directory.GetFiles("Ads").ToList();
                images = images.Where(s => s.EndsWith("mp4") | s.EndsWith("png", true, CultureInfo.CurrentCulture)).ToList();

                if (imageIndex >= images.Count)
                    imageIndex = 0;

                if (images[imageIndex].EndsWith("mp4"))
                {
                    video.Visibility = Visibility.Visible;
                    video.Source = new Uri(images[imageIndex], UriKind.Relative);
                    video.Play();
                    imageIndex++;
                    image.Fill = Brushes.White;
                    return;
                }

                showSpecificPriceText(images[imageIndex]);

                BitmapImage bi = new BitmapImage(new Uri(images[imageIndex], UriKind.Relative));
                if (images[imageIndex].Contains("alfa"))
                {
                    original = bi;
                    int stride = bi.PixelWidth * (bi.Format.BitsPerPixel / 8);
                    byte[] pixels = new byte[bi.PixelHeight * stride];
                    bi.CopyPixels(pixels, stride, 0);
                    for (int i = 0; i < pixels.Length; i++)
                    {
                        if ((i + 1) % 4 == 0) continue;
                        pixels[i] = (byte)(0xFF - pixels[i]);
                    }

                    inverted = new WriteableBitmap(bi);
                    inverted.WritePixels(new Int32Rect(0, 0, bi.PixelWidth, bi.PixelHeight), pixels, stride, 0);
                }

                image.Fill = new ImageBrush(bi);
                //bi.StreamSource.Close();
            }

            if (progressValue > max / 2 && progressValue % 3 == 0 && images[imageIndex].Contains("alfa"))
            {
                if (progressValue % 6 == 0)
                    image.Fill = new ImageBrush(original);
                else
                    image.Fill = new ImageBrush(inverted);
            }

            progress.Value = progressValue * 100 / max;
            progressValue += 1;

            if (progressValue > max)
            {
                progressValue = 0;
                imageIndex++;

                //RenderTargetBitmap image2 = new RenderTargetBitmap((int)imageCards.RenderSize.Width, (int)imageCards.RenderSize.Height, 96, 96, PixelFormats.Default);
                ////grid1.UpdateLayout();
                //image2.Render(grid1);
                //imageCards.Source =image2;
            }
        }

        private void showSpecificPriceText(string fileName)
        {
            if (fileName.Contains("pubg"))
            {
                textPubgPrice.Visibility = Visibility.Visible;
            }
            else if (fileName.Contains("ayam 10 days"))
            {
                textAyam10DaysPrice.Visibility = Visibility.Visible;
            }
            else if (fileName.Contains("start"))
            {
                textStartPrice2.Visibility = Visibility.Visible;
            }
            else if (fileName.Contains("dolar touch 10"))
            {
                text10DolarTouchPrice.Visibility = Visibility.Visible;
            }
            else if (fileName.Contains("touch ayam"))
            {
                textTouchAyamPrice.Visibility = Visibility.Visible;
            }
            else if (fileName.Contains("dolar alf 10") || fileName.Contains("dolar alfa 10"))
            {
                text10DolarAlfaPrice.Visibility = Visibility.Visible;
            }
        }

        void hideAllTexts()
        {
            textPubgPrice.Visibility = Visibility.Hidden;
            textAyam10DaysPrice.Visibility = Visibility.Hidden;
            textStartPrice2.Visibility = Visibility.Hidden;
            text10DolarTouchPrice.Visibility = Visibility.Hidden;
            textTouchAyamPrice.Visibility = Visibility.Hidden;
            text10DolarAlfaPrice.Visibility = Visibility.Hidden;
        }

        private void Clock_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            //progress.Value = 100 * mediaElement.Clock.CurrentProgress.Value;
        }

        public Grid GetMedia()
        {
            return mainGrid;
        }
        public void ChangeMax(int newMax)
        {
            max = newMax;
        }

        void getPrices()
        {
            textDate.Text = DateTime.Now.ToString("dd-MM-yyyy");

            string[] lines = File.ReadAllLines("Ads\\prices.txt");
            Dictionary<string, string> prices = new Dictionary<string, string>();
            foreach (string l in lines)
            {
                string[] values = l.Split(new char[] { '=' });
                prices.Add(values[0], values[1]);
            }

            textAlfaBigPrice.Text = prices["alfaBig"];
            textAlfaSmallPrice.Text = prices["alfaSmall"];
            textTouchBigPrice.Text = prices["touchBig"];
            textTouchSmallPrice.Text = prices["touchSmall"];
            textStartPrice.Text = prices["start"];
            textStartPrice2.Text = prices["start"];
            textPubgPrice.Text = prices["pubg770Price"];
            textAyam10DaysPrice.Text = prices["ayam10Days"];
            text10DolarTouchPrice.Text = prices["10DolarTouch"];
            text10DolarAlfaPrice.Text = prices["10DolarAlfa"];
            textTouchAyamPrice.Text = prices["ayamTouch"];

            if (prices["bannerAd"] == "alfa")
            {
                textBannerAyyam.Text = "ايام الفا + 1.9$";
                textBannerPrice.Text = prices["ayamAlfa"];
                imageBannerLogo.Source = new BitmapImage(new Uri("alfaLogo.png", UriKind.Relative));
            }
            else
            {

                textBannerAyyam.Text = "ايام تاتش + 1.8$";
                textBannerPrice.Text = prices["ayamTouch"];
                imageBannerLogo.Source = new BitmapImage(new Uri("touchLogo.jpg", UriKind.Relative));
            }

            if (prices["lines"] == "")
            {
                GridLines.Visibility = Visibility.Collapsed;
                GridRechargeScaleY.ScaleY = 1;
            }
            else
            {
                try
                {
                    GridLines.Visibility = Visibility.Visible;
                    GridRechargeScaleY.ScaleY = 0.56;
                    string[] touchLines = prices["lines"].Split(new char[] { ',' });
                    string x = "";
                    foreach (string t in touchLines)
                    {
                        x = x + t + "\r\n";
                    }
                    TextLines.Text = x;
                }
                catch { }
            }
        }

    }
}
