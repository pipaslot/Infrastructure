﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Pipaslot.Demo.Models;
using System;

namespace Pipaslot.Demo.Migrations
{
    [DbContext(typeof(AppDatabase))]
    partial class AppDatabaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Pipaslot.Infrastructure.Security.EntityFramework.Entities.Privilege<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsAllowed");

                    b.Property<string>("Permission");

                    b.Property<string>("Resource");

                    b.Property<int>("ResourceInstance");

                    b.Property<int>("Role");

                    b.HasKey("Id");

                    b.ToTable("SecurityPrivilege");
                });

            modelBuilder.Entity("Pipaslot.Infrastructure.Security.EntityFramework.Entities.Role<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("SecurityRole");
                });
#pragma warning restore 612, 618
        }
    }
}