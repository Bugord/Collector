﻿// <auto-generated />
using System;
using Collector.DAO.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Collector.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20181026152553_feedbackUpdateMigration")]
    partial class feedbackUpdateMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Collector.DAO.Entities.Debt", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<string>("Description")
                        .HasMaxLength(256);

                    b.Property<long>("FriendId");

                    b.Property<bool>("IsOwnerDebter");

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<long>("OwnerId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<bool>("Synchronize");

                    b.Property<float>("Value");

                    b.HasKey("Id");

                    b.ToTable("Debts");
                });

            modelBuilder.Entity("Collector.DAO.Entities.EmailConfirmation", b =>
                {
                    b.Property<long>("UserId");

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<long>("Id");

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("VerificationToken")
                        .IsRequired();

                    b.HasKey("UserId");

                    b.ToTable("EmailConfirmations");
                });

            modelBuilder.Entity("Collector.DAO.Entities.Feedback", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("ClosedById");

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<long?>("CreatorId");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<bool>("isClosed");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("Collector.DAO.Entities.FeedbackMessage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("AuthorId");

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<long?>("FeedbackId");

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("FeedbackId");

                    b.ToTable("FeedbackMessages");
                });

            modelBuilder.Entity("Collector.DAO.Entities.Friend", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<long?>("InviteId");

                    b.Property<bool>("IsSynchronized");

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<long>("OwnerId");

                    b.Property<string>("OwnersName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<long?>("UserId");

                    b.Property<string>("UsersName")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("Collector.DAO.Entities.Invite", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Approved");

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<long>("FriendId");

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<long>("OwnerId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Invites");
                });

            modelBuilder.Entity("Collector.DAO.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Confirmed");

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<int>("Role");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Collector.DAO.Entities.EmailConfirmation", b =>
                {
                    b.HasOne("Collector.DAO.Entities.User", "User")
                        .WithOne("EmailConfirmation")
                        .HasForeignKey("Collector.DAO.Entities.EmailConfirmation", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Collector.DAO.Entities.Feedback", b =>
                {
                    b.HasOne("Collector.DAO.Entities.User", "Creator")
                        .WithMany("Feedbacks")
                        .HasForeignKey("CreatorId");
                });

            modelBuilder.Entity("Collector.DAO.Entities.FeedbackMessage", b =>
                {
                    b.HasOne("Collector.DAO.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("Collector.DAO.Entities.Feedback", "Feedback")
                        .WithMany("Messages")
                        .HasForeignKey("FeedbackId");
                });
#pragma warning restore 612, 618
        }
    }
}