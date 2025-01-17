using System.ComponentModel;
using Controller;
using Model.Classes;

public class DataContext : INotifyPropertyChanged
{
    private RaceController _raceController;
    public string CurrentTrackName
    {
        get
        {
            return _raceController.Track.Name;
        }
    }
    public List<Driver> Participants
    {
        get
        {
            return _raceController._participants;
        }
    }
    public DataContext(RaceController raceController)
    {
        _raceController = raceController;
        _raceController.DriversChanged += RaceController_DriversChanged;
    }

    // Implement INotifyPropertyChanged interface
    public event PropertyChangedEventHandler PropertyChanged;

    // Event handler for DriversChanged event
    private void RaceController_DriversChanged(object sender, EventArgs e)
    {
        // Trigger the PropertyChanged event for all properties
        OnPropertyChanged(string.Empty);
        OnPropertyChanged(nameof(Participants));
        // additionally trigger the PropertyChanged event for the CurrentTrackName property
        OnPropertyChanged(nameof(CurrentTrackName));
    }

    // Helper method to trigger the PropertyChanged event
    protected virtual void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}