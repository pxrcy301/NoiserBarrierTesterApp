﻿using System;
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

            StringPotConstTextBox.Text = MWR.testSystemProperties.defaultStringPotentiometerConstant.ToString();
        }

        private void DefaultBtn_Click(object sender, RoutedEventArgs e)
        {
            StringPotConstTextBox.Text = MWR.testSystemProperties.defaultStringPotentiometerConstant.ToString();
        }

        private void ApplyCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StringPotConstTextBox.Text == "")
            {
                return;
            }

            MWR.plc.UpdateStringPotentiometerConstant(float.Parse(StringPotConstTextBox.Text));
            this.Close();
        }

        private void StringPotConstTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.OemPeriod || e.Key == Key.Decimal) || ((e.Key == Key.OemPeriod || e.Key == Key.Decimal) && StringPotConstTextBox.Text.Contains('.')))
            {
                e.Handled = true;
            }
        }
    }
}
