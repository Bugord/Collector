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
    [Migration("20190205084837_addedIsMoneyDebt")]
    partial class addedIsMoneyDebt
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Collector.DAO.Entities.Change", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("ChangedById");

                    b.Property<long>("ChangedDebtId");

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("ChangedById");

                    b.HasIndex("ChangedDebtId");

                    b.ToTable("Changes");
                });

            modelBuilder.Entity("Collector.DAO.Entities.ChatMessage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AuthorId");

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<long?>("SentToId");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("SentToId");

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("Collector.DAO.Entities.Debt", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<DateTime?>("DateOfOverdue");

                    b.Property<string>("Description")
                        .HasMaxLength(256);

                    b.Property<long>("FriendId");

                    b.Property<bool>("IsClosed");

                    b.Property<bool>("IsMoney");

                    b.Property<bool>("IsOwnerDebter");

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<long>("OwnerId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<bool>("Synchronize");

                    b.Property<float>("Value");

                    b.HasKey("Id");

                    b.HasIndex("FriendId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Debts");
                });

            modelBuilder.Entity("Collector.DAO.Entities.EmailConfirmation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("ConfirmationTime");

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<bool>("Used");

                    b.Property<long>("UserId");

                    b.Property<string>("VerificationToken")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("EmailConfirmations");
                });

            modelBuilder.Entity("Collector.DAO.Entities.Feedback", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("Closed");

                    b.Property<long?>("ClosedById");

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<long?>("CreatorId");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<bool>("IsClosed");

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("ClosedById");

                    b.HasIndex("CreatorId");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("Collector.DAO.Entities.FeedbackMessage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AuthorId");

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<long>("FeedbackId");

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

            modelBuilder.Entity("Collector.DAO.Entities.FieldChange", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ChangeId");

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<string>("FieldName")
                        .IsRequired();

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<string>("NewValue");

                    b.Property<string>("OldValue");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("ChangeId");

                    b.ToTable("FieldChanges");
                });

            modelBuilder.Entity("Collector.DAO.Entities.Friend", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<long?>("FriendUserId");

                    b.Property<long?>("InviteId");

                    b.Property<bool>("IsSynchronized");

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<long>("OwnerId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Id");

                    b.HasIndex("FriendUserId");

                    b.HasIndex("OwnerId");

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

                    b.HasIndex("UserId");

                    b.ToTable("Invites");
                });

            modelBuilder.Entity("Collector.DAO.Entities.PasswordReset", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<DateTime>("ExpirationTime");

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<DateTime?>("ResetDate");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<bool>("Used");

                    b.Property<long>("UserId");

                    b.Property<string>("VerificationToken")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("PasswordResets");
                });

            modelBuilder.Entity("Collector.DAO.Entities.Upload", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<long>("CreatedBy");

                    b.Property<string>("Extention")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<DateTime?>("Modified");

                    b.Property<long?>("ModifiedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("OriginalName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<long>("Size");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("Uploads");
                });

            modelBuilder.Entity("Collector.DAO.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AratarUrl");

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

            modelBuilder.Entity("Collector.DAO.Entities.Change", b =>
                {
                    b.HasOne("Collector.DAO.Entities.User", "ChangedBy")
                        .WithMany()
                        .HasForeignKey("ChangedById");

                    b.HasOne("Collector.DAO.Entities.Debt", "ChangedDebt")
                        .WithMany("Changes")
                        .HasForeignKey("ChangedDebtId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Collector.DAO.Entities.ChatMessage", b =>
                {
                    b.HasOne("Collector.DAO.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Collector.DAO.Entities.User", "SentTo")
                        .WithMany()
                        .HasForeignKey("SentToId");
                });

            modelBuilder.Entity("Collector.DAO.Entities.Debt", b =>
                {
                    b.HasOne("Collector.DAO.Entities.Friend", "Friend")
                        .WithMany("Debts")
                        .HasForeignKey("FriendId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Collector.DAO.Entities.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Collector.DAO.Entities.EmailConfirmation", b =>
                {
                    b.HasOne("Collector.DAO.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Collector.DAO.Entities.Feedback", b =>
                {
                    b.HasOne("Collector.DAO.Entities.User", "ClosedBy")
                        .WithMany()
                        .HasForeignKey("ClosedById");

                    b.HasOne("Collector.DAO.Entities.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId");
                });

            modelBuilder.Entity("Collector.DAO.Entities.FeedbackMessage", b =>
                {
                    b.HasOne("Collector.DAO.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Collector.DAO.Entities.Feedback", "Feedback")
                        .WithMany("Messages")
                        .HasForeignKey("FeedbackId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Collector.DAO.Entities.FieldChange", b =>
                {
                    b.HasOne("Collector.DAO.Entities.Change", "Change")
                        .WithMany("FieldChanges")
                        .HasForeignKey("ChangeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Collector.DAO.Entities.Friend", b =>
                {
                    b.HasOne("Collector.DAO.Entities.User", "FriendUser")
                        .WithMany()
                        .HasForeignKey("FriendUserId");

                    b.HasOne("Collector.DAO.Entities.User", "Owner")
                        .WithMany("Friends")
                        .HasForeignKey("OwnerId");
                });

            modelBuilder.Entity("Collector.DAO.Entities.Invite", b =>
                {
                    b.HasOne("Collector.DAO.Entities.User", "OwnerUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Collector.DAO.Entities.PasswordReset", b =>
                {
                    b.HasOne("Collector.DAO.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}