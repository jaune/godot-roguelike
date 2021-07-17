using System.Collections.Generic;

namespace Simulation
{
  public class State {
    public Actor player;
    public List<Actor> characters;

    public State() {
      this.player = new Actor("Player");
      this.characters = new List<Actor>(1);

      this.characters.Add(this.player);
    }
  }
}
