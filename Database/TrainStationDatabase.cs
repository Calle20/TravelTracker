using Microsoft.Maui.Storage;
using SQLite;
using System.Reflection;
using TravelTracker.Database.DataClasses;

namespace TravelTracker.Database
{
    public class TrainStationDatabase
    {
        SQLiteAsyncConnection database;

        public async Task Init()
        {
            if (database is not null)
                return;


            database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

            var result = await database.CreateTableAsync<TrainStation>();
            if (result == CreateTableResult.Created)
            {
                await FillDatabase();
            }
        }

        private async Task FillDatabase()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = await FileSystem.OpenAppPackageFileAsync("stations.csv");
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();

            string[] lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length < 2)
                return; // no data

            var stations = new List<TrainStation>();

            // Assuming CSV has header: ID,Name,Latitude,Longitude,Parent_station_id,Country
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');

                if (parts.Length < 9) // make sure we have enough columns
                    continue;

                if (!int.TryParse(parts[0], out int id))
                    continue;

                if (!double.TryParse(parts[5], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double lat))
                    continue;

                if (!double.TryParse(parts[6], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double lon))
                    continue;

                int parentId = 0;
                int.TryParse(parts[7], out parentId);

                stations.Add(new TrainStation
                {
                    ID = id,
                    Name = parts[1],
                    Latitude = lat,
                    Longitude = lon,
                    Parent_station_id = parentId,
                    Country = parts[8]
                });
            }

            Console.WriteLine($"Parsed {stations.Count} stations from CSV.");

            if (stations.Count > 0)
            {
                await database.RunInTransactionAsync(tran =>
                {
                    tran.InsertAll(stations);
                });
            }
        }

        public Task<List<TrainStation>> GetAllAsync() =>
            database.Table<TrainStation>().ToListAsync();

        public async Task<List<TrainStation>> GetItemsAsync()
        {
            await Init();
            return await database.Table<TrainStation>().ToListAsync();
        }

        public async Task<TrainStation> GetItemAsync(int id)
        {
            await Init();
            return await database.Table<TrainStation>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveItemAsync(TrainStation item)
        {
            await Init();
            if (item.ID != 0)
                return await database.UpdateAsync(item);
            else
                return await database.InsertAsync(item);
        }

        public async Task<int> DeleteItemAsync(TrainStation item)
        {
            await Init();
            return await database.DeleteAsync(item);
        }

        public async Task<List<TrainStation>> SearchStationsAsync(string keyword)
        {
            await Init();
            return await database.Table<TrainStation>()
                .Where(station => station.Name.ToLower().Contains(keyword.ToLower()))
                .ToListAsync();
        }
    }
}
