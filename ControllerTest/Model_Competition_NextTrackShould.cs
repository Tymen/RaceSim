using Model.Classes;
using Model.Enums;

namespace ControllerTest;

[TestFixture]
public class Model_Competition_NextTrackShould
{
    private Competition _competition;
    
    [SetUp]
    public void SetUp()
    {
        _competition = new Competition()
        {
            Tracks = new Queue<Track>()
        };
    }
    
    /*
     *  Description:
     *  Method to easily add a new track to the competition
     *
     *  Return Type: Track
     */
    public Track AddTrackToQueue(string name)
    {
        SectionTypes[] sectionTypes = new SectionTypes[1];
        sectionTypes[0] = SectionTypes.Straight;
        var track = new Track($"Track {name}", sectionTypes);
        _competition.Tracks.Enqueue(track);
        
        return track;
    }
    
    /*
     *  Description:
     *  Test if the Nextrack is null / empty
     */
    [Test]
    public void NextTrack_EmptyQueue_ReturnNull()
    {
        var result = _competition.NextTrack();
        Assert.IsNull(result);
    }
    
    /*
     *  Description:
     *  Test if Track returns the correct track
     */
    [Test]
    public void NextTrack_OneInQueue_ReturnTrack()
    {
        var track = AddTrackToQueue("1");
        var result = _competition.NextTrack();
        Assert.AreEqual(track, result);
    }

    /*
     *  Description:
     *  Test if the queue removes a track after NextTrack method is called.
     */
    [Test]
    public void NextTrack_OneInQueue_RemoveTrackFromQueue()
    {
        var track = AddTrackToQueue("1");
        Track result = null;
        
        for (int i = 0; i < 2; i++)
        {
            result = _competition.NextTrack();
        }
        
        Assert.IsNull(result);
    }

    /*
     *  Description:
     *  Tests if Queue is in the correct order.
     */
    [Test]
    public void NextTrack_TwoInQueue_ReturnNextTrack()
    {
        Track result = null;
        List<Track> tracks = new List<Track>()
        {
            AddTrackToQueue("1"),
            AddTrackToQueue("2")
        };

        for (int i = 0; i < tracks.Count; i++)
        {
            result = _competition.NextTrack();
            Assert.AreEqual(tracks[i], result);
        }
    }
}