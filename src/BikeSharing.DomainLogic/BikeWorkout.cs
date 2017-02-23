using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeSharing.DomainLogic;

namespace Training
{
    public class Workout
    {
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
        public double AverageHeartRate { get; set; }
        public string Notes;

        public Workout(DateTime date, TimeSpan duration, double averageHeartRate, string notes)
        {
            Date = date;
            Duration = duration;
            AverageHeartRate = averageHeartRate;
            if (notes == null)
            {
                throw new ArgumentNullException();
            }
            Notes = notes;
        }
    }

    public class DistanceWorkout : Workout
    {
        public double Distance { get; set; }
        public double Pace { get; }
        public DistanceWorkout(double distance, DateTime datetime, TimeSpan duration, double rate, string notes) : base(datetime, duration, rate, notes)
        {
            Distance = distance;
            Pace = distance / duration.TotalHours;
        }
    }

    public class BikeWorkout : DistanceWorkout
    {
        public WorkoutType Type { get; set; }

        public BikeWorkout(WorkoutType type, double distance, DateTime datetime, TimeSpan duration, double rate, string notes) : base(distance, datetime, duration, rate, notes)
        {
            Type = type;
        }
    }

    public class Athlete
    {
        private readonly string[] HASHTAGS = new string[]{
"#transformationtuesday",
"#mcm",
"#wcw",
"#fitfam",
"#fitspo",
"#fitness",
"#gymtime",
"#treadmill",
"#gainz",
"#workout",
"#getStrong",
"#getfit",
"#justdoit",
"#youcandoit",
"#bodybuilding",
"#fitspiration",
"#cardio",
"#ripped",
"#gym",
"#geekabs",
"#crossfit",
"#beachbody",
"#exercise",
"#weightraining",
"#training",
"#shredded",
"#abs",
"#sixpacks",
"#muscle",
"#strong",
"#lift",
"#weights",
"#Getfit",
"#weightloss",
"#wod",
"#aesthetic",
"#squad",
"#shreadding",
"#personaltrainer",
"#dreambitviral",
"#quote",
"#quotes",
"#inspiring",
"#motivation",
"#fitnessquote",
"#youcandoit",
"#justbringit",
"#dreambig",
"#success",
"#staypositive",
"#noexcuses"
    };

        public string Username { get; set; }
        public Gender Gender { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public double BasalMetabolicRate { get; }
        public List<Workout> Workouts { get; set; }

        public Athlete(string username, Gender gender, int age, double weight, double height)
        {
            Username = username;
            Gender = gender;
            Age = age;
            Weight = weight;
            Height = height;
            BasalMetabolicRate = GetBasalMetabolicRate();
        }

        public Athlete(string username, Gender gender, int age, double weight, double height, List<Workout> workouts)
        {
            Username = username;
            Gender = gender;
            Age = age;
            Weight = weight;
            Height = height;
            BasalMetabolicRate = GetBasalMetabolicRate();
            Workouts = workouts;
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

        private double GetBasalMetabolicRate()
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

        public double GetCaloriesBurned(Workout workout)
        {
            //var a = (Gender)5;
            switch (Gender)
            {
                case Gender.Male:
                    return (-55.0969 + (0.6309 * workout.AverageHeartRate) 
                           + (0.1988 * Weight) + (0.2017 * Age) / 4.184) 
                           * 60 * workout.Duration.TotalHours;
                case Gender.Female:
                    return (-20.4022 + (0.4472 * workout.AverageHeartRate) 
                           - (0.1263 * Weight) + (0.074 * Age) / 4.184) 
                           * 60 * workout.Duration.TotalHours;
                default:
                    return 0.0;
            }
        }

        public (Workout workout, double calories) GetWeeksBestWorkout()
        {
            var lastWeekWorkouts = Workouts.Where(w => w.Date > DateTime.Now.Date.AddDays(-7));
            var workoutWithMostCalsBurned = lastWeekWorkouts.Aggregate((w1, w2) => GetCaloriesBurned(w1) > GetCaloriesBurned(w2) ? w1 : w2);
            return (workoutWithMostCalsBurned, GetCaloriesBurned(workoutWithMostCalsBurned));
        }

        public string TweetBestWorkoutOfWeek()
        {
            var best = GetWeeksBestWorkout();
            if (best.Item2 > 200)
            {
                return Tweetify(best.Item1.Notes);
            }
            return Tweetify("Casual workout week...");
        }

        public string TweetTodaysWorkout()
        {
            if (Workouts != null)
            {
                var todaysWorkout = Workouts.Where(w => w.Date.Date == DateTime.Now.Date).FirstOrDefault();
                if (todaysWorkout != null)
                {
                    var bike = todaysWorkout as BikeWorkout;
                    if (bike != null)
                    {
                        return Tweetify($"I biked {bike.Distance:0.0} miles @ {bike.Pace:0.0} mph ({bike.Type}). {bike.Notes}");
                    }

                    var dist = todaysWorkout as DistanceWorkout;
                    if (dist != null)
                    {
                        return Tweetify($"I ran {dist.Distance:0.0} miles @ {dist.Pace:0.0} mph. {dist.Notes}");
                    }

                    return todaysWorkout.Notes.Length <= 140 ? todaysWorkout.Notes : Tweetify(todaysWorkout.Notes);
                }
            }
            return null;
        }

        private string Tweetify(string msg)
        {
            if (msg.Length >= 140)
                return msg.Substring(0, 137) + "...";
            else
                return msg + GetHashTags(140 - msg.Length);
        }

        private string GetHashTags(int charsLeft)
        {
            var hashes = "";
            var random = new Random();
            while (charsLeft > 0)
            {
                var r = random.Next(0, HASHTAGS.Length);
                var hashtag = " " + HASHTAGS[r];
                if (hashtag.Length > charsLeft)
                {
                    charsLeft = -1;
                }
                else
                {
                    charsLeft -= hashtag.Length;
                    hashes += hashtag;
                }
            }
            return hashes;
        }
    }
}

namespace BikeSharing.DomainLogic
{
    public enum WorkoutType
    {
        Indoor, Outdoor
    }

}