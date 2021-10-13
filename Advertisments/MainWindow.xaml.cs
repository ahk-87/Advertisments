using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;

namespace Advertisments
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool activated = false;
        WindowVideo win = new WindowVideo();
        public MainWindow()
        {
            InitializeComponent();
            
            //BitmapImage bi = new BitmapImage(new Uri("Dollars Alfa .PNG", UriKind.Relative));
            //int stride = (int)bi.PixelWidth * (bi.Format.BitsPerPixel / 8);
            //byte[] pixels = new byte[(int)bi.PixelHeight * stride];
            //bi.CopyPixels(pixels, stride, 0);
            //for (int i = 0; i < pixels.Length; i++)
            //{
            //    if ((i + 1) % 4 == 0) continue;
            //    pixels[i] = (byte)(0xFF - pixels[i]);
            //}

            //WriteableBitmap bi2 = new WriteableBitmap(bi);
            //bi2.WritePixels(new Int32Rect(0, 0, bi.PixelWidth, bi.PixelHeight), pixels, stride, 0);
            //PngBitmapEncoder encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(bi2));
            //FileStream stream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\inverted.png", FileMode.Create);
            //encoder.Save(stream);
            //stream.Dispose();

            slider.Value = (int)Properties.Settings.Default.Max;
            activated = (bool)Properties.Settings.Default.Activated;
            if (!activated)
            {
                try
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Ads Activation.txt";
                    string s = File.ReadAllText(path);
                    var crypto = System.Security.Cryptography.MD5.Create();
                    var data = crypto.ComputeHash(Encoding.UTF8.GetBytes(s));
                    var sBuilder = new StringBuilder();

                    // Loop through each byte of the hashed data
                    // and format each one as a hexadecimal string.
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }
                    if (sBuilder.ToString() == "4989045782be9b559a0976d8cb068885")
                    {
                        File.Delete(path);
                        activated = true;
                        Properties.Settings.Default.Activated = true;
                        Properties.Settings.Default.Save();

                    }
                }
                catch (Exception)
                {
                    activated = false;
                }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (!activated)
            {
                textError.Visibility = Visibility.Visible;
                return;
            }
            if (win.IsActive) return;
            var ss = Screen.AllScreens;
            double dpi = ss[0].Bounds.Width / SystemParameters.PrimaryScreenWidth;
            if (ss.Length == 2)
            {
                var workingArea = ss[1].Bounds;
                win.Left = workingArea.Left / dpi;
                win.Top = workingArea.Top;
                win.Height = workingArea.Height / dpi;
                win.Width = workingArea.Width / dpi;

            }
            win.Tag = dpi;
            win.Show();

            frame.Fill = new VisualBrush(win.GetMedia());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            win.Close();
            Properties.Settings.Default.Max = slider.Value;
            Properties.Settings.Default.Save();

        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            win.ChangeMax((int)slider.Value);
        }

        async private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(15000);
            button_Click(null, null);
        }
    }
}
