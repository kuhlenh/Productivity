using System;
using System.Linq;
using Training;
using Xunit;
using Assert = Xunit.Assert;

namespace BikeSharing.DomainLogic.Tests
{
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

        [Fact]
        public void TestAthleteAddWorkout()
        {
            var athlete = CreateFemaleAthleteWithWorkouts();
            var w = new DistanceWorkout(.99, DateTime.Now.AddDays(-6), TimeSpan.FromMinutes(20), 125, "Meh. Light jog on treadmill...");
            athlete.AddWorkout(w);
            Assert.Equal(3, athlete.Workouts.Count);
        }

        [Fact]
        public void TestAthleteNotes()
        {
            var athlete = CreateFemaleAthleteWithWorkouts();
            Assert.Equal(41, athlete.Workouts.First().Notes.Length);
        }

        [Fact]
        public void TestAthleteTweetTodayMessage()
        {
            var athlete = CreateFemaleAthleteWithWorkouts();
            var result = athlete.TweetTodaysWorkout();
            Assert.Equal(true, 140-result.Length >=0);
        }

        [Fact]
        public void TestWorkoutNotesNull()
        {
            var athlete = CreateFemaleAthleteNoWorkout();
            Assert.Throws<ArgumentNullException>(() =>
            {
                var w = new Workout(DateTime.Now, TimeSpan.FromMinutes(13), 84, null);
                athlete.AddWorkout(w);
                athlete.TweetTodaysWorkout();
            });
        }

        [Fact]
        public void TestAthleteTweetTodayBikeWorkout()
        {
            var athlete = CreateFemaleAthleteNoWorkout();
            var w = new BikeWorkout(WorkoutType.Outdoor, 8.21, DateTime.Now, TimeSpan.FromMinutes(67), 125, "Learning how to bike in the streets! :O");
            athlete.AddWorkout(w);
            var result = athlete.TweetTodaysWorkout();
            Assert.Equal(true, 140-result.Length >=0);
        }

        [Fact]
        public void TestAthleteTweetTodayWorkout()
        {
            var athlete = CreateFemaleAthleteNoWorkout();
            var w = new Workout(DateTime.Now, TimeSpan.FromMinutes(67), 125, "A squat a day keeps the doctor away!");
            athlete.AddWorkout(w);
            var result = athlete.TweetTodaysWorkout();
            Assert.Equal(true, 140-result.Length >=0);
        }

        [Fact]
        public void TestAthleteTweetTodayMessageEmpty()
        {
            var athlete = CreateMaleAthleteNoWorkout();
            var result = athlete.TweetTodaysWorkout();
            Assert.Equal(null, result);
        }

        [Fact]
        public void TestAthleteTweetTodayMessageNoToday()
        {
            var athlete = CreateMaleAthleteNoWorkout();
            var w = new DistanceWorkout(1.5, DateTime.Now.AddDays(-8), TimeSpan.FromMinutes(13), 84, "");
            athlete.AddWorkout(w);
            var result = athlete.TweetTodaysWorkout();
            Assert.Equal(null, result);
        }

        [Fact]
        public void TestAthleteBikeTweet()
        {
            var athlete = CreateMaleAthleteNoWorkout();
            var w = new BikeWorkout(WorkoutType.Outdoor, 8.21, DateTime.Now, TimeSpan.FromMinutes(67), 1323, "Beautiful day to bike! Jk, fam. It's raining out here...but I enjoyed teaching my girlfriend how to be street smart when riding bikes. STP 2018 here we come!");
            athlete.AddWorkout(w);
            var result = athlete.TweetTodaysWorkout();
            Assert.Equal(true, 140.0-result.Length>=0);
        }

        [Fact]
        public void TestBasalMetabolicRateMale()
        {
            var athlete = CreateMaleAthleteNoWorkout();
            var actual = athlete.BasalMetabolicRate;
            var expected = 66.47 + (13.75 * 201 * 0.453592) + (5.003 * 72 * 2.54) - (6.755 * 27);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestBasalMetabolicRateFemale()
        {
            var athlete = CreateFemaleAthleteNoWorkout();
            var actual = athlete.BasalMetabolicRate;
            var expected = 655.1 + (9.563 * 155 * 0.453592) + (1.850 * 71 * 2.54) - (4.676 * 25);
            Assert.Equal(true, Math.Abs(expected-actual) < .001);
        }

        [Fact]
        public void TestGetBestWeekWorkout()
        {
            var athlete = CreateFemaleAthleteWithWorkouts();
            var bestWorkout = athlete.GetWeeksBestWorkout();
            var now = DateTime.Now.Date;
            var actual = athlete.Workouts.First(w => w.Date.Date == now);
            Assert.Equal(actual, bestWorkout.workout);
            Assert.Equal(true, Math.Abs(527.066260994-bestWorkout.calories) < .001);
        }

        [Fact]
        public void TestTweetBestWorkoutWeek()
        {
            var athlete = CreateMaleAthleteWithWorkouts();
            var tweet = athlete.TweetBestWorkoutOfWeek();
            Assert.Equal(true, 140-tweet.Length >=0);
        }
    }
}