using ChatClient.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ChatClient
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

        private void send_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel context = (MainWindowViewModel)DataContext;

            context.SendMessage(message_TextBox.Text);
        }

        private void connect_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel context = (MainWindowViewModel)DataContext;

            context.Connect(username_TextBox.Text);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            MainWindowViewModel context = (MainWindowViewModel)DataContext;

            context.Disconnect();
        }
    }
}
