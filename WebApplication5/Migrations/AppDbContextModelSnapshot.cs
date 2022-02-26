﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication5.Models;

namespace WebApplication5.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WebApplication5.Models.Ans", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.ToTable("AnsSet");
                });

            modelBuilder.Entity("WebApplication5.Models.Ans1", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.ToTable("Ans1");
                });

            modelBuilder.Entity("WebApplication5.Models.Corrections", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CorBodyText")
                        .IsRequired();

                    b.Property<int>("CorNumber");

                    b.Property<DateTime>("CorTerm");

                    b.Property<int?>("ExecutorId");

                    b.Property<string>("ImageLink")
                        .IsRequired();

                    b.Property<int?>("ProjectId");

                    b.Property<DateTime>("RecieveDate");

                    b.Property<int>("Status");

                    b.Property<int?>("ans1Id");

                    b.HasKey("Id");

                    b.HasIndex("ExecutorId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("ans1Id");

                    b.ToTable("Cors");
                });

            modelBuilder.Entity("WebApplication5.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ContractNumber");

                    b.Property<string>("FullName")
                        .IsRequired();

                    b.Property<string>("InternalNum")
                        .IsRequired();

                    b.Property<int?>("ManagerId");

                    b.Property<string>("ShortName");

                    b.Property<bool>("ShowInMenuBar");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("ManagerId");

                    b.ToTable("ProjectSet");
                });

            modelBuilder.Entity("WebApplication5.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FirstName");

                    b.Property<string>("FullName");

                    b.Property<string>("LastName");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WebApplication5.Models.Corrections", b =>
                {
                    b.HasOne("WebApplication5.Models.User", "Executor")
                        .WithMany()
                        .HasForeignKey("ExecutorId");

                    b.HasOne("WebApplication5.Models.Project", "Project")
                        .WithMany("Corrections")
                        .HasForeignKey("ProjectId");

                    b.HasOne("WebApplication5.Models.Ans1", "ans1")
                        .WithMany()
                        .HasForeignKey("ans1Id");
                });

            modelBuilder.Entity("WebApplication5.Models.Project", b =>
                {
                    b.HasOne("WebApplication5.Models.User", "Manager")
                        .WithMany()
                        .HasForeignKey("ManagerId");
                });
#pragma warning restore 612, 618
        }
    }
}
