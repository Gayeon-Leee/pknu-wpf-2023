using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.RightsManagement;
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
using System.Xml.Linq;
using wp_test01.Logics;
using wp_test01.Models;

namespace wp_test01
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            CboField.ItemsSource = fieldList;
        }

        #region < 검색 콤보박스 바인딩 리스트 >
        public List<string> fieldList = new List<string>()
        {
            "관광지", "숙박", "식음료", "체험", "동물병원"
        };
        #endregion


        #region < 검색 버튼 이벤트 >
        private async void BtnSearchField_Click(object sender, RoutedEventArgs e)
        {
           if (string.IsNullOrEmpty(CboField.Text))
            {
                await Commons.ShowMessageAsync("검색", "검색할 분야를 선택하세요.");
                return;
            }

            try
            {
                SearchField();
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"오류 발생 : {ex.Message}");
            }
        }

        public async void SearchField()
        {
            // 콤보박스 선택값을 오픈API 검색하기 위한 코드값으로 변환
            string searchCode = string.Empty;
            if (CboField.Text == "관광지") searchCode = "PC03";
            else if (CboField.Text == "숙박") searchCode = "PC02";
            else if (CboField.Text == "식음료") searchCode = "PC01";
            else if (CboField.Text == "체험") searchCode = "PC04";
            else if (CboField.Text == "동물병원") searchCode = "PC05";


            string openApiUri = $"http://pettravel.kr/api/listPart.do?page=1&pageBlock=50&partCode={searchCode}";
            string result = string.Empty;

            // WebRequest, WebResponse 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri); // URL을 넣어서 객체를 생성
                res = await req.GetResponseAsync(); // 요청한 결과를 응답에 할당
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd(); // json결과 텍스트로 저장

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                throw ex; //  BtnSearchMovie_Click에서 오류메세지 보여주기 때문에 여기선 그냥 던져주기만 하면 됨
            }
            finally
            {
                reader.Close();
                res.Close();
            }

            // result를 json으로 변경(기존코드)
            // var jsonResult = JObject.Parse(result); // string -> json
            // 이 방식으로 하면 자꾸 오류남 => 받으려는 openAPI가 대괄호로 묶여있어 json배열로 인식

            var jsonResult = JArray.Parse(result); // 이렇게 하면 배열로 인식한 json을 읽을 수는 있음
            // 디버그 출력은 되는데.. 데이터 그리드에 어떻게 뿌리지?????? 이거 해결해야함
            
            
            //var test = JObject.Parse(jsonResult.ToString());
            //var items = test["resultList"];
            //var json_array = items as JArray;
            



            /*var data = jsonResult;
            var json_array = data as JArray;*/

            var locations = new List<Locations>(); // json에서 넘어온 배열을 담을 장소
            foreach (var val in jsonResult)
            {
                //JObject jobj = JObject.Parse(jsonResult.ToString());

                var Locations = new Locations()
                {
                    Id = 0,
                    ContentSeq = Convert.ToInt32(val["contentSeq"]),
                    AreaName = Convert.ToString(val["areaName"]),
                    PartName = Convert.ToString(val["partName"]),
                    Title = Convert.ToString(val["title"]),
                    Address = Convert.ToString(val["address"]),
                    Latitude = Convert.ToDouble(val["latitude"]),
                    Longitude = Convert.ToDouble(val["longitude"]),
                    Tel = Convert.ToString(val["tel"])

                };
                locations.Add(Locations);
                }
                this.DataContext = locations;
                StsResult.Content = $"OpenAPI {locations.Count}건 조회 완료";
        }



        #endregion
    }
}
