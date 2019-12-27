using System.Windows;
using MahApps.Metro.Controls;

namespace WPF_VS_Theme_Template
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuExit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuAboutButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This is a empty template project for a WPF window with a theme mimicking VS2019.", "About");
        }
    }
}
