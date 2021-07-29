using System;

namespace Simulation
{
  public class Actor {
    public readonly Guid Reference;
    public Location Location;
    public int CurrentHealth = 100;
    public int MaximumHealth = 100;
    public string DisplayName;

    public Actor(string name, Location location) {
      Reference = Guid.NewGuid();
      Location = location;
      DisplayName = name;
    }
  }
}
