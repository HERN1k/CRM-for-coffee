﻿// <auto-generated />
using System;
using CRM.Data.Contexts.ApplicationContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM.Data.Migrations
{
    [DbContext(typeof(AppDBContext))]
    [Migration("20240624090832_ChangeUser")]
    partial class ChangeUser
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CRM.Core.Entities.EntityAddOn", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric(10, 2)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("ProductId");

                    b.ToTable("AddOns");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityOrder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("OrderСreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PaymentMethod")
                        .HasColumnType("integer");

                    b.Property<decimal>("Taxes")
                        .HasColumnType("numeric(10, 2)");

                    b.Property<decimal>("Total")
                        .HasColumnType("numeric(10, 2)");

                    b.Property<Guid>("WorkerId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityOrderAddOn", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AddOnId")
                        .HasColumnType("uuid");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<Guid>("OrderProductId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("OrderProductId");

                    b.ToTable("OrderAddOns");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityOrderProduct", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("OrderId");

                    b.ToTable("OrderProducts");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityProduct", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric(10, 2)");

                    b.Property<Guid>("ProductCategoryId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("ProductCategoryId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityProductCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("ProductCategorys");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityRefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("RefreshTokenString")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Age")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FatherName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Post")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityAddOn", b =>
                {
                    b.HasOne("CRM.Core.Entities.EntityProduct", "Product")
                        .WithMany("AddOns")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityOrderAddOn", b =>
                {
                    b.HasOne("CRM.Core.Entities.EntityOrderProduct", "OrderProduct")
                        .WithMany("AddOns")
                        .HasForeignKey("OrderProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrderProduct");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityOrderProduct", b =>
                {
                    b.HasOne("CRM.Core.Entities.EntityOrder", "Order")
                        .WithMany("Products")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityProduct", b =>
                {
                    b.HasOne("CRM.Core.Entities.EntityProductCategory", "ProductCategory")
                        .WithMany("Products")
                        .HasForeignKey("ProductCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProductCategory");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityRefreshToken", b =>
                {
                    b.HasOne("CRM.Core.Entities.EntityUser", "User")
                        .WithOne("RefreshToken")
                        .HasForeignKey("CRM.Core.Entities.EntityRefreshToken", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityOrder", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityOrderProduct", b =>
                {
                    b.Navigation("AddOns");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityProduct", b =>
                {
                    b.Navigation("AddOns");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityProductCategory", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("CRM.Core.Entities.EntityUser", b =>
                {
                    b.Navigation("RefreshToken")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
