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
using System.Windows.Shapes;
using MessengerWPF.ViewModels;

namespace MessengerWPF.Views
{
    /// <summary>
    /// Interaktionslogik für MessengerView.xaml
    /// </summary>
    public partial class MessengerView : UserControl//Window
    {
        public MessengerView()
        {
            InitializeComponent();
            //this.DataContext = new MessengerViewModel();
        }
    }
}
