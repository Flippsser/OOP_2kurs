using System;

// Интерфейс управления авто
interface IAutoControl
{
    void Start();
    bool DoClone(); // одноименный метод
}

// Абстрактный класс транспортного средства
abstract class Transport
{
    public string Name { get; set; }
    public Transport(string name) { Name = name; }

    public abstract void Move();
    public abstract bool DoClone(); // одноименный метод

    public override string ToString() => $"Тип: {GetType().Name}, Название: {Name}";
}

// Класс Car
class Car : Transport, IAutoControl
{
    public Car(string name) : base(name) { }

    public override void Move() => Console.WriteLine($"{Name} едет");

    // Реализация интерфейса
    bool IAutoControl.DoClone()
    {
        Console.WriteLine("Клон через интерфейс");
        return true;
    }

    public void Start() => Console.WriteLine($"{Name} завелась");

    // Реализация абстрактного метода
    public override bool DoClone()
    {
        Console.WriteLine("Клон через базовый класс");
        return true;
    }
}


// Абстрактный класс разумного существа
abstract class Being
{
    public string Name { get; set; }
    public Being(string name) { Name = name; }
    public abstract void Think();
    public override string ToString() => $"Существо: {Name}";
}

// Класс Human
class Human : Being
{
    public Human(string name) : base(name) { }
    public override void Think() => Console.WriteLine($"{Name} думает");
}

// Sealed‑класс Transformer
sealed class Transformer : Car
{
    public Transformer(string name) : base(name) { }
    public override void Move() => Console.WriteLine($"{Name} трансформируется");
}

// Класс Printer
class Printer
{
    public void IAmPrinting(Transport obj) => Console.WriteLine(obj.ToString());
}

class Program
{
    static void Main()
    {
        Car car = new Car("BMW");
        Human h = new Human("Иван");
        Transformer t = new Transformer("Optimus");

        // Работа через интерфейс
        IAutoControl ctrl = car;
        ctrl.Start();
        ctrl.DoClone();

        // Работа через абстрактный класс
        Transport tr = t;
        tr.Move();
        tr.DoClone();

        // Проверка типов
        if (tr is Transformer) Console.WriteLine("Это трансформер!");

        // Printer
        Printer p = new Printer();
        Transport[] arr = { car, t };
        foreach (var obj in arr) p.IAmPrinting(obj);

        // Человек
        h.Think();
        Console.Read();
    }
}
