using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Ylli.Models;

namespace Ylli.Data
{
    public class TerminAPI : DbContext
    {
        public TerminAPI(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Termin> Termine { get; set; }
    }
}
