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
using System.IO;
using Microsoft.AspNetCore.Http;

namespace StockWolfTradingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly Encrypt _encrypt;
        public TokenController(IUserService userService, IOrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
            _encrypt = new Encrypt();
        }
        private JwtToken GetToken(User user)
        {
            JwtTokenBuilder jb = new JwtTokenBuilder()
                 .AddSecurityKey(JwtSecurityKey.Create("Test-secret-key-1234-SWT"))
                 .AddSubject($"{user.UserName}")
                 .AddIssuer("Test.Security.Bearer")
                 .AddAudience("Test.Security.Bearer")
                 //.AddClaim(user.UserRoles.ToList()[0].RoleRoleId == 1 ? "AdminClaim" : "UserClaim", user.UserId.ToString())
                 .AddExpiry(30);
            var token = jb.Build();
            return token;
        }


        [HttpGet("login")]
        public IActionResult Create(string email, string password)//[FromBody]JObject model)
        {
            //var modelUser = JsonConvert.DeserializeObject<User>(model.ToString());
            var user = _userService.GetUserByUsername(email); // modelUser.UserName);
            //var user = _userService.CheckUserCredentials(modelUser.UserName, _encrypt.EncryptPw(modelUser.Password));

            if (user == null || user.Active != true)
                return Unauthorized();

            if (user.UserProducts == null && user.UserProducts.Count > 0)
            {
                UserProducts up = new UserProducts();
                up.ProductProductId = 1; //Demo product
                up.DateAdded = DateTime.Now;
                user.UserProducts = new List<UserProducts>() { up };
            }
            var salt = user.Password.Take(32).ToArray();
            var pass_storage = user.Password.Skip(32).ToArray();
            var pass = Encoding.UTF8.GetBytes(password);
            byte[] bytes;
            var deriveBytes = new Rfc2898DeriveBytes(pass, salt, 100000, HashAlgorithmName.SHA256);
            bytes = deriveBytes.GetBytes(32);
            if (pass_storage.SequenceEqual(bytes))
            {
                var token = GetToken(user);
                return Ok(new { access_token = token.Value, username = user.UserName, usertype = 1/*user.UserRoles.ToList()[0].RoleRoleId*/, email = user.Email, isfirstlogin = user.IsFirstLogin });
            }
            return Unauthorized();
        }

        /// <summary>
        ///     Return all user Image
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("getuserimage")]
        public ActionResult GetUserImage(string email)
        {
            var dbuser = _userService.GetUserByUsername(email);
            if (dbuser == null)
            {
                return NotFound();
            }
            string contentType = dbuser.ImageType;
            byte[] fileres = dbuser.SignatureImage;

            if (fileres == null || fileres.Length == 0)
            {
                return Ok();
            }
            string base64String = BaseImages.GetImageBase64(contentType, fileres);
            return Ok(base64String);
        }

        [HttpPost("activateuser")]
        public IActionResult ActivateUser([FromBody] JObject model)
        {
            var userCode = JsonConvert.DeserializeObject<UserActivationViewModel>(model.ToString());
            var user = _userService.GetUserByVerificationCode(userCode.vcode);
            if (user == null)
            {
                return NotFound();
            }
            user.Active = true;
            user.VerificationCode = "";
            Order ord = new Order();
            OrderDetail ordet = new OrderDetail();
            ordet.Price = 1000;
            ordet.ProductRefId = 1;
            ord.OrderDetail = new List<OrderDetail>() { ordet };
            ord.TotalAmount = 1000;
            ord.DateCreated = DateTime.Now;
            ord.DateFinished = DateTime.Now;
            ord.UserRefId = user.UserId;
            UserRoles ur = new UserRoles() { RoleRoleId = 2 };
            user.UserRoles = new List<UserRoles>() { ur };
            _userService.EditUser(user);
            _orderService.AddOrder(ord);
            dynamic result = new System.Dynamic.ExpandoObject();
            result.activated = 0;
            result.email = user.Email;
            return Ok(result);
        }

        [HttpPost("signup")]
        public IActionResult RegisterUser([FromBody] JObject model)
        {
            var pUser = JsonConvert.DeserializeObject<UserViewModel>(model.ToString());
            Random rnd = new Random();
            Byte[] salt = new Byte[32];
            rnd.NextBytes(salt);
            var pass = Encoding.UTF8.GetBytes(pUser.Password);
            var exUser = _userService.GetUserByUsername(pUser.Email);
            if (exUser != null)
            {
                return BadRequest("User already exists");
            }
            var deriveBytes = new Rfc2898DeriveBytes(pass, salt, 100000, HashAlgorithmName.SHA256);
            var bytes = deriveBytes.GetBytes(32);
            var password = salt.Concat(bytes).ToArray();
            User usr = new User();
            usr.UserName = pUser.Email;
            usr.FirstName = pUser.FirstName;
            usr.LastName = pUser.LastName;
            usr.IsExpire = false;
            usr.IsFirstLogin = true;
            usr.Email = pUser.Email;
            usr.Expire = DateTime.Now.AddDays(1);
            usr.VerificationCode = Guid.NewGuid().ToString();
            usr.Password = password;
            UserSettings us = new UserSettings();
            us.Interval = "1D";
            us.CandleLow = float.MaxValue;
            usr.UserSettings = new List<UserSettings>() { us };
            UserProducts up = new UserProducts();
            up.ProductProductId = 1; //Demo product
            up.DateAdded = DateTime.Now;
            usr.UserProducts = new List<UserProducts>() { up };
            var user = _userService.AddUser(usr);
            if (user != null)
            {
                SendEmailToUser(user);
                return Ok(new { access_token = "", username = user.Email });
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        ///    Save user image
        /// </summary>
        /// <param name="file"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("saveuserimage")]
        public ActionResult SaveUserImage([FromForm] IFormFile file, [FromForm] string email)
        {
            
            //var image = file;
            var dbuser = _userService.GetUserByUsername(email);
            if (dbuser == null) return NotFound();

            if (file == null) return Ok();

            var extension = file.ContentType;
            byte[] p1;
            using (var memStream = new MemoryStream())
            {
                file.CopyTo(memStream);
                p1 = memStream.ToArray();
            }
            dbuser.SignatureImage = p1;
            dbuser.ImageType = extension;
            var res = _userService.EditUser(dbuser);
            if (res == false) return BadRequest();

            return Ok();
        }

        /// <summary>
        ///    Save user image
        /// </summary>
        /// <param name="file"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("sendmessage")]
        public ActionResult SendMessage([FromBody] JObject model)
        {
            var pMessage = JsonConvert.DeserializeObject<UserMessage>(model.ToString());
            var emailService = new Email { To = "dushko.murtovski@orcion.com", };
            var response = emailService.SendMessageEmail(pMessage.Name, pMessage.Email, pMessage.Message);

            return Ok(response.Result);
        }

        private void SendEmailToUser(User regUser)
        {
            //var bodyString = "Please validate your StockWolf account" + Environment.NewLine
            //    + "http://localhost:5000/signin-dark?validation=" + regUser.VerificationCode;
            var bs = $"<h3>Dear  {regUser.FirstName} {regUser.LastName}</h3>" +
                              $"<p>Please open: </p>" +
                              "<p>" + "https://localhost:3007/login?validation=" + regUser.VerificationCode + "</p><br/>" +
                              "<p>To verify your account</p>" +
                              "<br/>" +
                              "<p><string>With regards,</strong></p>" +
                              "<p><string>StockwolfTrading portal Team</strong></p>" +
                              "<div>" +
                        "</div>" + "<p style=\"color: green; font - size: 20px; font - weight: bold; \">StockwolfTrading Portal</p><br/><br/>";
            var emailService = new Email { To = regUser.Email, };
            var response = emailService.ExecuteUserActivated(bs);
        }
    }
}
