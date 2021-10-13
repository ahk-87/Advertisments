using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        string linesAvailable = "false";

        string timeShopOpen = "";

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

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += Timer_Tick;
            timer.Start();

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

        bool videoEnded = false;
        TimeSpan timeSpanEndVideo = new TimeSpan();
        int powerOff = 0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (image2.Visibility == Visibility.Visible)
            {
                TimeSpan timeClose = TimeSpan.Parse(timeShopOpen);
                if (DateTime.Now.Hour == (timeClose.Hours > 10 ? timeClose.Hours : timeClose.Hours + 12) && DateTime.Now.Minute > timeClose.Minutes)
                {
                    string[] lines = File.ReadAllLines("Ads\\prices.txt");
                    Dictionary<string, string> prices = new Dictionary<string, string>();
                    foreach (string l in lines)
                    {
                        string[] values = l.Split(new char[] { '=' });
                        prices.Add(values[0], values[1]);
                    }
                    prices["reopen"] = "";
                    StringBuilder sb = new StringBuilder();
                    foreach (KeyValuePair<string, string> k in prices)
                    {
                        sb.AppendLine(k.Key + "=" + k.Value);
                    }
                    File.WriteAllText("Ads\\prices.txt", sb.ToString());

                    image2.Visibility = Visibility.Hidden;
                    timeShopOpen = "";
                    textTimeOpen.Visibility = Visibility.Hidden;
                }
                return;
            }

            if (timeShopOpen != "")
            {
                image2.Source = new BitmapImage(new Uri("timeOpen.PNG", UriKind.RelativeOrAbsolute));
                image2.Visibility = Visibility.Visible;
                textTimeOpen.Text = timeShopOpen;
                textTimeOpen.Visibility = Visibility.Visible;

                return;
            }

            if (video.Visibility == Visibility.Visible)
            {
                if (timeSpanEndVideo.Seconds > 1 ? video.Position > timeSpanEndVideo : videoEnded)
                {
                    video.Visibility = Visibility.Collapsed;
                    video.Stop();
                    video.MediaEnded -= Video_MediaEnded;
                    videoEnded = false;
                    timeSpanEndVideo = new TimeSpan();
                    progressValue = 0;
                }
                return;
            }

            if (progressValue == 0)
            {
                hideAllTexts();
                getPrices();

                images = Directory.GetFiles("Ads").ToList();
                images = images.Where(s => s.EndsWith("mp4") | s.EndsWith("png", true, CultureInfo.CurrentCulture)
                    | s.EndsWith("jpg", true, CultureInfo.CurrentCulture) | s.EndsWith("jpeg", true, CultureInfo.CurrentCulture)).ToList();

                if (imageIndex >= images.Count)
                    imageIndex = 0;
                

                if (images[imageIndex].EndsWith("mp4"))
                {
                    string duration = images[imageIndex];
                    var match = Regex.Match(duration, "\\(\\d{1,3}\\)");
                    if (match.Success)
                        timeSpanEndVideo = new TimeSpan(0, 0, -1 * int.Parse(match.Value, NumberStyles.AllowParentheses));
                    else
                        video.MediaEnded += Video_MediaEnded;
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

                var ss = System.Windows.Forms.Screen.AllScreens;
                if (ss.Length == 1)
                {
                    powerOff = 3;
                    WindowState = WindowState.Minimized;
                }
                if (powerOff > 0)
                {
                    if (ss.Length == 2)
                    {
                        if (--powerOff == 0)
                        {
                            double dpi = ss[0].Bounds.Width / SystemParameters.PrimaryScreenWidth;
                            var workingArea = ss[1].Bounds;
                            Left = workingArea.Left / dpi;
                            Top = workingArea.Top;
                            WindowState = WindowState.Maximized;
                        }
                    }
                }
            }

            if (images[imageIndex].Contains("alfa") && progressValue > max / 2 && progressValue % 3 == 0)
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
                if (inverted != null)
                    inverted = null;
                progressValue = 0;
                imageIndex++;

                //RenderTargetBitmap image2 = new RenderTargetBitmap((int)imageCards.RenderSize.Width, (int)imageCards.RenderSize.Height, 96, 96, PixelFormats.Default);
                ////grid1.UpdateLayout();
                //image2.Render(grid1);
                //imageCards.Source =image2;
            }
        }

        private void Video_MediaEnded(object sender, RoutedEventArgs e)
        {
            videoEnded = true;
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
                textAyamTouchPrice.Visibility = Visibility.Visible;
            }
            else if (fileName.Contains("dolar alf 10") || fileName.Contains("dolar alfa 10"))
            {
                text10DolarAlfaPrice.Visibility = Visibility.Visible;
            }
            else if (fileName.Contains("ayam alfa") || fileName.Contains("ayam alf"))
            {
                textAyamAlfaPrice.Visibility = Visibility.Visible;
            }
            else if (fileName.Contains("touch line"))
            {
                textTouchLinePrice.Visibility = Visibility.Visible;
            }
        }

        void hideAllTexts()
        {
            textPubgPrice.Visibility = Visibility.Hidden;
            textAyam10DaysPrice.Visibility = Visibility.Hidden;
            textStartPrice2.Visibility = Visibility.Hidden;
            text10DolarTouchPrice.Visibility = Visibility.Hidden;
            textAyamTouchPrice.Visibility = Visibility.Hidden;
            text10DolarAlfaPrice.Visibility = Visibility.Hidden;
            textAyamAlfaPrice.Visibility = Visibility.Hidden;
            textTouchLinePrice.Visibility = Visibility.Hidden;
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
            textAyamTouchPrice.Text = prices["ayamTouch"];
            textAyamAlfaPrice.Text = prices["ayamAlfa"];
            timeShopOpen = prices["reopen"];
            textAyamTouchUpper.Text = prices["ayamTouch"];
            textAyamAlfaUpper.Text = prices["ayamAlfa"];
            textTouchLinePrice.Text = prices["lineTouch"];
            linesAvailable = prices["linesAvailable"];

            if (prices["bannerAd"] == "alfa")
            {
                textBannerAyyam.Text = "شهر ايام الفا + 1.9$";
                textBannerPrice.Text = prices["ayamAlfa"];
                imageBannerLogo.Source = new BitmapImage(new Uri("alfaLogo.png", UriKind.Relative));
            }
            else if (prices["bannerAd"] == "syria")
            {

                textBannerAyyam.Text = "١٠٠٠ رصيد سوري (تحويل سريع)  ";
                textBannerPrice.Text = prices["syria1000"];
                imageBannerLogo.Source = new BitmapImage(new Uri("mtnSyria.PNG", UriKind.Relative));
            }
            else if (prices["bannerAd"] == "freefire")
            {
                textBannerAyyam.Text = "تشريج 110 Diamonds";
                textBannerPrice.Text = prices["freefire110gems"];
                imageBannerLogo.Source = new BitmapImage(new Uri("freefire.PNG", UriKind.Relative));

            }
            else
            {

                textBannerAyyam.Text = "شهر ايام تاتش + 1.8$";
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
