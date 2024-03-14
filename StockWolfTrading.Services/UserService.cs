using StockWolfTrading.Core.Models;
using StockWolfTrading.DataModel.UnitOfWork;
using StockWolfTrading.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockWolfTrading.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public User GetUserById(int id) =>
            _unitOfWork.UserRepository.GetAll().FirstOrDefault(pu => pu.UserId == id);

        public User GetUserByUsername(string username) =>
            _unitOfWork.UserRepository.GetAll().FirstOrDefault(pu => pu.Email == username);

        public User GetUserByVerificationCode(string verificationCode) =>
            _unitOfWork.UserRepository.GetAll().FirstOrDefault(pu => pu.VerificationCode == verificationCode);

        public List<User> GetAllUsers() => _unitOfWork.UserRepository.GetAll().ToList();

        public User AddUser(User user)
        {
            try
            {
                var usr = _unitOfWork.UserRepository.Insert(user);
                _unitOfWork.Save();
                return usr;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool EditUser(User user)
        {
            try
            {
                _unitOfWork.UserRepository.Update(user);
                _unitOfWork.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //public User CheckUserCredentials(string username, string password)
        //{
        //    try
        //    {
        //        return _unitOfWork.UserRepository.GetAll()
        //        .FirstOrDefault(p => p.UserName == username && p.Password == password);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
