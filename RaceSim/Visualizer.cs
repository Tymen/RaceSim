using System.Diagnostics;
using Model.Classes;
using Model.Enums;

namespace RaceSim;

public static class Visualizer
{
    #region graphics
    private static string[] _finishHorizontal = { "-----", "   * ", "   * ", "   * ", "-----" };
    private static string[] _horizontal = { "-----", "     ", "     ", "     ", "-----" };
    private static string[] _topLeftCorner = { "/----", "|    ", "|    ", "|    ", "|    " };
    private static string[] _bottomLeftCorner = { "|    ", "|    ", "|    ", "|    " ,@"\----" };
    private static string[] _topRightCorner = { @"----\", "    |", "    |", "    |", "    |" };
    private static string[] _bottomRightCorner = { "    |", "    |", "    |", "    |", "----/" };
    #endregion

    private static int _trackSize;
    private static int _topPos = 0;
    private static int _leftPos = 0;
    public static void Initializer(int trackSize)
    {
        SetupConsole(trackSize);
    }

    public static void SetupConsole(int trackSize)
    {
        _trackSize = trackSize * 5;
        Console.SetWindowSize(_trackSize, _trackSize);
    }
    
    public static void DrawTrack(Track track)
    {
        LinkedList<Section> sections = track.Sections;
        foreach (Section section in sections)
        {
            switch(section.SectionType)
            {
                case SectionTypes.Finish:
                    DrawSection(_finishHorizontal);
                    break;
                case SectionTypes.Straight:
                    DrawSection(_horizontal);
                    break;
                case SectionTypes.BottomLeftCorner:
                    DrawSection(_bottomLeftCorner);
                    break;
            }
        }
    }

    private static void DrawSection(string[] section)
    {
        if (_leftPos >= _trackSize)
        {
            NewSectionRow();
        }        
        
        for (int i = 0; i < section.Length; i++)
        {
            Console.SetCursorPosition(_leftPos, _topPos + i);
            Console.WriteLine(section[i]);
        }

        _leftPos += 5;
    }

    public static void NewSectionRow()
    {
        _leftPos = 0;
        _topPos += 5;
    }
}