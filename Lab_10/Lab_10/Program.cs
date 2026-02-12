using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab_10
{
    // Класс Student 
    public class Student
    {
        public int Id { get; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Faculty { get; set; }
        public int Course { get; set; }
        public string Group { get; set; }

        public int Age => (int)((DateTime.Now - BirthDate).TotalDays / 365.25);

        private static int _counter = 0;

        public Student(string lastName, string firstName, string middleName,
                       DateTime birthDate, string address, string phone,
                       string faculty, int course, string group)
        {
            Id = ++_counter;
            LastName = lastName;
            FirstName = firstName;
            MiddleName = middleName;
            BirthDate = birthDate;
            Address = address;
            Phone = phone;
            Faculty = faculty;
            Course = course;
            Group = group;
        }

        public override string ToString()
        {
            return $"{Id}: {LastName} {FirstName} {MiddleName}, {Faculty}, гр. {Group}, {Age} лет";
        }
    }

    // Дополнительный класс для Join (информация о кураторах групп)
    public class GroupInfo
    {
        public string Group { get; set; }
        public string Faculty { get; set; }
        public string Curator { get; set; }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            MonthsQueries();
            Console.WriteLine(new string('-', 60));
            StudentQueries();

            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        // 1) Массив месяцев и запросы LINQ to Objects
        static void MonthsQueries()
        {
            string[] months =
            {
                "January","February","March","April","May","June",
                "July","August","September","October","November","December"
            };

            int n = 4;

            // a) Месяцы с длиной строки = n
            var lengthN =
                from m in months
                where m.Length == n
                select m;

            Console.WriteLine("Месяцы длиной = {0}:", n);
            Console.WriteLine(string.Join(", ", lengthN));

            // b) Только летние и зимние месяцы
            string[] summer = { "June", "July", "August" };
            string[] winter = { "December", "January", "February" };

            var summerWinter =
                from m in months
                where summer.Contains(m) || winter.Contains(m)
                select m;

            Console.WriteLine("\nЛетние и зимние месяцы:");
            Console.WriteLine(string.Join(", ", summerWinter));

            // c) Месяцы в алфавитном порядке
            var ordered =
                from m in months
                orderby m
                select m;

            Console.WriteLine("\nМесяцы в алфавитном порядке:");
            Console.WriteLine(string.Join(", ", ordered));

            // d) Месяцы, содержащие букву 'u' и длиной имени не менее 4
            var withU =
                from m in months
                where m.Contains('u') && m.Length >= 4
                select m;

            Console.WriteLine("\nМесяцы с буквой 'u' и длиной ≥ 4:");
            Console.WriteLine(string.Join(", ", withU));
        }

        // 2) Коллекция List<Student> и запросы 
        static void StudentQueries()
        {
            var students = new List<Student>
            {
                new Student("Ivanov", "Ivan", "Ivanovich",
                    new DateTime(2003, 5, 10), "Minsk", "111-11-11", "CS", 2, "CS-21"),
                new Student("Petrov", "Petr", "Petrovich",
                    new DateTime(2002, 3, 1), "Minsk", "222-22-22", "CS", 3, "CS-31"),
                new Student("Sidorov", "Sergey", "Nikolaevich",
                    new DateTime(2004, 11, 20), "Gomel", "333-33-33", "Math", 1, "M-11"),
                new Student("Smirnova", "Anna", "Igorevna",
                    new DateTime(2003, 1, 15), "Minsk", "444-44-44", "CS", 2, "CS-21"),
                new Student("Kuznetsova", "Elena", "Olegovna",
                    new DateTime(2005, 7, 5), "Brest", "555-55-55", "Math", 1, "M-11"),
                new Student("Orlov", "Dmitry", "Sergeevich",
                    new DateTime(2001, 9, 30), "Minsk", "666-66-66", "CS", 4, "CS-41"),
                new Student("Popov", "Ivan", "Alexeevich",
                    new DateTime(2002, 12, 2), "Grodno", "777-77-77", "CS", 3, "CS-31"),
                new Student("Fedorova", "Olga", "Petrovna",
                    new DateTime(2004, 4, 18), "Minsk", "888-88-88", "Math", 2, "M-21"),
                new Student("Nikolaev", "Nikolay", "Ivanovich",
                    new DateTime(2003, 6, 25), "Minsk", "999-99-99", "CS", 2, "CS-21"),
                new Student("Sokolov", "Alexey", "Vladimirovich",
                    new DateTime(2001, 2, 9), "Minsk", "000-00-00", "CS", 4, "CS-41")
            };

            string targetFaculty = "CS";
            string targetGroup = "CS-21";
            string targetName = "Ivan";

            // 1) список студентов заданной специальности (факультета) по алфавиту
            var byFaculty =
                from s in students
                where s.Faculty == targetFaculty
                orderby s.LastName, s.FirstName
                select s;

            Console.WriteLine($"Студенты факультета {targetFaculty} по алфавиту:");
            foreach (var s in byFaculty)
                Console.WriteLine(s);

            // 2) список заданной учебной группы и факультета, самый молодой студент
            var groupFaculty =
                from s in students
                where s.Faculty == targetFaculty && s.Group == targetGroup
                orderby s.BirthDate descending // самый молодой – с самой поздней датой рождения
                select s;

            var youngest = groupFaculty.FirstOrDefault();
            Console.WriteLine($"\nСамый молодой студент группы {targetGroup} факультета {targetFaculty}:");
            if (youngest != null)
                Console.WriteLine(youngest);
            else
                Console.WriteLine("Нет студентов.");

            // 3) количество студентов заданной группы, упорядоченных по фамилии
            var groupOrdered =
                from s in students
                where s.Group == targetGroup
                orderby s.LastName, s.FirstName
                select s;

            Console.WriteLine($"\nСтуденты группы {targetGroup}, упорядоченные по фамилии:");
            foreach (var s in groupOrdered)
                Console.WriteLine(s);
            Console.WriteLine($"Количество студентов в группе {targetGroup}: {groupOrdered.Count()}");

            // 4) первый студент с заданным именем
            var firstWithName =
                (from s in students
                 where s.FirstName == targetName
                 select s).FirstOrDefault();

            Console.WriteLine($"\nПервый студент с именем {targetName}:");
            Console.WriteLine(firstWithName != null ? firstWithName.ToString() : "Не найден.");

            // Собственный запрос (≥5 операторов разных категорий)
            // Условие (Where), проекция (Select), упорядочивание (OrderBy),
            // группировка (GroupBy), агрегирование (Count), квантор (Any), разбиение (Skip/Take)
            var complexQuery =
                students
                    .Where(s => s.Faculty == "CS" && s.Age >= 20)          // условие
                    .OrderBy(s => s.Group)                                // упорядочивание
                    .GroupBy(s => s.Group)                                // группировка
                    .Where(g => g.Count() >= 2)                           // агрегирование Count
                    .Select(g => new                                      // проекция
                    {
                        Group = g.Key,
                        Count = g.Count(),
                        HasIvan = g.Any(s => s.FirstName == "Ivan"),      // квантор Any
                        TopTwo = g.OrderBy(s => s.LastName).Take(2)       // разбиение Take
                    });

            Console.WriteLine("\nСобственный сложный запрос (CS, возраст ≥ 20, группы с ≥2 студентами):");
            foreach (var g in complexQuery)
            {
                Console.WriteLine($"\nГруппа: {g.Group}, Кол-во: {g.Count}, Есть ли Иван: {g.HasIvan}");
                foreach (var s in g.TopTwo)
                    Console.WriteLine("  " + s);
            }

            // Запрос с Join
            var groups = new List<GroupInfo>
            {
                new GroupInfo { Group = "CS-21", Faculty = "CS", Curator = "Dr. Brown" },
                new GroupInfo { Group = "CS-31", Faculty = "CS", Curator = "Dr. Green" },
                new GroupInfo { Group = "CS-41", Faculty = "CS", Curator = "Dr. Black" },
                new GroupInfo { Group = "M-11",  Faculty = "Math", Curator = "Dr. White" },
                new GroupInfo { Group = "M-21",  Faculty = "Math", Curator = "Dr. Gray" }
            };

            var joinQuery =
                from s in students
                join g in groups
                    on new { s.Group, s.Faculty } equals new { g.Group, g.Faculty }
                select new
                {
                    Student = s,
                    g.Curator
                };

            Console.WriteLine("\nЗапрос с Join (студент + куратор группы):");
            foreach (var item in joinQuery)
            {
                Console.WriteLine($"{item.Student.LastName} {item.Student.FirstName}, гр. {item.Student.Group}, куратор: {item.Curator}");
            }
        }
    }
}
