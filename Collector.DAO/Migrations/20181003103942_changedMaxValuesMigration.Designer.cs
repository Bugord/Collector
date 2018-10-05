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
    [Migration("20181003103942_changedMaxValuesMigration")]
    partial class changedMaxValuesMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
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

                    b.Property<bool>("Synchronize");

                    b.Property<float>("Value");

                    b.HasKey("Id");

                    b.ToTable("Debts");
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

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Invites");
                });

            modelBuilder.Entity("Collector.DAO.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
