using System;
using System.Collections;

namespace Common
{
    [Serializable]
    public class User
    {
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public bool LoggedIn { get; set; }
        public ArrayList SaleOrders { get; set; }
        public ArrayList BuyOrders { get; set; }

        public User(string name, string nickname, string password)
        {
            Name = name;
            Nickname = nickname;
            Password = password;
            LoggedIn = false;
            SaleOrders = new ArrayList();
            BuyOrders = new ArrayList();
        }

        public void AddSaleOrder(int quantity)
        {
            SaleOrders.Add(new SaleOrder(quantity));
        }

        public void AddBuyOrder(int quantity)
        {
            SaleOrders.Add(new SaleOrder(quantity));
        }

        public override string ToString()
        {
            return "User's" + " " + "nickname: " + Nickname;
        }
    }
}

