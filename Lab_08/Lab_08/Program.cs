using System;
using System.Collections.Generic;

namespace Lab_08
{
    // Делегат, описывающий изменение зарплаты
    public delegate void IzmenenieZarplaty(decimal summa);

    // Класс Директор 
    class Direktor
    {
        public event IzmenenieZarplaty PovysitZarplatu;

        public event IzmenenieZarplaty Shtrafovat;

        public void VypolnitPovyshenie(decimal summa) =>
            PovysitZarplatu?.Invoke(summa);

        public void VypolnitShtraf(decimal summa) =>
            Shtrafovat?.Invoke(summa);
    }

    // Базовый класс сотрудника
    abstract class Sotrudnik
    {
        public string FIO { get; }
        public decimal Zarplata { get; protected set; }

        protected Sotrudnik(string fio, decimal zarplata)
        {
            FIO = fio;
            Zarplata = zarplata;
        }

        // Реакция на повышение
        public virtual void NachislitPovyshenie(decimal summa) =>
            Zarplata += summa;

        // Реакция на штраф
        public virtual void NachislitShtraf(decimal summa) =>
            Zarplata = Math.Max(0, Zarplata - summa);

        public override string ToString() =>
            $"{GetType().Name} {FIO}: zarplata = {Zarplata}";
    }

    // Токарь — получает повышенный бонус
    class Tokar : Sotrudnik
    {
        public Tokar(string fio, decimal zarplata)
            : base(fio, zarplata) { }

        // Токарю повышение  на 10%
        public override void NachislitPovyshenie(decimal summa)
        {
            Zarplata += summa * 1.1m;
        }
    }

    // Заочник — получает штраф
    class Zaochnik : Sotrudnik
    {
        public Zaochnik(string fio, decimal zarplata)
            : base(fio, zarplata) { }

        // Заочнику штраф 
        public override void NachislitShtraf(decimal summa)
        {
            Zarplata = Math.Max(0, Zarplata - summa * 0.5m);
        }
    }

    class Program
    {
        static void Main()
        {
            // Создаём директора
            var direktor = new Direktor();

            // Создаём сотрудников
            var tokar1 = new Tokar("Petrov", 50000);
            var tokar2 = new Tokar("Sidorov", 52000);
            var zaochnik1 = new Zaochnik("Ivanova", 30000);
            var zaochnik2 = new Zaochnik("Kuznetsov", 28000);

            // Подписываем сотрудников на события
            direktor.PovysitZarplatu += tokar1.NachislitPovyshenie;
            direktor.PovysitZarplatu += zaochnik1.NachislitPovyshenie;
            direktor.PovysitZarplatu += zaochnik2.NachislitPovyshenie;

            direktor.Shtrafovat += tokar2.NachislitShtraf;
            direktor.Shtrafovat += zaochnik2.NachislitShtraf;

            Console.WriteLine("Состояние сотрудников ДО событий:");
            PechatSotrudnikov(tokar1, tokar2, zaochnik1, zaochnik2);

            // Генерируем события
            direktor.VypolnitPovyshenie(5000);
            direktor.VypolnitShtraf(3000);

            Console.WriteLine("\nСостояние сотрудников ПОСЛЕ событий:");
            PechatSotrudnikov(tokar1, tokar2, zaochnik1, zaochnik2);

            Console.WriteLine("\n--- Часть 2: обработка строк ---");

            // Исходная строка
            string ishodnayaStroka = "  Privet, mir!!!   Eto   stroka   s   probelami...  ";

            // Удаление знаков препинания
            Func<string, string> ubratZnaki = text =>
            {
                var list = new List<char>();
                foreach (var c in text)
                    if (!char.IsPunctuation(c)) list.Add(c);
                return new string(list.ToArray());
            };

            // Преобразование в верхний регистр
            Func<string, string> vVerhniyRegistr = text => text.ToUpper();

            // Удаление пробелов по краям
            Func<string, string> obrezatProbel = text => text.Trim();

            // Удаление лишних пробелов
            Func<string, string> ubratLishnieProbely = text =>
            {
                var parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return string.Join(" ", parts);
            };

            // Добавление скобок
            Func<string, string> vSkobki = text => "[" + text + "]";

            // Последовательная обработка строки
            string rezultat =
                vSkobki(
                    ubratLishnieProbely(
                        obrezatProbel(
                            vVerhniyRegistr(
                                ubratZnaki(ishodnayaStroka)))));

            Console.WriteLine(rezultat);

            // Action — просто вывод строки
            Action<string> vyvesti = s => Console.WriteLine("Action vyvodit: " + s);
            vyvesti(rezultat);

            // Predicate — проверка длины строки
            Predicate<string> dlinayaStroka = s => s.Length > 20;
            Console.WriteLine("Predicate: stroka dlinnee 20 simvolov = " + dlinayaStroka(rezultat));
            Console.ReadLine();
        }

        // Метод печати списка сотрудников
        static void PechatSotrudnikov(params Sotrudnik[] sotrudniki)
        {
            foreach (var s in sotrudniki)
                Console.WriteLine(s);
        }
    }
}
