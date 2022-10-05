using System.Collections.Generic;

namespace WorkBC.Admin.Areas.AdminAccounts.Models
{
    public class AdminUserSearchViewModel
    {
        public int TotalUsers { get; set; }
        public List<AdminUserRowViewModel> Results { get; set; }
    }
}