using Microsoft.EntityFrameworkCore;
using prj3beer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace prj3beer.Services
{
    public class SocialContext : DbContext
    {
        // DB set for Friends
        public DbSet<Friend> Friend { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Database Name
            string dbPath = "Social.db3";

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
