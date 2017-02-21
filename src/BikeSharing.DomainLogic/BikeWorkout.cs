using System;
using System.Collections.Generic;
using System.Linq;
using BikeSharing.DomainLogic;

namespace Training
{

    public interface IWorkout
    {
        DateTime Date { get; }
        TimeSpan Duration { get; }
        IHeartRate HeartRate { get; }
        string Notes { get; set; }
    }

    public interface IHeartRate
    {
        double GetHeartRate();
    }

    public interface IDistanceWorkout : IWorkout
    {
        double Distance { get; }
        double Pace { get; }
    }

    public interface IWorkoutType
    {
        WorkoutType Type { get; }
    }

    public interface IBikeWorkout : IDistanceWorkout, IWorkout
    {

    }

    public class HeartRate : IHeartRate
    {
        private double Rate;
        public double GetHeartRate() => Rate;

        public HeartRate(double rate) => this.Rate = rate;

    }

    public class Workout : IWorkout
    {
        private DateTime date;
        private TimeSpan duration;
        private HeartRate rate;
        private string notes;

        public Workout(DateTime datetime, TimeSpan duration, HeartRate rate, string notes)
        {
            this.date = datetime;
            this.duration = duration;
            this.rate = rate;
            if (notes == null)
            {
                throw new ArgumentNullException(nameof(notes));
            }
            this.notes = notes;
        }

        public DateTime Date => date;
        public TimeSpan Duration => duration;
        public IHeartRate HeartRate => rate;
        public string Notes { get => notes; set => notes = value; }
    }

    public class DistanceWorkout : IDistanceWorkout
    {
        private double distance;
        private TimeSpan duration;
        private DateTime date;
        private HeartRate rate;
        private string notes;

        public DistanceWorkout(double distance, DateTime date, TimeSpan duration,  HeartRate heartRate, string notes)
        {
            this.distance = distance;
            this.duration = duration;
            this.date = date;
            this.rate = heartRate;
            this.notes = notes;
        }

        public double Distance => distance;
        public TimeSpan Duration => duration;
        public double Pace => distance / duration.TotalHours;
        public DateTime Date => date;
        public IHeartRate HeartRate => rate;
        public string Notes { get => notes; set => notes = value; }
    }

    public class BikeWorkout : IBikeWorkout
    {
        private double distance;
        private TimeSpan duration;
        private DateTime date;
        private HeartRate rate;
        private string notes;

        public BikeWorkout(double distance, DateTime date, TimeSpan duration, HeartRate rate, string notes)
        {
            this.date = date;
            this.distance = distance;
            this.duration = duration;
            if (rate == null)
            {
                throw new ArgumentNullException(nameof(rate));
            }
            this.rate = rate;
            this.notes = notes;
            
        }

        public double Distance => distance;
        public TimeSpan Duration => duration;
        public double Pace => distance / duration.TotalHours;
        public DateTime Date => date;
        public IHeartRate HeartRate => rate;
        public string Notes { get => notes; set => notes = value; }
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
        public List<IWorkout> Workouts { get; set; }

        public Athlete(string username, Gender gender, int age, double weight, double height)
        {
            Username = username;
            Gender = gender;
            Age = age;
            Weight = weight;
            Height = height;
            BasalMetabolicRate = GetBasalMetabolicRate();
        }

        public Athlete(string username, Gender gender, int age, double weight, double height, List<IWorkout> workouts)
        {
            Username = username;
            Gender = gender;
            Age = age;
            Weight = weight;
            Height = height;
            BasalMetabolicRate = GetBasalMetabolicRate();
            Workouts = workouts;
        }

        public void AddWorkout(params IWorkout[] w)
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

        public double GetCaloriesBurned(IWorkout workout)
        {
            //var a = (Gender)5;
            switch (Gender)
            {
                case Gender.Male:
                    return (-55.0969 + (0.6309 * workout.HeartRate.GetHeartRate()) 
                           + (0.1988 * Weight) + (0.2017 * Age) / 4.184) 
                           * 60 * workout.Duration.TotalHours;
                case Gender.Female:
                    return (-20.4022 + (0.4472 * workout.HeartRate.GetHeartRate()) 
                           - (0.1263 * Weight) + (0.074 * Age) / 4.184) 
                           * 60 * workout.Duration.TotalHours;
                default:
                    return 0.0;
            }
        }

        public IWorkout GetWeeksBestWorkout()
        {
            var week = Workouts.Where(w => w.Date > DateTime.Now.Date.AddDays(-7));
            return week.Aggregate((w1, w2) => w1.HeartRate.GetHeartRate() > w2.HeartRate.GetHeartRate() ? w1 : w2);
        }

        public (bool success, string message) TweetifyTodaysWorkout()
        {
            if (Workouts != null)
            {
                var todaysWorkout = Workouts.Where(w => w.Date.Date == DateTime.Now.Date).FirstOrDefault();
                if (todaysWorkout != null)
                {
                    var bike = todaysWorkout as IBikeWorkout;
                    if (bike != null)
                    {
                        var bikeMessage = $"I biked {bike.Distance:0.0} miles @ {bike.Pace:0.0} mph. {bike.Notes}";
                        return (true, Tweetify(bikeMessage));
                    }

                    var dist = todaysWorkout as IDistanceWorkout;
                    if (dist != null)
                    {
                        var distanceMessage = $"I crushed {dist.Distance:0.0} miles @ {dist.Pace:0.0} mph. {dist.Notes}";
                        return (true, Tweetify(distanceMessage));
                    }

                    var defaultMessage = $"I worked out for {todaysWorkout.Duration.TotalMinutes:0} minutes. {todaysWorkout.Notes}";
                    return (true, Tweetify(defaultMessage));
                }
            }
            return (false, null);
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
                var hashtag = HASHTAGS[r];
                if (hashtag.Length > charsLeft)
                {
                    charsLeft = 0;
                }
                else
                {
                    hashes += " " + hashtag;
                    charsLeft -= hashes.Length;
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