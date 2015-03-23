using System;
using System.Collections;
using System.Collections.Generic;

namespace Common
{
    [Serializable]
    public class User
    {
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public bool LoggedIn { get; set; }
        public List<SaleOrder> SaleOrders { get; set; }
        public List<BuyOrder> BuyOrders { get; set; }

        public User(string name, string nickname, string password)
        {
            Name = name;
            Nickname = nickname;
            Password = password;
            LoggedIn = false;
            SaleOrders = new List<SaleOrder>();
            BuyOrders = new List<BuyOrder>();
        }

        public void AddSaleOrder(int quantity)
        {
            SaleOrders.Add(new SaleOrder(quantity));
        }

        public void EditSaleOrder(int orderIndex, int quantity)
        {
            SaleOrders[orderIndex].Quantity = quantity;
        }

        public void AddBuyOrder(int quantity)
        {
            SaleOrders.Add(new SaleOrder(quantity));
        }

        public void EditBuyOrder(int orderIndex, int quantity)
        {
            BuyOrders[orderIndex].Quantity = quantity;
        }

        public override string ToString()
        {
            return "User's" + " " + "nickname: " + Nickname;
        }
    }
}

