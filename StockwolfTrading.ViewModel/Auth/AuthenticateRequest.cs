using System.ComponentModel.DataAnnotations;

namespace StockwolfTrading.ViewModel.Auth
{
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
