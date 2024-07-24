using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountList
{
    public class AccountList
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public List<string> groupIds;
        public AccountList() { }
        public AccountList(string account, string password)
        {
            this.Account = account;
            this.Password = password;
        }
    }
}
