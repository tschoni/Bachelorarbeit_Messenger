using MessengerAPI.Models.DbModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Business
{
    public class IMServerDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<PublicKey> PublicKeys { get; set; }

        public DbSet<EphemeralKey> EphemeralKeys { get; set; }

        //public DbSet<SignedKey> SignedKeys { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = ".",
                InitialCatalog = "IMServerDb",
                IntegratedSecurity = true,
            };


            optionsBuilder.UseSqlServer(builder.ConnectionString);

            base.OnConfiguring(optionsBuilder);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Name).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.UserToken).IsUnique();

            modelBuilder.Entity<Message>().HasOne(x => x.Sender).WithMany(x => x.SentMessages); /*.OnDelete(DeleteBehavior.Cascade)*/
            modelBuilder.Entity<Message>().HasOne(x => x.Recipient).WithMany(x => x.ReceivedMessages); /*.OnDelete(DeleteBehavior.Cascade)*/

            modelBuilder.Entity<User>().HasMany(x => x.PublicKeys).WithOne(x => x.Owner).OnDelete(DeleteBehavior.Cascade); ;
            modelBuilder.Entity<EphemeralKey>().HasOne(x => x.Initiator).WithMany(x => x.EphemeralKeys).OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Group>().HasMany(x => x.Members).WithMany(x => x.Groups);
            modelBuilder.Entity<Group>().HasMany(x => x.Admins).WithMany(x=> x.AdminOfGroups);

            //var cascadeFKs = modelBuilder.Model.GetEntityTypes()
            //    .SelectMany(t => t.GetForeignKeys())
            //    .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            //foreach (var fk in cascadeFKs)
            //    fk.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(modelBuilder);
        }



    }
}
//        modelBuilder.Entity<Student>()
//            .HasOne<Grade>(s => s.Grade)
//            .WithMany(g => g.Students)