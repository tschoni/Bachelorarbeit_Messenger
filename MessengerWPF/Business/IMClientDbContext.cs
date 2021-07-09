using MessengerWPF.Models.DbModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.Business
{
    /// <summary>
    /// DbContext Klasse für Entitiy Framework core
    /// </summary>
    public class IMClientDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Key> Keys { get; set; }

        public DbSet<GroupTextMessage> GroupTextMessages { get; set; }

        public DbSet<GroupUpdateMessage> GroupUpdateMessages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = "IMClientDb",

                //Password?
            };


            optionsBuilder.UseSqlite(builder.ConnectionString);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>().HasOne(x => x.Sender).WithMany(x => x.Messages);

            modelBuilder.Entity<User>().HasMany(x => x.Keys).WithOne(x => x.AssociatedUser);
            modelBuilder.Entity<User>().HasMany(x => x.Contacts);

            modelBuilder.Entity<Group>().HasMany(x => x.Messages).WithOne(x => x.Group);
            modelBuilder.Entity<Group>().HasMany(x => x.Members).WithMany(x => x.Groups);
            modelBuilder.Entity<Group>().HasMany(x => x.Admins).WithMany(x => x.AdminOfGroups);

            //            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
            //                .SelectMany(t => t.GetForeignKeys())
            //                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            //            foreach (var fk in cascadeFKs)
            //                fk.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(modelBuilder);
        }
    }
}
