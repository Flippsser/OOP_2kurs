using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TPL_Examples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== 1. Длительная задача (поиск простых чисел) ===\n");

            // 1.1 Поиск простых чисел с использованием решета Эратосфена
            Task1_PrimeNumbers();

            Console.WriteLine("\n=== 2. Задача с токеном отмены ===\n");

            // 2. Задача с токеном отмены
            Task2_CancellationTokenExample();

            Console.WriteLine("\n=== 3. Три задачи с возвратом результата ===\n");

            // 3. Три задачи с возвратом результата
            Task3_MultipleTasksWithResults();

            Console.WriteLine("\n=== 4. Задача продолжения ===\n");

            // 4. Задачи продолжения
            Task4_ContinuationTasks();

            Console.WriteLine("\n=== 5. Parallel.For и Parallel.ForEach ===\n");

            // 5. Parallel.For и Parallel.ForEach
            Task5_ParallelLoops();

            Console.WriteLine("\n=== 6. Parallel.Invoke ===\n");

            // 6. Parallel.Invoke
            Task6_ParallelInvoke();

            Console.WriteLine("\n=== 7. BlockingCollection (поставщики и покупатели) ===\n");

            // 7. BlockingCollection
            Task7_BlockingCollectionExample();

            Console.WriteLine("\n=== 8. Async/Await ===\n");

            // 8. Async/Await
            await Task8_AsyncAwaitExample();

            Console.WriteLine("\nВсе задания выполнены!");
            Console.ReadKey();
        }

        // 1. Длительная задача - поиск простых чисел (решето Эратосфена)
        static void Task1_PrimeNumbers()
        {
            var stopwatch = Stopwatch.StartNew();

            // Создаем задачу
            Task<List<int>> primeTask = Task.Run(() => FindPrimesWithSieve(1000000)); // Уменьшим для скорости

            Console.WriteLine($"ID задачи: {primeTask.Id}");
            Console.WriteLine($"Задача завершена: {primeTask.IsCompleted}");
            Console.WriteLine($"Статус задачи: {primeTask.Status}");

            // Ждем завершения
            primeTask.Wait();

            stopwatch.Stop();

            var primes = primeTask.Result;
            Console.WriteLine($"Найдено {primes.Count} простых чисел");

            if (primes.Count >= 5)
            {
                Console.WriteLine($"Последние 5 простых чисел: {string.Join(", ", primes.Skip(primes.Count - 5))}");
            }
            else
            {
                Console.WriteLine($"Все простые числа: {string.Join(", ", primes)}");
            }

            Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
        }

        // Реализация решета Эратосфена
        static List<int> FindPrimesWithSieve(int limit)
        {
            if (limit < 2) return new List<int>();

            bool[] isPrime = new bool[limit + 1];
            for (int i = 2; i <= limit; i++) isPrime[i] = true;

            for (int p = 2; p * p <= limit; p++)
            {
                if (isPrime[p])
                {
                    for (int i = p * p; i <= limit; i += p)
                    {
                        isPrime[i] = false;
                    }
                }
            }

            var primes = new List<int>();
            for (int i = 2; i <= limit; i++)
            {
                if (isPrime[i]) primes.Add(i);
            }

            return primes;
        }

        // 2. Задача с токеном отмены
        static void Task2_CancellationTokenExample()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            Task longRunningTask = Task.Run(() =>
            {
                Console.WriteLine("Задача начата...");

                for (int i = 0; i < 100; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("\nЗадача отменена!");
                        token.ThrowIfCancellationRequested();
                    }

                    Thread.Sleep(50); // Уменьшим время для демонстрации
                    Console.Write(".");
                }

                Console.WriteLine("\nЗадача завершена успешно!");
            }, token);

            // Отменяем задачу через 0.5 секунды
            cts.CancelAfter(500);

            try
            {
                longRunningTask.Wait();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException)
                        Console.WriteLine("Задача была отменена через CancellationToken");
                }
            }
        }

        // 3. Три задачи с возвратом результата
        static void Task3_MultipleTasksWithResults()
        {
            // Создаем три задачи, которые возвращают результаты
            Task<double> task1 = Task.Run(() => CalculateCircleArea(5));
            Task<double> task2 = Task.Run(() => CalculateRectangleArea(4, 6));
            Task<double> task3 = Task.Run(() => CalculateTriangleArea(3, 7));

            // Ожидаем завершения всех задач
            Task.WaitAll(task1, task2, task3);

            // Используем результаты для выполнения четвертой задачи
            Task<double> task = Task.Run(() =>
            {
                double totalArea = task1.Result + task2.Result + task3.Result;
                Console.WriteLine($"Площадь круга: {task1.Result:F2}");
                Console.WriteLine($"Площадь прямоугольника: {task2.Result:F2}");
                Console.WriteLine($"Площадь треугольника: {task3.Result:F2}");
                Console.WriteLine($"Общая площадь: {totalArea:F2}");
                return totalArea;
            });

            task.Wait();
        }

        static double CalculateCircleArea(double radius) => Math.PI * radius * radius;
        static double CalculateRectangleArea(double width, double height) => width * height;
        static double CalculateTriangleArea(double basis, double height) => 0.5 * basis * height;

        // 4. Задачи продолжения
        static void Task4_ContinuationTasks()
        {
            Console.WriteLine("4.1 ContinueWith:");

            Task<int> initialTask = Task.Run(() =>
            {
                Console.WriteLine("Первоначальная задача выполняется...");
                Thread.Sleep(500);
                return 42;
            });

            // Продолжение с ContinueWith
            Task continuationTask = initialTask.ContinueWith(previousTask =>
            {
                Console.WriteLine($"Продолжение: результат предыдущей задачи = {previousTask.Result}");
                Console.WriteLine($"Продолжение выполняется в потоке {Thread.CurrentThread.ManagedThreadId}");
            });

            continuationTask.Wait();

            Console.WriteLine("\n4.2 GetAwaiter(), GetResult():");

            // Использование GetAwaiter()
            Task<string> awaiterTask = Task.Run(() =>
            {
                Thread.Sleep(300);
                return "Результат из задачи";
            });

            var awaiter = awaiterTask.GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                Console.WriteLine($"Задача завершена. Результат: {awaiter.GetResult()}");
            });

            Thread.Sleep(500); // Даем время на выполнение
        }

        // 5. Parallel.For и Parallel.ForEach
        static void Task5_ParallelLoops()
        {
            int size = 1000000; 
            double[] array = new double[size];

            // Обычный цикл
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
            {
                array[i] = Math.Sqrt(i + 1) * Math.Sin(i) * Math.Cos(i); // i+1 чтобы избежать Math.Sqrt(0)
            }
            stopwatch.Stop();
            Console.WriteLine($"Обычный цикл For: {stopwatch.ElapsedMilliseconds} мс");

            // Parallel.For
            stopwatch.Restart();
            Parallel.For(0, size, i =>
            {
                array[i] = Math.Sqrt(i + 1) * Math.Sin(i) * Math.Cos(i);
            });
            stopwatch.Stop();
            Console.WriteLine($"Parallel.For: {stopwatch.ElapsedMilliseconds} мс");

            // Обычный foreach
            List<int> numbers = Enumerable.Range(0, 100000).ToList();
            int[] squares = new int[100000];

            stopwatch.Restart();
            foreach (var number in numbers)
            {
                squares[number] = number * number;
            }
            stopwatch.Stop();

            Console.WriteLine($"Обычный foreach: {stopwatch.ElapsedMilliseconds} мс");

            // Parallel.ForEach
            List<int> numberss = Enumerable.Range(0, 100000).ToList(); 
            int[] squaress = new int[100000];

            stopwatch.Restart();
            Parallel.ForEach(numberss, number =>
            {
                squaress[number] = number * number;
            });
            stopwatch.Stop();
            Console.WriteLine($"Parallel.ForEach: {stopwatch.ElapsedMilliseconds} мс");
        }

        // 6. Parallel.Invoke
        static void Task6_ParallelInvoke()
        {
            Console.WriteLine("Parallel.Invoke - выполнение нескольких действий параллельно:");

            Parallel.Invoke(
                () => {
                    Console.WriteLine($"Действие 1 выполняется в потоке {Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(300);
                    Console.WriteLine("Действие 1 завершено");
                },
                () => {
                    Console.WriteLine($"Действие 2 выполняется в потоке {Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(500);
                    Console.WriteLine("Действие 2 завершено");
                },
                () => {
                    Console.WriteLine($"Действие 3 выполняется в потоке {Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(200);
                    Console.WriteLine("Действие 3 завершено");
                },
                () => {
                    Console.WriteLine($"Действие 4 выполняется в потоке {Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(400);
                    Console.WriteLine("Действие 4 завершено");
                }
            );
        }

        // 7. BlockingCollection
        static void Task7_BlockingCollectionExample()
        {
            BlockingCollection<string> warehouse = new BlockingCollection<string>(boundedCapacity: 10);

            // 5 поставщиков
            List<Task> suppliers = new List<Task>();
            for (int i = 1; i <= 5; i++)
            {
                int supplierId = i;
                suppliers.Add(Task.Run(() => Supplier(supplierId, warehouse)));
            }

            // 10 покупателей
            List<Task> consumers = new List<Task>();
            for (int i = 1; i <= 10; i++)
            {
                int consumerId = i;
                consumers.Add(Task.Run(() => Consumer(consumerId, warehouse)));
            }

            // Даем поработать 3 секунды
            Thread.Sleep(3000);

            // Завершаем добавление
            warehouse.CompleteAdding();

            Task.WaitAll(suppliers.Concat(consumers).ToArray());

            Console.WriteLine("Работа склада завершена.");
        }

        static void Supplier(int id, BlockingCollection<string> warehouse)
        {
            string[] products = { "Холодильник", "Телевизор", "Стиральная машина",
                                "Пылесос", "Микроволновка", "Кофемашина", "Утюг" };
            Random rnd = new Random();

            while (!warehouse.IsAddingCompleted)
            {
                string product = products[rnd.Next(products.Length)] + $" от поставщика {id}";
                if (warehouse.TryAdd(product, 100))
                {
                    Console.WriteLine($"Поставщик {id} доставил: {product}");
                    PrintWarehouseContents(warehouse);
                }

                Thread.Sleep(rnd.Next(300, 800)); // Случайная скорость завоза
            }
        }

        static void Consumer(int id, BlockingCollection<string> warehouse)
        {
            Random rnd = new Random();

            while (!warehouse.IsCompleted)
            {
                if (warehouse.TryTake(out string product, 100))
                {
                    Console.WriteLine($"Покупатель {id} купил: {product}");
                    PrintWarehouseContents(warehouse);
                }
                else
                {
                    Console.WriteLine($"Покупатель {id} ушел без покупки");
                }

                Thread.Sleep(rnd.Next(200, 600));
            }
        }

        static void PrintWarehouseContents(BlockingCollection<string> warehouse)
        {
            Console.WriteLine($"Товаров на складе: {warehouse.Count}");
            if (warehouse.Count > 0)
            {
                Console.WriteLine($"Текущие товары: {string.Join(", ", warehouse)}");
            }
            Console.WriteLine("---");
        }

        // 8. Async/Await
        static async Task Task8_AsyncAwaitExample()
        {
            Console.WriteLine("Начало асинхронного метода...");

            // Асинхронное выполнение нескольких операций
            var result1 = await CalculateAsync("Операция 1", 1000);
            var result2 = await CalculateAsync("Операция 2", 500);

            // Параллельное выполнение
            var task3 = CalculateAsync("Операция 3", 800);
            var task4 = CalculateAsync("Операция 4", 600);

            await Task.WhenAll(task3, task4);

            Console.WriteLine($"Результаты: {result1}, {result2}, {task3.Result}, {task4.Result}");

            // Асинхронный HTTP-запрос (имитация)
            string data = await FetchDataAsync();
            Console.WriteLine($"Получены данные: {data}");
        }

        static async Task<int> CalculateAsync(string operationName, int delayMs)
        {
            Console.WriteLine($"Начало {operationName} в потоке {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(delayMs);
            Console.WriteLine($"Завершение {operationName}");
            return delayMs / 100;
        }

        static async Task<string> FetchDataAsync()
        {
            await Task.Delay(500);
            return "Данные с сервера";
        }
    }
}