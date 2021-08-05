using NUnit.Framework;

namespace RoguelikeSimulation.Tests
{
    public class PerceptionField
    {
      public static void Print(Simulation.PerceptionField f) {
        var size = (int)System.Math.Ceiling(System.Math.Sqrt(f.Data.Length));

        System.Console.WriteLine($"Length: {f.Data.Length}");

        for (var x = 0; x < size; x++) {
          for (var y = 0; y < size; y++) {
            var i = x + (y * size);

            System.Console.Write($"{f.Data[i]}");
          }
          System.Console.Write("\n");
        }
      }

      [Test]
      public void Test_Basic () {
        {
          var f = new Simulation.PerceptionField(new Simulation.Vector2i(0, 0), 0.3f);

          Assert.True(f.GetValueAt(new Simulation.Vector2i(0, 0)) == 1);
          Assert.AreEqual(new float[]{
            1
          }, f.Data);
        }

        {
          var f = new Simulation.PerceptionField(new Simulation.Vector2i(0, 0), 2.5f);

          Print(f);

          Assert.True(f.GetValueAt(new Simulation.Vector2i(0, 0)) == 1);
          Assert.AreEqual(new float[]{
            0,1,1,1,0,
            1,1,1,1,1,
            1,1,1,1,1,
            1,1,1,1,1,
            0,1,1,1,0,
          }, f.Data);
        }

        {
          var f = new Simulation.PerceptionField(new Simulation.Vector2i(0, 0), 2.3f);

          Assert.True(f.GetValueAt(new Simulation.Vector2i(0, 0)) == 1);
          Assert.AreEqual(new float[]{
            0,1,1,1,0,
            1,1,1,1,1,
            1,1,1,1,1,
            1,1,1,1,1,
            0,1,1,1,0,
          }, f.Data);
        }

        {
          var f = new Simulation.PerceptionField(new Simulation.Vector2i(0, 0), 2.7f);

          Assert.True(f.GetValueAt(new Simulation.Vector2i(-2, -2)) == 0);
          Assert.True(f.GetValueAt(new Simulation.Vector2i(2, 2)) == 0);
          Assert.True(f.GetValueAt(new Simulation.Vector2i(3, 3)) == 0);
          Assert.True(f.GetValueAt(new Simulation.Vector2i(100, 100)) == 0);
          Assert.True(f.GetValueAt(new Simulation.Vector2i(0, 0)) == 1);
          Assert.AreEqual(new float[]{
            0,0,0,0,0,0,
            0,0,1,1,1,0,
            0,1,1,1,1,1,
            0,1,1,1,1,1,
            0,1,1,1,1,1,
            0,0,1,1,1,0,
          }, f.Data);
        }
      }
    }
}
