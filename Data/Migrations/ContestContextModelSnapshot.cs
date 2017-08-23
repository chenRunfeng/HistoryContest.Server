﻿// <auto-generated />
using HistoryContest.Server.Data;
using HistoryContest.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;

namespace HistoryContest.Server.Data.Migrations
{
    [DbContext(typeof(ContestContext))]
    partial class ContestContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HistoryContest.Server.Models.Entities.Administrator", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("HashedPassword");

                    b.Property<string>("Name");

                    b.Property<string>("Salt");

                    b.Property<string>("UserName");

                    b.HasKey("ID");

                    b.ToTable("Administrators");
                });

            modelBuilder.Entity("HistoryContest.Server.Models.Entities.AQuestionBase", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<byte>("Answer");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int>("Points");

                    b.Property<string>("Question");

                    b.HasKey("ID");

                    b.ToTable("Questions");

                    b.HasDiscriminator<string>("Discriminator").HasValue("AQuestionBase");
                });

            modelBuilder.Entity("HistoryContest.Server.Models.Entities.Counselor", b =>
                {
                    b.Property<int>("ID");

                    b.Property<int>("Department");

                    b.Property<string>("Name");

                    b.Property<int>("PhoneNumber");

                    b.HasKey("ID");

                    b.HasIndex("Department");

                    b.ToTable("Counselors");
                });

            modelBuilder.Entity("HistoryContest.Server.Models.Entities.QuestionSeed", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("_questionIDs")
                        .IsRequired()
                        .HasColumnName("QuestionIDs");

                    b.HasKey("ID");

                    b.ToTable("QuestionSeeds");
                });

            modelBuilder.Entity("HistoryContest.Server.Models.Entities.Student", b =>
                {
                    b.Property<int>("ID");

                    b.Property<int>("CardID");

                    b.Property<int>("CounselorID");

                    b.Property<DateTime?>("DateTimeFinished");

                    b.Property<string>("Name");

                    b.Property<int?>("QuestionSeedID");

                    b.Property<int?>("Score");

                    b.Property<long?>("State");

                    b.Property<TimeSpan?>("TimeConsumed");

                    b.HasKey("ID");

                    b.HasIndex("CounselorID");

                    b.HasIndex("QuestionSeedID");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("HistoryContest.Server.Models.Entities.ChoiceQuestion", b =>
                {
                    b.HasBaseType("HistoryContest.Server.Models.Entities.AQuestionBase");

                    b.Property<string>("ChoiceA");

                    b.Property<string>("ChoiceB");

                    b.Property<string>("ChoiceC");

                    b.Property<string>("ChoiceD");

                    b.ToTable("Questions");

                    b.HasDiscriminator().HasValue("ChoiceQuestion");
                });

            modelBuilder.Entity("HistoryContest.Server.Models.Entities.TrueFalseQuestion", b =>
                {
                    b.HasBaseType("HistoryContest.Server.Models.Entities.AQuestionBase");


                    b.ToTable("Questions");

                    b.HasDiscriminator().HasValue("TrueFalseQuestion");
                });

            modelBuilder.Entity("HistoryContest.Server.Models.Entities.Student", b =>
                {
                    b.HasOne("HistoryContest.Server.Models.Entities.Counselor", "Counselor")
                        .WithMany("Students")
                        .HasForeignKey("CounselorID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HistoryContest.Server.Models.Entities.QuestionSeed", "QuestionSeed")
                        .WithMany()
                        .HasForeignKey("QuestionSeedID");
                });
#pragma warning restore 612, 618
        }
    }
}
