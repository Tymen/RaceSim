using Model.Interfaces;

namespace Model.Classes;

public class Competition
{
    public List<Driver> Participants { set; get; }
    public Queue<Track> Tracks { set; get; }

    public Track NextTrack()
    {
        return Tracks != null && Tracks.Count > 0 ? Tracks.Dequeue() : null;
    }
}