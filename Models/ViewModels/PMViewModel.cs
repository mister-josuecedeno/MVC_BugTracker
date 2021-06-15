using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Models.ViewModels
{
    public class PMViewModel
    {
        public Project Project { get; set; } = new();
        public SelectList Users { get; set; }  // populates list box
        public string SelectedUser { get; set; }  // receives selected users
    }
}
