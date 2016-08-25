using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Horrible_Charades_ASP.Models
{
    /// <summary>
    /// Mappar mot Databasen HorribleCharades
    /// </summary>
    public class CharadeContext : DbContext
    {
        public CharadeContext() : base("connString")
        {

        }

        /// <summary>
        /// Mappar mot DB-tabeller
        /// </summary>
        public DbSet<Noun> Nouns { get; set; }
        public DbSet<Adjective> Adjectives { get; set; }
        public DbSet<Verb> Verbs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Game> Games { get; set; }


    }
}