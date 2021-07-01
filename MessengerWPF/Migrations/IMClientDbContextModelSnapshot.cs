﻿// <auto-generated />
using System;
using MessengerWPF.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MessengerWPF.Migrations
{
    [DbContext(typeof(IMClientDbContext))]
    partial class IMClientDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.7");

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.Property<long>("GroupsId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("MembersId")
                        .HasColumnType("INTEGER");

                    b.HasKey("GroupsId", "MembersId");

                    b.HasIndex("MembersId");

                    b.ToTable("GroupUser");
                });

            modelBuilder.Entity("MessengerWPF.Models.DbModels.Group", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("MessengerWPF.Models.DbModels.Key", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long?>("AssociatedUserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("KeyString")
                        .HasColumnType("TEXT");

                    b.Property<int>("KeyType")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AssociatedUserId");

                    b.ToTable("Keys");
                });

            modelBuilder.Entity("MessengerWPF.Models.DbModels.Message", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("MessageState")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("SenderId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SenderId");

                    b.ToTable("Message");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Message");
                });

            modelBuilder.Entity("MessengerWPF.Models.DbModels.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<long?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MessengerWPF.Models.DbModels.GroupMessage", b =>
                {
                    b.HasBaseType("MessengerWPF.Models.DbModels.Message");

                    b.Property<long?>("GroupId")
                        .HasColumnType("INTEGER");

                    b.HasIndex("GroupId");

                    b.HasDiscriminator().HasValue("GroupMessage");
                });

            modelBuilder.Entity("MessengerWPF.Models.DbModels.GroupTextMessage", b =>
                {
                    b.HasBaseType("MessengerWPF.Models.DbModels.GroupMessage");

                    b.Property<string>("Text")
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue("GroupTextMessage");
                });

            modelBuilder.Entity("MessengerWPF.Models.DbModels.GroupUpdateMessage", b =>
                {
                    b.HasBaseType("MessengerWPF.Models.DbModels.GroupMessage");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasIndex("UserId");

                    b.HasDiscriminator().HasValue("GroupUpdateMessage");
                });

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.HasOne("MessengerWPF.Models.DbModels.Group", null)
                        .WithMany()
                        .HasForeignKey("GroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MessengerWPF.Models.DbModels.User", null)
                        .WithMany()
                        .HasForeignKey("MembersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MessengerWPF.Models.DbModels.Key", b =>
                {
                    b.HasOne("MessengerWPF.Models.DbModels.User", "AssociatedUser")
                        .WithMany("Keys")
                        .HasForeignKey("AssociatedUserId");

                    b.Navigation("AssociatedUser");
                });

            modelBuilder.Entity("MessengerWPF.Models.DbModels.Message", b =>
                {
                    b.HasOne("MessengerWPF.Models.DbModels.User", "Sender")
                        .WithMany("Messages")
                        .HasForeignKey("SenderId");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("MessengerWPF.Models.DbModels.User", b =>
                {
                    b.HasOne("MessengerWPF.Models.DbModels.User", null)
                        .WithMany("Contacts")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("MessengerWPF.Models.DbModels.GroupMessage", b =>
                {
                    b.HasOne("MessengerWPF.Models.DbModels.Group", "Group")
                        .WithMany("Messages")
                        .HasForeignKey("GroupId");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("MessengerWPF.Models.DbModels.GroupUpdateMessage", b =>
                {
                    b.HasOne("MessengerWPF.Models.DbModels.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MessengerWPF.Models.DbModels.Group", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("MessengerWPF.Models.DbModels.User", b =>
                {
                    b.Navigation("Contacts");

                    b.Navigation("Keys");

                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
