using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockWolfTrading.Core.Models
{
    public partial class User
    {
        public User()
        {
            Order = new HashSet<Order>();
            UserRoles = new HashSet<UserRoles>();
            UserSettings = new HashSet<UserSettings>();
            UserProducts = new HashSet<UserProducts>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public byte[] Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MiddleName { get; set; }
        public bool Active { get; set; }
        public bool IsFirstLogin { get; set; }
        public DateTime? Expire { get; set; }
        public bool IsExpire { get; set; }
        public bool? IsExistUserInSafetyReport { get; set; }
        public bool? IsAuthenticated { get; set; }
        public string VerificationCode { get; set; }
        public DateTime? LastLogin { get; set; }
        public byte[] SignatureImage { get; set; }
        [StringLength(100)]
        public string ImageType { get; set; }

        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<UserRoles> UserRoles { get; set; }
        public virtual ICollection<UserSettings> UserSettings { get; set; }
        public virtual ICollection<UserProducts> UserProducts { get; set; }
    }
}
