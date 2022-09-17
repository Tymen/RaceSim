using Model.Classes;
using Model.Interfaces;

namespace Controller;

public class RaceController
{
    public Track Track;
    public List<IParticipant> Participants;
    public DateTime StartTime;

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