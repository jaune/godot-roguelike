using System;

namespace Simulation
{
  public enum Appearance {
    Unknown = 0,
    Human = 1,
    Zombie = 2
  }

  public class Actor {
    public readonly Guid Reference;
    public Location Location;
    public int CurrentHealth = 100;
    public int MaximumHealth = 100;
    public string DisplayName;
    public Appearance Appearance;

    public Actor(string name, Location location) {
      Reference = Guid.NewGuid();
      Location = location;
      DisplayName = name;
      Appearance = Appearance.Unknown;
    }
  }
}
