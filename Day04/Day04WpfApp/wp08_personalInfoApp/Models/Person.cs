using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wp08_personalInfoApp.Logics;

namespace wp08_personalInfoApp.Models
{
    internal class Person
    {
        // private - 외부에서 접근 불가
        private string firstName;
        private string lastName;
        private string email;
        private DateTime date;

        public string FirstName { get; set; } // Auto Property => 위에 private string firstName; 만들 필요 없음
        public string LastName { get; set; }
        public string Email { 
            get => email; 
            set
            {
                if (Commons.IsValidEmail(value) != true) // 형식에 일치하지 않는 이메일
                {
                    throw new Exception("유효하지 않은 이메일 형식");
                }
                else
                {
                    email = value;
                }
            } 
        }
        public DateTime Date { 
            get => date;
            set
            {
                var result = Commons.GetAge(value);
                if (result >120 || result <= 0)
                {
                    throw new Exception("유효하지 않은 생일");
                }
                else
                {
                    date = value;
                }

            }
        }

        public bool IsAdult
        {
            get => Commons.GetAge(date) > 18; //19살 이상이면 true
        } // 람다식

        public bool IsBirthday
        {
            get
            {
                return DateTime.Now.Month == date.Month && DateTime.Now.Day == date.Day; // 오늘하고 월, 일이 같으면 생일
            }
        } // 일반적인 property 사용

        public string Zodiac
        {
            get => Commons.GetZodiac(date); // 12지로 띠를 받아옴
        } // 람다식

        public Person(string firstName, string lastName, string email, DateTime date)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Date = date;
        }
    }
}
