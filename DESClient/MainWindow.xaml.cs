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
        private string _username;
        private string _password;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var user = Username.Text;
            var password = Password.Password;
            if (user != null && password != null)
            {

                var status = App.IDes.Login(user, password);
                if (status == "Login successful!")
                {
                    _username = user;
                    _password = password;
                    register.Visibility = Visibility.Hidden;
                    login.Visibility = Visibility.Hidden;
                    main.Visibility = Visibility.Visible;
                    menu.Header = "Welcome " + user + "!";
                }
                else
                    infobox.Text = status;
            }
            else
            {
                infobox.Text = "Fields can't be empty";
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
           
            var user = UsernameNew.Text;
            var password = PasswordNew.Password;
            var passwordR = PasswordNewR.Password;
            var name = NameNew.Text;
            if (passwordR != null && name != null && password != null && user != null && user.Length >= 6 && password.Length >= 6 && name.Length >= 6 && passwordR.Length == password.Length)
            {
                if (password == passwordR)
                {
                    var status = App.IDes.AddUser(name, user, password);
                    if (status == "User added successfully!")
                    {
                        App.IDes.Login(user, password);
                        _username = user;
                        _password = password;
                        show_dashboard();
                        
                    }
                    else
                        infobox.Text = status;
                    
                }
                else
                {
                    infoboxreg.Text = "Passwords don't match.";
                }
            }
            else
            {
                infoboxreg.Text = "All fields must have more than 6 characters.";
            }

        }

        private void Registerbox_Click(object sender, RoutedEventArgs e)
        {
            login.Visibility = Visibility.Hidden;
            register.Visibility = Visibility.Visible;
        }

        private void LoginBack_Click(object sender, RoutedEventArgs e)
        {
            register.Visibility = Visibility.Hidden;
            login.Visibility = Visibility.Visible;
        }

        private void logout_click(object sender, RoutedEventArgs e)
        {
            var status = App.IDes.Logout(_username, _password);
         
            if (status.Equals("Logout successful!"))
            {
                main.Visibility = Visibility.Hidden;
                register.Visibility = Visibility.Hidden;
                login.Visibility = Visibility.Visible;
               
            }
        }

        private void show_dashboard()
        {
            register.Visibility = Visibility.Hidden;
            login.Visibility = Visibility.Hidden;
            main.Visibility = Visibility.Visible;
            menu.Header = "Welcome " + _username + "!";
            //offers;
        }

        private void Changepw_click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Buy_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Sell_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
