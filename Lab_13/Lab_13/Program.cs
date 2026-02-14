using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Linq;

namespace Lab13Serialization
{
    [Serializable]
    public class Department
    {
        public string Name { get; set; }
        public int Floor { get; set; }
    }

    [Serializable]
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        [NonSerialized]
        [XmlIgnore]
        [JsonIgnore]
        public string SecretInfo;
    }

    [Serializable]
    public class Employee : Person
    {
        public string Position { get; set; }
        public Department Department { get; set; }
    }

    public interface ISerializer
    {
        void Serialize<T>(T obj, string file);
        T Deserialize<T>(string file);
    }

    public class BinaryDataSerializer : ISerializer
    {
        public void Serialize<T>(T obj, string file)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                formatter.Serialize(fs, obj);
            }
        }

        public T Deserialize<T>(string file)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                return (T)formatter.Deserialize(fs);
            }
        }
    }

    public class SoapDataSerializer : ISerializer
    {
        public void Serialize<T>(T obj, string file)
        {
            SoapFormatter formatter = new SoapFormatter();
            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                formatter.Serialize(fs, obj);
            }
        }

        public T Deserialize<T>(string file)
        {
            SoapFormatter formatter = new SoapFormatter();
            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                return (T)formatter.Deserialize(fs);
            }
        }
    }

    public class XmlDataSerializer : ISerializer
    {
        public void Serialize<T>(T obj, string file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                serializer.Serialize(fs, obj);
            }
        }

        public T Deserialize<T>(string file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                return (T)serializer.Deserialize(fs);
            }
        }
    }

    public class JsonDataSerializer : ISerializer
    {
        public void Serialize<T>(T obj, string file)
        {
            string json = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(file, json);
        }

        public T Deserialize<T>(string file)
        {
            string json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public static class SerializerFactory
    {
        public static ISerializer Get(string type)
        {
            switch (type)
            {
                case "binary": return new BinaryDataSerializer();
                case "soap": return new SoapDataSerializer();
                case "xml": return new XmlDataSerializer();
                case "json": return new JsonDataSerializer();
                default: throw new Exception("Unknown serializer");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Employee emp = new Employee
            {
                Name = "Alex",
                Age = 30,
                SecretInfo = "TOP SECRET",
                Position = "Developer",
                Department = new Department { Name = "IT", Floor = 5 }
            };

            Console.WriteLine("Сериализация одного объекта");

            TestSerializer("binary", emp, "employee.bin");
            TestSerializer("soap", emp, "employee.soap");
            TestSerializer("xml", emp, "employee.xml");
            TestSerializer("json", emp, "employee.json");

            Console.WriteLine();
            Console.WriteLine("Сериализация коллекции объектов");

            List<Employee> employees = new List<Employee>();
            employees.Add(emp);
            employees.Add(new Employee
            {
                Name = "John",
                Age = 25,
                Position = "Tester",
                Department = new Department { Name = "QA", Floor = 2 }
            });

            ISerializer xml = SerializerFactory.Get("xml");
            xml.Serialize(employees, "employees.xml");
            List<Employee> loadedList = xml.Deserialize<List<Employee>>("employees.xml");

            Console.WriteLine("Количество загруженных сотрудников: " + loadedList.Count);

            Console.WriteLine();
            Console.WriteLine("Работа с XPath");

            XmlDocument doc = new XmlDocument();
            doc.Load("employee.xml");

            string name = doc.SelectSingleNode("//Name").InnerText;
            string position = doc.SelectSingleNode("//Employee/Position").InnerText;

            Console.WriteLine("Имя найденное XPath: " + name);
            Console.WriteLine("Должность найденная XPath: " + position);

            Console.WriteLine();
            Console.WriteLine("Работа с LINQ to XML");

            XDocument xdoc = new XDocument(
                new XElement("Employees",
                    employees.Select(e =>
                        new XElement("Employee",
                            new XElement("Name", e.Name),
                            new XElement("Age", e.Age),
                            new XElement("Position", e.Position),
                            new XElement("Department", e.Department.Name)
                        )
                    )
                )
            );

            xdoc.Save("linqEmployees.xml");

            IEnumerable<string> allNames = xdoc.Descendants("Name").Select(x => x.Value);
            Console.WriteLine("Список имен сотрудников: " + string.Join(", ", allNames));

            IEnumerable<XElement> developers = xdoc.Descendants("Employee")
                .Where(e => e.Element("Position").Value == "Developer");

            Console.WriteLine("Список разработчиков: " +
                string.Join(", ", developers.Select(d => d.Element("Name").Value)));

            Console.WriteLine();
            Console.WriteLine("Работа программы завершена");
            Console.ReadLine();
        }

        static void TestSerializer(string type, Employee emp, string file)
        {
            ISerializer serializer = SerializerFactory.Get(type);
            serializer.Serialize(emp, file);
            Employee loaded = serializer.Deserialize<Employee>(file);

            Console.WriteLine(type.ToUpper() + " результат: имя " + loaded.Name +
                ", SecretInfo = " + (loaded.SecretInfo == null ? "NULL" : loaded.SecretInfo));
        }
    }
}
