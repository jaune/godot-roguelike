using NUnit.Framework;

namespace RoguelikeSimulation.Tests
{
    public class RegionField
    {
      [Test]
      public void Test_Basic () {
        var f = new Simulation.RegionField<float>(new Simulation.Vector2i(0, 0), 4, 0.5f);

        Assert.True(f.GetValueAt(new Simulation.Vector2i(0, 0)) == 0.5f);
      }
    }
}
