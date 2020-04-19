using Microsoft.EntityFrameworkCore;
using prj3beer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace prj3beer.Services
{
    public class BeerContext : DbContext
    {
        // DB set for Beverages
        public DbSet<Beverage> Beverage { get; set; }

        // DB Set for Preferences
        public DbSet<Preference> Preference { get; set; }

        //DB Set for Brands
        public DbSet<Brand> Brand { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Database Name
            string dbPath = "Beer.db3";

            // Switch Case for getting Device Type
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", dbPath);
                    break;
                case Device.Android:
                    dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), dbPath);
                    break;
            }
            // Assemble the SQLite Database using the DB Path. 
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}
