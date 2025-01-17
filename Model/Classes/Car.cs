using Model.Interfaces;

namespace Model.Classes;

public class Car : IEquipment
{
    public int Quality { get; set; }
    public int Performance { get; set; }
    public int Speed { get; set; }
    public bool IsBroken { get; set; }
    public double BreakdownChance { get; set; }
    public double FixChance { get; set; }
}