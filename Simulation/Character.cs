using System;

namespace Simulation
{
  public class Actor {
    public readonly Guid Reference;
    public Position Position;
    public int CurrentHealth = 100;
    public int MaximumHealth = 100;
    public string DisplayName;

    public Actor(string name) {
      Reference = Guid.NewGuid();
      Position = new Position();
      DisplayName = name;
    }
  }
}
