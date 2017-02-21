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
            var w = new BikeWorkout(8.21, DateTime.Now, new TimeSpan(1, 7, 23), new HeartRate(106), "Test drove the new bike around Greenlake!");
            var w2 = new Workout(DateTime.Now.AddDays(-2), new TimeSpan(0, 14, 3), new HeartRate(124), "Single leg squats FTW!");
            athlete.AddWorkout(w, w2);
        }

        public void CreateMaleAthleteWithWorkouts()
        {
            athlete = new Athlete("eweber", Gender.Male, 27, 201, 72);
            var w = new BikeWorkout(8.21, DateTime.Now, new TimeSpan(1, 7, 23), new HeartRate(113), "Learning how to bike in the streets :O");
            var w2 = new Workout(DateTime.Now.AddDays(-4), new TimeSpan(1, 2, 43), new HeartRate(132), "500 lb squat day. #gainz");
            athlete.AddWorkout(w, w2);
        }

        [TestMethod]
        public void TestAthleteAddWorkout()
        {
            CreateFemaleAthleteWithWorkouts();
            var w = new DistanceWorkout(.99, DateTime.Now.AddDays(-6), new TimeSpan(0, 20, 24), new HeartRate(125), "Meh. Light jog on treadmill...");
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
                var w = new Workout(DateTime.Now, new TimeSpan(0, 13, 0), new HeartRate(84), null);
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
        public void TestAthleteBikeTweet()
        {
            CreateMaleAthleteNoWorkout();
            var w = new BikeWorkout(8.21, DateTime.Now, new TimeSpan(1, 7, 23), new HeartRate(113), "Learning how to bike in the streets :O");
            athlete.AddWorkout(w);
            var result = athlete.TweetifyTodaysWorkout();
            //Assert.AreEqual();
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
            return new BikeWorkout(21.6, DateTime.Now.AddDays(-5), new TimeSpan(1, 23, 14),  new HeartRate(117), "Biking to Red Hook Brewery on the Burke-Gilman. What a day to be alive!");
        }

        private IWorkout CreateDistanceWorkout()
        {
            return new DistanceWorkout(5.2,  DateTime.Now.AddDays(-2),new TimeSpan(0, 37, 20), new HeartRate(112), "5K run around Greenlake with the bf ;)");
        }

        private IWorkout CreateWorkout()
        {
            return new Workout(DateTime.Now, new TimeSpan(0,25,0), new HeartRate(93), "Pumpin' some iron.");
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
            Assert.AreEqual(117, bike.HeartRate.GetHeartRate());
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
            Assert.AreEqual(112, distance.HeartRate.GetHeartRate());
        }

        [TestMethod]
        public void TestWorkoutNotes()
        {
            Create("workout");
            var workout = (Workout)_workout;
            Assert.AreEqual(18, workout.Notes.Length);
        }
    } 


    //[TestClass]
    //public class WorkoutTests
    //{
    //    User _user;

    //    private void CreateFemaleAthlete()
    //    {
    //        var today = DateTime.Now;
    //        _user = new User(1, "kaseyu", 25, 150, 71, Gender.Female);
    //        var workout = new Workout2(1, new TimeSpan(0, 25, 3), 93,
    //                                  today.AddDays(-1),
    //                                  _user,
    //                                  "Bodyweight circuit Wednesday!");
    //        var workout2 = new BikeWorkout(2, new TimeSpan(0, 32, 8), 153,
    //                                       today.AddDays(-3), 3.01,
    //                                       _user,
    //                                       "Run around the lake");
    //        var workout3 = new BikeWorkout(3, new TimeSpan(2, 16, 34), 151,
    //                                       today.AddDays(-4), 14.27,
    //                                       _user,
    //                                       "Biking to Red Hook!");
    //        _user.AddWorkout(workout, workout2, workout3);
    //    }

    //    private void CreateNullAthlete()
    //    {
    //        _user = null;
    //    }

    //    private void CreateMaleAthlete()
    //    {
    //        var today = DateTime.Now;
    //        _user = new User(2, "eweb", 27, 208, 72.5, Gender.Male);
    //        var workout = new BikeWorkout(1, new TimeSpan(0, 32, 5), 128,
    //                                  today.AddDays(-2),
    //                                  3.14,
    //                                  _user,
    //                                  "Running with the puppy!");
    //        var workout2 = new Workout2(1, new TimeSpan(0, 55, 14), 113,
    //                                  today.AddDays(-3),
    //                                  _user,
    //                                  "Pumping iron.");
    //        _user.AddWorkout(workout, workout2);
    //    }


    //    [TestMethod]
    //    public void TestAddWorkoutNull()
    //    {
    //        CreateNullAthlete();
    //        var today = DateTime.Now;
    //        Assert.ThrowsException<ArgumentNullException>(() => new BikeWorkout(1, new TimeSpan(0, 32, 5),
    //                                                                            128,
    //                                                                            today.AddDays(-2),
    //                                                                            3.14,
    //                                                                            _user,
    //                                                                            "Running with the puppy!"));
    //    }

    //    [TestMethod]
    //    public void TestAddWorkoutFemale()
    //    {
    //        CreateFemaleAthlete();
    //        Assert.AreEqual(_user.Workouts.Count, 3);
    //    }

    //    [TestMethod]
    //    public void TestAddAnotherWorkoutFemale()
    //    {
    //        CreateFemaleAthlete();
    //        var w = new Workout2(4, new TimeSpan(0, 14, 23), 85, new DateTime(2017, 2, 10), _user, "meh workout");
    //        _user.AddWorkout(w);
    //        Assert.AreEqual(_user.Workouts.Count, 4);
    //    }

    //    [TestMethod]
    //    public void TestAddWorkoutMale()
    //    {
    //        CreateMaleAthlete();
    //        Assert.AreEqual(_user.Workouts.Count, 2);
    //    }

    //    [TestMethod]
    //    public void TestHealthStatusFemale()
    //    {
    //        CreateFemaleAthlete();
    //        var score = _user.GetWeekHealthStatus();
    //        var avg = (93 + 153 + 151) / 3.0;
    //        var duration = new TimeSpan(3, 13, 45).TotalHours;
    //        var nominator = (-20.4022 + (0.4472 * avg) - (0.1263 * _user.Weight) + (0.074 * _user.Age) / 4.184) * 60 * duration;
    //        var actual = nominator / _user.BMR;
    //        Assert.AreEqual(score, actual, 0.00000001);
    //    }

    //    [TestMethod]
    //    public void TestHealthStatusMale()
    //    {
    //        CreateMaleAthlete();
    //        var score = _user.GetWeekHealthStatus();
    //        var avg = (128 + 113) / 2.0;
    //        var duration = new TimeSpan(1, 27, 19).TotalHours;
    //        var nominator = (-55.0969 + (0.6309 * avg) + (0.1988 * _user.Weight) + (0.2017 * _user.Age) / 4.184) * 60 * duration;
    //        var actual = nominator / _user.BMR;
    //        Assert.AreEqual(score, actual, 0.00000001);
    //    }

    //    [TestMethod]
    //    public void TestToStringFemale()
    //    {
    //        CreateFemaleAthlete();
    //        var str = _user.Workouts.First().ToString();
    //        Assert.AreEqual($"{DateTime.Now.Date.AddDays(-1)}: Bodyweight circuit Wednesday! (25 minutes)", str);
    //    }

    //    [TestMethod]
    //    public void TestToStringFemaleBike()
    //    {
    //        CreateFemaleAthlete();
    //        var str = _user.Workouts.Skip(1).First().ToString();
    //        Assert.AreEqual($"{DateTime.Now.Date.AddDays(-3)}: Run around the lake (32 minutes, 3.01 miles)", str);
    //    }

    //    [TestMethod]
    //    public void TestBestWorkout1()
    //    {
    //        CreateFemaleAthlete();
    //        var best = _user.GetBestWorkoutThisWeek();
    //        var workout = new BikeWorkout(3, new TimeSpan(2, 16, 34), 151,
    //                                      DateTime.Now.AddDays(-4), 14.27,
    //                                      _user,
    //                                      "Biking to Red Hook!");
    //        Assert.AreEqual((workout, 3), best);
    //    }
    //}
}

