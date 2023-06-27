using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebTestb1.Models;

namespace WebTestb1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<WebTestb1.Models.ProjectTask> ProjectTask { get; set; }
        public DbSet<WebTestb1.Models.Project> Project { get; set; }
        public DbSet<WebTestb1.Models.Worker> Worker { get; set; }
    }
}
