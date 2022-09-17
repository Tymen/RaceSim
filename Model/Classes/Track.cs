using System.Collections;
using Model.Enums;

namespace Model.Classes;

public class Track
{
    public string Name { get; set; }
    public LinkedList<Section> Sections { get; set; }

    public Track(string name, SectionTypes[] sectionTypes)
    {
        Name = name;
        LinkedList<Section> sections = GetSections(sectionTypes);
        

        
        Sections = sections;
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
}