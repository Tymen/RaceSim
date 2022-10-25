using Model.Classes;

namespace Controller.EventArgs;

public class DriversChangedEventArgs : System.EventArgs
{
    public Track track { get; set; }
    public Dictionary<Section, SectionData> positions { get; set; }
}