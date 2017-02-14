using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Training;

namespace Trainer.Tests
{

    [TestClass]
    public class WorkoutTests
    {
        User _user;

        private void CreateFemaleAthlete()
        {
            var today = DateTime.Now;
            _user = new User(1, "kaseyu", 25, 150, 71, Gender.Female);
            var workout = new Workout(1, new TimeSpan(0, 25, 3), 93,
                                      today.AddDays(-1),
                                      _user,
                                      "Bodyweight circuit Wednesday!");
            var workout2 = new BikeWorkout(2, new TimeSpan(0, 32, 8), 153,
                                           today.AddDays(-3), 3.01,
                                           _user,
                                           "Run around the lake");
            var workout3 = new BikeWorkout(3, new TimeSpan(2, 16, 34), 151,
                                           today.AddDays(-4), 14.27,
                                           _user,
                                           "Biking to Red Hook!");
            _user.AddWorkout(workout, workout2, workout3);
        }

        private void CreateNullAthlete()
        {
            _user = null;
        }

        private void CreateMaleAthlete()
        {
            var today = DateTime.Now;
            _user = new User(2, "eweb", 27, 208, 72.5, Gender.Male);
            var workout = new BikeWorkout(1, new TimeSpan(0, 32, 5), 128,
                                      today.AddDays(-2),
                                      3.14,
                                      _user,
                                      "Running with the puppy!");
            var workout2 = new Workout(1, new TimeSpan(0, 55, 14), 113,
                                      today.AddDays(-3),
                                      _user,
                                      "Pumping iron.");
            _user.AddWorkout(workout, workout2);
        }


        [TestMethod]
        public void TestAddWorkoutNull()
        {
            CreateNullAthlete();
            var today = DateTime.Now;
            Assert.ThrowsException<ArgumentNullException>(() => new BikeWorkout(1, new TimeSpan(0, 32, 5),
                                                                                128,
                                                                                today.AddDays(-2),
                                                                                3.14,
                                                                                _user,
                                                                                "Running with the puppy!"));
        }

        [TestMethod]
        public void TestAddWorkoutFemale()
        {
            CreateFemaleAthlete();
            Assert.AreEqual(_user.Workouts.Count, 3);
        }

        [TestMethod]
        public void TestAddAnotherWorkoutFemale()
        {
            CreateFemaleAthlete();
            var w = new Workout(4, new TimeSpan(0, 14, 23), 85, new DateTime(2017, 2, 10), _user, "meh workout");
            _user.AddWorkout(w);
            Assert.AreEqual(_user.Workouts.Count, 4);
        }

        [TestMethod]
        public void TestAddWorkoutMale()
        {
            CreateMaleAthlete();
            Assert.AreEqual(_user.Workouts.Count, 2);
        }

        [TestMethod]
        public void TestHealthStatusFemale()
        {
            CreateFemaleAthlete();
            var score = _user.GetWeekHealthStatus();
            var avg = (93 + 153 + 151) / 3.0;
            var duration = new TimeSpan(3, 13, 45).TotalHours;
            var nominator = (-20.4022 + (0.4472 * avg) - (0.1263 * _user.Weight) + (0.074 * _user.Age) / 4.184) * 60 * duration;
            var actual = nominator / _user.BMR;
            Assert.AreEqual(score, actual, 0.00000001);
        }

        [TestMethod]
        public void TestHealthStatusMale()
        {
            CreateMaleAthlete();
            var score = _user.GetWeekHealthStatus();
            var avg = (128 + 113) / 2.0;
            var duration = new TimeSpan(1, 27, 19).TotalHours;
            var nominator = (-55.0969 + (0.6309 * avg) + (0.1988 * _user.Weight) + (0.2017 * _user.Age) / 4.184) * 60 * duration;
            var actual = nominator / _user.BMR;
            Assert.AreEqual(score, actual, 0.00000001);
        }

        [TestMethod]
        public void TestToStringFemale()
        {
            CreateFemaleAthlete();
            var str = _user.Workouts.First().ToString();
            Assert.AreEqual($"{DateTime.Now.Date.AddDays(-1)}: Bodyweight circuit Wednesday! (25 minutes)", str);
        }

        [TestMethod]
        public void TestToStringFemaleBike()
        {
            CreateFemaleAthlete();
            var str = _user.Workouts.Skip(1).First().ToString();
            Assert.AreEqual($"{DateTime.Now.Date.AddDays(-3)}: Run around the lake (32 minutes, 3.01 miles)", str);
        }

        [TestMethod]
        public void TestBestWorkout1()
        {
            CreateFemaleAthlete();
            var best = _user.GetBestWorkoutThisWeek();
            var workout = new BikeWorkout(3, new TimeSpan(2, 16, 34), 151,
                                          DateTime.Now.AddDays(-4), 14.27,
                                          _user,
                                          "Biking to Red Hook!");
            Assert.AreEqual((workout, 3), best);
        }
    }
}

