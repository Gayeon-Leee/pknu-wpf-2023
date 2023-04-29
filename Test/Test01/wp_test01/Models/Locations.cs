using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wp_test01.Models
{
    public class Locations
    {
        /*
         "contentSeq": 15,
                "areaName": "춘천시",
                "partName": "식음료",
                "title": "카페 캐빈",
                "address": "강원 춘천시 남면 버들길 60-7",
                "latitude": "37.747381503046",
                "longitude": "127.630154310527",
                "tel": "010-2246-7491"
        */

        public int Id { get; set; }
        public int ContentSeq { get; set; }
        public string AreaName { get; set; }
        public string PartName { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Tel { get; set; }
    }
}
