using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace StockWolfTradingAPI.Provider
{
    public class JwtSecurityKey
    {
        public static SymmetricSecurityKey Create(string secret)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        }
    }
}
