using System;
using System.Collections.Generic;

namespace SQLLINQ.Models
{
    public partial class Company
    {
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
    }
}
