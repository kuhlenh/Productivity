using System;
using System.Collections.Generic;
using System.Linq;
using BikeSharing.DomainLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Training;

namespace Trainer.Tests
{

    [TestClass]
    public class TestAthlete
    {
        public Athlete CreateFemaleAthleteNoWorkout()
        {
            return new Athlete("kaseyu", Gender.Female, 25, 155, 71);
        }

        public Athlete CreateMaleAthleteNoWorkout()
        {
            return new Athlete("eweber", Gender.Male, 27, 201, 72);
        }

        public Athlete CreateFemaleAthleteWithWorkouts()
        {
            var athlete = new Athlete("kaseyu", Gender.Female, 25, 155, 71);
            var w = new BikeWorkout(WorkoutType.Outdoor, 8.21, DateTime.Now, TimeSpan.FromMinutes(67), 106, "Test drove the new bike around Greenlake!");
            var w2 = new Workout(DateTime.Now.AddDays(-2), new TimeSpan(0, 14, 3), 124, "Single leg squats FTW!");
            athlete.AddWorkout(w, w2);
            return athlete;
        }

        public Athlete CreateMaleAthleteWithWorkouts()
        {
            var athlete = new Athlete("eweber", Gender.Male, 27, 201, 72);
            var w = new BikeWorkout(WorkoutType.Outdoor, 8.21, DateTime.Now, TimeSpan.FromMinutes(67), 113, "Learning how to bike in the streets :O");
            var w2 = new Workout(DateTime.Now.AddDays(-4), new TimeSpan(1, 2, 43), 132, "500 lb squat day. #gainz");
            athlete.AddWorkout(w, w2);
            return athlete;
        }

        [TestMethod]
        public void TestAthleteAddWorkout()
        {
            var athlete = CreateFemaleAthleteWithWorkouts();
            var w = new DistanceWorkout(.99, DateTime.Now.AddDays(-6), TimeSpan.FromMinutes(20), 125, "Meh. Light jog on treadmill...");
            athlete.AddWorkout(w);
            Assert.AreEqual(3, athlete.Workouts.Count);
        }

        [TestMethod]
        public void TestAthleteNotes()
        {
            var athlete = CreateFemaleAthleteWithWorkouts();
            Assert.AreEqual(41, athlete.Workouts.First().Notes.Length);
        }

        [TestMethod]
        public void TestAthleteTweetTodayMessage()
        {
            var athlete = CreateFemaleAthleteWithWorkouts();
            var result = athlete.TweetTodaysWorkout();
            Assert.AreEqual(120, result.Length, 20);
        }

        [TestMethod]
        public void TestWorkoutNotesNull()
        {
            var athlete = CreateFemaleAthleteNoWorkout();
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var w = new Workout(DateTime.Now, TimeSpan.FromMinutes(13), 84, null);
                athlete.AddWorkout(w);
                athlete.TweetTodaysWorkout();
            });
        }

        [TestMethod]
        public void TestAthleteTweetTodayBikeWorkout()
        {
            var athlete = CreateFemaleAthleteNoWorkout();
            var w = new BikeWorkout(WorkoutType.Outdoor, 8.21, DateTime.Now, TimeSpan.FromMinutes(67), 125, "Learning how to bike in the streets! :O");
            athlete.AddWorkout(w);
            var result = athlete.TweetTodaysWorkout();
            Assert.AreEqual(120, result.Length, 20);
        }

        //[TestMethod]
        //public void TestAthleteTweetTodayDistanceWorkout()
        //{
        //    CreateFemaleAthleteNoWorkout();
        //    var w = new DistanceWorkout(.99, DateTime.Now, TimeSpan.FromMinutes(20), 125, "Meh. Light jog on treadmill...");
        //    athlete.AddWorkout(w);
        //    var result = athlete.TweetTodaysWorkout();
        //    Assert.AreEqual(120, result.Length,20);
        //}

        [TestMethod]
        public void TestAthleteTweetTodayMessageEmpty()
        {
            var athlete = CreateMaleAthleteNoWorkout();
            var result = athlete.TweetTodaysWorkout();
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void TestAthleteTweetTodayMessageNoToday()
        {
            var athlete = CreateMaleAthleteNoWorkout();
            var w = new DistanceWorkout(1.5, DateTime.Now.AddDays(-8), TimeSpan.FromMinutes(13), 84, "");
            athlete.AddWorkout(w);
            var result = athlete.TweetTodaysWorkout();
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void TestAthleteBikeTweet()
        {
            var athlete = CreateMaleAthleteNoWorkout();
            var w = new BikeWorkout(WorkoutType.Outdoor, 8.21, DateTime.Now, TimeSpan.FromMinutes(67), 1323, "Beautiful day to bike! Jk, fam. It's raining out here...but I enjoyed teaching my girlfriend how to be street smart when riding bikes. STP 2018 here we come!");
            athlete.AddWorkout(w);
            var result = athlete.TweetTodaysWorkout();
            Assert.AreEqual(120, result.Length, 20);
        }

        [TestMethod]
        public void TestGetCaloriesBurnedNullTimeSpan()
        {
            var athlete = CreateMaleAthleteNoWorkout();
            var w = new BikeWorkout(WorkoutType.Outdoor, 8.21, DateTime.Now, TimeSpan.Zero, 93, "Test drove the new bike around Greenlake!");
            athlete.AddWorkout(w);
            var calories = athlete.GetCaloriesBurned(w);
        }

        [TestMethod]
        public void TestGetBestWeekWorkout()
        {
            var athlete = CreateFemaleAthleteWithWorkouts();
            var bestWorkout = athlete.GetWeeksBestWorkout();
            var actual = athlete.Workouts.Where(w => w.Date.Date == DateTime.Now.Date).First();
            Assert.AreEqual(actual, bestWorkout.workout);
            Assert.AreEqual(527.066260994, bestWorkout.calories, .000001);
        }

        [TestMethod]
        public void TestTweetBestWorkoutWeek()
        {
            var athlete = CreateMaleAthleteWithWorkouts();
            var tweet = athlete.TweetBestWorkoutOfWeek();
            Assert.AreEqual(120, tweet.Length, 20);
        }
    }


    [TestClass]
    public class TestIWorkout
    {
        Workout _workout;

        public void Create(string workoutName)
        {
            switch (workoutName)
            {
                case "bike":
                    _workout = CreateBikeWorkout();
                    break;
                case "distance":
                    _workout = CreateDistanceWorkout();
                    break;
                case "workout":
                    _workout = CreateWorkout();
                    break;
                default:
                    break;

            }
        }

        private Workout CreateBikeWorkout()
        {
            return new BikeWorkout(WorkoutType.Outdoor, 21.6, DateTime.Now.AddDays(-5), TimeSpan.FromMinutes(83),  117, "Biking to Red Hook Brewery on the Burke-Gilman. What a day to be alive!");
        }

        private Workout CreateDistanceWorkout()
        {
            return new DistanceWorkout(5.2, DateTime.Now.AddDays(-2), TimeSpan.FromMinutes(37), 112, "5K run around Greenlake with the bf ;)");
        }

        private Workout CreateWorkout()
        {
            return new Workout(DateTime.Now, TimeSpan.FromMinutes(25), 93, "Pumpin' some iron.");
        }

        [TestMethod]
        public void TestBikeWorkoutPace()
        {
            Create("bike");
            var bike = (BikeWorkout)_workout;
            Assert.AreEqual(15.6144578, bike.Pace, .000001);
        }

        [TestMethod]
        public void TestBikeWorkoutNotes()
        {
            Create("bike");
            var bike = (BikeWorkout)_workout;
            Assert.AreEqual(71, bike.Notes.Length);
        }

        [TestMethod]
        public void TestBikeWorkoutHeartRate()
        {
            Create("bike");
            var bike = (BikeWorkout)_workout;
            Assert.AreEqual(117, bike.AverageHeartRate);
        }

        [TestMethod]
        public void TestDistanceWorkoutPace()
        {
            Create("distance");
            var distance = (DistanceWorkout)_workout;
            Assert.AreEqual(8.4324324, distance.Pace, .000001);
        }

        [TestMethod]
        public void TestDistanceWorkoutHeartRate()
        {
            Create("distance");
            var distance = (DistanceWorkout)_workout;
            Assert.AreEqual(112, distance.AverageHeartRate);
        }

        [TestMethod]
        public void TestWorkoutNotes()
        {
            Create("workout");
            var workout = (Workout)_workout;
            Assert.AreEqual(18, workout.Notes.Length);
        }
    }
}