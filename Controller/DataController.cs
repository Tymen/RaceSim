using Model;
using Model.Classes;
using Model.Enums;
using Model.Interfaces;

namespace Controller;

public static class DataController
{
    public static Competition Competition;
    public static RaceController CurrentRace;
    
    /*
     *  Description:
     *  Because there is no constructor in a static class.
     *  We need to have a way to initialize the new Competition
     */
    public static void Initialize()
    {
        Competition = new Competition();
        Competition.Participants = new List<IParticipant>();
        Competition.Tracks = new Queue<Track>();
        
        AddParticipant(5);
        AddTrack();
    }

    /*
     *  Description:
     *  Check if there are tracks available in the ocmpetition. If this is the case it will start a new race.
     *  Adds these Participant object to the Competition Participants list.
     */
    public static void NextRace()
    {
        Track nextTrack = Competition.NextTrack();
        if (!nextTrack.Equals(null))
        {
            CurrentRace = new RaceController(nextTrack, Competition.Participants);
        }
    }
    
    /*
     *  Description:
     *  Creates multiple new Participant objects.
     *  Adds these Participant object to the Competition Participants list.
     */
    public static void AddParticipant(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Competition.Participants.Add(GenerateParticipant(i));
        }
    }
    
    /*
     *  Description:
     *  Creates multiple new Track objects.
     *  Adds these Track object to the Competition Tracks Queue
     */
    public static void AddTrack()
    {
        int nmbrOfTracks = 5;
        for (int i = 0; i < nmbrOfTracks; i++)
        {
            Track track = new Track($"Track {i}", GetSecions(2));

            Competition.Tracks.Enqueue(track);
        }
    }
    
    /*
     *  Description: Creates a new Participant object
     *  Return Type: Section
     */
    private static Driver GenerateParticipant(int nmbr)
    {
        Array values = Enum.GetValues(typeof(TeamColors));
        Random random = new Random();
        
        Driver participant = new Driver(GetCar());
        participant.Name = $"Gerald {nmbr}";
        participant.Points = 0;
        participant.TeamColors = (TeamColors)values.GetValue(random.Next(values.Length));

        return participant;
    }
    
    /*
     *  Description: Creates a new Car object
     *  Return Type: Car
     */
    private static Car GetCar()
    {
        Car car = new Car();
            
        car.Performance = 0;
        car.Quality = 0;
        car.Speed = 0;
        car.IsBroken = false;

        return car;
    }
    
    /*
     *  Description: Creates a new LinkedList with sections.
     *  Return Type: SectionTypes[]
     */
    private static SectionTypes[] GetSecions(int amount)
    {
        SectionTypes[] sectionsTypes = new SectionTypes[amount];
        for (int i = 0; i < sectionsTypes.Length; i++)
        {
            sectionsTypes[i] = SectionTypes.Straight;
        }
        return sectionsTypes;
    }
    
    /*
     *  Description: Creates a new Section object
     *  Return Type: Section
     */
    private static Section GetSection()
    {
        Section section = new Section();
        section.SectionType = SectionTypes.StartGrid;
        return section;
    }
}