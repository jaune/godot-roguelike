using NUnit.Framework;

namespace RoguelikeSimulation.Tests
{
    public class Database
    {
      [Test]
      public void Test_Insert_Prototype () {
        Simulation.Database.Database Db = new Simulation.Database.Database();

        var Zombie = Db.NewPrototype((p) => {
          p.With(new Simulation.Health(100, 1000));
        });

        var z = Db.Insert(Zombie);

        var zHealth = Db.GetTable<Simulation.Health>().Select(z);

        Assert.AreEqual(100, zHealth.Current);
        Assert.AreEqual(1000, zHealth.Max);
      }

      [Test]
      public void Test_Override_Prototype () {
        Simulation.Database.Database Db = new Simulation.Database.Database();

        var Zombie = Db.NewPrototype((p) => {
          p.With(new Simulation.Health(100, 1000));
        });

        var z = Db.Insert(Zombie, (p) => {
          p.With(new Simulation.Health(10, 1000));
        });

        var zHealth = Db.GetTable<Simulation.Health>().Select(z);

        Assert.AreEqual(10, zHealth.Current);
        Assert.AreEqual(1000, zHealth.Max);
      }

      [Test]
      public void Test_Basic_Join () {
        Simulation.Database.Database Db = new Simulation.Database.Database();

        Db.Insert((p) => {
          p.With(new Simulation.Health(45, 100));
        });

        Db.Insert((p) => {
          p.With(new Simulation.Health(10, 1000));
        });

        var rows = Db.Select()
          .Include<Simulation.Health>()
          .Execute();

        foreach (var row in rows) {
          Assert.AreEqual(1000, row.Item1.Max);
        }
      }


   }
}
