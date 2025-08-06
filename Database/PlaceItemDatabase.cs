using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelTracker.Database.DataClasses;

namespace TravelTracker.Database
{
    internal class PlaceItemDatabase
    {
        SQLiteAsyncConnection database;

        async Task Init()
        {
            if (database is not null)
                return;

            database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            var result = await database.CreateTableAsync<PlaceItem>();
        }

        public async Task<List<PlaceItem>> GetItemsAsync()
        {
            await Init();
            return await database.Table<PlaceItem>().ToListAsync();
        }

        public async Task<PlaceItem> GetItemAsync(int id)
        {
            await Init();
            return await database.Table<PlaceItem>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveItemAsync(PlaceItem item)
        {
            await Init();
            if (item.ID != 0)
                return await database.UpdateAsync(item);
            else
                return await database.InsertAsync(item);
        }

        public async Task<int> DeleteItemAsync(PlaceItem item)
        {
            await Init();
            return await database.DeleteAsync(item);
        }
    }
}
