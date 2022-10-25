using Model.Classes;
using Model.Enums;
using Model.Interfaces;
using Newtonsoft.Json;

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
    public static void Initialize(int driverAmount)
    {
        Competition = new Competition();
        Competition.Participants = new List<IParticipant>();
        Competition.Tracks = new Queue<Track>();
        SetTracks();
        AddParticipant(driverAmount);
        CurrentRace = new RaceController(Competition.NextTrack(), Competition.Participants);
    }

    /*
     *  Description:
     *  Check if there are tracks available in the ocmpetition. If this is the case it will start a new race.
     *  Adds these Participant object to the Competition Participants list.
     */
    public static void NextRace()
    {
        Competition.NextTrack();
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
    public static void AddTrack(Track track)
    {
        Competition.Tracks.Enqueue(track);
    }

    public static void SetTracks()
    {
        DirectoryInfo d = new DirectoryInfo(@"../../../../Controller/Tracks"); //Assuming Test is your Folder

        FileInfo[] Files = d.GetFiles("*.json"); //Getting Text files

        foreach(FileInfo file in Files )
        {
            Track track = JsonConvert.DeserializeObject<Track>(File.ReadAllText($@"../../../../Controller/Tracks/{file.Name}"));
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
    
    private static SectionTypes[] simpleTrack()
    {
        SectionTypes[] result = new SectionTypes[15];
        int index = 0;
        for (int y = 0; y < 3; y++)
        {
            switch (y)
            {
                case 0:
                    result[index] = SectionTypes.TopLeftCorner;
                    index++;
                    result[index] = SectionTypes.Straight;
                    index++;
                    result[index] = SectionTypes.Finish;
                    index++;
                    result[index] = SectionTypes.Straight;
                    index++;
                    result[index] = SectionTypes.TopRightCorner;
                    index++;
                    break;
                case 1:
                    result[index] = SectionTypes.Vertical;
                    index++;
                    result[index] = SectionTypes.Null;
                    index++;
                    result[index] = SectionTypes.Null;
                    index++;
                    result[index] = SectionTypes.Null;
                    index++;
                    result[index] = SectionTypes.Vertical;
                    index++;
                    break;
                default:
                    result[index] = SectionTypes.BottomLeftCorner;
                    index++;
                    result[index] = SectionTypes.Straight;
                    index++;
                    result[index] = SectionTypes.Straight;
                    index++;
                    result[index] = SectionTypes.Straight;
                    index++;
                    result[index] = SectionTypes.BottomRightCorner;
                    index++;
                    break;
            }
        }
        return result;
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