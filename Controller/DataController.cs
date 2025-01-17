using System.ComponentModel;
using Controller.EventArgs;
using Model.Classes;
using Model.Enums;
using Model.Interfaces;
using Newtonsoft.Json;
using RaceSim;

namespace Controller;

public static class DataController
{
    private static Competition _competition;
    public static RaceController CurrentRace;
    private static Random _random;
    public static EventHandler<NextRaceEventArgs> IsNextRace;
    public static event PropertyChangedEventHandler PropertyChanged;
    
    /*
     *  Description:
     *  Because there is no constructor in a static class.
     *  We need to have a way to initialize the new Competition
     */
    public static void Initialize(int driverAmount)
    {
        _random = new Random(DateTime.Now.Millisecond);
        _competition = new Competition();
        _competition.Participants = new List<Driver>();
        _competition.Tracks = new Queue<Track>();
        SetTracks();
        AddParticipant(driverAmount);
        SetParticipantsEquipment();
        CurrentRace = new RaceController(_competition.NextTrack(), _competition.Participants);
        CurrentRace.IsFinished += NextRace;
        VisualController.main(CurrentRace); 
        CurrentRace.Start();
    }

    /*
     *  Description:
     *  Check if there are tracks available in the ocmpetition. If this is the case it will start a new race.
     *  Adds these Participant object to the Competition Participants list.
     */
    private static void NextRace(object sender, IsFinishedEventArgs e)
    {
        CurrentRace.Reset();
        var nextTrack = _competition.NextTrack();
        if (nextTrack != null)
        {
            CurrentRace = new RaceController(nextTrack, e.ParticipantsList);
            CurrentRace.IsFinished += NextRace;
            VisualController.main(CurrentRace);
            IsNextRace.Invoke("DataController", new NextRaceEventArgs() {RaceController = CurrentRace});
            CurrentRace.Start();
        }
        else
        {
            Console.WriteLine("Competition Finished!");
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
            _competition.Participants.Add(GenerateParticipant(i));
        }
    }
    /*
 *  Description:
 *  Sets the equipment for every participant
 */
    public static void SetParticipantsEquipment()
    {
        for (int i = 0; i < _competition.Participants.Count; i++)
        {
            _competition.Participants[i].Equipment = RandomizedEquipment();
        }
    }
    
    /*
     *  Description:
     *  Generates random car equipment for participants
     *  Return Type: IEquipment
     */
    
    public static IEquipment RandomizedEquipment()
    {
        IEquipment equipment = new Car();
        equipment.Performance = _random.Next(7, 10);
        equipment.Quality = _random.Next(85, 100);
        equipment.Speed = _random.Next(8, 20);
        equipment.IsBroken = false;
        equipment.BreakdownChance = 1 - Math.Pow((equipment.Quality / 100.0), 2);
        equipment.FixChance = Math.Pow((equipment.Quality / 100.0), 2);
        
        return equipment;
    }
    /*
     *  Description:
     *  Creates multiple new Track objects.
     *  Adds these Track object to the Competition Tracks Queue
     */
    public static void AddTrack(Track track)
    {
        _competition.Tracks.Enqueue(track);
    }

    public static void SetTracks()
    {
        DirectoryInfo d = new DirectoryInfo(@"../../../../Controller/Tracks"); //Assuming Test is your Folder

        FileInfo[] Files = d.GetFiles("*.json"); //Getting Text files

        foreach(FileInfo file in Files )
        {
            Track track = JsonConvert.DeserializeObject<Track>(File.ReadAllText($@"../../../../Controller/Tracks/{file.Name}"));
            track.laps = 2;
            AddTrack(track);
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
        participant.Name = $"{nmbr + 1}";
        participant.Points = 0;
        participant.TeamColors = (TeamColors)values.GetValue(random.Next(values.Length));
        participant.lap = 1;
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