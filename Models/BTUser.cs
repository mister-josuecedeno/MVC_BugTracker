using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace MVC_BugTracker.Models
{
    public class BTUser : IdentityUser
    {
        public int? CompanyId { get; set; }

        // Data Fields
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        
        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        // User Avatar
        [NotMapped]
        [DataType(DataType.Upload)]
        [Display(Name = "Avatar File")]
        public IFormFile AvatarFormFile { get; set; }

        [Display(Name = "Avatar File Name")]
        public string AvatarFileName { get; set; }

        [Display(Name = "Avatar File Data")]
        public byte[] AvatarFileData { get; set; }

        [Display(Name = "Avatar File Extension")]
        public string AvatarContentType { get; set; }

        
        // Navigation
        public virtual Company Company { get; set; }

        // Many-to-Many with Projects
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

    }
}
