using System;
using System.Collections.Generic;
using System.Linq;

public class Set
{
    private readonly HashSet<int> elements;

    public class Production
    {
        public int Id { get; }
        public string Name { get; }

        public Production(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => $"Production: Id={Id}, Name={Name}";
    }

    public class Developer
    {
        public int Id { get; }
        public string FullName { get; }
        public string Department { get; }

        public Developer(int id, string fullName, string department)
        {
            Id = id;
            FullName = fullName;
            Department = department;
        }

        public override string ToString() => $"Developer: {FullName}, Id={Id}, Dept={Department}";
    }

    public Production Prod { get; set; }
    public Developer Dev { get; set; }

    public Set(IEnumerable<int> collection)
    {
        elements = new HashSet<int>(collection);
    }

    // Перегрузка операторов
    public static Set operator +(Set set, int item)
    {
        var result = new Set(set.elements);
        result.elements.Add(item);
        return result;
    }

    public static Set operator +(Set a, Set b)
    {
        return new Set(a.elements.Union(b.elements));
    }

    public static Set operator *(Set a, Set b)
    {
        return new Set(a.elements.Intersect(b.elements));
    }

    public static explicit operator int(Set set) => set.elements.Count;

    public static bool operator true(Set set) => set.elements.Count >= 2 && set.elements.Count <= 10;

    public static bool operator false(Set set) => !(set.elements.Count >= 2 && set.elements.Count <= 10);

    public override string ToString() => "{" + string.Join(", ", elements) + "}";

    public IEnumerable<int> Elements => elements;
}

public static class StatisticOperation
{
    public static int Sum(Set set) => set.Elements.Sum();

    public static int Difference(Set set) =>
        set.Elements.Max() - set.Elements.Min();

    public static int Count(Set set) => set.Elements.Count();
}

public static class Extensions
{
    public static string AddCommaAfterWords(this string str)
    {
        var words = str.Split(new[] { ' ' });
        return string.Join(", ", words);
    }

    public static Set RemoveDuplicates(this Set set)
    {
        return new Set(set.Elements.Distinct());
    }
}

class Program
{
    static void Main()
    {
        var set1 = new Set(new[] { 1, 2, 3, 4 });
        var set2 = new Set(new[] { 3, 4, 5, 6 });

        set1.Prod = new Set.Production(1, "BelSoft");
        set1.Dev = new Set.Developer(101, "Бразовский М.А.", "Отдел разработки");

        Console.WriteLine(set1.Prod);
        Console.WriteLine(set1.Dev);

        Console.WriteLine("Set1: " + set1);
        Console.WriteLine("Set2: " + set2);

        Console.WriteLine("Set1 + 10: " + (set1 + 10));
        Console.WriteLine("Set1 + Set2: " + (set1 + set2));
        Console.WriteLine("Set1 * Set2: " + (set1 * set2));

        Console.WriteLine("Мощность Set1: " + (int)set1);

        Console.WriteLine("Sum(Set1): " + StatisticOperation.Sum(set1));
        Console.WriteLine("Diff(Set1): " + StatisticOperation.Difference(set1));
        Console.WriteLine("Count(Set1): " + StatisticOperation.Count(set1));

        string text = "Это тестовая строка";
        Console.WriteLine("Метод расширения строки: " + text.AddCommaAfterWords());

        var sett3 = new Set(new[] { 1, 1, 2, 2, 3 });
        Console.WriteLine("Удаление дубликатов: " + sett3.RemoveDuplicates());

        if (set1)
            Console.WriteLine("Set1 имеет допустимый размер (2-10 элементов)");
        else
            Console.WriteLine("Set1 вне диапазона");

        Console.ReadLine();
    }
}
