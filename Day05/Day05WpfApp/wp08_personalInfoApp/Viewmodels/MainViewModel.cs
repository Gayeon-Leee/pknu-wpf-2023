using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using wp08_personalInfoApp.Models;

namespace wp08_personalInfoApp.Viewmodels
{
    internal class MainViewModel : ViewModelBase
    {
        // 1. View에서 사용할 멤버변수 선언
        
        // 입력쪽 변수
        private string inFirstName;
        private string inLastName;
        private string inEmail;
        private DateTime inDate = DateTime.Now; // 생일 입력란 현재 날짜로 셋팅
        
        //결과 출력쪽 변수
        private string outFirstName;
        private string outLastName;
        private string outEmail;
        private string outDate; // 생일 날짜 출력할 때는 문자열로 대체
        private string outAdult;
        private string outBirthday;
        private string outZodiac;

        // 2. 실제로 사용할 속성
        // 입력
        public string InFirstName { 
            get => inFirstName; 
            set
            {
                inFirstName = value;
                RaisePropertyChanged(nameof(InFirstName)); // InFirstName 이 바뀌면 알려주는 것
            }
        }

        public string InLastName { 
            get => inLastName;
            set
            {
                inLastName = value;
                RaisePropertyChanged(nameof(InLastName));
            }
        }

        public string InEmail { 
            get => inEmail; 
            set
            {
                inEmail = value;
                RaisePropertyChanged(nameof(InEmail));
            }
        }

        public DateTime InDate { 
            get => inDate;
            set
            {
                inDate = value;
                RaisePropertyChanged(nameof(InDate));
            }
        }


        
        // 출력
        public string OutFirstName { 
            get => outFirstName;
            set
            {
                outFirstName = value;
                RaisePropertyChanged(nameof(OutFirstName));
            }
        }

        public string OutLastName { 
            get => outLastName;
            set
            {
                outLastName = value;
                RaisePropertyChanged(nameof(OutLastName));
            }
        }
        
        public string OutEmail { 
            get => outEmail;
            set
            {
                outEmail = value;
                RaisePropertyChanged(nameof(OutEmail));
            }
        }

        public string OutDate { 
            get => outDate;
            set
            {
                outDate = value;
                RaisePropertyChanged(nameof(OutDate));
            }
        }

        public string OutAdult { 
            get => outAdult;
            set
            {
                outAdult = value;
                RaisePropertyChanged(nameof(OutAdult));
            }
        }

        public string OutBirthday { 
            get => outBirthday;
            set
            {
                outBirthday = value;
                RaisePropertyChanged(nameof(OutBirthday));
            }
        }
        public string OutZodiac { 
            get => outZodiac;
            set
            {
                outZodiac = value;
                RaisePropertyChanged(nameof(OutZodiac));
            }
        }

        // 3. 버튼 클릭에 대한 이벤트 처리(명령 / Command)
        private ICommand proceedCommand;
        public ICommand ProceedCommand
        {
            get
            {
                return proceedCommand ?? (proceedCommand = new RelayCommand<object>(
                    o => Proceed(), o => !string.IsNullOrEmpty(inFirstName) && 
                                         !string.IsNullOrEmpty(InLastName) &&
                                         !string.IsNullOrEmpty(inEmail)&&
                                         !string.IsNullOrEmpty(inDate.ToString())));    // 값이 다 있어야 저장 버튼 활성화됨
            }
        }

        private void Proceed() // 버튼 클릭시 실제로 처리 수행하는 메서드
        {
            try
            {
                Person person = new Person(inFirstName, inLastName, InEmail, inDate);

                OutFirstName = person.FirstName;
                OutLastName = person.LastName;
                OutEmail = person.Email;
                OutDate = person.Date.ToString("yyyy년 MM월 dd일");
                OutAdult = person.IsAdult == true ? "성년" : "미성년";
                OutBirthday = person.IsBirthday == true ? "생일" : "-";
                OutZodiac = person.Zodiac;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"예외발생 {ex.Message}");
            }
        }
    }
}
