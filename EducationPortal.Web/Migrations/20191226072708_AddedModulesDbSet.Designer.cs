﻿// <auto-generated />
using System;
using EducationPortal.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EducationPortal.Web.Migrations
{
    [DbContext(typeof(EducationPortalDbContext))]
    [Migration("20191226072708_AddedModulesDbSet")]
    partial class AddedModulesDbSet
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.Answer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content");

                    b.Property<bool>("IsCorrect");

                    b.Property<int>("QuestionId");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.AnswerHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AnswerHistoryDataId");

                    b.Property<int>("AnswerId");

                    b.Property<bool>("IsCorrect");

                    b.Property<string>("TextInput");

                    b.HasKey("Id");

                    b.HasIndex("AnswerHistoryDataId");

                    b.HasIndex("AnswerId");

                    b.ToTable("AnswerHistories");
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.AnswerHistoryData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AttemptId");

                    b.Property<DateTime>("Date");

                    b.Property<int>("QuestionId");

                    b.HasKey("Id");

                    b.HasIndex("AttemptId");

                    b.HasIndex("QuestionId");

                    b.ToTable("AnswerHistoryData");
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.Attempt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<string>("Name");

                    b.Property<int>("Score");

                    b.Property<int>("TestCompletionId");

                    b.HasKey("Id");

                    b.HasIndex("TestCompletionId");

                    b.ToTable("Attempts");
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CourseComplexity");

                    b.Property<string>("Name");

                    b.Property<int>("TrainingHours");

                    b.HasKey("Id");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.EducationMaterial", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ContentType");

                    b.Property<byte[]>("Data");

                    b.Property<int>("MaterialImportance");

                    b.Property<int>("ModuleId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("ModuleId");

                    b.ToTable("EducationMaterials");
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.Module", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CourseId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("Modules");
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content");

                    b.Property<byte[]>("Image");

                    b.Property<string>("ImageContentType");

                    b.Property<int>("QuestionType");

                    b.Property<int>("TestId");

                    b.HasKey("Id");

                    b.HasIndex("TestId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.Test", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("MaxNumberOfAttempts");

                    b.Property<int>("ModuleId");

                    b.Property<string>("Name");

                    b.Property<int>("TimeLimit");

                    b.HasKey("Id");

                    b.HasIndex("ModuleId");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.TestCompletion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("TestId");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("TestId");

                    b.ToTable("TestCompletions");
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.Answer", b =>
                {
                    b.HasOne("EducationPortal.Web.Data.Entities.Question", "Question")
                        .WithMany("Answers")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.AnswerHistory", b =>
                {
                    b.HasOne("EducationPortal.Web.Data.Entities.AnswerHistoryData", "AnswerHistoryData")
                        .WithMany("AnswerHistories")
                        .HasForeignKey("AnswerHistoryDataId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EducationPortal.Web.Data.Entities.Answer", "Answer")
                        .WithMany()
                        .HasForeignKey("AnswerId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.AnswerHistoryData", b =>
                {
                    b.HasOne("EducationPortal.Web.Data.Entities.Attempt", "Attempt")
                        .WithMany("AnswerHistoryData")
                        .HasForeignKey("AttemptId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("EducationPortal.Web.Data.Entities.Question", "Question")
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.Attempt", b =>
                {
                    b.HasOne("EducationPortal.Web.Data.Entities.TestCompletion", "TestCompletion")
                        .WithMany("Attempts")
                        .HasForeignKey("TestCompletionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.EducationMaterial", b =>
                {
                    b.HasOne("EducationPortal.Web.Data.Entities.Module", "Module")
                        .WithMany("EducationMaterials")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.Module", b =>
                {
                    b.HasOne("EducationPortal.Web.Data.Entities.Course", "Course")
                        .WithMany("Modules")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.Question", b =>
                {
                    b.HasOne("EducationPortal.Web.Data.Entities.Test", "Test")
                        .WithMany("Questions")
                        .HasForeignKey("TestId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.Test", b =>
                {
                    b.HasOne("EducationPortal.Web.Data.Entities.Module", "Module")
                        .WithMany("Tests")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EducationPortal.Web.Data.Entities.TestCompletion", b =>
                {
                    b.HasOne("EducationPortal.Web.Data.Entities.Test", "Test")
                        .WithMany()
                        .HasForeignKey("TestId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
