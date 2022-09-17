// See https://aka.ms/new-console-template for more information
using Controller;
using RaceSim;

DataController.Initialize();
DataController.NextRace();
Visualizer.Initializer(2);
Visualizer.DrawTrack(DataController.CurrentRace.Track);

for (; ; )
{
    Thread.Sleep(100);
}