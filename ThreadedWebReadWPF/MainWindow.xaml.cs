using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
            button_StopTask.IsEnabled = false;
        }

        private CancellationTokenSource m_cts = null;
        private WebClient wc = new WebClient();

        private void button_Click(object sender, RoutedEventArgs e)
        {
            button.IsEnabled = false;
            Task<string> tsk = wc.DownloadStringTaskAsync("http://localhost:9240/Slow.ashx");

            // FromCurrentSynchronizationContext() returns backs the context of the current thread which is the UI thread.
            TaskScheduler taskSchedular = TaskScheduler.FromCurrentSynchronizationContext();
            Task t2 = tsk.ContinueWith(t =>
            {
                // At this point we are not on main thread which is controlling the WPF-UI; we are on a worker thread.
                textBlock.Text = t.Result;
                button.IsEnabled = true;
            }, taskSchedular); // The UI context(by the help of task scheduler) is passes to t2 
            // thread so that it can change the UI. 

        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;
            
            string cont = await wc.DownloadStringTaskAsync("http://localhost:9240/Slow.ashx");

            // With async/await - after executioin of the worker thread we will automatically get back to the main WPF-UI thread
            button1.IsEnabled = true;
            textBlock.Text = cont;
        }

        private void button_TaskStart_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();

            Task<string> t1 = Task<string>.Factory.StartNew(() =>
            {
                using (WebClient wc = new WebClient())
                {
                    String content = wc.DownloadString(new Uri(@"http://localhost:9240/Slow.ashx"));
                    return content;
                }                
            });

            Task<string> t2 = Task<string>.Factory.StartNew(() =>
            {
                using (WebClient wc = new WebClient())
                {
                    String content = wc.DownloadString(new Uri(@"http://localhost:9240/Slow.ashx"));
                    return content;
                }
            });


            Task<string> t3 = Task<string>.Factory.StartNew(() =>
            {
                using (WebClient wc = new WebClient())
                {
                    //String content = wc.DownloadString(new Uri(@"http://localhost:9240/Faulty.ashx"));
                    String content = wc.DownloadString(new Uri(@"http://localhost:9240/Slow.ashx"));
                    return content;
                }
            });


            TaskScheduler taskSchedular = TaskScheduler.FromCurrentSynchronizationContext();
            var tasks = new List<Task<string>>() { t1,t2,t3 };
            // A thread that will be waiting for the fast success and update the UI
            Task updateUI = Task.Factory.ContinueWhenAny(tasks.ToArray(), (winner) =>
            {
                try
                {
                    listBox.Items.Add(winner.Result);
                }
                catch (AggregateException ae)
                {
                    listBox.Items.Add(ae.InnerException.Message);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            },CancellationToken.None,TaskContinuationOptions.None, taskSchedular);
            

        }

        private void button_StopTask_Click(object sender, RoutedEventArgs e)
        {
            m_cts.Cancel();
            button_StopTask.IsEnabled = false;

        }

        private void button_TaskStartAll_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
            button_StopTask.IsEnabled = false;

            Task<string> t1 = Task<string>.Factory.StartNew(() =>
            {
                using (WebClient wc = new WebClient())
                {
                    String content = wc.DownloadString(new Uri(@"http://localhost:9240/Slow.ashx"));
                    return content;
                }
            });

            // A cancelable task
            m_cts = new CancellationTokenSource();
            CancellationToken token = m_cts.Token;
            Task<string> t2 = Task<string>.Run(async ()=>
            {
                WebClient wc = new WebClient();
                token.Register(() => wc.CancelAsync());
                var t = await wc.DownloadStringTaskAsync(new Uri(@"http://localhost:9240/Slow.ashx"));
                return t;
            }, token);


            Task<string> t3 = Task<string>.Factory.StartNew(() =>
            {
                using (WebClient wc = new WebClient())
                {
                    String content = wc.DownloadString(new Uri(@"http://localhost:9240/Faulty.ashx"));
                    //String content = wc.DownloadString(new Uri(@"http://localhost:9240/Slow.ashx"));
                    return content;
                }
            });


            TaskScheduler taskSchedular = TaskScheduler.FromCurrentSynchronizationContext();
            var tasks = new List<Task<string>>() { t1, t2, t3 };
            // A thread that will be waiting for the fast success and update the UI
            Task updateUI = Task.Factory.ContinueWhenAll(tasks.ToArray(), (AllTasks) =>
            {
                try
                {
                    foreach (Task<string> t in AllTasks)
                    {
                        if (t.Exception == null)
                        {
                            listBox.Items.Add(t.Result);
                        }
                        else
                        {
                            Type tp = t.Exception.InnerException.GetType();

                            Console.WriteLine(tp);
                            if (t.Exception.InnerException != null)
                                listBox.Items.Add(t.Exception.InnerException.Message);
                            else
                                listBox.Items.Add("<<Error>>");
                        }

                    }
                }
                catch (AggregateException ae)
                {
                    listBox.Items.Add(ae.InnerException.Message);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }, CancellationToken.None, TaskContinuationOptions.None, taskSchedular);
            
        }

        private void button_TaskStartOneByOne_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
            button_StopTask.IsEnabled = true;

            Task<string> t1 = Task<string>.Factory.StartNew(() =>
            {
                using (WebClient wc = new WebClient())
                {
                    String content = wc.DownloadString(new Uri(@"http://localhost:9240/Slow.ashx"));
                    return content;
                }
            });

            // A cancelable task
            m_cts = new CancellationTokenSource();
            CancellationToken token = m_cts.Token;
            Task<string> t2 = Task<string>.Run(async () =>
            {
                WebClient wc = new WebClient();
                // Register a cancellation activity by the cancellation token
                token.Register(() => wc.CancelAsync() );
                var t = await wc.DownloadStringTaskAsync(new Uri(@"http://localhost:9240/Slow.ashx"));
                return t;
            }, token);


            Task<string> t3 = Task<string>.Factory.StartNew(() =>
            {
                using (WebClient wc = new WebClient())
                {
                    String content = wc.DownloadString(new Uri(@"http://localhost:9240/Faulty.ashx"));
                    return content;
                }
            });


            TaskScheduler taskSchedular = TaskScheduler.FromCurrentSynchronizationContext();
            var tasks = new List<Task<string>>() { t1, t2, t3 };
            // Create a continuation thread for all the code-thread and update the UI for all.
            foreach(var t in tasks)
            {
                t.ContinueWith((antecedent) =>
                {
                    try
                    {
                        listBox.Items.Add(antecedent.Result);
                    }
                    catch (AggregateException aex)
                    {
                        foreach (var exx in aex.InnerExceptions)
                        {
                            if (exx is OperationCanceledException)
                            {
                                listBox.Items.Add("<<Canceled>>");
                            }
                            else
                            {
                                listBox.Items.Add(exx.Message);
                            }
                        }
                        //if (aex.InnerException.InnerException is OperationCanceledException)
                        //    listBox.Items.Add("<<Canceled>>");
                        //else
                        //    listBox.Items.Add(aex.InnerException.Message);
                    }
                    catch (Exception ex)
                    {
                        listBox.Items.Add(ex.Message);
                    }
                }, taskSchedular);
            }
            

        }

    }
}
