using System;
using System.Collections.Generic;
using System.Linq;

namespace Training
{

    public class User
    {
        public string Username { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public Gender Gender { get; set; }
        public int MHR { get; }
        public double BMR { get; }
        public List<Workout> Workouts { get; set; }
        public int Id { get; set; }

        public User(int id, string username, int age, double weight, double height, Gender gender)
        {
            Id = id;
            Username = username;
            Age = age;
            Weight = weight;
            Height = height;
            Gender = gender;
            MHR = 220 - age;
            BMR = BasalMetabolicRate();
        }

        public void AddWorkout(params Workout[] w)
        {
            if (Workouts != null)
            {
                Workouts.AddRange(w);
            }
            else
            {
                Workouts = w.ToList();
            }
        }

        // Return the fraction of average calories burned in the week
        // from workouts over the basal metabolic rate calories for the 
        // user
        public double GetWeekHealthStatus()
        {
            var today = DateTime.Now;
            var thisWeek = Workouts.Where(w => w.Date > today.AddDays(-7)).ToList();

            var totalDuration = thisWeek.Sum(w => w.Duration.TotalHours);
            var averageHeartRate = thisWeek.Select(w => w.AHR).Average();

            var avgCaloriesBurned = CalculateCalories(averageHeartRate, totalDuration);
            var healthMetric = avgCaloriesBurned / BMR;

            return healthMetric;
        }

        //http://www.shapesense.com/fitness-exercise/calculators/heart-rate-based-calorie-burn-calculator.shtml
        // Determine number of calories burned in a workout based on 
        // heart rate when VO2Max is unknown
        private double CalculateCalories(double averageHeartRate, double totalDuration)
        {
            switch (Gender)
            {
                case Gender.Male:
                    return (-55.0969 + (0.6309 * averageHeartRate) + (0.1988 * Weight) + (0.2017 * Age) / 4.184) * 60 * totalDuration;
                case Gender.Female:
                    return (-20.4022 + (0.4472 * averageHeartRate) - (0.1263 * Weight) + (0.074 * Age) / 4.184) * 60 * totalDuration;
                default:
                    return 0.0;
            }
        }

        public Workout GetBestWorkoutThisWeek()
        {
            var week = Workouts.Where(w => w.Date > DateTime.Now.Date.AddDays(-7));
            var longest = week.Aggregate((w1, w2) => w1.Duration > w2.Duration ? w1 : w2);
            var vigorous = week.Where(w => w.Level == Intensity.Vigorous).Aggregate((w1, w2) => w1.Duration > w2.Duration ? w1 : w2);
            return vigorous ?? longest;
        }

        // DEMO: Introduce local for weight and height calculations
        // http://www.globalrph.com/harris-benedict-equation.htm
        // Determine amount of energy required to maintain normal metabolic 
        // activity via the Harris-Benedict Equation (in kg and cm)
        private double BasalMetabolicRate()
        {
            switch (Gender)
            {
                case Gender.Male:
                    return 66.47 + (13.75 * Weight * 0.453592) + (5.003 * Height * 2.54) - (6.755 * Age);
                case Gender.Female:
                    return 655.1 + (9.563 * Weight * 0.453592) + (1.850 * Height * 2.54) - (4.676 * Age);
                default:
                    return 0.0;
            }
        }
    }

    public enum Gender
    {
        Male, Female
    }
}