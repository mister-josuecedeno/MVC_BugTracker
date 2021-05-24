using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC_BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MVC_BugTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<BTUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
