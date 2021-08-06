using System;
using System.Collections.Generic;

namespace Simulation.Database
{
  public class Entry {
    private Guid Id = Guid.NewGuid();

    public Guid GetId() {
      return Id;
    }
  }

  public class Database {
    private Dictionary<Type, ITable> Tables = new Dictionary<Type, ITable>();

    Table<T> CreateTable<T>() where T: class {
      var t = new Table<T>();

      Tables.Add(typeof(T), t);

      return t;
    }

    public bool HasTable<T>() {
      return Tables.ContainsKey(typeof(T));
    }

    public Table<T> GetTable<T>() where T: class {
      return Tables[typeof(T)] as Table<T> ?? throw new Exception("Table item do not match type");
    }

    public ITable GetTable(Type t) {
      return Tables[t] ?? throw new Exception("Table item do not match type");
    }

    public Table<T> EnsureTable<T>() where T: class {
      return HasTable<T>() ? GetTable<T>() : CreateTable<T>();
    }

    public Entry Insert(Prototype p, Action<PrototypeBuilder> build) {
      var builder = new PrototypeBuilder(this, p);

      build(builder);

      return Insert(builder.Build());
    }

    public Entry Insert(Prototype p) {
      var e = new Entry();

      foreach (var prop in p.Properties) {
        GetTable(prop.Value.GetValueType()).Insert(e, prop.Value.GetValue());
      }

      return e;
    }

    public Entry Insert(Action<PrototypeBuilder> build) {
      var builder = new PrototypeBuilder(this);

      build(builder);

      return Insert(builder.Build());
    }

    public Prototype NewPrototype(Action<PrototypeBuilder> build) {
      var b = new PrototypeBuilder(this);

      build(b);

      return b.Build();
    }

    public Selection Select() {
      return new Selection();
    }

    public class SelectionInclude<T> {
      public Type GetWithType() {
        return typeof(T);
      }

      public IEnumerable<ValueTuple<T>> Execute() {
        return new ValueTuple<T>[]{
        };
      }
    }

    public class Selection {
      public SelectionInclude<T> Include<T>() {
        return new SelectionInclude<T>();
      }
    }
  }

  public interface IProperty {
    Type GetValueType();
    object GetValue();
  }

  public class Property<T>: IProperty where T: notnull {
    private T Value;

    public Property(T value) {
      Value = value;
    }

    public Type GetValueType() {
      return typeof(T);
    }

    public object GetValue() {
      return Value;
    }
  }

  public class PrototypeBuilder {
    Prototype Prototype;
    Database Db;

    public PrototypeBuilder(Database db, Prototype? p = null) {
      Prototype = p == null ? new Prototype() : p;
      Db = db;
    }

    public void With<T>(T data) where T: class {
      Db.EnsureTable<T>();
      Prototype.Properties[typeof(T)] = new Property<T>(data);
    }

    public Prototype Build () {
      return Prototype;
    }
  }

  public interface ITable {
    void Insert(Entry e, object data);
  }

  public class Table<T>: ITable where T: class {
    private Dictionary<Guid, T> Rows = new Dictionary<Guid, T>();

    public void Insert(Entry e, object data) {
      System.Console.WriteLine($"Insert (object) ==== {e.GetId()}");

      var d = data as T ?? throw new Exception("TODO");
      Insert(e, d);
    }

    public void Insert(Entry e, T data) {
      System.Console.WriteLine($"Insert ==== {e.GetId()}");
      Rows[e.GetId()] = data;
    }

    public T Select(Entry e) {
      System.Console.WriteLine($"Select ==== {e.GetId()}");
      return Rows[e.GetId()];
    }
  }

  public class Prototype {
    public Dictionary<Type, IProperty> Properties = new Dictionary<Type, IProperty>();
  }
}
