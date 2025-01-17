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
    private static string[] _finishHorizontal =
    {
        "-----", 
        " x*  ", 
        "  *  ", 
        " y*  ", 
        "-----"
    };
    private static string[] _startHorizontal =
    {
        "-----", 
        " x#  ", 
        "     ", 
        " y#  ", 
        "-----"
    };
    private static string[] _vertical =
    {
        "|   |", 
        "|   |", 
        "|x y|", 
        "|   |", 
        "|   |"
    };
    private static string[] _horizontal =
    {
        "-----", 
        "  x  ", 
        "     ", 
        "  y  ", 
        "-----"
    };
    private static string[] _topLeftCorner =
    {
        "/----", 
        "|y   ", 
        "|    ", 
        "|   x", 
        "|   /"
    };
    private static string[] _bottomLeftCorner =
    {
        @"|   \", 
         "|   x", 
         "|    ", 
         "|y   ",
        @"\----"
    };
    private static string[] _topRightCorner =
    {
        @"----\", 
         "  y |", 
         "    |", 
         "x   |", 
        @"\   |"
    };
    private static string[] _bottomRightCorner =
    {
        "/   |", 
        "x   |", 
        "    |", 
        "   y|", 
        "----/"
    };
    private static string[] _blank = { "     ", "     ", "     ", "     ", "     " };
    #endregion

    private static int _trackSize;
    private static int _topPos = 0;
    private static int _leftPos = 0;
    private static float _width;
    private static float _height;

    public static void main(RaceController raceController)
    {
        Console.Clear();
        raceController.DriversChanged += onDriversChanged;
        raceController.IsFinished += onIsFinished;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
    }
    
    private static void onDriversChanged(object sender, DriversChangedEventArgs e)
    {
        DrawTrack(e.positions);
    }
    private static void onIsFinished(object sender, IsFinishedEventArgs e)
    {
        Console.Clear();
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
        var keys = new List<Section>(sectionDataDictionary.Keys);

        for(int i = 0; i < keys.Count; i++)
        {
            Section section = keys[i];
            SectionData sectionData = sectionDataDictionary[section];
            Section? lastSection = i > 0 ? keys[i - 1] : null;
            
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
        string[] visualData = new string[visualSection.Length];
        for (int i = 0; i < visualSection.Length; i++)
        {
            string getSection = visualSection[i];

            Driver driver;
            // if there's more than one driver, assign according to Left and Right
            if (sectionData.Left != null && sectionData.Right != null)
            {
                getSection = getSection.Replace("x", sectionData.Left.Equipment.IsBroken ? "@" : sectionData.Left.Name[0].ToString());
                getSection = getSection.Replace("y", sectionData.Right.Equipment.IsBroken ? "@" : sectionData.Right.Name[0].ToString());
            }
            else
            {
                // otherwise, use the single driver (if any)
                driver = sectionData.Left ?? sectionData.Right;
                if (driver != null) {
                    string driverSymbol = driver.Equipment.IsBroken ? "@" : driver.Name[0].ToString();

                    if (sectionData.Left != null) {
                        getSection = getSection.Replace("x", driverSymbol);
                        getSection = getSection.Replace("y", " ");
                    }
                    else if (sectionData.Right != null) {
                        getSection = getSection.Replace("y", driverSymbol);
                        getSection = getSection.Replace("x", " ");
                    }
                }
                else
                {
                    getSection = getSection.Replace("x", " ");
                    getSection = getSection.Replace("y", " ");
                }
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
            Console.SetCursorPosition(x + 5, y + i + 5);
            Console.WriteLine(sectionArray[i]);
        }
    }
}