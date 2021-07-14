using System.Collections.Generic;

namespace Simulation
{
  public class State {
    public Character player;
    public List<Character> characters;

    public State() {
      this.player = new Character();
      this.characters = new List<Character>(1);

      this.characters.Add(this.player);
    }
  }
}
