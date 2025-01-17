// See https://aka.ms/new-console-template for more information
using Controller;
using RaceSim;

StartUp();

for (; ; )
{
    Thread.Sleep(5);
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
    Console.WriteLine("Name: ");
    var name = (Console.ReadLine()).ToLower();
    TrackController trackController = new TrackController(name);
    ConsoleKeyInfo keyinfo;
    do
    {
        keyinfo = Console.ReadKey();
        Console.Clear();
        if (keyinfo.Key == ConsoleKey.S)
        {
            trackController.SaveTrack();
            StartUp();
        }
        else
        {
            trackController.AddTrackSection(keyinfo);
        }
    }
    while (keyinfo.Key != ConsoleKey.X);
}

void StartSim()
{
    DataController.Initialize(4);
    Thread thread = new Thread(new ThreadStart(() =>
    {
        // Replace 'WPF' with the namespace of your WPF application
        WPF.MainWindow app = new WPF.MainWindow(DataController.CurrentRace);
        app.InitializeComponent();
        app.ShowDialog(); // This will show your window
    }));

    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();
}