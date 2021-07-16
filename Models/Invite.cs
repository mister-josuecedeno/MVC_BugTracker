using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Models
{
    public class Invite
    {
        // PK / FK
        public int Id { get; set; }

        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Display(Name = "Project")]
        public int? ProjectId { get; set; }

        [Display(Name = "Invitee")]
        public string InviteeId { get; set; }

        [Display(Name = "Invitor")]
        public string InvitorId { get; set; }

        // Data Fields

        [Display(Name = "Invite Date")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset InviteDate { get; set; }

        [Display(Name = "Company Token")]
        public Guid CompanyToken { get; set; }

        [Display(Name = "Email")]
        public string InviteeEmail { get; set; }

        [Display(Name = "First Name")]
        public string InviteeFirstName { get; set; }

        [Display(Name = "Last Name")]
        public string InviteeLastName { get; set; }

        public bool IsValid { get; set; }

        // Navigation

        // 1-to-1
        public virtual Company Company { get; set; }
        public virtual Project Project { get; set; }
        public virtual BTUser Invitee { get; set; }
        public virtual BTUser Invitor { get; set; }

    }
}
