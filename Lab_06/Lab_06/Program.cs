using System;
using System.Diagnostics; // для Assert

// === 1. Иерархия собственных исключений ===
class InvalidBirthYearException : Exception
{
    public InvalidBirthYearException(string message) : base(message) { }
}

class NegativePowerException : Exception
{
    public NegativePowerException(string message) : base(message) { }
}

class EmptyNameException : ArgumentException
{
    public EmptyNameException(string message) : base(message) { }
}

// === Базовый класс боевой единицы ===
abstract class BattleUnit
{
    public string Name { get; set; }
    public int Year { get; set; }
    public string Place { get; set; }
    public int Power { get; set; }

    protected BattleUnit(string name, int year, string place, int power)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new EmptyNameException("Имя не может быть пустым");

        if (year < 1900 || year > DateTime.Now.Year)
            throw new InvalidBirthYearException($"Недопустимый год: {year}");

        if (power < 0)
            throw new NegativePowerException("Мощность не может быть отрицательной");

        Name = name;
        Year = year;
        Place = place;
        Power = power;
    }

    public abstract void Action();
}

// === Класс Human ===
class Human : BattleUnit
{
    public Human(string name, int year, string place)
        : base(name, year, place, 0) { }

    public override void Action() => Console.WriteLine($"{Name} думает");
}

// === Класс Transformer ===
class Transformer : BattleUnit
{
    public Transformer(string name, int year, string place, int power)
        : base(name, year, place, power) { }

    public override void Action() => Console.WriteLine($"{Name} трансформируется");
}

class Program
{
    static void Main()
    {
        try
        {
            
            Console.WriteLine("=== Тестирование исключений ===");

            // 1) Пустое имя
            try
            {
                var h1 = new Human("", 1990, "Минск");
            }
            catch (EmptyNameException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            // 2) Неверный год
            try
            {
                var h2 = new Human("Иван", 3000, "Минск");
            }
            catch (InvalidBirthYearException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            // 3) Отрицательная мощность
            try
            {
                var t1 = new Transformer("Optimus", 2005, "Cybertron", -50);
            }
            catch (NegativePowerException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            // 4) Деление на ноль
            try
            {
                int x = 10, y = 0;
                int z = x / y;
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine($"Ошибка деления: {ex.Message}");
            }

            // 5) Неверный индекс
            try
            {
                int[] arr = new int[3];
                arr[5] = 10;
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine($"Ошибка индекса: {ex.Message}");
            }

            // 6) NullReference
            try
            {
                string s = null;
                Console.WriteLine(s.Length);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Ошибка null: {ex.Message}");
            }

            // 7) Assert
            int[] aa = null/* { }*/;
            Debug.Assert(aa != null, "Массив не должен быть null");
            // Если условие не выполняется, программа остановится в режиме отладки

        }
        catch (Exception ex) // универсальный обработчик
        {
            Console.WriteLine($"Общее исключение: {ex.GetType().Name}, {ex.Message}");
        }
        finally
        {
            Console.WriteLine("=== Блок finally: завершение работы ===");
        }
        Console.ReadLine();
    }
}
