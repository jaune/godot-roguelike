using System;

namespace Simulation
{
  public class Character {
    public readonly Guid Reference;
    public Position Position;
    public int CurrentHealth = 100;
    public int MaximumHealth = 100;

    public Character() {
      Reference = Guid.NewGuid();
      Position = new Position();
    }
  }
}
