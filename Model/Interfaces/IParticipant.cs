using Model.Classes;
using Model.Enums;

namespace Model.Interfaces;

public interface IParticipant
{
    public string Name { get; set; }
    public int Points { get; set; }
    public Section _position { get; set; }
    public IEquipment Equipment { get; set; }
    public TeamColors TeamColors { get; set; }
}