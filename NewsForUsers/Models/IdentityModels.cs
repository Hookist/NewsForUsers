using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewsForUsers.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            LastVisitDate = DateTime.Now;
        }

        [Required]
        public DateTime LastVisitDate { get; set; }
    }
}