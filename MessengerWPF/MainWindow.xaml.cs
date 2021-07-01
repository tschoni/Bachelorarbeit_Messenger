using MessengerWPF.Business;
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

namespace MessengerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static SignalRClient signalRClient = new SignalRClient();

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (!signalRClient.IsConnected)
            {
                signalRClient.StartAsync().GetAwaiter().GetResult();
            }

            TextBox1.Text = signalRClient.IsConnected.ToString();
        }
    }
}
