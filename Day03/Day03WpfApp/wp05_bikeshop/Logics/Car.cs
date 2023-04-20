using System.Windows.Media;

namespace wp05_bikeshop.Logics
{
    internal class Car : Notifier // 값이 바뀌는걸 인지해서 처리하기 위해 Notifier
    {
        private string names;
        public string Name 
        { 
            get => names; 
            set // 프로퍼티 변경
            {
                names = value;
                OnPropertyChanged("Names"); // Names 프로퍼티가 바뀌었으니 처리해야한다는 뜻
            }
        }
        private double speed;
        public double Speed
        {
            get => speed;
            set
            {
                speed = value;
                OnPropertyChanged(nameof(Speed)); // == "Speed"
            }
        }
        public Color Colorz { get; set; } // Auto Property
        public Human Driver { get; set; } 
    }
}
