using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Models
{
    public class Company
    {
        // PK
        public int Id { get; set; }

        // Data Fields
        [Display(Name = "Company Name")]
        public string Name { get; set; }
        public string Description { get; set; }

        // Image
        [NotMapped]
        [DataType(DataType.Upload)]
        [Display(Name = "Image File")]
        public IFormFile ImageFormFile { get; set; }

        [Display(Name = "File Name")]
        public string ImageFileName { get; set; }

        [Display(Name = "File Data")]
        public byte[] ImageFileData { get; set; }

        [Display(Name = "File Extension")]
        public string ImageContentType { get; set; }

        // Navigation
        
        // 1-to-Many (Lazy Loading)
        // Adding the hashset triggers eager loading (design decision)
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();
    }
}
