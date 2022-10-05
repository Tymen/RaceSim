using System.Diagnostics;
using Model.Classes;
using Model.Enums;
using Model.Interfaces;

namespace RaceSim;

public static class VisualController
{
    #region graphics
    private static string[] _finishHorizontal = { "-----", "  *  ", "  *  ", "  *  ", "-----" };
    private static string[] _startHorizontal = { "-----", "  #  ", "     ", "  #  ", "-----" };
    private static string[] _vertical = { "|   |", "|   |", "|   |", "|   |", "|   |" };
    private static string[] _horizontal = { "-----", "     ", "     ", "     ", "-----" };
    private static string[] _topLeftCorner = { "/----", "|    ", "|    ", "|    ", "|   /" };
    private static string[] _bottomLeftCorner = { @"|   \", "|    ", "|    ", "|    " ,@"\----" };
    private static string[] _topRightCorner = { @"----\", "    |", "    |", "    |", @"\   |" };
    private static string[] _bottomRightCorner = { "/   |", "    |", "    |", "    |", "----/" };
    private static string[] _blank = { "     ", "     ", "     ", "     ", "     " };
    #endregion

    private static int _trackSize;
    private static int _topPos = 0;
    private static int _leftPos = 0;
    private static float _width;
    private static float _height;

    /*
     *  Description:
     *  Loop through every section of the track to visualize it correctly in the console
     */
    public static void DrawTrack(Track track)
    {
        SetTrackSize(track);
        Console.SetWindowSize(Convert.ToInt32(_width), Convert.ToInt32(_height));
        LinkedList<Section> sections = track.Sections;
        foreach (Section section in sections)
        {
            switch(section.SectionType)
            {
                case SectionTypes.Finish:
                    DrawSection(_finishHorizontal, section);
                    break;
                case SectionTypes.Straight:
                    DrawSection(_horizontal, section);
                    break;
                case SectionTypes.Vertical:
                    DrawSection(_vertical, section);
                    break;
                case SectionTypes.BottomLeftCorner:
                    DrawSection(_bottomLeftCorner, section);
                    break;
                case SectionTypes.TopLeftCorner:
                    DrawSection(_topLeftCorner, section);
                    break;
                case SectionTypes.BottomRightCorner:
                    DrawSection(_bottomRightCorner, section);
                    break;
                case SectionTypes.TopRightCorner:
                    DrawSection(_topRightCorner, section);
                    break;
                case SectionTypes.Start:
                    DrawSection(_startHorizontal, section);
                    break;
                default:
                    DrawSection(_blank, section);
                    break;
            }
        }
    }

    public static void DrawParticipants(Dictionary<Section, SectionData> sectionData)
    {
        foreach(var item in sectionData)
        {
            DrawSection(GetVisualParticipantSection(item.Key, item.Value), item.Key);
        }
    }

    private static string[] GetVisualParticipantSection(Section section, SectionData sectionData)
    {
        switch (section.SectionType)
        {
            case SectionTypes.Vertical:
                return new[] { "|   |", "|   |", $"|{(sectionData.Left != null ? sectionData.Left.Name[0].ToString() : ' '.ToString())} {(sectionData.Right != null ? sectionData.Right.Name[0].ToString() : ' '.ToString())}|", "|   |", "|   |" };
            case SectionTypes.Straight:
                return new[]{ "-----", $"   {(sectionData.Left != null ? sectionData.Left.Name[0].ToString() : ' '.ToString())}  ", "     ", $"   {(sectionData.Right != null ? sectionData.Right.Name[0].ToString() : ' '.ToString())}  ", "-----" };
            case SectionTypes.Start:
                return new[]{ "-----", $"  {(sectionData.Left != null ? sectionData.Left.Name[0].ToString() : ' '.ToString())}#  ", "     ", $"  {(sectionData.Right != null ? sectionData.Right.Name[0].ToString() : ' '.ToString())}#  ", "-----" };
            default:
                string[] defaultList = new string[5];
                return defaultList;
        }
    }
    /*
     *  Description:
     *  set de max width and heigth of the track;
     */
    private static void SetTrackSize(Track track)
    {
        LinkedList<Section> sections = track.Sections;
        _width = -1;
        _height = -1;
        foreach (Section section in sections)
        {
            _width = section.VectorPosition.X > _width ? ((section.VectorPosition.X == 0) ? 5 : section.VectorPosition.X) : _width;
            _height = section.VectorPosition.Y > _height ? ((section.VectorPosition.Y == 0) ? 5 : section.VectorPosition.Y) : _height;
        }

        _height += 5;
    }
    
    /*
     *  Description:
     *  Write the section in the console on the correct location based on the VectorPosition
     */
    private static void DrawSection(string[] sectionArray, Section section)
    {
        int x = Convert.ToInt32(section.VectorPosition.X);
        int y = Convert.ToInt32(section.VectorPosition.Y);

        for (int i = 0; i < sectionArray.Length; i++)
        {
            Console.SetCursorPosition(x, y + i);
            Console.WriteLine(sectionArray[i]);
        }
    }
}