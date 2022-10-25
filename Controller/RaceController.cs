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

    private int sectionLength = 100;
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
    private void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        OnDriversChanged(new DriversChangedEventArgs() { track = Track, positions = _position });
        UpdateDriversPosition();
    }
    protected virtual void OnDriversChanged(DriversChangedEventArgs e)
    {
        DriversChanged.Invoke(this, e);
    }
    public void Start()
    {
        _timer.Enabled = true;
    }

    private void UpdateDriversPosition()
    {
        Queue<SectionData> nextSection = new Queue<SectionData>();
        for(LinkedListNode<Section> node = Track.Sections.First; node != null; node=node.Next){
            SectionData sectionData = _position[node.Value];
            SectionData newNextSection = null;
            Driver leftDriver = sectionData.Left;
            int leftDistance = sectionData.DistanceLeft;
            
            Driver rightDriver = sectionData.Right;
            int rightDistance = sectionData.DistanceRight;
            
            if (nextSection.Count > 0)
            {
                SectionData newSectionData = nextSection.Peek();
                if (leftDriver != null)
                {
                    nextSection.Dequeue();
                }
            
                if (rightDriver != null)
                {
                    nextSection.Dequeue();
                }
                if (leftDriver == null && newSectionData.Left != null)
                {
                    leftDriver = newSectionData.Left;
                    leftDistance = newSectionData.DistanceLeft;
                    newSectionData.Left = null;
                    newSectionData.DistanceLeft = 0;
                }
                if (rightDriver == null && newSectionData.Right != null)
                {
                    rightDriver = newSectionData.Right;
                    rightDistance = newSectionData.DistanceRight;
                    newSectionData.Right = null;
                    newSectionData.DistanceRight = 0;
                }
            
                if (newSectionData.Left == null && newSectionData.Right == null)
                {
                    nextSection.Dequeue();      
                }
            }
            // node = node.Next;
            // if (node == null)
            // {
            //     sectionData = _position[Track.Sections.First.Value];
            //     node = node.Previous;
            // }
            if (leftDriver != null)
            {
                leftDistance = CalculateDistance(leftDriver, leftDistance);
                node = node.Next;
                if (leftDistance > sectionLength && _position[node.Value].Left == null)
                {
                    if (newNextSection == null)
                    {
                        newNextSection = new SectionData();
                    }
                    newNextSection.Left = leftDriver;
                    newNextSection.DistanceLeft = leftDistance - sectionLength;
            
                    leftDriver = null;
                    leftDistance = 0;
                }
                else
                {
                    leftDistance = 100;
                }

                node = node.Previous;
                sectionData.DistanceLeft = leftDistance;
                sectionData.Left = leftDriver;
            }
            if (rightDriver != null)
            {
                rightDistance = CalculateDistance(rightDriver, rightDistance);
                node = node.Next;
                if (rightDistance > sectionLength && _position[node.Value].Right == null)
                {
                    if (newNextSection == null)
                    {
                        newNextSection = new SectionData();
                    }
                    newNextSection.Right = rightDriver;
                    newNextSection.DistanceRight = rightDistance - sectionLength;
            
                    rightDriver = null;
                    rightDistance = 0;
                }
                else
                {
                    rightDistance = 100;
                }

                node = node.Previous;
                
                sectionData.DistanceRight = rightDistance;
                sectionData.Right = rightDriver;
            }
            
            if (newNextSection != null)
            {
                nextSection.Enqueue(newNextSection);
            }
        } 
    }
    private int CalculateDistance(Driver driver, int distance)
    {
        return distance + (driver.Equipment.Performance * driver.Equipment.Speed);
    }
    public void SetParticipantsStartPosition()
    {
        SetParticipantsEquipment();
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
        foreach (Driver participant in Participants)
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
        equipment.Performance = _random.Next(0, 10);
        equipment.Quality = _random.Next(0, 20);
        equipment.Speed = _random.Next(0, 8);
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