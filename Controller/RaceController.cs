using System.Timers;
using Controller.EventArgs;
using Model.Classes;
using Model.Enums;
using Model.Interfaces;
using Timer = System.Timers.Timer;

namespace Controller;

public class RaceController
{
    public Track Track;
    public List<IParticipant> Participants;
    public DateTime StartTime;
    public event EventHandler<DriversChangedEventArgs> DriversChanged;
    
    private Timer _timer;
    private Random _random;
    private Dictionary<Section, SectionData> _position;
    /*
     *  Description: Constructor for initializing properties
     */
    public RaceController(Track track, List<IParticipant> participants)
    {
        Track = track;
        Participants = participants;
        _random = new Random(DateTime.Now.Millisecond);
        _timer = new Timer(500);
        _timer.Elapsed += OnTimedEvent;
        SetParticipantsStartPosition();
    }
    private static void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        // Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
        //     e.SignalTime);
    }
    protected virtual void OnDriversChanged(DriversChangedEventArgs e)
    {
        Console.WriteLine("invoked");
        DriversChanged.Invoke(this, e);
    }
    public void Start()
    {
        _timer.Enabled = true;
        OnDriversChanged(new DriversChangedEventArgs(){ track = Track});
    }
    public void SetParticipantsStartPosition()
    {
        LinkedListNode<Section> lastSection;
        Queue<Section> startSections = new Queue<Section>();
        _position = new Dictionary<Section, SectionData>();
        foreach (Section section in Track.Sections)
        {
            _position.Add(section, new SectionData());
            if (section.SectionType == SectionTypes.Start)
            {
                startSections.Enqueue(section);
            }
        }
        startSections = new Queue<Section>(startSections.Reverse());
        foreach (IParticipant participant in Participants)
        {
            SectionData startSectionData = GetSectionData(startSections.Peek());
            if (startSectionData.Left == null)
            {
                startSectionData.Left = participant;
                GetPosition()[startSections.Peek()] = startSectionData;
            } 
            else if (startSectionData.Right == null)
            {
                startSectionData.Right = participant;
                GetPosition()[startSections.Peek()] = startSectionData;
            }
            else
            {
                GetPosition()[startSections.Peek()] = startSectionData;
                startSections.Dequeue();
                GetSectionData(startSections.Peek()).Left = participant;
            }
        }
    }

    /*
     *  Description:
     *  Sets the equipment for every participant
     */
    public void SetParticipantsEquipment()
    {
        for (int i = 0; i < Participants.Count; i++)
        {
            Participants[i].Equipment = RandomizedEquipment();
        }
    }
    
    /*
     *  Description:
     *  Generates random car equipment for participants
     *  Return Type: IEquipment
     */
    public IEquipment RandomizedEquipment()
    {
        IEquipment equipment = new Car();
        equipment.Performance = _random.Next(0, 100);
        equipment.Quality = _random.Next(0, 20);
        equipment.Speed = _random.Next(0, 200);
        equipment.IsBroken = false;
        
        return equipment;
    }
    
    /*
     *  Description:
     *  Looks for corresponding SectionData based on the Section as Key
     *  Return Type: SectionData
     */
    public SectionData GetSectionData(Section section)
    {
        return GetPosition()[section];
    }

    /*
     *  Description:
     *  When there is no paramter given it will add a new section to the position.
     *  Return Type: SectionData
     */
    public SectionData GetSectionData()
    {
        SectionData sectionData = new SectionData();
        Section section = new Section();
        
        GetPosition().Add(section, sectionData);

        return sectionData;
    }
    
    /*
     *  Description: Gets the private Position property.
     *  Return Type: Dictionary<Sectiom, SectionData>
     */
    public Dictionary<Section, SectionData> GetPosition()
    {
        return _position;
    }
}