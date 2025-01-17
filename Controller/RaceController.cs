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
    public readonly List<Driver> _participants;
    public List<Driver> _finished;
    private List<Driver> _brokenDrivers = new List<Driver>();
    private Random _random = new Random(DateTime.Now.Millisecond);
    public DateTime StartTime;
    public event EventHandler<DriversChangedEventArgs> DriversChanged;
    public event EventHandler<IsFinishedEventArgs> IsFinished;
    
    private Timer _timer;
    private Dictionary<Section, SectionData> _position;
    private const int SectionLength = 100;
    private bool _raceFinished = false;
    
    /*
     *  Description: Constructor for initializing properties
     */
    public RaceController(Track track, List<Driver> participants)
    {
        Track = track;
        _participants = participants;
        _finished = new List<Driver>();
        _timer = new Timer(500);
        _timer.Elapsed += OnTimedEvent;
        SetParticipantsStartPosition();
    }
    
    private void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        if (_raceFinished)
        {
            return;
        }
        try
        {
            // Check each driver's equipment for breakage
            foreach (var driver in _participants)
            {
                if (!_brokenDrivers.Contains(driver) && _random.NextDouble() < driver.Equipment.BreakdownChance)
                {
                    driver.Equipment.IsBroken = true;
                    driver.Equipment.Speed = _random.Next(3, 20);
                    _brokenDrivers.Add(driver);
                }
            }

            // Attempt to fix each broken driver's equipment
            foreach (var driver in _brokenDrivers.ToList())
            {
                if (_random.NextDouble() < driver.Equipment.FixChance)
                {
                    driver.Equipment.IsBroken = false;
                    driver.Equipment.Speed -= _random.Next(1, 8); // Decrease speed as a penalty
                    _brokenDrivers.Remove(driver);
                }
            }
            OnDriversChanged(new DriversChangedEventArgs() { track = Track, positions = _position });
            UpdateDriversPosition();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception in OnTimedEvent: " + ex);
        }
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
        _timer.Start();
    }

    private void isTrackEmpty()
    {
        if (_finished.Count >= _participants.Count)
        {
            _raceFinished = true;
            OnFinished(new IsFinishedEventArgs() {ParticipantsList = _finished});
        }
    }

    public void Reset()
    {
        // Clearing track and lists
        Track = null;

        // Stopping the timer
        _timer.Stop();
        _timer.Elapsed -= OnTimedEvent;
        _timer = null;

        // Unsubscribing all event handlers
        if(DriversChanged != null)
            foreach (Delegate d in DriversChanged.GetInvocationList())
                DriversChanged -= (EventHandler<DriversChangedEventArgs>)d;
    
        if(IsFinished != null)
            foreach (Delegate d in IsFinished.GetInvocationList())
                IsFinished -= (EventHandler<IsFinishedEventArgs>)d;
    }

    private void UpdateDriversPosition()
    {
        Queue<SectionData> nextSection = PrepareNextSectionQueue();

        for (LinkedListNode<Section> node = Track.Sections.First; node != null; node = node.Next)
        {
            if (node?.Value != null)
            {
                SectionData sectionData = _position[node.Value];

                if (sectionData != null)
                {
                    SectionData newNextSection = null;
        
                    var leftResult = HandleDriverOnSection(sectionData.Left, sectionData.DistanceLeft, sectionData, true);
                    var rightResult = HandleDriverOnSection(sectionData.Right, sectionData.DistanceRight, sectionData, false);

                    Driver leftDriver = leftResult.Item1;
                    int leftDistance = leftResult.Item2;
        
                    Driver rightDriver = rightResult.Item1;
                    int rightDistance = rightResult.Item2;
                    
                    newNextSection = HandleSectionDataQueue(nextSection, newNextSection, node);

                    UpdateDriverOnSection(leftDriver, leftDistance, sectionData, node.Value.SectionType, node, true);
                    UpdateDriverOnSection(rightDriver, rightDistance, sectionData, node.Value.SectionType, node, false);

                    if (newNextSection != null)
                    {
                        nextSection.Enqueue(newNextSection);
                    }
                } 
            }
            else
            {
                throw new Exception("Node is null");
            }
        }
    }
    
    private Queue<SectionData> PrepareNextSectionQueue()
    {
        return new Queue<SectionData>();
    }
    
    private Tuple<Driver, int> HandleDriverOnSection(Driver driver, int distance, SectionData sectionData, bool isLeftDriver)
    {
        Driver newDriver = driver;
        int newDistance = distance;
        if (driver != null)
        {
            if (!driver.Equipment.IsBroken)
            {
                if (driver != null)
                {
                    driver.Equipment.Speed = _random.Next(1, 10);
                }
                newDistance = driver == null ? distance : CalculateDistance(driver, distance);

                if (newDriver != null)
                {
                    newDriver = isLeftDriver ? sectionData.Left : sectionData.Right;
                    if (isLeftDriver)
                    {
                        sectionData.DistanceLeft = newDistance;
                    }
                    else
                    {
                        sectionData.DistanceRight = newDistance;
                    }
                }
                else if (isLeftDriver ? sectionData.Left != null : sectionData.Right != null)
                {
                    newDistance = CalculateDistance(isLeftDriver ? sectionData.Left : sectionData.Right,
                        isLeftDriver ? sectionData.DistanceLeft : sectionData.DistanceRight);
                }    
            }
        }
        
        return Tuple.Create(newDriver, newDistance);
    }
    
    /*
     * Description: This function handles the logic for moving drivers from one section to the next
     */
    private Tuple<Driver, int> UpdateDriverOnSection(Driver driver, int distance, SectionData sectionData, SectionTypes sectionType, LinkedListNode<Section> node, bool isLeftDriver)
    {
        if (driver == null)
        {
            return Tuple.Create<Driver, int>(null, 0);
        }

        if (distance <= SectionLength)
        {
            UpdateCurrentSectionData(sectionData, driver, distance, isLeftDriver);
            return Tuple.Create<Driver, int>(null, 0);
        }

        // Prepare for the next section
        LinkedListNode<Section> nextNode;
        if (node.Next == null) // This is the last section
        {
            nextNode = Track.Sections.First; // Wrap around to the first section
        }
        else if (node == Track.Sections.First.Previous) // This is the section before the first section
        {
            nextNode = Track.Sections.First; // Move to the first section
        }
        else
        {
            nextNode = node.Next;
        }
        SectionData nextSectionData = GetOrCreateSectionData(nextNode);


        // Both spots are occupied, driver has to wait
        if (nextSectionData.Left != null && nextSectionData.Right != null)
        {
            return Tuple.Create(driver, distance);
        }

        // Move the driver to the next section, prefer left spot if available
        if (nextSectionData.Left == null)
        {
            if (nextNode.Value.SectionType == SectionTypes.Finish)
            {
                if (driver.lap > Track.laps)
                {
                    driver.resetDriverForNextRace();
                    _finished.Add(driver);
                    isTrackEmpty();
                }
                else
                {
                    nextSectionData.Left = driver;
                    nextSectionData.DistanceLeft = distance - SectionLength;
                    driver.lap++;
                }

            }
            else
            {
                nextSectionData.Left = driver;
                nextSectionData.DistanceLeft = distance - SectionLength;
            }

        }
        else if (nextSectionData.Right == null)
        {
            if (nextNode.Value.SectionType == SectionTypes.Finish)
            {
                if (driver.lap > Track.laps)
                {
                    driver.resetDriverForNextRace();
                    _finished.Add(driver);
                    isTrackEmpty();
                }
                else
                {
                    nextSectionData.Right = driver;
                    nextSectionData.DistanceLeft = distance - SectionLength;
                    driver.lap++;
                }

            }
            else
            {
                nextSectionData.Right = driver;
                nextSectionData.DistanceLeft = distance - SectionLength;
            }
        }

        // Clean up the current section data
        ClearCurrentSectionData(sectionData, isLeftDriver);

        return Tuple.Create(driver, distance - SectionLength);
    }

    /*
     * description: This function is used to clear the data for a driver on the current section once the driver has moved to the next section
     */
    private void ClearCurrentSectionData(SectionData sectionData, bool isLeftDriver)
    {
        if (isLeftDriver)
        {
            sectionData.Left = null;
            sectionData.DistanceLeft = 0;
        }
        else
        {
            sectionData.Right = null;
            sectionData.DistanceRight = 0;
        }
    }
    
    /*
     * Description: This function is used to update a driver's data on the current section
     */
    private void UpdateCurrentSectionData(SectionData sectionData, Driver driver, int distance, bool isLeftDriver)
    {
        if (isLeftDriver)
        {
            sectionData.Left = driver;
            sectionData.DistanceLeft = distance;
        }
        else
        {
            sectionData.Right = driver;
            sectionData.DistanceRight = distance;
        }
    }

    /*
     * Description:  This function returns the SectionData for a given section.
     *               If the SectionData does not exist,
     *               it creates a new SectionData object and adds it to the position map before returning it.
     */
    private SectionData GetOrCreateSectionData(LinkedListNode<Section> node)
    {
        if (!_position.TryGetValue(node.Value, out SectionData nextSectionData))
        {
            nextSectionData = new SectionData();
            _position[node.Value] = nextSectionData;
        }

        return nextSectionData;
    }

    private int CalculateDistance(Driver driver, int distance)
    {
        return distance + (driver.Equipment.Performance * driver.Equipment.Speed);
    }

    private SectionData HandleSectionDataQueue(Queue<SectionData> nextSection, SectionData newNextSection, LinkedListNode<Section> node)
    {
        if (nextSection.Count > 0)
        {
            SectionData newSectionData = nextSection.Peek();
            bool isFinish = node.Value.SectionType == SectionTypes.Finish;
        
            // This part needs to call HandleDriverOnSection() and UpdateDriverOnSection() 
            // with the appropriate conditions as in your original method
        
            if (newSectionData.Left == null && newSectionData.Right == null)
            {
                nextSection.Dequeue();
            }
        }
    
        return newNextSection;
    }

    public void SetParticipantsStartPosition()
    {
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
        foreach (Driver participant in _participants)
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
     *  Looks for corresponding SectionData based on the Section as Key
     *  Return Type: SectionData
     */
    public SectionData GetSectionData(Section section)
    {
        return GetPosition()[section];
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