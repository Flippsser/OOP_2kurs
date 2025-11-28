using System;
using System.Linq;

namespace Lab_02
{
    public class Student
    {
        // Поля
        private string lastName, firstName, middleName, faculty, address, phone;
        private DateTime birthDate;
        private int course, group;

        public readonly int Id;
        private static int studentCount;

        public const string University = "BSTU";

        // Свойства
        public string LastName { get => lastName; set => lastName = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string MiddleName { get => middleName; set => middleName = value; }
        public DateTime BirthDate { get => birthDate; set => birthDate = value; }
        public string Faculty { get => faculty; set => faculty = value; }
        public string Address { get => address; set => address = value; }
        public string Phone { get => phone; set => phone = value; }
        public int Course { get => course; set => course = value; }
        public int Group { get => group; set => group = value; }

        // Конструкторы
        public Student()
        {
            Id = Guid.NewGuid().GetHashCode();
            studentCount++;
        }

        public Student(string ln, string fn, DateTime bd) : this()
        {
            LastName = ln;
            FirstName = fn;
            BirthDate = bd;
        }

        public Student(string ln, string fn, string mn, DateTime bd,
                       string fac, int course, int group, string addr, string ph) : this(ln, fn, bd)
        {
            MiddleName = mn;
            Faculty = fac;
            Course = course;
            Group = group;
            Address = addr;
            Phone = ph;
        }

   
        public Student(
            string ln = "Иванов",
            string fn = "Иван",
            string mn = "Иванович",
            DateTime? bd = null,
            string fac = "ФИТ",
            int course = 1,
            int group = 1,
            string addr = "Минск",
            string ph = "0000000") : this()
        {
            LastName = ln;
            FirstName = fn;
            MiddleName = mn;
            BirthDate = bd ?? new DateTime(2000, 1, 1); 
            Faculty = fac;
            Course = course;
            Group = group;
            Address = addr;
            Phone = ph;
        }

        static Student() { studentCount = 0; }

        // Метод с ref/out
        public void UpdateCourse(ref int newCourse, out bool ok)
        {
            if (newCourse >= 1 && newCourse <= 6) { Course = newCourse; ok = true; }
            else ok = false;
        }

        // Возраст
        public int GetAge()
        {
            int age = DateTime.Now.Year - BirthDate.Year;
            if (DateTime.Now < BirthDate.AddYears(age)) age--;
            return age;
        }

        // Статический метод
        public static void PrintInfo() => Console.WriteLine($"Создано студентов: {studentCount}");

        // Переопределения
        public override string ToString() =>
            $"{LastName} {FirstName} {MiddleName}, {Faculty}, курс {Course}, гр.{Group}, возраст {GetAge()}, адрес: {Address}, тел: {Phone}";
        public override bool Equals(object obj) => obj is Student s && Id == s.Id;
        public override int GetHashCode() => Id.GetHashCode();
    }

    internal class Program
    {
        static void Main()
        {
            var s1 = new Student("Иванов", "Иван", new DateTime(2006, 5, 12));
            var s2 = new Student("Петров", "Пётр", "Сергеевич", new DateTime(2005, 3, 20),
                                 "ФИТ", 2, 5, "Минск", "1234567");
            var s3 = new Student("Сидоров", "Алексей", "Игоревич", new DateTime(2007, 7, 15),
                                 "ФИТ", 1, 3, "Гомель", "7654321");

            
            var s4 = new Student("Иванов", "Иван","Иванович"); 

            Console.WriteLine("=== Список студентов ===");
            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(s3);
            Console.WriteLine(s4);

            // Проверка ref/out
            int newCourse = 3;
            s2.UpdateCourse(ref newCourse, out bool ok);
            Console.WriteLine($"Обновление курса: {(ok ? "успешно" : "ошибка")}");

            // Сравнение объектов
            Console.WriteLine($"\nСравнение s1 и s2: {s1.Equals(s2)}");
            Console.WriteLine($"Сравнение s1 и s1: {s1.Equals(s1)}");

            // Проверка типа
            Console.WriteLine($"s1 является Student? {(s1 is Student)}");
            Console.WriteLine($"Тип объекта s2: {s2.GetType()}");

            // Массив студентов
            Student[] students = { s1, s2, s3, s4 };

            Console.WriteLine("\nСтуденты ФИТ:");
            foreach (var st in students.Where(st => st.Faculty == "ФИТ")) Console.WriteLine(st);

            Console.WriteLine("\nСтуденты группы 5:");
            foreach (var st in students.Where(st => st.Group == 5)) Console.WriteLine(st);

            // Анонимный тип
            var anon = new { s2.LastName, s2.FirstName, s2.Faculty, s2.Address };
            Console.WriteLine($"\nАнонимный тип: {anon}");

            Student.PrintInfo();
            Console.WriteLine("Нажмите Enter для окончания...");
            Console.ReadLine();
        }
    }
}
