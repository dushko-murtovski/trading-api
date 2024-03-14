using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockwolfTrading.ViewModel;
using StockWolfTrading.Core.Models;
using StockWolfTrading.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using StockWolfTradingAPI.Common;
using StockWolfTradingAPI.Provider;
using System.Collections.Generic;
using StockwolfTrading.ViewModel.Auth;
using StockWolfTradingAPI.Authorization;
using AutoMapper;

namespace StockWolfTradingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IJwtUtils jwtUtils, IMapper mapper)
        {
            _userService = userService;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
        }

        /// <summary>
        ///     Authenticate user on log in 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var user = _userService.GetUserByUsername(model.Username);
            if (user is not { Active: true }) return Unauthorized();
            if (user.UserProducts == null || user.UserProducts.Count == 0)
            {
                UserProducts up = new UserProducts();
                up.ProductProductId = 1; //Demo product
                up.DateAdded = DateTime.Now;
                user.UserProducts = new List<UserProducts>() { up };
            }
            byte[] salt = user.Password.Take(32).ToArray();
            var pass_storage = user.Password.Skip(32).ToArray();
            var pass = Encoding.UTF8.GetBytes(model.Password);
            var deriveBytes = new Rfc2898DeriveBytes(pass, salt, 100000, HashAlgorithmName.SHA256);
            var bytes = deriveBytes.GetBytes(32);

            if (!pass_storage.SequenceEqual(bytes)) return Unauthorized();

            user.LastLogin = DateTime.Now;
            if (user.IsFirstLogin == true)
                user.IsFirstLogin = false;
            _userService.EditUser(user);
            var response = _mapper.Map<AuthenticateResponse>(user);
            var prod = user.UserProducts.OrderBy(p => p.DateAdded).First();
            response.ProductId = prod.ProductProductId;
            response.DateAdded = prod.DateAdded;
            response.Token = _jwtUtils.GenerateToken(user);
            return Ok(response);
        }

        [HttpPost("loadprofile")]
        public IActionResult LoadProfile([FromBody] JObject model)
        {
            var puser = JsonConvert.DeserializeObject<UserViewModel>(model.ToString());
            var user = _userService.GetUserByUsername(puser.Email);
            dynamic result = new System.Dynamic.ExpandoObject();
            result.firstname = user.FirstName;
            result.lastname = user.LastName;
            return Ok(result);
        }

        [HttpPost("updateprofile")]
        public IActionResult UpdateProfile([FromBody] JObject model)
        {
            var puser = JsonConvert.DeserializeObject<UserViewModel>(model.ToString());
            var user = _userService.GetUserByUsername(puser.Email);
            if (user != null)
            {
                user.FirstName = puser.FirstName;
                user.LastName = puser.LastName;
                user.Active = puser.Status == 1 ? true : false;
                _userService.EditUser(user);
                //dynamic result = new System.Dynamic.ExpandoObject();
                //result.firstname = user.FirstName;
                //result.lastname = user.LastName;
                return Ok(); // result);
                //Test for CI
            }

            return NotFound();
        }

        [HttpPost("updatepass")]
        public IActionResult UpdatePass([FromBody] JObject model)
        {
            var puser = JsonConvert.DeserializeObject<PassChangeViewModel>(model.ToString());
            var user = _userService.GetUserByUsername(puser.email);
            if (user != null)
            {
                var salt = user.Password.Take(32).ToArray();
                var pass_storage = user.Password.Skip(32).ToArray();
                var pass = Encoding.UTF8.GetBytes(puser.oldpass);
                byte[] bytes;
                var deriveBytes = new Rfc2898DeriveBytes(pass, salt, 100000, HashAlgorithmName.SHA256);
                bytes = deriveBytes.GetBytes(32);
                if (pass_storage.SequenceEqual(bytes))
                {
                    Random rnd = new Random();
                    Byte[] newsalt = new Byte[32];
                    rnd.NextBytes(newsalt);
                    var newpass = Encoding.UTF8.GetBytes(puser.password);
                    deriveBytes = new Rfc2898DeriveBytes(newpass, newsalt, 100000, HashAlgorithmName.SHA256);
                    bytes = deriveBytes.GetBytes(32);
                    var password = newsalt.Concat(bytes).ToArray();
                    user.Password = password;
                    _userService.EditUser(user);
                    dynamic result1 = new System.Dynamic.ExpandoObject();
                    result1.response = 0;
                    return Ok(result1);
                }
                
                dynamic result = new System.Dynamic.ExpandoObject();
                result.response = -1;
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPost("setfirstlogin")]
        public IActionResult SetFirsLogin([FromBody] JObject data)
        {
            var email = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.ToString());
            var user = _userService.GetUserByUsername(email["email"]);
            if (user == null)
                return NotFound();

            user.IsFirstLogin = false;
            _userService.EditUser(user);
            return Ok();
        }

        [HttpGet("getall")]
        public ActionResult GetAll()
        {
            try
            {
                var res = _userService.GetAllUsers();
                var usersList = res.ToList().Select(user => new UserViewModel
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Status = user.Active ? 1 : 0
                }).ToList();
                return Ok(usersList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getuser")]
        public ActionResult GetUser(int id)
        {
            try
            {
                var user = _userService.GetUserById(id);
                if (user != null)
                {
                    var res = new UserViewModel
                    {
                        UserId = user.UserId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Status = user.Active ? 1 : 0
                    };
                    return Ok(res);
                }
                else
                {
                    return NotFound();
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
