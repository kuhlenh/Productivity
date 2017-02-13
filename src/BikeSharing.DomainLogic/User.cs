using System;
using System.Collections.Generic;
using System.Linq;
using BikeSharing.DomainLogic;

namespace Training
{

    public class User
    {
        public string Username { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public int MHR { get; }
        public double BMR { get; }
        public Gender Sex { get; set; }
        public List<Workout> Workouts { get; set; }
        public int Id { get; set; }

        public User(int id, string username, int age, double weight, double height, Gender gender)
        {
            Id = id;
            Username = username;
            Age = age;
            Weight = weight;
            Height = height;
            MHR = 220 - age;
            BMR = BasalMetabolicRate();
            Sex = gender;
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
            switch (Sex)
            {
                case Gender.Male:
                    return (-55.0969 + (0.6309 * averageHeartRate) + (0.1988 * Weight) + (0.2017 * Age) / 4.184) * 60 * totalDuration;
                case Gender.Female:
                    return (-20.4022 + (0.4472 * averageHeartRate) - (0.1263 * Weight) + (0.074 * Age) / 4.184) * 60 * totalDuration;
                default:
                    return 0.0;
            }
        }

        // DEMO: Introduce local for weight and height calculations
        // http://www.globalrph.com/harris-benedict-equation.htm
        // Determine amount of energy required to maintain normal metabolic 
        // activity via the Harris-Benedict Equation (in kg and cm)
        private double BasalMetabolicRate()
        {
            switch (Sex)
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
}

namespace BikeSharing.DomainLogic
{
    public enum Gender
    {
        Male, Female
    }
}



//namespace Trainer
//{
//	public class Trainer
//	{
//		private List<WorkOut> _workOuts;
//		public int Goal { get; set; }

//		public int MilesTravelled
//		{
//			get
//			{
//				int count = 0;
//				foreach (var work in _workOuts)
//				{
//					count += work.Miles;
//				}
//				return count;
//			}
//		}

//		public Trainer(int goal)
//		{
//			_workOuts = new List<WorkOut>();
//			Goal = goal; ;
//		}

//		public Trainer()
//		{
//			_workOuts = new List<WorkOut>();
//		}

//		public void RegisterWorkout(int miles, TimeSpan duration, string notes)
//		{
//			_workOuts.Add(new WorkOut(miles, duration, notes));
//		}

//		public bool HasMetGoal()
//		{
//			if (MilesTravelled == Goal)
//			{
//				return true;
//			}
//			return false;
//		}

//		public static double GetMilePace(WorkOut workout) => workout.Duration.TotalMinutes / (double)workout.Miles;

//		public WorkOut GetMostMilesTraveled()
//		{
//			int mostMiles = 0;
//			WorkOut FurthestWorkout = null;
//			foreach (var workout in _workOuts)
//			{
//				if (workout.Miles > mostMiles)
//				{
//					FurthestWorkout = workout;
//					mostMiles = workout.Miles;
//				}
//			}
//			return FurthestWorkout;
//		}

//		public static Intensity GetWorkoutIntensity(WorkOut workout)
//		{
//			double milePace = GetMilePace(workout);

//			if (workout == null)
//				return Intensity.None;

//			if (milePace < 3.5)
//				return Intensity.Hard;
//			else if (milePace < 6.0)
//				return Intensity.Medium;
//			else
//				return Intensity.Easy;
//		}

//		public int GetWorkoutIntensityCount(Intensity desiredIntensity)
//		{
//			int intensityCount = 0;
//			foreach (var workout in _workOuts)
//			{
//				var intensity = GetWorkoutIntensity(workout);
//				if (desiredIntensity == intensity)
//				{
//					intensityCount++;
//				}
//			}
//			return intensityCount;
//		}

//		public Dictionary<Intensity, int> GetAllIntensities()
//		{
//			Dictionary<Intensity, int> dictionary = new Dictionary<Intensity, int>();
//			foreach (var workout in _workOuts)
//			{
//				var intensity = GetWorkoutIntensity(workout);
//				if (dictionary.ContainsKey(intensity))
//					dictionary[intensity] += 1;
//				else
//					dictionary.Add(intensity, 1);
//			}

//			return dictionary;
//		}

//		public (Intensity, int) MostFrequentIntensity()
//		{
//			var IntensityDictionary = GetAllIntensities();
//			var highestCount = (intensity:Intensity.None, count:0);
//			foreach (var (key, val) in IntensityDictionary)
//			{
//				if (val > highestCount.Item2)
//				{
//					highestCount = (key, val);
//				}
//			}
//			return highestCount;
//		}

//		public async Task<bool> SaveIntensitySummary(string url)
//		{
//			using (StreamWriter writer = File.CreateText(url))
//			{
//				await writer.WriteLineAsync("Intensity, Count");
//				var intensities = GetAllIntensities();

//				foreach (var (k,v) in intensities)
//				{
//					await writer.WriteLineAsync(string.Format("{0},{1}", k, v));
//				}
//			}
//			return true;
//		}

//		public List<string> TweetifyWorkouts()
//		{
//			var listOfTweets = new List<string>();
//			foreach (var workout in _workOuts)
//			{
//				var intensity = GetWorkoutIntensity(workout);
//				if (intensity == Intensity.Easy || intensity == Intensity.None)
//				{
//					listOfTweets.Add("Pumping iron at the gym!");
//				}
//				else
//				{
//					var buffer = 11;
//					var charRemaining = 140 - (workout.Miles.ToString().Length + 
//											   workout.Duration.Minutes.ToString().Length)
//											   - buffer;

//					var tweetReady = workout.Notes.Length < 120 ? workout.Notes : workout.Notes.Substring(0, 120);
//					listOfTweets.Add(string.Format("{0} mi/{1} min : {2}", 
//										workout.Miles, workout.Duration.Minutes, tweetReady));
//				}
//			}

//			return listOfTweets;
//		}
//	}

//	public class WorkOut
//	{
//		public string Notes;
//		public int Miles { get; }
//		public TimeSpan Duration { get; }

//		public WorkOut(int miles, TimeSpan duration, string notes)
//		{
//			Miles = miles;
//			Duration = duration;
//			Notes = notes;
//		}

//		public override string ToString()
//		{
//            return string.Format("Workout: {0} Miles, {1} Minutes", Miles, Duration.TotalMinutes);
//		}
//	}

//    public enum Intensity
//    {
//        None, Hard, Medium, Easy
//    }

//    public static class Extensions
//    {
//        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
//        {
//            key = kvp.Key;
//            value = kvp.Value;
//        }
//    }

//    public class BikeWorkout : WorkOut
//    {
//        public BikeWorkout(int miles, TimeSpan duration, string notes) : base(miles, duration, notes)
//        {
//        }
//    }
//}
