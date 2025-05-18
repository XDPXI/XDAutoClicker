using System;
using System.Windows;
using System.Windows.Input;

namespace xdautoclicker
{
    public partial class MainWindow : Window
    {
        private readonly string _appVersion = "3.0.0";

        public MainWindow()
        {
            InitializeComponent();
            VersionText.Text = $"v{_appVersion}";
        }

        private void Button1_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button3_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button4_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}