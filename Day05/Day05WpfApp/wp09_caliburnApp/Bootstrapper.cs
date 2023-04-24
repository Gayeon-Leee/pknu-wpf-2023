using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using wp09_caliburnApp.ViewModels;

namespace wp09_caliburnApp
{
    // Caliburn으로 MVVM 실행할 때 주요 설정 진행
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper() {
            Initialize(); // Caliburn MVVM 초기화
        }

        protected async override void OnStartup(object sender, StartupEventArgs e) // 시작한 후에 초기화 진행
        {
            // base.OnStartup(sender, e); // 부모클래스 실행은 주석처리
            await DisplayRootViewForAsync<MainViewModel>();
        }
    }

}
