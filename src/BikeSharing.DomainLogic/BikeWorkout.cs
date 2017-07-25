﻿using System;

namespace BikeSharing.DomainLogic
{
    public class Workout
    {
        public DateTime Date { get; }
        public TimeSpan Duration { get; set; }
        public double AverageHeartRate { get; set; }
        public string Notes;

        public Workout(DateTime date, TimeSpan duration, double averageHeartRate, string notes)
        {
            Date = date;
            Duration = duration;
            AverageHeartRate = averageHeartRate;
            Notes = notes ?? throw new ArgumentNullException(nameof(notes));
        }
    }

    public class DistanceWorkout : Workout
    {
        public double Distance { get; }
        public double Pace { get; }
        public DistanceWorkout(double distance, DateTime datetime, TimeSpan duration, double rate, string notes) 
            : base(datetime, duration, rate, notes)
        {
            Distance = distance;
            Pace = distance / duration.TotalHours;
        }

        void DoStuff(Workout w)
        {
            var x = w.Notes;
           
        }
    }

    public class BikeWorkout : DistanceWorkout
    {
        public WorkoutType Type { get; }

        public BikeWorkout(WorkoutType type, double distance, DateTime datetime, TimeSpan duration, double rate, string notes) 
            : base(distance, datetime, duration, rate, notes)
        {
            Type = type;
        }
    }
}