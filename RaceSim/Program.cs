// See https://aka.ms/new-console-template for more information
using Controller;
using RaceSim;

StartUp();

for (; ; )
{
    Thread.Sleep(25);
}

void StartUp()
{
    bool startup = true;
    string question = "Want to start the simulation(sim) or create a track(track)? ";
    var result = "";
    while (startup)
    {
        Console.WriteLine("------------");
        Console.WriteLine("");
        Console.WriteLine(question);
        Console.WriteLine("");
        Console.WriteLine("------------");
        result = (Console.ReadLine()).ToLower();

        if (result.Equals("sim"))
        {
            startup = false;
            Console.Clear();
            StartSim();
        } else if (result.Equals("track"))
        {
            startup = false;
            Console.Clear();
            StartTrackBuilder();
        }
    }
}

void StartTrackBuilder()
{
    ConsoleKeyInfo keyinfo;
    do
    {
        keyinfo = Console.ReadKey();
        Console.Clear();
    }
    while (keyinfo.Key != ConsoleKey.X);
}

void StartSim()
{
    DataController.Initialize();
    DataController.NextRace();
    Visualizer.Initializer(5);
    Visualizer.DrawTrack(DataController.CurrentRace.Track);
}