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
    public List<Driver> Participants;
    public DateTime StartTime;
    public event EventHandler<DriversChangedEventArgs> DriversChanged;
    public event EventHandler<IsFinishedEventArgs> IsFinished;

    private int finishedCount = 0;
    private Timer _timer;
    private Random _random;
    private Dictionary<Section, SectionData> _position;
    private int sectionLength = 100;
    
    /*
     *  Description: Constructor for initializing properties
     */
    public RaceController(Track track, List<Driver> participants)
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
    
    protected virtual void OnFinished(IsFinishedEventArgs e)
    {
        IsFinished.Invoke(this, e);
    }
    
    public void Start()
    {
        _timer.Enabled = true;
    }

    private void isTrackEmpty()
    {
        if (finishedCount >= Participants.Count)
        {
            OnFinished(new IsFinishedEventArgs() {ParticipantsList = Participants});
            //clearEventHandlers();
        }
    }

    private void clearEventHandlers()
    {
        DriversChanged = null;
        IsFinished = null;
    }
    
    // The Code is not working properly yet
    private void UpdateDriversPosition()
    {
        Queue<SectionData> nextSection = new Queue<SectionData>();
        for(LinkedListNode<Section> node = Track.Sections.First; node != null; node=node.Next){
            SectionData sectionData = _position[node.Value];
            SectionData newNextSection = null;
            
            Driver leftDriver = sectionData.Left;
            int leftDistance = leftDriver == null ? sectionData.DistanceLeft : CalculateDistance(leftDriver, sectionData.DistanceLeft);
            
            Driver rightDriver = sectionData.Right;
            int rightDistance = rightDriver == null ? sectionData.DistanceRight : CalculateDistance(rightDriver, sectionData.DistanceRight);
            if (nextSection.Count > 0)
            {
                SectionData newSectionData = nextSection.Peek();
                bool isFinish = node.Value.SectionType == SectionTypes.Finish;
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
                    if (isFinish)
                    {
                        leftDriver.lap += 1;
                        if (leftDriver.lap > Track.laps)
                        {
                            leftDriver = null;
                            leftDistance = 0;
                            finishedCount += 1;
                            isTrackEmpty();
                        }
                    }
                    newSectionData.Left = null;
                    newSectionData.DistanceLeft = 0;
                } else if (newSectionData.Left != null)
                {
                    newNextSection.DistanceLeft = CalculateDistance(newNextSection.Left, newNextSection.DistanceLeft);
                    if (newNextSection.DistanceLeft > sectionLength)
                    {
                        node = node.Next;
                        SectionData getNextSection = _position[node.Value];
                        if (getNextSection.Left == null)
                        {
                            getNextSection.Left = newNextSection.Left;
                            getNextSection.DistanceLeft = newNextSection.DistanceLeft;
                        } else if (getNextSection.Right == null)
                        {
                            getNextSection.Right = newNextSection.Left;
                            getNextSection.DistanceRight = newNextSection.DistanceLeft;
                        }
                        newSectionData.Right = null;
                        newSectionData.DistanceRight = 0;
                        node = node.Previous;
                    }
                }
                
                if (rightDriver == null && newSectionData.Right != null)
                {
                    rightDriver = newSectionData.Right;
                    rightDistance = newSectionData.DistanceRight;
                    if (isFinish)
                    {
                        rightDriver.lap += 1;
                        if (rightDriver.lap > Track.laps)
                        {
                            rightDriver = null;
                            rightDistance = 0;
                            finishedCount += 1;
                            isTrackEmpty();
                        }
                    }
                    newSectionData.Right = null;
                    newSectionData.DistanceRight = 0;
                } else if (newSectionData.Right != null)
                {
                    newNextSection.DistanceRight = CalculateDistance(newNextSection.Right, newNextSection.DistanceRight);
                    if (newNextSection.DistanceRight > sectionLength)
                    {
                        node = node.Next;
                        SectionData getNextSection = _position[node.Value];
                        if (getNextSection.Left == null)
                        {
                            getNextSection.Left = newNextSection.Right;
                            getNextSection.DistanceLeft = newNextSection.DistanceRight;
                        } else if (getNextSection.Right == null)
                        {
                            getNextSection.Right = newNextSection.Right;
                            getNextSection.DistanceRight = newNextSection.DistanceRight;
                        }
                        newSectionData.Right = null;
                        newSectionData.DistanceRight = 0;
                        node = node.Previous;
                    }
                }
            
                if (newSectionData.Left == null && newSectionData.Right == null)
                {
                    nextSection.Dequeue();      
                }
            }
            
            if (leftDriver != null)
            {
                node = node.Next;
                if (node.Next == null)
                {
                    SectionData connectSection = _position[Track.Sections.First.Value];
                    connectSection.Left = leftDriver;
                    connectSection.DistanceLeft = leftDistance - sectionLength;
                    
                    leftDriver = null;
                    leftDistance = 0;
                } else if (leftDistance > sectionLength)
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

                node = node.Previous;
                sectionData.DistanceLeft = leftDistance;
                sectionData.Left = leftDriver;
            }
            if (rightDriver != null)
            {
                node = node.Next;
                if (node.Next == null)
                {
                    SectionData connectSection = _position[Track.Sections.First.Value];
                    connectSection.Right = rightDriver;
                    connectSection.DistanceRight = rightDistance - sectionLength;
                    rightDriver = null;
                    rightDistance = 0;
                } else if (rightDistance > sectionLength)
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
        equipment.Performance = _random.Next(1, 7);
        equipment.Quality = _random.Next(1, 20);
        equipment.Speed = _random.Next(1, 8);
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