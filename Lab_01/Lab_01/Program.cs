using System;
using System.Activities;
using System.Activities.Statements;
using System.Linq;
using System.Text;

namespace Lab_01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //1 
            byte b = 255;
            sbyte sb = -128;
            short s = -32768;
            ushort us = 65535;
            int i = 367;
            uint ui = 1234567890;
            long l = -9876543210;
            ulong ul = 12345678901234567890;

            float f = 3.14f;
            double d = 2.718281828;
            decimal dec = 12345.6789m;

            char c = 'C';
            bool flag = true;
            string str = "Hi";
            object obj = 42;

            Console.WriteLine("Результаты:");
            Console.WriteLine($"byte: {b}");
            Console.WriteLine($"sbyte: {sb}");
            Console.WriteLine($"short: {s}");
            Console.WriteLine($"ushort: {us}");
            Console.WriteLine($"int: {i}");
            Console.WriteLine($"uint: {ui}");
            Console.WriteLine($"long: {l}");
            Console.WriteLine($"ulong: {ul}");
            Console.WriteLine($"float: {f}");
            Console.WriteLine($"double: {d}");
            Console.WriteLine($"decimal: {dec}");
            Console.WriteLine($"char: {c}");
            Console.WriteLine($"bool: {flag}");
            Console.WriteLine($"string: {str}");

            Console.Write("\nВведите число: ");
            int number = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine($"Введено: {number}");

            // Неявное
            int a = 10;
            long b1 = a;
            float f1 = a;
            double d1 = f1;
            char ch = 'B';
            int code = ch;
            uint ux = 100;
            ulong uy = ux;

            // Явное
            double pi = 3.14;
            int intPi = (int)pi;
            long big = 9999999999;
            int small = (int)big;
            decimal money = 123.45m;
            float fl = (float)money;
            string strNum = "123";
            int num = Convert.ToInt32(strNum);
            bool flag2 = true;
            int boolInt = Convert.ToInt32(flag2);

            Console.WriteLine("\n=== Неявное приведение ===");
            Console.WriteLine($"int -> long: {b1}");
            Console.WriteLine($"int -> float: {f1}");
            Console.WriteLine($"float -> double: {d1}");
            Console.WriteLine($"char -> int: {code}");
            Console.WriteLine($"uint -> ulong: {uy}");

            Console.WriteLine("\n=== Явное приведение ===");
            Console.WriteLine($"double -> int: {intPi}");
            Console.WriteLine($"long -> int: {small}");
            Console.WriteLine($"decimal -> float: {fl}");
            Console.WriteLine($"string -> int: {num}");
            Console.WriteLine($"bool -> int: {boolInt}\n");

            // c) Упаковка и распаковка
            int val = 123;
            object objVal = val;   // упаковка
            int val2 = (int)objVal; // распаковка
            Console.WriteLine($"Упаковка: {objVal}, Распаковка: {val2}");

            // d) Неявно типизированная переменная
            var message = "Hello1!";
            Console.WriteLine($"\nПеременная var: {message}");
            Console.WriteLine($"Тип переменной message: {message.GetType()}");

            // e) Nullable
            int? nullableInt = null;
            Console.WriteLine($"\nHasValue: {nullableInt.HasValue}");
            nullableInt = 42;
            Console.WriteLine($"Value: {nullableInt.Value}");

            // f) var
            var xVar = 100; 
             //xVar = "Text";
            dynamic dyn = 100;
            dyn = "Text";
            Console.WriteLine($"Dynamic value: {dyn}");

            //2 
            //a)
            string str1 = "Hello";
            string str2 = "World";
            string str3 = "Hello";

            Console.WriteLine($"\nstr1 == str2: {str1 == str2}"); // false
            Console.WriteLine($"str1 == str3: {str1 == str3}"); // true
            Console.WriteLine($"str1.Equals(str3): {str1.Equals(str3)}"); // true
            Console.WriteLine($"ReferenceEquals(str1, str3): {object.ReferenceEquals(str1, str3)}"); // true

            //b)
            string s1 = "C#";
            string s2 = "is";
            string s3 = "great";

            // конкатенация
            string concat = s1 + " " + s2 + " " + s3;
            Console.WriteLine($"\nСцепление: {concat}");

            // копирование
            string copy = String.Copy(s1);
            Console.WriteLine($"Копия: {copy}");

            // подстрока
                string substring = concat.Substring(3, 2);
                Console.WriteLine($"Подстрока: {substring}");

            // разделение на слова
            string[] words = concat.Split(' ');
            Console.WriteLine("Слова:");
            foreach (var word in words)
                Console.WriteLine(word);

            // вставка подстроки
            string insert = concat.Insert(2, " language ");
            Console.WriteLine($"Вставка: {insert}");

            // удаление подстроки
            string remove = concat.Remove(2, 3);
            Console.WriteLine($"Удаление: {remove}");

            // интерполяция
            int year = 2025;
            string interp = $"{s1} {s2} {s3} in {year}";
            Console.WriteLine($"Интерполяция: {interp}");

            //с)
            string empty = "";
            string nullStr = null;

            Console.WriteLine($"\nempty == \"\": {string.IsNullOrEmpty(empty)}");
            Console.WriteLine($"nullStr == null: {string.IsNullOrEmpty(nullStr)}");

            // Можно проверять длину
            Console.WriteLine($"Длина empty: {empty.Length}"); // 0
            // Console.WriteLine($"Длина nullStr: {nullStr.Length}"); //  Ошибка (NullReferenceException)

            // Сравнение с другими строками
            Console.WriteLine($"Сравнение empty и ' ': {empty == " "}"); // false

            //d)
            StringBuilder builder = new StringBuilder("Hello, World!");
            Console.WriteLine("\nHello, World!");

            // Удаление
            builder.Remove(5, 7); // удаляем ", World"
            Console.WriteLine($"После удаления: {builder}");

            // Добавление в начало
            builder.Insert(0, ">>> ");
            Console.WriteLine($"После вставки в начало: {builder}");

            // Добавление в конец
            builder.Append(" <<<");
            Console.WriteLine($"После вставки в конец: {builder}");

            //3 
            // a) Двумерный массив
            int[,] matrix = {
                    {1, 2, 3},
                    {4, 5, 6},
                    {7, 8, 9}
                };

            Console.WriteLine("Двумерный массив (матрица):");
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    Console.Write($"{matrix[row, col],4}");
                }
                Console.WriteLine();
            }

            // b) Одномерный массив строк
            string[] wordz = { "C#", "JS", "Python", "C++" };

            Console.WriteLine("\nОдномерный массив строк:");
            foreach (var word in wordz)
            {
                Console.WriteLine(word);
            }
            Console.WriteLine($"Длина массива: {wordz.Length}");

            // замена 
            Console.Write("Введите позицию (0..3): ");
            int pos = Convert.ToInt32(Console.ReadLine());
            Console.Write("Введите новое значение: ");
            string newVal = Console.ReadLine();

            if (pos >= 0 && pos < wordz.Length)
            {
                wordz[pos] = newVal;
            }
            else
            {
                Console.WriteLine("Некорректная позиция!");
            }

            // Вывод обновлённого массива
            Console.WriteLine("Обновленный массив:");
            foreach (var word in wordz)
            {
                Console.WriteLine(word);
            }

            // c) Ступенчатый массив
            double[][] jagged = new double[3][];
            jagged[0] = new double[2];
            jagged[1] = new double[3];
            jagged[2] = new double[4];

            for (int r = 0; r < jagged.Length; r++)
            {
                for (int h = 0; h < jagged[r].Length; h++)
                {
                    Console.Write($"Введите jagged[{r}][{h}]: ");
                    jagged[r][h] = Convert.ToDouble(Console.ReadLine());
                }
            }

            Console.WriteLine("\nСтупенчатый массив:");
            for (int rw = 0; rw < jagged.Length; rw++)
            {
                for (int cl = 0; cl < jagged[rw].Length; cl++)
                {
                    Console.Write($"{jagged[rw][cl],6}");
                }
                Console.WriteLine();
            }

            // d) Неявно типизированные переменные
            var autoArray = new[] { 1, 2, 3, 4, 5 };
            var autoString = "Неявно типизированная строка";

            Console.WriteLine($"\nautoArray type: {autoArray.GetType()}");
            Console.WriteLine($"autoString type: {autoString.GetType()}");

            //4 кортеж
            // a) Создание кортежа из 5 элементов
            var tuple1 = (42, "Hello", 'A', "World", 123456789UL);

            // b) Вывод кортежа
            Console.WriteLine($"\nКортеж целиком: {tuple1}");
            Console.WriteLine($"Элементы 1, 3, 4: {tuple1.Item1}, {tuple1.Item3}, {tuple1.Item4}");

            // c) Распаковка 
            // Полная распаковка с уникальными именами
            (int t1Num, string t1Str1, char t1Char, string t1Str2, ulong t1Long) = tuple1;
            Console.WriteLine($"Распакованные: {t1Num}, {t1Str1}, {t1Char}, {t1Str2}, {t1Long}");


            (var t2Num, _, var t2Char, _, var t2Long) = tuple1;
            Console.WriteLine($"Выбранные: {t2Num}, {t2Char}, {t2Long}");

            // d) Сравнение кортежей
            var tuple2 = (42, "Hello", 'A', "World", 123456789UL);
            Console.WriteLine($"tuple1 == tuple2? {tuple1 == tuple2}"); // True

            //5 
            int[] arrNumbers = { 1, 2, 3, 4, 5 };
            string inputText = "Hello";

            // Локальная функция с явным типом кортежа
            (int maximum, int minimum, int totalSum, char firstLetter) AnalyzeArrayAndString(int[] arr, string textParam)
            {
                int maxVal = arr.Max();
                int minVal = arr.Min();
                int sumVal = arr.Sum();
                char first = textParam[0];
                return (maxVal, minVal, sumVal, first);
            }

            var analysisResult = AnalyzeArrayAndString(arrNumbers, inputText);

            Console.WriteLine($"\nMax: {analysisResult.maximum}, Min: {analysisResult.minimum}, Sum: {analysisResult.totalSum}, FirstChar: {analysisResult.firstLetter}");

            //6 
            // a) Локальные функции
            void CheckedFunction()
            {
                checked
                {
                    int maxInt = int.MaxValue;
                    Console.WriteLine(maxInt);
                    //maxInt += 1;
                }
            }

            void UncheckedFunction()
            {
                unchecked
                {
                    int maxInt = int.MaxValue;
                    Console.WriteLine(maxInt);
                    maxInt += 1;
                }
            }

            CheckedFunction();
            UncheckedFunction();

            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
