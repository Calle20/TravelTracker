using System.Reflection;
using TravelTracker.Database;
using TravelTracker.Database.DataClasses;

namespace TravelTracker;

public partial class New : ContentPage
{
    public static List<TrainStation> trainStations { get; private set; } = new();
    TrainStationDatabase trainStationDatabase;
    private CancellationTokenSource? _cts;
    public New()
    {
        InitializeComponent();
        trainStationDatabase = new TrainStationDatabase();
        FillDatabase();
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

            trainStations.Add(new TrainStation
            {
                ID = id,
                Name = parts[1],
                Latitude = lat,
                Longitude = lon,
                Parent_station_id = parentId,
                Country = parts[8]
            });
        }
    }

    public static async Task<IEnumerable<TrainStation>> SearchAsync(string keyword, int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return Enumerable.Empty<TrainStation>();

        keyword = keyword.ToLowerInvariant();

        // Run the search on a background thread
        return await Task.Run(() =>
        {
            return trainStations
                .Where(s => s.Name.ToLower().StartsWith(keyword.ToLower()))
                .Take(limit)
                .ToList();
        });
    }

    private void OnAddNewTravelClicked(object sender, EventArgs e)
    {
       
    }
    private void OnFillAutomaticallyClicked(object sender, EventArgs e)
    {

    }

    private async void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var keyword = e.NewTextValue;
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        if (string.IsNullOrWhiteSpace(keyword) || keyword.Length < 3)
        {
            return;
        }

        try
        {
            await Task.Delay(200, _cts.Token); // debounce 200ms

            var results = await SearchAsync(keyword, 10);
            SuggestionsList.ItemsSource = results.Select(s=>s.Name);
        }
        catch (TaskCanceledException)
        {
            // Ignore, new keystroke came in
        }
    }

    private void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is string selected)
        {
            StartStation.Text = selected;
            SuggestionsList.ItemsSource = null;
        }
    }
}