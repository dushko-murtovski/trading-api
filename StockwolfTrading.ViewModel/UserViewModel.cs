using System;
using System.Collections.Generic;
using System.Text;

namespace StockwolfTrading.ViewModel
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
    }

    public class UserActivationViewModel
    {
        public string vcode { get; set; }

    }
}
