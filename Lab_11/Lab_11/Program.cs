using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ReflectionLab
{
    public static class Reflector
    {
        private const string OutputFile = "reflection_info.txt";

        public static void Write(string text)
        {
            File.AppendAllText(OutputFile, text + Environment.NewLine);
        }

        // a. Определение информации о классе
        public static void WriteClassInfo(string className)
        {
            Type? type = Type.GetType(className);
            if (type == null) return;

            Write($"Library info: {type.Assembly.FullName}");
        }

        // b. Есть ли публичные конструкторы
        public static void HasPublicConstructors(string className)
        {
            Type? type = Type.GetType(className);
            if (type == null) return;

            bool hasPublic = type.GetConstructors().Length > 0;
            Write($"Has public constructors: {hasPublic}");
        }

        // c. Все публичные методы
        public static IEnumerable<string> GetPublicMethods(string className)
        {
            Type? type = Type.GetType(className);
            if (type == null) yield break;
                                                                        //методы экземпляра
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                yield return method.Name;
        }

        // d. Поля и свойства
        public static IEnumerable<string> GetFieldsAndProperties(string className)
        {
            Type? type = Type.GetType(className);
            if (type == null) yield break;

            foreach (var field in type.GetFields())
                yield return "Field: " + field.Name;

            foreach (var prop in type.GetProperties())
                yield return "Property: " + prop.Name;
        }

        // e. Интерфейсы
        public static IEnumerable<string> GetInterfaces(string className)
        {
            Type? type = Type.GetType(className);
            if (type == null) yield break;

            foreach (var i in type.GetInterfaces())
                yield return i.Name;
        }

        // f. Методы, содержащие параметр заданного типа
        public static IEnumerable<string> GetMethodsWithParameter(string className, string parameterTypeName)
        {
            Type? type = Type.GetType(className);
            Type? paramType = Type.GetType(parameterTypeName);

            if (type == null || paramType == null) yield break;

            foreach (var method in type.GetMethods())
            {
                if (method.GetParameters().Any(p => p.ParameterType == paramType))
                    yield return method.Name;
            }
        }

        // g. Обычный Invoke
        public static object? Invoke(object obj, string methodName, object[] parameters)
        {
            MethodInfo? method = obj.GetType().GetMethod(methodName);
            if (method == null) return null;

            return method.Invoke(obj, parameters);
        }

        // g1. Invoke с чтением параметров из файла
        public static object? InvokeFromFile(object obj, string methodName, string filePath)
        {
            MethodInfo? method = obj.GetType().GetMethod(methodName);
            if (method == null) return null;

            object[] parameters = ReadParametersFromFile(filePath);
            return method.Invoke(obj, parameters);
        }

        // g2. Invoke с автоматической генерацией параметров
        public static object? InvokeGenerated(object obj, string methodName)
        {
            MethodInfo? method = obj.GetType().GetMethod(methodName);
            if (method == null) return null;

            var parameters = method.GetParameters()
                                   .Select(p => GenerateValue(p.ParameterType))
                                   .ToArray();

            return method.Invoke(obj, parameters);
        }

        // Чтение параметров из файла
        public static object[] ReadParametersFromFile(string file)
        {
            var lines = File.ReadAllLines(file);
            return lines.Select(ParseValue).ToArray();
        }

        // Генерация параметров по типу
        public static object GenerateValue(Type t)
        {
            if (t == typeof(int)) return 42;
            if (t == typeof(double)) return 3.14;
            if (t == typeof(string)) return "GeneratedString";
            if (t == typeof(bool)) return true;

            return Activator.CreateInstance(t)!;
        }

        private static object ParseValue(string s)
        {
            if (int.TryParse(s, out int i)) return i;
            if (double.TryParse(s, out double d)) return d;
            if (bool.TryParse(s, out bool b)) return b;
            return s;
        }

        // 2. Обобщённый метод Create<T>()
        public static T Create<T>()
        {
            Type type = typeof(T);
            ConstructorInfo? ctor = type.GetConstructors().FirstOrDefault();

            if (ctor == null)
                throw new Exception("No public constructors found");

            var parameters = ctor.GetParameters()
                                 .Select(p => GenerateValue(p.ParameterType))
                                 .ToArray();

            return (T)ctor.Invoke(parameters);
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age;

        public Person() { }
        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public void SayHello(string msg)
        {
            Console.WriteLine($"{Name} says: {msg}");
        }
    }

    public class MathOps
    {
        public int Add(int a, int b) => a + b;
        public double Multiply(double x, double y) => x * y;
    }

    class Program
    {
        static void Main()
        {
            File.WriteAllText("reflection_info.txt", "=== Reflection Info ===\n");

            string class1 = typeof(Person).FullName!;
            string class2 = typeof(MathOps).FullName!;
            string class3 = typeof(List<int>).FullName!;

            Reflector.WriteClassInfo(class1);
            Reflector.HasPublicConstructors(class1);

            Reflector.Write("Public methods:");
            foreach (var m in Reflector.GetPublicMethods(class1))
                Reflector.Write("  " + m);

            Reflector.Write("Fields and properties:");
            foreach (var fp in Reflector.GetFieldsAndProperties(class1))
                Reflector.Write("  " + fp);

            Reflector.Write("Interfaces of List<int>:");
            foreach (var i in Reflector.GetInterfaces(class3))
                Reflector.Write("  " + i);

            var methods = Reflector.GetMethodsWithParameter(class1, typeof(string).FullName!);
            Console.WriteLine("Methods with string parameter: " + string.Join(", ", methods));

            var person = new Person("Alex", 25);
            Reflector.Invoke(person, "SayHello", new object[] { "Reflection works!" });

            File.WriteAllText("params.txt", "Hello from file!");
            Reflector.InvokeFromFile(person, "SayHello", "params.txt");

            var math = new MathOps();
            var result = Reflector.InvokeGenerated(math, "Add");
            Console.WriteLine("Generated Add() result: " + result);

            var generatedPerson = Reflector.Create<Person>();
            Console.WriteLine($"Generated person: {generatedPerson.Name}, {generatedPerson.Age}");
        }
    }
}
