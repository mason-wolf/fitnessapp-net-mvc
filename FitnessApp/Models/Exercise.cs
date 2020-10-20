namespace FitnessApp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Exercise")]
    public partial class Exercise
    {
        public int ExerciseId { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public int? Sets { get; set; }

        public int? Reps { get; set; }

        public int? WorkoutId { get; set; }

        public virtual Workout Workout { get; set; }
    }
}
