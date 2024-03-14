using StockWolfTrading.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockWolfTrading.Services.Interfaces
{
    public interface IUserService
    {
        User GetUserById(int id);
        User GetUserByUsername(string username);
        User AddUser(User user);
        bool EditUser(User user);
        User GetUserByVerificationCode(string verificationCode);
        List<User> GetAllUsers();
    }
}
