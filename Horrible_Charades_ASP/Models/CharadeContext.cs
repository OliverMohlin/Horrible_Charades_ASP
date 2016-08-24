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
        /// Mappar mot DB-tabellen Nouns
        /// </summary>
        public DbSet<Noun> Nouns { get; set; }
        public DbSet<Adjective> Adjectives { get; set; }
        public DbSet<Verb> Verbs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Game> Games { get; set; }


        /// <summary>
        /// Kan tas bort, eftersom att den inte finns som tabell i databasen.
        /// </summary>
        //[NotMapped]
        //public DbSet<Word> Words { get; set; }

    }
}