using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        bool isFavorite = false; // false -> openAPi 검색해온 결과, true -> 즐겨찾기 보기

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
           if (string.IsNullOrEmpty(CboField.Text)) // 입력검증
            {
                await Commons.ShowMessageAsync("검색", "검색할 분야를 선택하세요.");
                return;
            }

            try // 실제 검색
            {
                await SearchField();
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"오류 발생 : {ex.Message}");
            }
        }

        private void CboField_KeyDown(object sender, KeyEventArgs e) // 콤보박스에서 값 선택하고 엔터키 쳐도 검색하는 이벤트
        {
            if (e.Key == Key.Enter)
            {
                BtnSearchField_Click(sender, e);
            }
        }

        public async Task SearchField()
        {
            // 콤보박스 선택값을 오픈API 검색하기 위한 코드값으로 변환
            string searchCode = string.Empty;
            if (CboField.Text == "관광지") searchCode = "PC03";
            else if (CboField.Text == "숙박") searchCode = "PC02";
            else if (CboField.Text == "식음료") searchCode = "PC01";
            else if (CboField.Text == "체험") searchCode = "PC04";
            else if (CboField.Text == "동물병원") searchCode = "PC05";


            string openApiUri = $"http://pettravel.kr/api/listPart.do?page=1&pageBlock=50&partCode={searchCode}"; // openAPI 요청
            string result = string.Empty; //결과값 초기화

            // API 실행할 WebRequest, WebResponse 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try // API 요청
            {
                req = WebRequest.Create(openApiUri); // URL을 넣어서 객체를 생성
                res = await req.GetResponseAsync(); // 요청한 결과를 응답에 할당
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd(); // json결과 텍스트로 저장
            }
            catch (Exception ex)
            {
                throw ex; //  BtnSearchMovie_Click에서 오류메세지 보여주기 때문에 여기선 그냥 던져주기
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
            var jsonObj = jsonResult[0]; // 받아오려는 API가 json 배열로 이루어져 있음 => 배열의 0번 인덱스로 접근 => 0번 인덱스는 json 객체임
            var items = jsonObj["resultList"]; // json 객체의 key값인 resultList로 value에 접근
            var json_array = items as JArray; // 이렇게 안하고 그냥 jsonResult나 jsonObj로 foreach문 돌리면 child값에 접근할 수 없다고 나옴
            // 오류메세지 : cannot access child value on Newtonsoft.json.linq.jproperty 쌤께 여쭤보기!!

            Debug.WriteLine(jsonObj);


            var locations = new List<Locations>(); // json에서 넘어온 배열을 담을 장소
            foreach (var val in json_array)
            {
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
                this.DataContext = locations; // 그리드 바인딩
                isFavorite = false;
                StsResult.Content = $"OpenAPI {locations.Count}건 조회 완료";
        }


        #endregion

        #region < 지도 표시 >

        private void GrdResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

            if (GrdResult.SelectedItem is Locations) // openAPI로 검색된 장소의 지도 표시
            {
                var loc = GrdResult.SelectedItem as Locations;
                string title = loc.Title;
                BrsLoc.Address = $"https://map.naver.com/v5/search/{title}";
            }

            else if (GrdResult.SelectedItem is FavoriteLocations) // 즐겨찾기 DB에서 가져온 장소 표시
            {
                var loc = GrdResult.SelectedItem as FavoriteLocations;
                string title = loc.Title;
                BrsLoc.Address = $"https://map.naver.com/v5/search/{title}";
            }
        }
        #endregion

        #region < 즐겨찾기 추가 >
        private async void BtnAddFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기에 추가할 장소를 선택하세요");
                return;
            }
            if (isFavorite)
            {
                await Commons.ShowMessageAsync("오류", "이미 즐겨찾기한 장소입니다.");
                return;
            }

            try
            {
                using(MySqlConnection conn = new MySqlConnection(Commons.MyConnString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();
                    var query = @"INSERT INTO favoritelocations
                                             (Id,
                                              ContentSeq,
                                              AreaName,
                                              PartName,
                                              Title,
                                              Address,
                                              Latitude,
                                              Longitude,
                                              Tel)
                                         VALUES
                                             (@Id,
                                              @ContentSeq,
                                              @AreaName,
                                              @PartName,
                                              @Title,
                                              @Address,
                                              @Latitude,
                                              @Longitude,
                                              @Tel)";

                    var insRes = 0;
                    foreach(Locations item in GrdResult.SelectedItems)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);

                        cmd.Parameters.AddWithValue(@"Id", item.Id);
                        cmd.Parameters.AddWithValue(@"ContentSeq", item.ContentSeq);
                        cmd.Parameters.AddWithValue(@"AreaName", item.AreaName);
                        cmd.Parameters.AddWithValue(@"PartName", item.PartName);
                        cmd.Parameters.AddWithValue(@"Title", item.Title);
                        cmd.Parameters.AddWithValue(@"Address", item.Address);
                        cmd.Parameters.AddWithValue(@"Latitude", item.Latitude);
                        cmd.Parameters.AddWithValue(@"Longitude", item.Longitude);
                        cmd.Parameters.AddWithValue(@"Tel", item.Tel);

                        insRes += cmd.ExecuteNonQuery();
                    }

                    if (GrdResult.SelectedItems.Count == insRes)
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장 성공");
                        StsResult.Content = $"즐겨찾기 {insRes}건 저장 완료";
                    }
                    else
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장오류 관리자에게 문의하세요.");
                    }
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB저장 오류 {ex.Message}");
            }
        }
        #endregion

        #region < 즐겨찾기 보기 >
        private async void BtnViewFavorite_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            CboField.Text = string.Empty;

            List<FavoriteLocations> list = new List<FavoriteLocations>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.MyConnString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    var query = @"SELECT Id,
                                         ContentSeq,
                                         AreaName,
                                         PartName,
                                         Title,
                                         Address,
                                         Latitude,
                                         Longitude,
                                         Tel
                                    FROM favoritelocations
                                   ORDER BY AreaName ASC";

                    var cmd = new MySqlCommand(query, conn);
                    var adapter = new MySqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "FavoriteLocations");

                    foreach (DataRow dr in dSet.Tables["FavoriteLocations"].Rows)
                    {
                        list.Add(new FavoriteLocations
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            ContentSeq = Convert.ToInt32(dr["ContentSeq"]),
                            AreaName = Convert.ToString(dr["AreaName"]),
                            PartName = Convert.ToString(dr["PartName"]),
                            Title = Convert.ToString(dr["Title"]),
                            Address = Convert.ToString(dr["Address"]),
                            Latitude = Convert.ToDouble(dr["Latitude"]),
                            Longitude = Convert.ToDouble(dr["Longitude"]),
                            Tel = Convert.ToString(dr["Tel"]),
                        });
                    }

                    this.DataContext = list;
                    isFavorite = true;
                    StsResult.Content = $"즐겨찾기 {list.Count}건 조회 완료";
                }
            }
            catch(Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB조회 오류 {ex.Message}");

            }
        }

        #endregion

        #region < 즐겨찾기 삭제 >
        private async void BtnDelFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (isFavorite == false)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기만 삭제할 수 있습니다.");
                return;
            }

            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "삭제할 장소를 선택하세요.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.MyConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    var query = "DELETE FROM favoritelocations WHERE Id = @Id";
                    var delRes = 0;

                    foreach (FavoriteLocations item in GrdResult.SelectedItems)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", item.Id);

                        delRes += cmd.ExecuteNonQuery();

                    }

                    if (delRes == GrdResult.SelectedItems.Count)
                    {
                        await Commons.ShowMessageAsync("삭제", "DB 삭제 성공!");
                        StsResult.Content = $"즐겨찾기 {delRes}건 삭제 완료"; // 이건 화면에 안나옴

                    }
                    else
                    {
                        await Commons.ShowMessageAsync("삭제", "DB 삭제 일부 성공!"); // 발생할 일이 거의 없긴 함

                    }
                }
            }
            catch(Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB삭제 오류 {ex.Message}");
            }
            
            BtnViewFavorite_Click(sender, e); // 삭제하고 자동으로 즐겨찾기 보기 이멘트 핸들러 한 번 실행

        }

        #endregion

    }
}
