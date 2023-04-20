using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wp05_bikeshop.Logics;

namespace wp05_bikeshop
{
    /// <summary>
    /// SupportPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TestPage : Page
    {
        Car myCar = null;

        public TestPage()
        {
            InitializeComponent();
            InitCar();
        }

        private void InitCar()
        {
            // 일반적인 C#에서의 클랙스 객체 인스턴스 사용방법과 동일
            myCar = new Car();
            myCar.Name = "아이오닉";
            myCar.Colorz = Colors.White;
            myCar.Speed = 220;

            // 리스트박스에 바인딩 하기 위한 Car 리스트
            List<Car> cars = new List<Car>();
            var rand = new Random(); // 컬러 랜덤으로 받기 위함
            
            for (int i = 0; i < 10; i++)
            {
                cars.Add(new Car()
                {
                    Speed = i * 10,
                    Colorz = Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))
                }) ; ;
            }

            this.DataContext = cars; // DataContext에 cars 할당하는거
            // -- 중요함!! 코드비하인드에서 만든 데이터(DB, excel, etc..)를 xaml에 있는 컨트롤에 바인딩할 때 쓰는게 DataContext
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // TxtSample.Text = myCar.Speed.ToString(); // 전통적인 윈폼 방식
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("버튼입니다.");
        }
    }
}
