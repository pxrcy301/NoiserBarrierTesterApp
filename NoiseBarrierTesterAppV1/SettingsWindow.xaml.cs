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
    public partial class SettingsWindow : Window
    {

        MainWindow MWR;

        public SettingsWindow(MainWindow mainWindowReference)
        {
            InitializeComponent();
            MWR = mainWindowReference;

            UpperStringPotConstTextBox.Text = MWR.testSystemProperties.upperStringPotentiometerConstant.ToString();
            LowerStringPotConstTextBox.Text = MWR.testSystemProperties.lowerStringPotentiometerConstant.ToString();
            LeftLoadCellConstTextBox.Text = MWR.testSystemProperties.leftLoadCellConstant.ToString();
            RightLoadCellConstTextBox.Text = MWR.testSystemProperties.rightLoadCellConstant.ToString();
            PTextBox.Text = MWR.testSystemProperties.KP.ToString();
            ITextBox.Text = MWR.testSystemProperties.KI.ToString();
            DTextBox.Text = MWR.testSystemProperties.KD.ToString();
        }

        private void DefaultBtn_Click(object sender, RoutedEventArgs e)
        {
            UpperStringPotConstTextBox.Text = MWR.testSystemProperties.defaultUpperStringPotentiometerConstant.ToString();
            LowerStringPotConstTextBox.Text = MWR.testSystemProperties.defaultLowerStringPotentiometerConstant.ToString();
            LeftLoadCellConstTextBox.Text = MWR.testSystemProperties.defaultLeftLoadCellConstant.ToString();
            RightLoadCellConstTextBox.Text = MWR.testSystemProperties.defaultRightLoadCellConstant.ToString();
            PTextBox.Text = MWR.testSystemProperties.defaultKP.ToString();
            ITextBox.Text = MWR.testSystemProperties.defaultKI.ToString();
            DTextBox.Text = MWR.testSystemProperties.defaultKD.ToString();
        }

        private void ApplyCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UpperStringPotConstTextBox.Text == "" || LowerStringPotConstTextBox.Text == "" || LeftLoadCellConstTextBox.Text == "" || RightLoadCellConstTextBox.Text == "")
            {
                return;
            }

            MWR.testSystemProperties.upperStringPotentiometerConstant = float.Parse(UpperStringPotConstTextBox.Text);
            MWR.testSystemProperties.lowerStringPotentiometerConstant= float.Parse(LowerStringPotConstTextBox.Text);
            MWR.testSystemProperties.leftLoadCellConstant = float.Parse(LeftLoadCellConstTextBox.Text);
            MWR.testSystemProperties.rightLoadCellConstant = float.Parse(RightLoadCellConstTextBox.Text);
            MWR.testSystemProperties.KP = float.Parse(PTextBox.Text);
            MWR.testSystemProperties.KI = float.Parse(ITextBox.Text);
            MWR.testSystemProperties.KD = float.Parse(DTextBox.Text);

            MWR.plc.UpdateUpperStringPotentiometerConstant(MWR.testSystemProperties.upperStringPotentiometerConstant);
            MWR.plc.UpdateLowerStringPotentiometerConstant(MWR.testSystemProperties.lowerStringPotentiometerConstant);
            MWR.plc.UpdateLeftLoadCellConstant(MWR.testSystemProperties.leftLoadCellConstant);
            MWR.plc.UpdateRightLoadCellConstant(MWR.testSystemProperties.rightLoadCellConstant);
            MWR.plc.UpdateKP(MWR.testSystemProperties.KP);
            MWR.plc.UpdateKI(MWR.testSystemProperties.KI);
            MWR.plc.UpdateKD(MWR.testSystemProperties.KD);

            this.Close();
        }

        private void UpperStringPotConstTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.OemPeriod || e.Key == Key.Decimal) || ((e.Key == Key.OemPeriod || e.Key == Key.Decimal) && UpperStringPotConstTextBox.Text.Contains('.')))
            {
                e.Handled = true;
            }
        }
        private void LowerStringPotConstTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.OemPeriod || e.Key == Key.Decimal) || ((e.Key == Key.OemPeriod || e.Key == Key.Decimal) && LowerStringPotConstTextBox.Text.Contains('.')))
            {
                e.Handled = true;
            }
        }
        private void LeftLoadCellConstTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.OemPeriod || e.Key == Key.Decimal) || ((e.Key == Key.OemPeriod || e.Key == Key.Decimal) && LeftLoadCellConstTextBox.Text.Contains('.')))
            {
                e.Handled = true;
            }
        }
        private void RightLoadCellConstTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.OemPeriod || e.Key == Key.Decimal) || ((e.Key == Key.OemPeriod || e.Key == Key.Decimal) && RightLoadCellConstTextBox.Text.Contains('.')))
            {
                e.Handled = true;
            }
        }

        private void PTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.OemPeriod || e.Key == Key.Decimal) || ((e.Key == Key.OemPeriod || e.Key == Key.Decimal) && PTextBox.Text.Contains('.')))
            {
                e.Handled = true;
            }
        }

        private void ITextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.OemPeriod || e.Key == Key.Decimal) || ((e.Key == Key.OemPeriod || e.Key == Key.Decimal) && ITextBox.Text.Contains('.')))
            {
                e.Handled = true;
            }
        }

        private void DTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.OemPeriod || e.Key == Key.Decimal) || ((e.Key == Key.OemPeriod || e.Key == Key.Decimal) && DTextBox.Text.Contains('.')))
            {
                e.Handled = true;
            }
        }


    }
}
