using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelTracker.Database.DataClasses
{
    //Stores informations about visited places
    internal class Place
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public double Long { get; set; }
        public double Lat { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public Country Country { get; set; } = null!;
    }
}
