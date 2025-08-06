using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelTracker.Database.DataClasses
{
    internal class CountryItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public required string Name { get; set; }
    }
}
