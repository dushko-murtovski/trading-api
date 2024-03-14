using System;
using System.Collections.Generic;


namespace StockwolfTrading.ViewModel.Auth
{
    public class AuthenticateResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public bool? IsFirstLogin { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int ProductId { get; set; }
        public DateTime DateAdded { get; set; }
        public List<string> Langs { get; set; }
    }
}
