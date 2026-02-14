using System;
using System.IO;
using System.Linq;
using System.IO.Compression;

// Класс для логирования действий пользователя
public class BMALog
{
    private readonly string logFile = "bmalogfile.txt";

    public BMALog()
    {
        if (!File.Exists(logFile))
            File.Create(logFile).Close();
    }

    public void Write(string action, string details)
    {
        try
        {
            using StreamWriter sw = new StreamWriter(logFile, true);
            sw.WriteLine($"{DateTime.Now} | {action} | {details}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка записи в лог: {ex.Message}");
        }
    }

    public string[] ReadAll()
    {
        try
        {
            return File.ReadAllLines(logFile);
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    public string[] Search(string keyword)
    {
        return ReadAll().Where(l => l.Contains(keyword)).ToArray();
    }

    public string[] Search(DateTime from, DateTime to)
    {
        return ReadAll().Where(l =>
        {
            var date = DateTime.Parse(l.Split('|')[0]);
            return date >= from && date <= to;
        }).ToArray();
    }

    public void KeepOnlyCurrentHour()
    {
        var now = DateTime.Now;
        var filtered = ReadAll().Where(l =>
        {
            var date = DateTime.Parse(l.Split('|')[0]);
            return date.Hour == now.Hour && date.Date == now.Date;
        });

        File.WriteAllLines(logFile, filtered);
    }
}

// Класс для работы с информацией о дисках
public class BMADiskInfo
{
    public void ShowAllDrives()
    {
        foreach (var d in DriveInfo.GetDrives())
        {
            try
            {
                if (!d.IsReady)
                {
                    Console.WriteLine($"Диск {d.Name} не готов.\n");
                    continue;
                }

                Console.WriteLine($"Диск: {d.Name}");
                Console.WriteLine($"Объем: {d.TotalSize / 1024 / 1024 / 1024} GB");
                Console.WriteLine($"Доступно: {d.AvailableFreeSpace / 1024 / 1024 / 1024} GB");
                Console.WriteLine($"Метка тома: {d.VolumeLabel}");
                Console.WriteLine($"Формат диска: {d.DriveFormat}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении диска {d.Name}: {ex.Message}\n");
            }
        }
    }

}

// Класс для вывода информации о файле
public class BMAFileInfo
{
    public void ShowInfo(string path)
    {
        try
        {
            FileInfo fi = new FileInfo(path);

            Console.WriteLine($"Полный путь: {fi.FullName}");
            Console.WriteLine($"Размер: {fi.Length} байт");
            Console.WriteLine($"Расширение: {fi.Extension}");
            Console.WriteLine($"Имя: {fi.Name}");
            Console.WriteLine($"Создан: {fi.CreationTime}");
            Console.WriteLine($"Изменён: {fi.LastWriteTime}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

// Класс для вывода информации о директории
public class BMADirInfo
{
    public void ShowInfo(string path)
    {
        try
        {
            DirectoryInfo di = new DirectoryInfo(path);

            Console.WriteLine($"Количество файлов: {di.GetFiles().Length}");
            Console.WriteLine($"Время создания: {di.CreationTime}");
            Console.WriteLine($"Количество поддиректориев: {di.GetDirectories().Length}");

            Console.WriteLine("Родительские директории:");
            DirectoryInfo? parent = di.Parent;
            while (parent != null)
            {
                Console.WriteLine(parent.FullName);
                parent = parent.Parent;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

// Класс для выполнения операций с файлами и директориями
public class BMAFileManager
{
    private readonly BMALog log = new BMALog();

    public void InspectDrive(string drive)
    {
        string inspectDir = "BMAInspect";
        Directory.CreateDirectory(inspectDir);

        string infoFile = Path.Combine(inspectDir, "bmadirinfo.txt");

        using (StreamWriter sw = new StreamWriter(infoFile))
        {
            foreach (var f in Directory.GetFiles(drive))
                sw.WriteLine("FILE: " + f);

            foreach (var d in Directory.GetDirectories(drive))
                sw.WriteLine("DIR: " + d);
        }

        string copy = Path.Combine(inspectDir, "copy.txt");
        File.Copy(infoFile, copy, true);
        File.Delete(infoFile);

        log.Write("InspectDrive", $"Сканирование диска {drive}");
    }

    public void CopyFilesByExtension(string sourceDir, string ext)
    {
        string inspectDir = "BMAInspect";
        string filesDir = "BMAFiles";

        Directory.CreateDirectory(inspectDir);
        Directory.CreateDirectory(filesDir);

        foreach (var file in Directory.GetFiles(sourceDir, $"*.{ext}"))
        {
            string dest = Path.Combine(filesDir, Path.GetFileName(file));
            File.Copy(file, dest, true);
        }

        Directory.Move(filesDir, Path.Combine(inspectDir, filesDir));

        log.Write("CopyFilesByExtension", $"Копирование *.{ext} из {sourceDir}");
    }

    public void ArchiveFiles()
    {
        string filesDir = Path.Combine("BMAInspect", "BMAFiles");
        string zipPath = "BMAFiles.zip";
        string extractDir = "BMAExtracted";

        ZipFile.CreateFromDirectory(filesDir, zipPath);
        ZipFile.ExtractToDirectory(zipPath, extractDir);

        log.Write("ArchiveFiles", "Архивация и разархивация выполнены");
    }
}

// Главная программа
class Program
{
    static void Main(string[] args)
    {
        BMALog log = new BMALog();
        BMADiskInfo diskInfo = new BMADiskInfo();
        BMAFileInfo fileInfo = new BMAFileInfo();
        BMADirInfo dirInfo = new BMADirInfo();
        BMAFileManager fileManager = new BMAFileManager();

        Console.WriteLine("=== BMA System Utility Demo ===");

        try
        {
            Console.WriteLine("\n--- Disk Info ---");
            diskInfo.ShowAllDrives();
            log.Write("DiskInfo", "Просмотр информации о дисках");

            Console.WriteLine("\n--- File Info ---");
            string testFile = "test.txt";
            File.WriteAllText(testFile, "Hello BMA!");
            fileInfo.ShowInfo(testFile);
            log.Write("FileInfo", $"Просмотр информации о файле {testFile}");

            Console.WriteLine("\n--- Directory Info ---");
            string testDir = Directory.GetCurrentDirectory();
            dirInfo.ShowInfo(testDir);
            log.Write("DirInfo", $"Просмотр информации о директории {testDir}");

            Console.WriteLine("\n--- FileManager: Inspect Drive ---");
            string drive = Path.GetPathRoot(Environment.CurrentDirectory);
            fileManager.InspectDrive(drive);

            Console.WriteLine("\n--- FileManager: Copy Files by Extension ---");
            fileManager.CopyFilesByExtension(testDir, "txt");

            Console.WriteLine("\n--- FileManager: Archive Files ---");
            fileManager.ArchiveFiles();

            Console.WriteLine("\n--- Log Search ---");
            var today = log.Search(DateTime.Today, DateTime.Now);
            Console.WriteLine($"Записей за сегодня: {today.Length}");

            var keyword = log.Search("FileInfo");
            Console.WriteLine($"Записей по ключевому слову 'FileInfo': {keyword.Length}");

            Console.WriteLine("\nОставляем только записи за текущий час...");
            log.KeepOnlyCurrentHour();

            Console.WriteLine("Готово!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения программы: {ex.Message}");
            log.Write("Error", ex.Message);
        }

        Console.WriteLine("\n=== Работа завершена ===");
    }
}
