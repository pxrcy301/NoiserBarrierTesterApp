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

namespace NoiseBarrierTesterAppV1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        public bool acknowledged = false;

        public PopupWindow(string popupMessage, string buttonText = "Acknowledge")
        {
            InitializeComponent();
            MessageTextBlock.Text = popupMessage;
            AcknowledgeBtn.Content = buttonText;

            this.Activate();
            this.Topmost = true;
            this.Topmost = false;
            this.Focus();


        }

        private void AcknowledgeBtn_Click(object sender, RoutedEventArgs e)
        {
            acknowledged = true;
            DialogResult = true;
        }
    }
}
