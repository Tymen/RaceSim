using System.Collections;
using Model.Enums;

namespace Model.Classes;

public class Track
{
    public string Name { get; set; }
    public LinkedList<Section> Sections { get; set; }
    public Section _currentSection { get; set; }

    public Track()
    {
        
    }
    public Track(string name, SectionTypes[] sectionTypes)
    {
        Name = name;
        LinkedList<Section> sections = GetSections(sectionTypes);
        
        Sections = sections;
    }
    public Track(string name, LinkedList<Section> sectionsList)
    {
        Name = name;
        LinkedList<Section> sections = sectionsList;

        Sections = sections;
        _currentSection = Sections.First!.Value;
    }
    private LinkedList<Section> GetSections(SectionTypes[] sectionTypes)
    {
        LinkedList<Section> result = new LinkedList<Section>();
        
        for (int i = 0; i < sectionTypes.Length; i++)
        {
            result.AddLast(new Section()
            {
                SectionType = sectionTypes[i]
            });
        }
        return result;
    }
    public void NextSection()
    {
        // Find the current node
        var curNode = Sections.Find(_currentSection);
        // Point to the next
        LinkedListNode<Section> nextNode = curNode.Next;

        // Check if at the end of the list
        nextNode = nextNode == null ? Sections.First : nextNode;
        _currentSection = nextNode.Value;
    }
    
    public void PreviousSection()
    {
        // Find the current node
        var curNode = Sections.Find(_currentSection);

        // Point to the next
        LinkedListNode<Section> nextNode = curNode.Previous;
        
        nextNode = nextNode == null ? Sections.Last : nextNode;
        _currentSection = nextNode.Value;
    }
}