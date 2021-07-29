using System.Collections.Generic;

namespace Simulation
{
  public class Builder {
    List<Map> MapList = new List<Map>();

    public Map AddMap(Map w) {
      if (MapList.IndexOf(w) != -1) {
        throw new System.Exception($"Simulation.Builder: {w} already add.");
      }

      MapList.Add(w);

      return w;
    }

    public Simulation Build() {
      return Simulation.Create(MapList.ToArray());
    }
  }
}
