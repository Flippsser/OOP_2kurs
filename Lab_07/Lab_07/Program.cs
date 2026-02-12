using System;
using System.Collections.Generic;
using System.IO;

// 1. Обобщённый интерфейс
public interface ICollectionOps<T>
{
    void Add(T item);
    bool Remove(T item);
    void View();
    List<T> Find(Predicate<T> pred);
}

// 2. Обобщённый класс с ограничением
public class CollectionType<T> : ICollectionOps<T> where T : IComparable<T>
{
    private List<T> items = new List<T>();

    public void Add(T item)
    {
        try
        {
            items.Add(item);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка: " + ex.Message);
        }
        finally
        {
            Console.WriteLine("Add завершён");
        }
    }

    public bool Remove(T item)
    {
        try
        {
            return items.Remove(item);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка: " + ex.Message);
            return false;
        }
        finally
        {
            Console.WriteLine("Remove завершён");
        }
    }

    public List<T> Find(Predicate<T> pred)
    {
        try
        {
            return items.FindAll(pred);
        }
        finally
        {
            Console.WriteLine("Find завершён");
        }
    }

    public void View()
    {
        if (items.Count == 0)
        {
            Console.WriteLine("<пусто>");
            return;
        }

        foreach (var i in items)
            Console.WriteLine(i);
    }

    // Сохранение в TXT
    public void Save(string path)
    {
        using (var w = new StreamWriter(path))
        {
            foreach (var i in items)
                w.WriteLine(i);
        }
    }

    // Загрузка из TXT (для int, double, string, Car)
    public void Load(string path)
    {
        items.Clear();
        using (var r = new StreamReader(path))
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                if (typeof(T) == typeof(int))
                {
                    int v;
                    if (int.TryParse(line, out v))
                        items.Add((T)(object)v);
                }
                else if (typeof(T) == typeof(double))
                {
                    double v;
                    if (double.TryParse(line, out v))
                        items.Add((T)(object)v);
                }
                else if (typeof(T) == typeof(string))
                {
                    items.Add((T)(object)line);
                }
                else if (typeof(T) == typeof(Car))
                {
                    // Формат: "BMW,250"
                    var p = line.Split(',');
                    if (p.Length == 2)
                    {
                        int sp;
                        if (int.TryParse(p[1], out sp))
                            items.Add((T)(object)new Car(p[0], sp));
                    }
                }
            }
        }
    }
}

// 4. Пользовательский класс из ЛР4
public class Car : IComparable<Car>
{
    public string Name { get; set; }
    public int Speed { get; set; }

    public Car(string n, int s)
    {
        Name = n;
        Speed = s;
    }

    public int CompareTo(Car other)
    {
        return Speed.CompareTo(other.Speed);
    }

    public override string ToString()
    {
        return Name + "," + Speed;
    }
}

// Демонстрация
class Program
{
    static void Main()
    {
        Console.WriteLine("=== int ===");
        var ints = new CollectionType<int>();
        ints.Add(5);
        ints.Add(10);
        ints.View();

        Console.WriteLine("\n=== double ===");
        var doubles = new CollectionType<double>();
        doubles.Add(1.5);
        doubles.Add(3.14);
        doubles.View();

        Console.WriteLine("\n=== string ===");
        var strings = new CollectionType<string>();
        strings.Add("hello");
        strings.Add("world");
        strings.View();

        Console.WriteLine("\n=== Car (из ЛР4) ===");
        var cars = new CollectionType<Car>();
        cars.Add(new Car("BMW", 250));
        cars.Add(new Car("Audi", 220));
        cars.Add(new Car("Tesla", 300));
        cars.View();

        Console.WriteLine("\nМашины быстрее 230:");
        foreach (var c in cars.Find(x => x.Speed > 230))
            Console.WriteLine(c);

        Console.WriteLine("\nСохранение и загрузка:");
        cars.Save("cars.txt");

        var loaded = new CollectionType<Car>();
        loaded.Load("cars.txt");
        loaded.View();
        Console.Read();
    }
}
