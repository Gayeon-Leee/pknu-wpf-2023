using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using wp11_movieFinder.Logics;
using wp11_movieFinder.Models;

namespace wp11_movieFinder
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
        }

        private async void BtnNaverMovie_Click(object sender, RoutedEventArgs e) // 비동기 async
        {
            await Commons.ShowMessageAsync("네이버영화", "네이버영화 사이트로 연결합니다."); // 비동기 await
        }

        // 검색버튼
        private async void BtnSearchMovie_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtMovieName.Text)) // 입력검증
            {
                await Commons.ShowMessageAsync("검색", "검색할 영화명을 입력하세요.");
                return;
            }

            //if (TxtMovieName.Text.Length <= 2)
            //{
            //    await Commons.ShowMessageAsync("검색", "검색어를 2자 이상 입력하세요.");
            //    return;
            //}

            try
            {
                SearchMovie(TxtMovieName.Text);
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"오류 발생 : {ex.Message}");
            }
        }

        
        // 텍스트박스에서 키를 입력할 때 엔터를 누르면 검색 시작
        private void TxtMovieName_KeyDown(object sender, KeyEventArgs e) 
        {
            if(e.Key== Key.Enter)
            {
                BtnSearchMovie_Click(sender, e);
            }
        }

        // 실제 검색 메서드
        private async Task SearchMovie(string movieName)
        {
            string tmdb_apiKey = "71c2cce9ff09e884f695f79cd958a4a0";
            string encoding_movieName = HttpUtility.UrlEncode(movieName, Encoding.UTF8);
            string openApiUri = $@"https://api.themoviedb.org/3/search/movie?api_key={tmdb_apiKey}&language=ko-KR&page=1&include_adult=false&query={encoding_movieName}"; 
            string result = string.Empty; // 결과값

            // api 실행할 객체
            WebRequest req = null;
            WebResponse resp = null;
            StreamReader reader = null;

            // Naver API 요청
            try
            {
                req = WebRequest.Create(openApiUri); // URL을 넣어서 객체를 생성
                resp = await req.GetResponseAsync(); // 요청한 결과를 응답에 할당
                reader = new StreamReader(resp.GetResponseStream());
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
                resp.Close();
            }

            // result를 json으로 변경
            var jsonResult = JObject.Parse(result); // string -> json

            var total = Convert.ToInt32(jsonResult["total_results"]); // 전체 검색 결과 수
            // await Commons.ShowMessageAsync("검색결과", total.ToString());
            
            var items = jsonResult["results"];
            var json_array = items as JArray; // items를 데이터 그리드에 표시

            var movieItems = new List<MoviItem>(); // json에서 넘어온 배열을 담을 장소
            foreach (var val in json_array)
            {
                var MovieItem = new MoviItem()
                {
                    Adult = Convert.ToBoolean(val["adult"]),
                    Id = Convert.ToInt32(val["id"]),
                    Original_Language = Convert.ToString(val["original_language"]),
                    Original_Title = Convert.ToString(val["original_title"]),
                    Title = Convert.ToString(val["title"]),
                    Popularity = Convert.ToDouble(val["popularity"]),
                    Poster_Path = Convert.ToString(val["poster_path"]),
                    Overview = Convert.ToString(val["overview"]),
                    Release_Date = Convert.ToString(val["release_date"]),
                    Vote_Average = Convert.ToDouble(val["vote_average"]),

                };
                movieItems.Add(MovieItem);
            }

            this.DataContext = movieItems;
            
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtMovieName.Focus();
        }

        // 그리드에서 셀 선택하면
        private void GrdResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try 
            {
                var movie = GrdResult.SelectedItem as MoviItem;
                Debug.WriteLine(movie.Poster_Path);
                if (string.IsNullOrEmpty(movie.Poster_Path)) // 포스터 이미지가 없으면 NO_Picture
                {
                    ImgPoster.Source = new BitmapImage(new Uri("/No_Picture.png", UriKind.RelativeOrAbsolute));
                }
                else // 포스터 이미지 경로 있으면
                {
                    var base_url = "https://image.tmdb.org/t/p/w300_and_h450_bestv2";
                    ImgPoster.Source = new BitmapImage(new Uri($"{base_url}{movie.Poster_Path}", UriKind.RelativeOrAbsolute));
                }
            }
            catch
            {

            }
           
        }

        // 영화 예고편 유튭 보기
        private async void BtnWatchTrailer_Click(object sender, RoutedEventArgs e)
        {
            if(GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("유튜브", "영화를 선택하세요.");
                return;
            }
            if(GrdResult.SelectedItems.Count > 1)
            {
                await Commons.ShowMessageAsync("유튜브", "영화를 하나만 선택하세요.");
                return;
            }

            string movieName = string.Empty;

            movieName = (GrdResult.SelectedItem as MoviItem).Title;
            //var movie = GrdResult?.SelectedItem as MoviItem;

            //await Commons.ShowMessageAsync("유튜브", $"예고편 볼 영화 {movieName}");
            var trailerWinow = new TrailerWindow(movieName);
            trailerWinow.Owner = this; // TrailerWindow의 부모는 MainWindow
            trailerWinow.WindowStartupLocation = WindowStartupLocation.CenterOwner; // 부모창의 정중앙에 위치
            // trailerWinow.Show(); // 모달리스 => 자식창 연 상태에서 부모창을 건들 수 있어서 사용X
            trailerWinow.ShowDialog();  // 모달창
        }

        // 검색결과 중에서 좋아하는 영화 저장
        private async void BtnAddFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기에 추가할 영화를 선택하세요(복수선택 가능)");
                return;
            }

            if (isFavorite)
            {
                await Commons.ShowMessageAsync("오류", "이미 즐겨찾기한 영화입니다.");
                return;
            }

            List<FavoriteMovieItem> list = new List<FavoriteMovieItem>();
            foreach(MoviItem item in GrdResult.SelectedItems)
            {
                var favoriteMovieItem = new FavoriteMovieItem()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Original_Title = item.Original_Title,
                    Original_Language = item.Original_Language,
                    Adult = item.Adult,
                    Overview = item.Overview,
                    Release_Date = item.Release_Date,
                    Vote_Average = item.Vote_Average,
                    Popularity = item.Popularity,
                    Poster_Path = item.Poster_Path,
                    Reg_Date = DateTime.Now
                };

                list.Add(favoriteMovieItem);
            }

            //await Commons.ShowMessageAsync("저장할 데이터 수", list.Count.ToString());
            //return;

            try
            {
                // DB 데이터 입력
                using (SqlConnection conn = new SqlConnection(Commons.connString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    // 
                    var query = @"INSERT INTO [dbo].[FavoriteMovieItem]
                                            ( [Id]
                                            , [Title]
                                            , [Original_Title]
                                            , [Release_Date]
                                            , [Original_Language]
                                            , [Adult]
                                            , [Popularity]
                                            , [Vote_Average]
                                            , [Poster_Path]
                                            , [Overview]
                                            , [Reg_Date])
                                       VALUES
                                            ( @Id
                                            , @Title
                                            , @Original_Title
                                            , @Release_Date
                                            , @Original_Language
                                            , @Adult
                                            , @Popularity
                                            , @Vote_Average
                                            , @Poster_Path
                                            , @Overview
                                            , @Reg_Date )";

                    var insRes = 0;
                    foreach (FavoriteMovieItem item in list)
                    {
                        SqlCommand cmd = new SqlCommand(query, conn);

                        cmd.Parameters.AddWithValue(@"Id", item.Id);
                        cmd.Parameters.AddWithValue(@"Title", item.Title);
                        cmd.Parameters.AddWithValue(@"Original_Title", item.Original_Title);
                        cmd.Parameters.AddWithValue(@"Release_Date", item.Release_Date);
                        cmd.Parameters.AddWithValue(@"Original_Language", item.Original_Language);
                        cmd.Parameters.AddWithValue(@"Adult", item.Adult);
                        cmd.Parameters.AddWithValue(@"Popularity", item.Popularity);
                        cmd.Parameters.AddWithValue(@"Vote_Average", item.Vote_Average);
                        cmd.Parameters.AddWithValue(@"Poster_Path", item.Poster_Path);
                        cmd.Parameters.AddWithValue(@"Overview", item.Overview);
                        cmd.Parameters.AddWithValue(@"Reg_Date", item.Reg_Date);

                        insRes += cmd.ExecuteNonQuery();
                    }

                    // await Commons.ShowMessageAsync("데이터 개수", result.ToString());
                    
                    if (list.Count == insRes)
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장 성공");
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
    }
}
