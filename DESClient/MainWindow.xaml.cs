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

namespace DESClient
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

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            App.IDes.Login(Username.ToString(), Password.ToString());
        }

        private static int count = 0;
        private void Register_Click(object sender, RoutedEventArgs e)
        {
           
            if(count==0)
            {
                Login.Visibility = Visibility.Hidden;
                NameLabel.Visibility = Visibility.Visible;
                Name.Visibility = Visibility.Visible;
                count++;
            }
            else if(count==1)
            {
                var user = Username.ToString();
                var name = Name.ToString();
                var password = Password.ToString();
                App.IDes.AddUser(user,name,password);
                App.IDes.Login(user, password);
                count=0;
            }
        }
    }
}
