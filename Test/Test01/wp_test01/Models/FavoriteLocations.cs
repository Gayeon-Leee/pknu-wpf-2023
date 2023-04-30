using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wp_test01.Models
{
    public class FavoriteLocations
    {
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
