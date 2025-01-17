using System.ComponentModel;
using System.Runtime.CompilerServices;
using Model.Enums;
using Model.Interfaces;

namespace Model.Classes;

public class Driver : IParticipant
{
    public string Name { get; set; }
    public int Points { get; set; }
    public int lap { get; set; }
    public Section _position { get; set; }
    public IEquipment Equipment { get; set; }
    public TeamColors TeamColors { get; set; }

    public Driver(Car car)
    {
        Equipment = car;
        lap = 1;
    }
    public void resetDriverForNextRace()
    {
        lap = 1;
        _position = null;
    }
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}