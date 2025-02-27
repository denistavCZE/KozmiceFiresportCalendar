﻿using FiresportCalendar.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FiresportCalendar.Data
{
    public class ApplicationDbContext : IdentityDbContext<Person>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Race> Races { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<TeamRace> TeamRaces { get; set; }
        public DbSet<EventPerson> EventPeople { get; set; }
        public DbSet<TeamRacePerson> TeamRacePeople { get; set; }
        public DbSet<League> Leagues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // League
            modelBuilder.Entity<League>()
                .HasKey(l => l.Id);

            // Race
            modelBuilder.Entity<Race>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Race>()
                .HasOne(r => r.League)
                .WithMany(l => l.Races)
                .HasForeignKey(r => r.LeagueId)
                .OnDelete(DeleteBehavior.Cascade);


            // Team
            modelBuilder.Entity<Team>()
                .HasKey(t => t.Id);

            // User is already defined in IdentityDbContext

            // Event
            modelBuilder.Entity<Event>()
                .HasKey(e => e.Id);

            // TeamRace
            modelBuilder.Entity<TeamRace>()
                .HasKey(tr => new { tr.TeamId, tr.RaceId });

            modelBuilder.Entity<TeamRace>()
                .HasOne(tr => tr.Team)
                .WithMany(t => t.TeamRaces)
                .HasForeignKey(tr => tr.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            

            modelBuilder.Entity<TeamRace>()
                .HasOne(tr => tr.Race)
                .WithMany(r => r.TeamRaces)
                .HasForeignKey(tr => tr.RaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // EventUser
            modelBuilder.Entity<EventPerson>()
                .HasKey(eu => new { eu.EventId, eu.PersonId });

            modelBuilder.Entity<EventPerson>()
                .HasOne(eu => eu.Event)
                .WithMany(e => e.EventPeople)
                .HasForeignKey(eu => eu.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventPerson>()
                .HasOne(eu => eu.Person)
                .WithMany(u => u.EventPeople)
                .HasForeignKey(eu => eu.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            // TeamRaceUser
            modelBuilder.Entity<TeamRacePerson>()
                .HasKey(trp => new { trp.TeamId, trp.RaceId, trp.PersonId });

            modelBuilder.Entity<TeamRacePerson>()
                .HasOne(trp => trp.TeamRace)
                .WithMany(tr => tr.TeamRacePeople)
                .HasForeignKey(trp => new { trp.TeamId, trp.RaceId })
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamRacePerson>()
                .HasOne(trp => trp.Person)
                .WithMany(u => u.TeamRacePeople)
                .HasForeignKey(trp => trp.PersonId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<TeamRacePerson>()
                .Property(trp => trp.Position)
                .IsRequired();

            // Team to User (Many-to-Many)
            modelBuilder.Entity<Team>()
                .HasMany(t => t.People)
                .WithMany(p => p.Teams)
                .UsingEntity(j => j.ToTable("TeamPeople"));

            // Team to League (Many-to-Many)
            modelBuilder.Entity<Team>()
                .HasMany(t => t.Leagues)
                .WithMany(l => l.Teams)
                .UsingEntity(j => j.ToTable("TeamLeagues"));
        }
    }
}
