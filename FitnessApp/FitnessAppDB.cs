namespace FitnessApp
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class FitnessAppDb : DbContext
    {
        public FitnessAppDb()
            : base("name=FitnessAppDB")
        {
        }

        public virtual DbSet<Exercise> Exercises { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<Workout> Workouts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Exercise>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<UserProfile>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<UserProfile>()
                .Property(e => e.Password)
                .IsUnicode(false);
        }
    }
}
