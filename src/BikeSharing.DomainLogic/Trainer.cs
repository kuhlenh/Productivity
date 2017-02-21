using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Training
{
    public class Workout2
    {
        public int id { get; set; }
        public User User { get; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
        public int AHR { get; set; }
        public Intensity Level { get; }
        public string Notes { get; set; }

        public Workout2(int id, TimeSpan duration, int heartRate, DateTime date, User user, string notes)
        {
            this.id = id;
            if (user == null)
                throw new ArgumentNullException(nameof(User));
            User = user;
            Duration = duration;
            AHR = heartRate;
            Date = date;
            Level = CalculateIntensity();
            Notes = notes;
        }

        private Intensity CalculateIntensity()
        {
            var percent = AHR / (double)User.MHR;
            
            if (percent <= .5)
                return Intensity.Light;
            else if (percent > .5 && percent <= .7 )
                return Intensity.Moderate;
            else
                return Intensity.Vigorous;
        }

        public override string ToString()
        {
            return string.Format("{0}: {2} ({1:0} minutes)", Date.Date, Duration.TotalMinutes, Notes);
        }
    }

    public enum Intensity
    {
        Light, Moderate, Vigorous
    }
}