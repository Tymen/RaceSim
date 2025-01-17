namespace Model.Interfaces;

public interface IEquipment
{
    public int Quality { get; set; }
    public int Performance { get; set; }
    public int Speed { get; set; }
    public bool IsBroken { get; set; }
    double BreakdownChance { get; set; } // The likelihood of a breakdown
    double FixChance { get; set; } 
}