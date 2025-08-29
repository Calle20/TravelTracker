using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelTracker.Database.DataClasses
{
    public class TrainStation
    {

        [PrimaryKey]
        public int ID { get; set; }
        [NotNull]
        public string Name { get; set; }
        [NotNull]
        public double Latitude { get; set; }
        [NotNull]
        public double Longitude { get; set; }
        public int Parent_station_id { get; set; }
        [NotNull]
        public string Country { get; set; }
    }
}
