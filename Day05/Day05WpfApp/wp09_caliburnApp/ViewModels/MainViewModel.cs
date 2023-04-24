using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wp09_caliburnApp.Models;

namespace wp09_caliburnApp.ViewModels
{
    public class MainViewModel : Screen
    {
        public string firstName = "Kihyun";

        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                NotifyOfPropertyChange(() => FirstName);
                NotifyOfPropertyChange(nameof(CanClearName));
                NotifyOfPropertyChange(nameof(FullName));
            }
        }

        private string lastName = "Yoo";

        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                NotifyOfPropertyChange(() => LastName);
                NotifyOfPropertyChange(nameof(CanClearName));
                NotifyOfPropertyChange(nameof(FullName)); ; // 변화통보
            }
        }

        public string FullName
        {
            get => $"{LastName} {FirstName}";
        }

        private BindableCollection<Person> managers = new BindableCollection<Person>(); // 여기서는 var 사용 불가

        public BindableCollection<Person> Managers
        { 
            get => managers;
            set => managers = value;
        }


        private Person selectedManager;
        public Person SelectedManager // 콤보박스에 선택ㄱ된 값을 지정
        {
            get => selectedManager;
            set
            {
                selectedManager = value;
                LastName = selectedManager.LastName;
                FirstName = selectedManager.FirstName;
                NotifyOfPropertyChange(nameof(selectedManager));
            }
        }

        public MainViewModel()
        {
            // DB를 사용하면 여기서 DB접속 - 데이터 Selet까지
            Managers.Add(new Person { FirstName = "Hyunwoo", LastName = "Son" });
            Managers.Add(new Person { FirstName = "Minhyuk", LastName = "Lee" });
            Managers.Add(new Person { FirstName = "Kihyun", LastName = "Yoo" });
            Managers.Add(new Person { FirstName = "Hyungwon", LastName = "Chae" });
            Managers.Add(new Person { FirstName = "Jooheon", LastName = "Lee" });
            Managers.Add(new Person { FirstName = "Changkyun", LastName = "Im" });

        }

        public void ClearName()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
         
        }

        public bool CanClearName // 메서드와 이름 동일하게 하되 앞에 Can을 붙임
        {
            get => !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName);
        }

    }
}
