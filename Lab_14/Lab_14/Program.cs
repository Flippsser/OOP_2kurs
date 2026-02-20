using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace ThreadsAndProcessesTasks
{
    class Program
    {
        private static readonly string OutputFile = "task_output.txt";
        private static readonly object FileLock = new object();

        static void Main(string[] args)
        {
            try
            {
                File.WriteAllText(OutputFile, $"=== Запуск заданий {DateTime.Now} ==={Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось подготовить файл вывода: " + ex.Message);
            }

            // 1. Процессы
            WriteHeader("Задание 1. Информация о процессах");
            Task1_ListProcesses();

            // 2. Домен приложения
            WriteHeader("Задание 2. Информация о домене приложения, создание и выгрузка домена");
            Task2_AppDomainInfoAndUnload();

            // 3. Поток для вычисления простых чисел (start/pause/resume/stop)
            WriteHeader("Задание 3. Поток: вычисление простых чисел с управлением");
            Task3_PrimesThreadDemo();

            // 4. Два потока: чётные и нечётные, синхронизация
            WriteHeader("Задание 4. Два потока: чётные и нечётные числа");
            Task4_EvenOddThreadsDemo();

            // 5. Повторяющаяся задача на основе Timer
            WriteHeader("Задание 5. Повторяющаяся задача (Timer)");
            Task5_TimerDemo();

            WriteLineToAll("=== Все задания выполнены ===");
            Console.WriteLine("Нажмите Enter для выхода...");
            Console.ReadLine();
        }

        #region Utilities

        static void WriteHeader(string title)
        {
            string s = Environment.NewLine + "№№№№№ " + title + " №№№№№" + Environment.NewLine;
            Console.WriteLine(s);
            AppendToFile(s);
        }

        static void WriteLineToAll(string line)
        {
            Console.WriteLine(line);
            AppendToFile(line + Environment.NewLine);
        }

        static void AppendToFile(string text)
        {
            try
            {
                lock (FileLock)
                {
                    File.AppendAllText(OutputFile, text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка записи в файл: " + ex.Message);
            }
        }

        #endregion

        #region Task 1 - Processes

        static void Task1_ListProcesses()
        {
            try
            {
                var processes = Process.GetProcesses().OrderBy(p => p.ProcessName).ToArray();
                string header = $"Найдено процессов: {processes.Length}";
                WriteLineToAll(header);

                foreach (var p in processes)
                {
                    try
                    {
                        int id = p.Id;
                        string name = p.ProcessName;
                        string priority = "N/A";
                        try
                        {
                            priority = p.PriorityClass.ToString();
                        }
                        catch { /* доступ запрещён */ }

                        string startTime = "N/A";
                        try
                        {
                            startTime = p.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        catch { /* доступ запрещён */ }

                        string responding = "N/A";
                        try
                        {
                            responding = p.Responding ? "Responding" : "Not responding";
                        }
                        catch { }

                        string totalCpu = "N/A";
                        try
                        {
                            totalCpu = p.TotalProcessorTime.ToString();
                        }
                        catch { }

                        string line = $"PID={id}, Name={name}, Priority={priority}, Start={startTime}, State={responding}, CPU={totalCpu}";
                        WriteLineToAll(line);
                    }
                    catch (Exception exInner)
                    {
                        WriteLineToAll($"Ошибка при обработке процесса: {exInner.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLineToAll("Ошибка получения списка процессов: " + ex.Message);
            }
        }

        #endregion

        #region Task 2 - AppDomain

        static void Task2_AppDomainInfoAndUnload()
        {
            try
            {
                var current = AppDomain.CurrentDomain;
                WriteLineToAll($"Текущий домен: {current.FriendlyName}");
                WriteLineToAll($"Базовый каталог: {current.BaseDirectory}");
                WriteLineToAll($"Конфигурация: {current.SetupInformation?.ApplicationBase ?? "N/A"}");

                WriteLineToAll("Сборки, загруженные в текущий домен:");
                foreach (var asm in current.GetAssemblies().OrderBy(a => a.FullName))
                {
                    try
                    {
                        WriteLineToAll($" - {asm.GetName().Name}, Version={asm.GetName().Version}, Location={SafeAssemblyLocation(asm)}");
                    }
                    catch { }
                }

                bool canCreateDomain = true;
                try
                {
                    var test = AppDomain.CreateDomain("TempDomainForLoadTest");
                    AppDomain.Unload(test);
                }
                catch (PlatformNotSupportedException)
                {
                    canCreateDomain = false;
                }
                catch
                {
                    canCreateDomain = true;
                }

                if (!canCreateDomain)
                {
                    WriteLineToAll("Создание AppDomain не поддерживается в этой среде (например, .NET Core). Для выгрузки сборок используйте AssemblyLoadContext.");
                    return;
                }

                var newDomain = AppDomain.CreateDomain("MyTempDomain");
                WriteLineToAll($"Создан новый домен: {newDomain.FriendlyName}");

                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                try
                {
                    var handle = newDomain.CreateInstanceFrom(assemblyPath, typeof(DomainHelper).FullName);
                    var helper = (DomainHelper)handle.Unwrap();
                    var list = helper.ListAssemblies();
                    WriteLineToAll("Сборки, загруженные в новом домене:");
                    foreach (var s in list)
                    {
                        WriteLineToAll(" - " + s);
                    }
                }
                catch (Exception ex)
                {
                    WriteLineToAll("Ошибка при загрузке сборки в новый домен: " + ex.Message);
                }

                try
                {
                    AppDomain.Unload(newDomain);
                    WriteLineToAll("Новый домен успешно выгружен.");
                }
                catch (Exception ex)
                {
                    WriteLineToAll("Ошибка при выгрузке домена: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                WriteLineToAll("Ошибка в задании 2: " + ex.Message);
            }
        }

        public class DomainHelper : MarshalByRefObject
        {
            public string[] ListAssemblies()
            {
                try
                {
                    var asms = AppDomain.CurrentDomain.GetAssemblies();
                    return asms.Select(a => $"{a.GetName().Name}, v{a.GetName().Version}, loc={SafeAssemblyLocation(a)}").ToArray();
                }
                catch (Exception ex)
                {
                    return new[] { "Ошибка: " + ex.Message };
                }
            }
        }

        static string SafeAssemblyLocation(Assembly asm)
        {
            try
            {
                return string.IsNullOrEmpty(asm.Location) ? "(in-memory)" : asm.Location;
            }
            catch
            {
                return "(unknown)";
            }
        }

        #endregion

        #region Task 3 - Primes thread with control

        // Класс-обёртка для управления потоком вычисления простых чисел
        class PrimesWorker
        {
            private readonly int _n;
            private readonly Thread _thread;
            private readonly ManualResetEventSlim _pauseEvent = new ManualResetEventSlim(true);
            private volatile bool _stopRequested = false;
            private readonly string _outFile;
            private readonly int _sleepMs;

            public PrimesWorker(int n, string outFile, int sleepMs = 200)
            {
                _n = n;
                _outFile = outFile;
                _sleepMs = sleepMs;
                _thread = new Thread(Run) { IsBackground = true, Name = "PrimesWorker" };
            }

            public void Start() => _thread.Start();

            public void Pause() => _pauseEvent.Reset();

            public void Resume() => _pauseEvent.Set();

            public void Stop() => _stopRequested = true;

            public void Join() => _thread.Join();

            private void Run()
            {
                Append($"[PrimesWorker] Thread started. Calculating primes up to {_n}.");
                for (int i = 2; i <= _n && !_stopRequested; i++)
                {
                    _pauseEvent.Wait();
                    if (IsPrime(i))
                    {
                        string line = $"Prime: {i}";
                        Append(line);
                    }
                    Thread.Sleep(_sleepMs);
                }
                Append("[PrimesWorker] Thread finished.");
            }

            private bool IsPrime(int x)
            {
                if (x < 2) return false;
                if (x % 2 == 0) return x == 2;
                int r = (int)Math.Sqrt(x);
                for (int i = 3; i <= r; i += 2)
                    if (x % i == 0) return false;
                return true;
            }

            private void Append(string text)
            {
                string line = $"[{DateTime.Now:HH:mm:ss}] {text}";
                Console.WriteLine(line);
                lock (FileLock)
                {
                    File.AppendAllText(_outFile, line + Environment.NewLine);
                }
            }
        }

        static void Task3_PrimesThreadDemo()
        {
            try
            {
                int n = 50;
                WriteLineToAll($"Будем вычислять простые числа до {n} в отдельном потоке.");
                var worker = new PrimesWorker(n, OutputFile, sleepMs: 100);

                WriteLineToAll("Запуск потока...");
                worker.Start();
                Thread.Sleep(700);

                WriteLineToAll("Приостановка потока на 2 секунды...");
                worker.Pause();
                Thread.Sleep(2000);

                WriteLineToAll("Возобновление потока...");
                worker.Resume();
                Thread.Sleep(1000);

                WriteLineToAll("Снова приостановка на 1 секунду...");
                worker.Pause();
                Thread.Sleep(1000);

                WriteLineToAll("Возобновление и ожидание завершения...");
                worker.Resume();

                worker.Join();
                WriteLineToAll("Поток вычисления простых чисел завершён.");
            }
            catch (Exception ex)
            {
                WriteLineToAll("Ошибка в задании 3: " + ex.Message);
            }
        }

        #endregion

        #region Task 4 - Even/Odd threads



        static void Task4_EvenOddThreadsDemo()
        {
            try
            {
                int n = 30;
                WriteLineToAll($"Будем выводить числа до {n} двумя потоками (чётные/нечётные).");

                // (a) Разные скорости и изменение приоритета
                WriteLineToAll("Запуск варианта (1): сначала все чётные, затем все нечётные.");
                EvenOddSequential(n);

                WriteLineToAll("Запуск варианта (2): поочерёдно одно чётное, одно нечётное.");
                EvenOddAlternating(n);
            }
            catch (Exception ex)
            {
                WriteLineToAll("Ошибка в задании 4: " + ex.Message);
            }
        }

        static void EvenOddSequential(int n)
        {
            Thread evenThread = new Thread(() =>
            {
                for (int i = 0; i <= n; i += 2)
                {
                    string s = $"Even: {i}";
                    WriteLineToAll(s);
                    Thread.Sleep(50);
                }
            })
            { IsBackground = true, Name = "EvenThread" };

            Thread oddThread = new Thread(() =>
            {
                for (int i = 1; i <= n; i += 2)
                {
                    string s = $"Odd: {i}";
                    WriteLineToAll(s);
                    Thread.Sleep(150);
                }
            })
            { IsBackground = true, Name = "OddThread" };

            try
            {
                evenThread.Priority = ThreadPriority.AboveNormal;
                oddThread.Priority = ThreadPriority.BelowNormal;
            }
            catch { }

            evenThread.Start();
            evenThread.Join();
            oddThread.Start();
            oddThread.Join();
            WriteLineToAll("Вариант (1) завершён.");
        }

        static void EvenOddAlternating(int n)
        {
            AutoResetEvent evenEvent = new AutoResetEvent(true);
            AutoResetEvent oddEvent = new AutoResetEvent(false);

            object sync = new object();

            Thread evenThread = new Thread(() =>
            {
                for (int i = 0; i <= n; i += 2)
                {
                    evenEvent.WaitOne();
                    lock (sync)
                    {
                        WriteLineToAll($"Even: {i}");
                    }
                    Thread.Sleep(80);
                    oddEvent.Set();
                }
            })
            { IsBackground = true, Name = "EvenThreadAlt" };

            Thread oddThread = new Thread(() =>
            {
                for (int i = 1; i <= n; i += 2)
                {
                    oddEvent.WaitOne();
                    lock (sync)
                    {
                        WriteLineToAll($"Odd: {i}");
                    }
                    Thread.Sleep(120);
                    evenEvent.Set();
                }
            })
            { IsBackground = true, Name = "OddThreadAlt" };



            evenThread.Start();
            oddThread.Start();

            evenThread.Join();
            oddThread.Join();

            WriteLineToAll("Вариант (2) (чередование) завершён.");
        }

        #endregion

        #region Task 5 - Timer

        static void Task5_TimerDemo()
        {
            // Повторяющаяся задача: каждые 2 секунды записывать метку времени в файл и консоль.
            using (AutoResetEvent autoEvent = new AutoResetEvent(false))
            {
                TimerCallback callback = state =>
                {
                    string msg = $"[Timer] {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                    WriteLineToAll(msg);
                };

                using (Timer timer = new Timer(callback, null, 0, 2000))
                {
                    WriteLineToAll("Timer запущен. Он будет работать 8 секунд (4 срабатывания).");
                    Thread.Sleep(8000);
                    WriteLineToAll("Timer будет остановлен.");
                }
            }
        }

        #endregion
    }
}