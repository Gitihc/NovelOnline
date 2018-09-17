﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Repository;

namespace Repository.Migrations
{
    [DbContext(typeof(HLDBContext))]
    [Migration("20180829060426_modify4")]
    partial class modify4
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Repository.Domain.Chapter", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<long>("ChapterEndPosition")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0L);

                    b.Property<long>("ChapterStartPosition")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0L);

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("NovelId")
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("NovelName")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("OriginLink")
                        .HasColumnType("varchar(200)")
                        .HasMaxLength(200);

                    b.Property<int>("Sort")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<int>("State")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.ToTable("chapter");
                });

            modelBuilder.Entity("Repository.Domain.Module", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("CascadeId")
                        .HasColumnType("varchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("Code")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<DateTime>("CreateDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Icon")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<int>("IsEnable")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<string>("Link")
                        .HasColumnType("varchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("ParentId")
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("ParentName")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<int>("Sort")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<int>("Type")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.ToTable("module");
                });

            modelBuilder.Entity("Repository.Domain.ModuleElement", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("Class")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("DomId")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Icon")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("ModuleId")
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Remark")
                        .HasColumnType("text");

                    b.Property<int>("Sort")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<int>("Type")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.ToTable("moduleelement");
                });

            modelBuilder.Entity("Repository.Domain.Novel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreateDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<int>("FromType")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("OriginLink")
                        .HasColumnType("varchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("PhysicalPath");

                    b.Property<int>("State")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.ToTable("novel");
                });

            modelBuilder.Entity("Repository.Domain.Relevance", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("FirstId")
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("Key")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<DateTime>("OperateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("OperatorId")
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("SecondId")
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.ToTable("relevance");
                });

            modelBuilder.Entity("Repository.Domain.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("Account")
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreateDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("CreatorId")
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("Name")
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("Password")
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<int>("Sex")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.ToTable("user");
                });

            modelBuilder.Entity("Repository.Domain.UserNovel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("LastChapterId")
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<DateTime>("LastOpenTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("NovelId")
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("NovelName");

                    b.Property<int>("State")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("usernovel");
                });

            modelBuilder.Entity("Repository.Domain.Website", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreateDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("CreatorId")
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("OriginLink")
                        .HasColumnType("varchar(200)")
                        .HasMaxLength(200);

                    b.Property<int>("State")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.ToTable("website");
                });

            modelBuilder.Entity("Repository.Domain.WebsiteNovel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("Author")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<DateTime>("CreateDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("OriginLink")
                        .HasColumnType("varchar(200)")
                        .HasMaxLength(200);

                    b.Property<int>("State")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<string>("WebSiteId")
                        .HasColumnType("varchar(64)")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.ToTable("websitenovel");
                });
#pragma warning restore 612, 618
        }
    }
}