using BikeSharing.DomainLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Training;

namespace Trainer.Tests
{

    [TestClass]
    public class TestAthlete
    {
        Athlete athlete;

        public void CreateFemaleAthleteNoWorkout()
        {
            athlete = new Athlete("kaseyu", Gender.Female, 25, 155, 71);
        }

        public void CreateMaleAthleteNoWorkout()
        {
            athlete = new Athlete("eweber", Gender.Male, 27, 201, 72);
        }

        public void CreateFemaleAthleteWithWorkouts()
        {
            athlete = new Athlete("kaseyu", Gender.Female, 25, 155, 71);
            var w = new BikeWorkout(WorkoutType.Outdoor, 8.21, DateTime.Now, new TimeSpan(1, 7, 23), 106, "Test drove the new bike around Greenlake!");
            var w2 = new Workout(DateTime.Now.AddDays(-2), new TimeSpan(0, 14, 3), 124, "Single leg squats FTW!");
            athlete.AddWorkout(w, w2);
        }

        public void CreateMaleAthleteWithWorkouts()
        {
            athlete = new Athlete("eweber", Gender.Male, 27, 201, 72);
            var w = new BikeWorkout(WorkoutType.Outdoor, 8.21, DateTime.Now, new TimeSpan(1, 7, 23), 113, "Learning how to bike in the streets :O");
            var w2 = new Workout(DateTime.Now.AddDays(-4), new TimeSpan(1, 2, 43), 132, "500 lb squat day. #gainz");
            athlete.AddWorkout(w, w2);
        }

        [TestMethod]
        public void TestAthleteAddWorkout()
        {
            CreateFemaleAthleteWithWorkouts();
            var w = new DistanceWorkout(.99, DateTime.Now.AddDays(-6), new TimeSpan(0, 20, 24), 125, "Meh. Light jog on treadmill...");
            athlete.AddWorkout(w);
            Assert.AreEqual(3, athlete.Workouts.Count);
        }

        [TestMethod]
        public void TestAthleteNotes()
        {
            CreateFemaleAthleteWithWorkouts();
            Assert.AreEqual(41, athlete.Workouts.First().Notes.Length);
        }

        [TestMethod]
        public void TestAthleteTweetTodaySuccess()
        {
            CreateFemaleAthleteWithWorkouts();
            var result = athlete.TweetifyTodaysWorkout();
            Assert.AreEqual(true, result.success);
        }

        [TestMethod]
        public void TestAthleteTweetTodayMessage()
        {
            CreateFemaleAthleteWithWorkouts();
            var result = athlete.TweetifyTodaysWorkout();
            Assert.AreEqual(120, result.message.Length, 20);
        }

        [TestMethod]
        public void TestWorkoutNotesNull()
        {
            CreateFemaleAthleteNoWorkout();
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var w = new Workout(DateTime.Now, new TimeSpan(0, 13, 0), 84, null);
                athlete.AddWorkout(w);
                athlete.TweetifyTodaysWorkout();
            });
        }

        [TestMethod]
        public void TestAthleteTweetTodayMessageEmpty()
        {
            CreateMaleAthleteNoWorkout();
            var result = athlete.TweetifyTodaysWorkout();
            Assert.AreEqual((false, null), result);
        }

        [TestMethod]
        public void TestAthleteTweetTodayMessageNoToday()
        {
            CreateMaleAthleteNoWorkout();
            var w = new DistanceWorkout(1.5, DateTime.Now.AddDays(-4), new TimeSpan(0, 13, 0), 84, null);
            athlete.AddWorkout(w);
            var result = athlete.TweetifyTodaysWorkout();
            Assert.AreEqual((false, null), result);
        }

        [TestMethod]
        public void TestAthleteBikeTweet()
        {
            CreateMaleAthleteNoWorkout();
            var w = new BikeWorkout(WorkoutType.Outdoor, 8.21, DateTime.Now, new TimeSpan(1, 7, 23), 113, "Learning how to bike in the streets :O");
            athlete.AddWorkout(w);
            var result = athlete.TweetifyTodaysWorkout();
            //Assert.AreEqual();
        }

        [TestMethod]
        public void TestGetCaloriesBurnedNullTimeSpan()
        {
            CreateMaleAthleteNoWorkout();
            var w = new BikeWorkout(WorkoutType.Outdoor, 8.21, DateTime.Now, TimeSpan.Zero, 93, "Test drove the new bike around Greenlake!");
            athlete.AddWorkout(w);
            var calories = athlete.GetCaloriesBurned(w);
        }
    }


    [TestClass]
    public class TestIWorkout
    {
        IWorkout _workout;

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

        private IWorkout CreateBikeWorkout()
        {
            return new BikeWorkout(WorkoutType.Outdoor, 21.6, DateTime.Now.AddDays(-5), new TimeSpan(1, 23, 14),  117, "Biking to Red Hook Brewery on the Burke-Gilman. What a day to be alive!");
        }

        private IWorkout CreateDistanceWorkout()
        {
            return new DistanceWorkout(5.2, DateTime.Now.AddDays(-2),new TimeSpan(0, 37, 20), 112, "5K run around Greenlake with the bf ;)");
        }

        private IWorkout CreateWorkout()
        {
            return new Workout(DateTime.Now, new TimeSpan(0,25,0), 93, "Pumpin' some iron.");
        }

        [TestMethod]
        public void TestBikeWorkoutPace()
        {
            Create("bike");
            var bike = (BikeWorkout)_workout;
            Assert.AreEqual(15.570684, bike.Pace, .000001);
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
            Assert.AreEqual(8.3571428, distance.Pace, .000001);
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

