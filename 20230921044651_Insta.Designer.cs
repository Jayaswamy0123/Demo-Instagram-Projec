﻿// <auto-generated />
using System;
using Instagram.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Instagram.Migrations
{
    [DbContext(typeof(InstagramContext))]
    [Migration("20230921044651_Insta")]
    partial class Insta
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Instagram.Entities.Followers", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long>("FollowersId")
                        .HasColumnType("bigint")
                        .HasColumnName("followers_id");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("FollowersId");

                    b.HasIndex("UserId");

                    b.ToTable("Followers");
                });

            modelBuilder.Entity("Instagram.Entities.PasswordResetToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_date");

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("email");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("expiry_date");

                    b.Property<string>("Token")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("token");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_date");

                    b.HasKey("Id");

                    b.ToTable("password_reset_token");
                });

            modelBuilder.Entity("Instagram.Entities.Role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_date");

                    b.Property<string>("Name")
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("name");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_date");

                    b.HasKey("Id");

                    b.ToTable("role");
                });

            modelBuilder.Entity("Instagram.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .HasColumnType("longtext")
                        .HasColumnName("email");

                    b.Property<string>("FullName")
                        .HasColumnType("longtext")
                        .HasColumnName("full_name");

                    b.Property<long>("MobileNumber")
                        .HasColumnType("bigint")
                        .HasColumnName("mobile_number");

                    b.Property<string>("Password")
                        .HasColumnType("longtext")
                        .HasColumnName("password");

                    b.Property<string>("UserName")
                        .HasColumnType("longtext")
                        .HasColumnName("user_name");

                    b.HasKey("Id");

                    b.ToTable("user");
                });

            modelBuilder.Entity("Instagram.Entities.UserMedia", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_date");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_active");

                    b.Property<string>("MediaUrl")
                        .HasColumnType("longtext")
                        .HasColumnName("media_url");

                    b.Property<int>("Type")
                        .HasColumnType("int")
                        .HasColumnName("type");

                    b.Property<string>("TypeName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("type_name");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_date");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("user_media");
                });

            modelBuilder.Entity("Instagram.Entities.UserRole", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint")
                        .HasColumnName("role_id");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("user_role");
                });

            modelBuilder.Entity("Instagram.Entities.Followers", b =>
                {
                    b.HasOne("Instagram.Entities.User", "UserFollowers")
                        .WithMany()
                        .HasForeignKey("FollowersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Instagram.Entities.User", "User")
                        .WithMany("Followers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("UserFollowers");
                });

            modelBuilder.Entity("Instagram.Entities.UserMedia", b =>
                {
                    b.HasOne("Instagram.Entities.User", "User")
                        .WithMany("UserMedia")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Instagram.Entities.UserRole", b =>
                {
                    b.HasOne("Instagram.Entities.Role", "Role")
                        .WithMany("UserRole")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Instagram.Entities.User", "User")
                        .WithMany("UserRole")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Instagram.Entities.Role", b =>
                {
                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("Instagram.Entities.User", b =>
                {
                    b.Navigation("Followers");

                    b.Navigation("UserMedia");

                    b.Navigation("UserRole");
                });
#pragma warning restore 612, 618
        }
    }
}