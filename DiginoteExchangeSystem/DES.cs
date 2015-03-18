using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiginoteExchangeSystem;

namespace DiginoteExchangeSystem {
    public class DES : MarshalByRefObject, IDES
    {
        ArrayList userList;
        public DES()
        {
            userList = new ArrayList();
            var user = new User("xpto1", "Peter", "xpto1");
            var user1 = new User("xpto2", "Peter", "xpto2");

            userList.Add(user);
            userList.Add(user1);
        }

        public Boolean Login(String nickname, String password)
        {
            foreach (User user in userList)
            {
                if (user.Name.Equals(nickname) && user.Password.Equals(password))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public interface IDES
    {
        Boolean Login(String nickname, String password);

    }
}
