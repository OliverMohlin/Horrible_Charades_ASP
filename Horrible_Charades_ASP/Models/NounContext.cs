using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Horrible_Charades_ASP.Models
{
    /// <summary>
    /// Mappar mot Databasen
    /// </summary>
    public class NounContext : DbContext
    {
        public NounContext() : base("connString")
        {

        }

        /// <summary>
        /// Mappar mot DB-tabellen Nouns
        /// </summary>
        public DbSet<Noun> Nouns { get; set; }
    }
}