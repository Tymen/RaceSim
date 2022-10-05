using System.Numerics;
using Model.Classes;
using Model.Enums;
using Newtonsoft.Json;
using RaceSim;

namespace Controller;

public class TrackController
{
    private Track _track;
    #region graphics
    private static string[] _finishHorizontal = { "-----", "  *  ", "  *  ", "  *  ", "-----" };
    private static string[] _vertical = { "|   |", "|   |", "|   |", "|   |", "|   |" };
    private static string[] _horizontal = { "-----", "     ", "     ", "     ", "-----" };
    private static string[] _topLeftCorner = { "/----", "|    ", "|    ", "|    ", "|   /" };
    private static string[] _bottomLeftCorner = { @"|   \", "|    ", "|    ", "|    " ,@"\----" };
    private static string[] _topRightCorner = { @"----\", "    |", "    |", "    |", @"\   |" };
    private static string[] _bottomRightCorner = { "/   |", "    |", "    |", "    |", "----/" };
    private static string[] _blank = { "     ", "     ", "     ", "     ", "     " };
    #endregion
    
    private int _topPos = 0;
    private int _leftPos = 0;
    private float _width;
    private float _height;
    private float _nextSectionX;
    private float _nextSectionY;
    
    public TrackController(string name)
    {
        LinkedList<Section> sections = new LinkedList<Section>();
        sections.AddLast(new Section() { VectorPosition = new Vector2(0, 0), SectionType = SectionTypes.Straight });
        _track = new Track(name, sections);
    }
    
    public void AddTrackSection(ConsoleKeyInfo keyinfo)
    {
        switch (keyinfo.Key)
        {
            case ConsoleKey.RightArrow:
                _track.Sections.AddLast(GetSection(SectionTypes.Straight, keyinfo.Key));
                break;
            case ConsoleKey.LeftArrow:
                _track.Sections.AddLast(GetSection(SectionTypes.Straight, keyinfo.Key));
                break;
            case ConsoleKey.UpArrow:
                _track.Sections.AddLast(GetSection(SectionTypes.Vertical, keyinfo.Key));
                break;
            case ConsoleKey.DownArrow:
                _track.Sections.AddLast(GetSection(SectionTypes.Vertical, keyinfo.Key));
                break;
            case ConsoleKey.Spacebar:
                _track.Sections.AddLast(GetSection(SectionTypes.Finish, keyinfo.Key));
                break;
            case ConsoleKey.D:
                _track.Sections.AddLast(GetSection(SectionTypes.Start, keyinfo.Key));
                break;
        }
        
        _track.NextSection();
        Console.Clear();
        VisualController.DrawTrack(_track);
    }

    public Section GetSection(SectionTypes sectionType, ConsoleKey key)
    {
        Section section = new Section();
        Section lastSection = _track._currentSection;

        section.SectionType = sectionType;
        if (_track.Sections.Count > 0)
        {
            UpdateLastSection(section, lastSection, key);
        }
        SetNextSectionLocation(lastSection, key);

        section.VectorPosition = new Vector2(_nextSectionX, _nextSectionY);
        return section;
    }

    public async  void  SaveTrack()
    {
        var jsonString = JsonConvert.SerializeObject(_track) ?? throw new ArgumentNullException("Track is empty");
        await File.WriteAllTextAsync($@"../../../../Controller/Tracks/{_track.Name}.json", jsonString);
    }
    private void SetNextSectionLocation(Section lastSection, ConsoleKey key)
    {
        float x = lastSection.VectorPosition.X;
        float y = lastSection.VectorPosition.Y;
        if (lastSection.SectionType != SectionTypes.Vertical)
        {
            if (lastSection.SectionType is SectionTypes.Straight or SectionTypes.Finish or SectionTypes.Start)
            {
                x = key == ConsoleKey.LeftArrow ? lastSection.VectorPosition.X - 5 : lastSection.VectorPosition.X + 5;
            }
        }

        if (lastSection.SectionType is not (SectionTypes.Straight and SectionTypes.Finish and SectionTypes.Start))
        {
            if (lastSection.SectionType == SectionTypes.Vertical)
            {
                y = key == ConsoleKey.UpArrow ? lastSection.VectorPosition.Y - 5 : lastSection.VectorPosition.Y + 5;
            }
        } 
        if ((lastSection.SectionType == SectionTypes.TopRightCorner || lastSection.SectionType == SectionTypes.TopLeftCorner) && key == ConsoleKey.DownArrow )
        {
            y = lastSection.VectorPosition.Y + 5;
        } else if ((lastSection.SectionType == SectionTypes.BottomRightCorner || lastSection.SectionType == SectionTypes.BottomLeftCorner) && key == ConsoleKey.UpArrow)
        {
            y = lastSection.VectorPosition.Y - 5;
        }
        else if (lastSection.SectionType == SectionTypes.TopRightCorner || lastSection.SectionType == SectionTypes.BottomRightCorner && key == ConsoleKey.LeftArrow)
        {
            x = lastSection.VectorPosition.X - 5;
        }else if ((lastSection.SectionType == SectionTypes.BottomLeftCorner || (lastSection.SectionType == SectionTypes.TopLeftCorner) && key == ConsoleKey.RightArrow))
        {
            x = lastSection.VectorPosition.X + 5;
        }

        _nextSectionX = x;
        _nextSectionY = y;
    }
    
    private void UpdateLastSection(Section currentSection, Section lastSection, ConsoleKey key)
    {
        _track.PreviousSection();
        Section sectionBeforeLast = _track._currentSection;
        
        if (lastSection.SectionType == SectionTypes.Vertical && currentSection.SectionType == SectionTypes.Straight)
        {
            if (sectionBeforeLast.VectorPosition.Y < lastSection.VectorPosition.Y)
            {
                lastSection.SectionType = key == ConsoleKey.LeftArrow ? SectionTypes.BottomRightCorner : SectionTypes.BottomLeftCorner;
            }
            else
            {
                lastSection.SectionType = key == ConsoleKey.LeftArrow ? SectionTypes.TopRightCorner : SectionTypes.TopLeftCorner;
            }
            
        } else if (lastSection.SectionType == SectionTypes.Straight && currentSection.SectionType == SectionTypes.Vertical)
        {
            if (sectionBeforeLast.VectorPosition.X < lastSection.VectorPosition.X)
            {
                lastSection.SectionType = key == ConsoleKey.UpArrow ? SectionTypes.BottomRightCorner : SectionTypes.TopRightCorner;
            }
            else
            {
                lastSection.SectionType = key == ConsoleKey.UpArrow ? SectionTypes.BottomLeftCorner : SectionTypes.TopLeftCorner;
            }
        }
        
        _track.NextSection();
    }
}