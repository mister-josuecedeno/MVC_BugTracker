using Microsoft.AspNetCore.Http;
using MVC_BugTracker.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }

        [Display(Name = "Ticket")]
        public int TicketId { get; set; }

        [Display(Name = "Team Member")]
        public string UserId { get; set; }

        public string Description { get; set; }

        [Display(Name = "Date Created")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset Created { get; set; }

        
        // File Attachment
        [NotMapped]
        [DataType(DataType.Upload)]
        [Display(Name = "Form File")]
        [MaxFileSize(2 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".gif", ".doc", ".docx", ".xls", ".xlsx", ".pdf"})]
        public IFormFile FormFile { get; set; }

        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "File Data")]
        public byte[] FileData { get; set; }

        [Display(Name = "File Extension")]
        public string FileContentType { get; set; }


        // Navigation
        public virtual Ticket Ticket { get; set; }
        public virtual BTUser User { get; set; }
    }
}
