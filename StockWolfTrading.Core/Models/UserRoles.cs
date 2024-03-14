using System;
using System.Collections.Generic;

namespace StockWolfTrading.Core.Models
{
    public partial class UserRoles
    {
        public int UserUserId { get; set; }
        public int RoleRoleId { get; set; }

        public virtual Role RoleRole { get; set; }
        public virtual User UserUser { get; set; }
    }
}
