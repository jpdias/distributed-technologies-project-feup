using System;
using System.Collections.Generic;
using System.Windows;
using Common;

namespace DESClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _username;
        private string _password;
        private User loggedUser;

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void LoadValues()
        {
            Quantity.Text = App.IDes.GetDiginotes(ref loggedUser).Count.ToString();
            StockVal.Text = App.IDes.GetQuote().ToString();
            List<SaleOrder> saleOrders = App.IDes.GetSaleOrders(ref loggedUser);
            sell_list.ItemsSource = saleOrders;
            List<BuyOrder> buyOrders = App.IDes.GetBuyOrders(ref loggedUser);
            buy_list.ItemsSource = buyOrders;
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
                    loggedUser = App.IDes.GetUser(user);
                    LoadValues();
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
                        loggedUser = App.IDes.GetUser(user);
                        LoadValues();
                        
                    }
                    else
                        infoboxreg.Text = status;
                    
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
            //idEdited quantEdited valueEdited statusEdited sellValEdited
            BuyOrder current = (BuyOrder) e.Source;
            idEdited.Text = current.Id.ToString();
        }

        private void Sell_OnClick(object sender, RoutedEventArgs e)
        {
            //idEdited quantEdited valueEdited statusEdited sellValEdited
            SaleOrder current = (SaleOrder)e.Source;
            idEdited.Text = current.Id.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadValues();
        }

        private void Add_Sell_Click(object sender, RoutedEventArgs e)
        {
            string result = App.IDes.AddSaleOrder(ref loggedUser, Convert.ToInt32(Sell_Val.Text));
            InfoBox_Dash.Text = result;
        }

        private void Add_Buy_Click(object sender, RoutedEventArgs e)
        {
            string result = App.IDes.AddBuyOrder(ref loggedUser, Convert.ToInt32(BuyVal.Text));
            InfoBox_Dash.Text = result;
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
