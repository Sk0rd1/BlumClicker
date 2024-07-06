using System;
using System.Collections.Generic;
using System.Linq;
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

namespace BlumClicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ClickEngine clickEngine;
        bool isActive = false;
        CancellationTokenSource cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();
            clickEngine = new ClickEngine();

            HotkeysManager.SetupSystemHook();

            GlobalHotkey firstHotkey = new GlobalHotkey(ModifierKeys.Control, Key.F2, StartHotkey);
            GlobalHotkey secondHotkey = new GlobalHotkey(ModifierKeys.Control, Key.F4, StopHotkey);
            HotkeysManager.AddHotkey(firstHotkey);
            HotkeysManager.AddHotkey(secondHotkey);

            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HotkeysManager.ShutdownSystemHook();
        }

        private void OnbClick(object sender, RoutedEventArgs e)
        {
            if(isActive)
            {
                bClick.Content = "Start(Ctrl+F2)";
                clickEngine.Stop();
                //cancellationTokenSource.Cancel();
            }
            else
            {
                bClick.Content = "Stop(Ctrl+F4)";
                clickEngine.Start();
                cancellationTokenSource = new CancellationTokenSource();
                //StartClickingAsync(cancellationTokenSource.Token);
            }
            isActive = !isActive;
        }

        public void StartHotkey()
        {
            if(!isActive)
            {
                bClick.Content = "Stop(Ctrl+F4)";
                clickEngine.Start();
                isActive = !isActive;
            }
        }

        public void StopHotkey()
        {
            if (isActive)
            {
                bClick.Content = "Start(Ctrl+F2)";
                clickEngine.Stop();
                isActive = !isActive;
            }
        }

        private async Task StartClickingAsync(CancellationToken cancellationToken)
        {
            try
            {
                clickEngine.Start();
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                clickEngine.Stop();

                Dispatcher.Invoke(() =>
                {
                    bClick.Content = "Start(Ctrl+F2)";
                    isActive = !isActive;
                });
            }
            catch (TaskCanceledException) 
            {

            }
        }
    }
}
