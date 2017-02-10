using System.ComponentModel;

namespace Training
{
    public class Athlete : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string username;

        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;

            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(nameof(username)));
            }
        }



    }
}