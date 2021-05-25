using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Models
{
    public class Project
    {
        // PK/FK
        public int Id { get; set; }
        public int? ProjectPriorityId { get; set; }
        public int? CompanyId { get; set; }

        // Data Fields
        public string Name { get; set; }
        public string Description { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset? EndDate { get; set; }
        
        public bool Archived { get; set; }

        [Display(Name = "Archived Date")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset? ArchivedDate { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        [Display(Name = "Image File")]
        public IFormFile ImageFormFile { get; set; }

        [Display(Name = "Image File Name")]
        public string ImageFileName { get; set; }

        [Display(Name = "Image File Data")]
        public byte[] ImageFileData { get; set; }

        [Display(Name = "Image File Extension")]
        public string ImageContentType { get; set; }

        // Navigation

        // 1-to-1
        public virtual Company Company { get; set; }
        public virtual ProjectPriority ProjectPriority { get; set; }


        // 1-to-Many (Eager Loading)
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();


}
}
