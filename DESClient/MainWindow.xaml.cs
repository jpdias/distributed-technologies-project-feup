using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common;

namespace DESClient
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static AlterEventRepeater _evRepeater;
        private string _password;
        private string _username;
        private User loggedUser;
        private static bool status = false;
        public MainWindow()
        {
            InitializeComponent();
            _evRepeater = new AlterEventRepeater();
            _evRepeater.alterEvent += DoAlterations;
            App.IDes.alterEvent += _evRepeater.Repeater;
        }


        public void DoAlterations(DES.Operation op)
        {
            if (op == DES.Operation.Change || op == DES.Operation.Add)
                Dispatcher.Invoke(LoadValues);
            else if (op == DES.Operation.StartSuspension)
            {
                
                if (App.IDes.GetSaleOrders(ref loggedUser).Any(order => !order.Processed))
                {

                    alertChange(true);
                    status = true;
                }
                else if (App.IDes.GetBuyOrders(ref loggedUser).Any(order => !order.Processed))
                {
                    alertChange(true);
                    status = true;
                }
            }
            else if (op == DES.Operation.EndSuspension && status)
            {
                alertChange(false);
                status = false;
               
            }
        }

        private void changeStatus()
        {
            MarketStatus.Text = status ? "Normal" : "Suspended";
        }

        private void LoadValues()
        {
            Quantity.Text = App.IDes.GetDiginotes(ref loggedUser).Count.ToString();
            StockVal.Text = App.IDes.GetQuote().ToString();
            var saleOrders = App.IDes.GetSaleOrders(ref loggedUser);
            sell_list.ItemsSource = saleOrders;
            var buyOrders = App.IDes.GetBuyOrders(ref loggedUser);
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
            if (passwordR != null && name != null && password != null && user != null && user.Length >= 6 &&
                password.Length >= 6 && name.Length >= 6 && passwordR.Length == password.Length)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadValues();
        }

        private void Add_Sell_Click(object sender, RoutedEventArgs e)
        {
            double value;
            string result;
            if (Double.TryParse(Sell_Val.Text, out value))
            {
                result = App.IDes.AddSaleOrder(ref loggedUser, Convert.ToInt32(value));
                InfoBox_Dash.Text = result;
            }
            else
            {
                InfoBox_Dash.Text = "Check your input value.";
            }
        }

        private void Add_Buy_Click(object sender, RoutedEventArgs e)
        {
            double value;
            string result;
            if (Double.TryParse(BuyVal.Text, out value))
            {
                result = App.IDes.AddBuyOrder(ref loggedUser, Convert.ToInt32(value));
                InfoBox_Dash.Text = result;
            }
            else
            {
                InfoBox_Dash.Text = "Check your input value.";
            }
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            double value;
            string result;
            if (Double.TryParse(idEdited.Text, out value))
            {
                result = typeofOp.Text == "Sell"
                    ? App.IDes.EditSaleOrder(Convert.ToInt32(value), Convert.ToSingle(valEdited.Text))
                    : App.IDes.EditBuyOrder(Convert.ToInt32(idEdited.Text), Convert.ToSingle(valEdited.Text));
                InfoBox_Dash.Text = result;
            }
            else
            {
                InfoBox_Dash.Text = "Check your input value.";
            }
        }

        private void buy_list_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Update.Visibility = Visibility.Visible;
                var selected = sender as DataGrid;
                //idEdited quantEdited valueEdited statusEdited valEdited
                var current = (BuyOrder) selected.CurrentItem;
                idEdited.Text = current.Id.ToString();
                quantEdited.Text = current.Quantity.ToString();
                valueEdited.Text = current.Value.ToString();
                statusEdited.Text = current.Processed.ToString();
                valEdited.Text = current.Value.ToString();
                typeofOp.Text = "Buy";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void sell_list_click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Update.Visibility = Visibility.Visible;
                //idEdited quantEdited valueEdited statusEdited valEdited
                var selected = sender as DataGrid;
                var current = (SaleOrder) selected.CurrentItem;
                idEdited.Text = current.Id.ToString();
                quantEdited.Text = current.Quantity.ToString();
                valueEdited.Text = current.Value.ToString();
                statusEdited.Text = current.Processed.ToString();
                valEdited.Text = current.Value.ToString();
                typeofOp.Text = "Sell";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void deleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if(typeofOp.Text == "Sell")
                App.IDes.RemoveSaleOrder(Convert.ToInt32(idEdited.Text));
            else
                App.IDes.RemoveBuyOrder(Convert.ToInt32(idEdited.Text));

            Update.Visibility = Visibility.Hidden;
        }

        private void alertChange(bool status)
        {
            string alert;
            Dispatcher.Invoke(changeStatus);
            if (status)
                alert = "Market Quote changed. \nYou have 1 minute to change your orders. \nMarket Status: Suspended.";
            else
            {
                alert = "Time to change ended. \nMarket Status: Normal";
            }
            var result = MessageBox.Show(alert, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}