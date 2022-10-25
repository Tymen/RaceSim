using System.Diagnostics;
using Controller;
using Controller.EventArgs;
using Model.Classes;
using Model.Enums;
using Model.Interfaces;

namespace RaceSim;

public static class VisualController
{
    #region graphics
    private static string[] _finishHorizontal = { "-----", "  *  ", "  *  ", "  *  ", "-----" };
    private static string[] _startHorizontal = { "-----", " x#  ", "     ", " x#  ", "-----" };
    private static string[] _vertical = { "|   |", "|   |", "|x x|", "|   |", "|   |" };
    private static string[] _horizontal = { "-----", "  x  ", "     ", "  x  ", "-----" };
    private static string[] _topLeftCorner = { "/----", "|x   ", "|    ", "|   x", "|   /" };
    private static string[] _bottomLeftCorner = { @"|   \", "|   x", "|    ", "|x   " ,@"\----" };
    private static string[] _topRightCorner = { @"----\", "   x|", "    |", "x   |", @"\   |" };
    private static string[] _bottomRightCorner = { "/   |", "x   |", "    |", "   x|", "----/" };
    private static string[] _blank = { "     ", "     ", "     ", "     ", "     " };
    #endregion

    private static int _trackSize;
    private static int _topPos = 0;
    private static int _leftPos = 0;
    private static float _width;
    private static float _height;

    public static void main(RaceController raceController)
    {
        raceController.DriversChanged += onDriversChanged;
    }
    static void onDriversChanged(object sender, DriversChangedEventArgs e)
    {
        DrawTrack(e.positions);
    }
    /*
     *  Description:
     *  Loop through every section of the track to visualize it correctly in the console
     */
    public static void DrawTrack(Track track)
    {
        SetTrackSize(track);
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

    public static void DrawTrack(Dictionary<Section, SectionData> sectionDataDictionary)
    {
        foreach(var item in sectionDataDictionary)
        {
            Section section = item.Key;
            SectionData sectionData = item.Value;

            switch(section.SectionType)
            {
                case SectionTypes.Finish:
                    DrawSection(GetVisualSection(sectionData, _finishHorizontal), section);
                    break;
                case SectionTypes.Straight:
                    DrawSection(GetVisualSection(sectionData, _horizontal), section);
                    break;
                case SectionTypes.Vertical:
                    DrawSection(GetVisualSection(sectionData, _vertical), section);
                    break;
                case SectionTypes.BottomLeftCorner:
                    DrawSection(GetVisualSection(sectionData, _bottomLeftCorner), section);
                    break;
                case SectionTypes.TopLeftCorner:
                    DrawSection(GetVisualSection(sectionData, _topLeftCorner), section);
                    break;
                case SectionTypes.BottomRightCorner:
                    DrawSection(GetVisualSection(sectionData, _bottomRightCorner), section);
                    break;
                case SectionTypes.TopRightCorner:
                    DrawSection(GetVisualSection(sectionData, _topRightCorner), section);
                    break;
                case SectionTypes.Start:
                    DrawSection(GetVisualSection(sectionData, _startHorizontal), section);
                    break;
                default:
                    DrawSection(GetVisualSection(sectionData, _blank), section);
                    break;
            }
        }
    }

    private static string[] GetVisualSection(SectionData sectionData, string[] visualSection)
    {
        bool left = false;
        string[] visualData = new string[visualSection.Length];
        for (int i = 0; i < visualSection.Length; i++)
        {
            string getSection = visualSection[i];
            if (getSection.Contains("x"))
            {
                int XCount = getSection.Count(x => x == 'x');
                if (sectionData.Left != null && XCount > 1)
                {
                    getSection = getSection.Replace("x", sectionData.Left.Name[0].ToString());
                    XCount = getSection.Count(x => x == 'x');
                }

                if (sectionData.Left != null && !left && XCount == 1)
                {
                    getSection = getSection.Replace("x", sectionData.Left.Name[0].ToString());
                    XCount = getSection.Count(x => x == 'x');
                    left = true;
                }

                if (sectionData.Right != null && left && XCount == 1)
                {
                    getSection = getSection.Replace("x", sectionData.Right.Name[0].ToString());
                }
            }
            if (sectionData.Left == null && sectionData.Right == null)
            {
                getSection = getSection.Replace("x", " ");
            }    
            visualData[i] = getSection;
        }
        
        return visualData;
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