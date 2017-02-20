using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace ThreadedWebReadWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private WebClient wc = new WebClient();

        private void button_Click(object sender, RoutedEventArgs e)
        {
            button.IsEnabled = false;
            Task<string> tsk = wc.DownloadStringTaskAsync("http://localhost:9240/Slow.ashx");

            // FromCurrentSynchronizationContext() returns backs the context thread from where main thread can be returned.
            TaskScheduler taskSchedular = TaskScheduler.FromCurrentSynchronizationContext();

            tsk.ContinueWith(t =>
            {
                // At this point we are not on main thread which is controlling the WPF-UI; we are on a worker thread.
                textBlock.Text = t.Result;
                button.IsEnabled = true;
            }, taskSchedular);

        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;
            
            string cont = await wc.DownloadStringTaskAsync("http://localhost:9240/Slow.ashx");

            // With async/await - after executioin of the worker thread we will automatically get back to the main WPF-UI thread
            button1.IsEnabled = true;
            textBlock.Text = cont;
        }
    }
}
