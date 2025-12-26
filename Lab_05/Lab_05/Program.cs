using System;
using System.Collections.Generic;
using System.Linq;

// Перечисление типов боевых единиц
enum UnitType
{
    Human,
    Transformer
}

// Структура для хранения даты рождения/создания
struct BirthInfo
{
    public int Year;
    public string Place;

    public BirthInfo(int year, string place)
    {
        Year = year;
        Place = place;
    }

    public override string ToString() => $"{Year}, {Place}";
}

// Абстрактный класс боевой единицы
abstract partial class BattleUnit
{
    public string Name { get; set; }
    public BirthInfo Birth { get; set; }
    public UnitType Type { get; set; }

    protected BattleUnit(string name, BirthInfo birth, UnitType type)
    {
        Name = name;
        Birth = birth;
        Type = type;
    }

    public abstract void Action();
    public override string ToString() => $"{Type}: {Name}, {Birth}";
}

// Вторая часть partial‑класса
partial class BattleUnit
{
    public virtual int Power { get; set; }
}

// Класс Human
class Human : BattleUnit
{
    public Human(string name, int year, string place)
        : base(name, new BirthInfo(year, place), UnitType.Human) { }

    public override void Action() => Console.WriteLine($"{Name} думает");
}

// Класс Transformer
sealed class Transformer : BattleUnit
{
    public Transformer(string name, int year, string place, int power)
        : base(name, new BirthInfo(year, place), UnitType.Transformer)
    {
        Power = power;
    }

    public override void Action() => Console.WriteLine($"{Name} трансформируется");
}

// Класс‑Контейнер
class ArmyContainer
{
    private List<BattleUnit> units = new List<BattleUnit>();

    public void Add(BattleUnit unit) => units.Add(unit);
    public void Remove(BattleUnit unit) => units.Remove(unit);
    public List<BattleUnit> GetAll() => units;

    public void PrintAll()
    {
        foreach (var u in units) Console.WriteLine(u);
    }
}

// Класс‑Контроллер
class ArmyController
{
    private ArmyContainer container;

    public ArmyController(ArmyContainer c) { container = c; }

    // Найти боевую единицу по году рождения/создания
    public IEnumerable<BattleUnit> FindByYear(int year) =>
        container.GetAll().Where(u => u.Birth.Year == year);

    // Вывести имена трансформеров заданной мощности
    public IEnumerable<string> FindTransformersByPower(int power) =>
        container.GetAll()
                 .Where(u => u.Type == UnitType.Transformer && u.Power == power)
                 .Select(u => u.Name);

    // Количество боевых единиц
    public int CountUnits() => container.GetAll().Count;
}

class Program
{
    static void Main()
    {
        ArmyContainer army = new ArmyContainer();
        army.Add(new Human("Иван", 1990, "Минск"));
        army.Add(new Transformer("Optimus", 2005, "Cybertron", 100));
        army.Add(new Transformer("Bumblebee", 2007, "Cybertron", 80));

        ArmyController controller = new ArmyController(army);

        Console.WriteLine("=== Вся армия ===");
        army.PrintAll();

        Console.WriteLine("\n=== Найти по году 2005 ===");
        foreach (var u in controller.FindByYear(2005))
            Console.WriteLine(u);

        Console.WriteLine("\n=== Трансформеры с мощностью 80 ===");
        foreach (var name in controller.FindTransformersByPower(80))
            Console.WriteLine(name);

        Console.WriteLine($"\n=== Количество боевых единиц: {controller.CountUnits()} ===");
        System.Console.ReadLine();
    }
}
