using GMap.NET.WindowsPresentation;
using GMap.NET;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GMap.NET.MapProviders;
using wp_test01.Logics;
using wp_test01.Models;
using MySql.Data.MySqlClient;
using System.Data;
using MahApps.Metro.Controls.Dialogs;

namespace wp_test01
{
    /// <summary>
    /// SubWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SubWindow : MetroWindow
    {
        PointLatLng start;
        PointLatLng end;
        bool isFavoriate = false;

        // marker
        GMapMarker currentMarker;

        // zones list
        List<GMapMarker> Circles = new List<GMapMarker>();

        public SubWindow()
        {
            InitializeComponent();

            // map 초기 설정
            gmap.MapProvider = GMapProviders.OpenStreetMap;
            gmap.Position = new PointLatLng(37.724962, 128.3009629);
            gmap.MinZoom = 2;
            gmap.MaxZoom = 17;
            gmap.Zoom = 9;
            gmap.ShowCenter = false;

        }


        #region < 즐겨찾기 보기 >
        private async void BtnViewFavorite_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;

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


                    StsResult.Content = $"즐겨찾기 {list.Count}건 조회 완료";

                }

                
            }
            catch (Exception ex)
            {
                //await Commons.ShowMessageAsync("오류", $"DB조회 오류 {ex.Message}");
                await this.ShowMessageAsync("오류", $"DB조회 오류 {ex.Message}", MessageDialogStyle.Affirmative, null);
            }


            // 불러온 위치데이터를 바로 지도에 표시
            gmap.Markers.Clear();

            foreach (FavoriteLocations item in GrdResult.Items)
            {
                double mapLat = item.Latitude;
                double mapLon = item.Longitude;
                PointLatLng point = new PointLatLng(mapLat, mapLon);
                GMapMarker marker = new GMapMarker(point);
                
                marker.Shape = new Ellipse
                {
                    Width = 12,
                    Height = 12,
                    Stroke = Brushes.Red,
                    StrokeThickness = 2.5
                };
                gmap.Markers.Add(marker);
            }


        }
        #endregion

        
        #region < 즐겨찾기 삭제 >
        private async void BtnDelFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                //await Commons.ShowMessageAsync("오류", "삭제할 장소를 선택하세요.");
                await this.ShowMessageAsync("오류", "삭제할 장소를 선택하세요", MessageDialogStyle.Affirmative, null);
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
                        await this.ShowMessageAsync("삭제", "DB 삭제 성공!", MessageDialogStyle.Affirmative, null);

                        //await Commons.ShowMessageAsync("삭제", "DB 삭제 성공!");
                    }
                    else
                    {
                        await this.ShowMessageAsync("삭제", "DB 삭제 일부  성공!", MessageDialogStyle.Affirmative, null);

                        //await Commons.ShowMessageAsync("삭제", "DB 삭제 일부 성공!"); // 발생할 일이 거의 없긴 함

                    }
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB삭제 오류 {ex.Message}");
            }

            BtnViewFavorite_Click(sender, e); // 삭제하고 자동으로 즐겨찾기 보기 이벤트 핸들러 한 번 실행
        }
        #endregion
    }

}
