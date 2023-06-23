using Model.Classes;

namespace Controller.EventArgs;

public class IsFinishedEventArgs : System.EventArgs
{
    public List<Driver> ParticipantsList { get; set; }
}