using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Lab9
{
    // Класс "Товар"
    public class Product
    {
        public string Name { get; set; }
        public int Price { get; set; }

        public Product(string name, int price)
        {
            Name = name;
            Price = price;
        }

        public override string ToString()
        {
            return Name + " (" + Price + " руб.)";
        }
    }

    // Реализация коллекции IOrderedDictionary
    public class ProductDictionary : IOrderedDictionary
    {
        private ArrayList keys = new ArrayList();
        private ArrayList values = new ArrayList();

        public object this[int index]
        {
            get { return values[index]; }
            set { values[index] = value; }
        }

        public object this[object key]
        {
            get { return values[keys.IndexOf(key)]; }
            set { values[keys.IndexOf(key)] = value; }
        }

        public ICollection Keys { get { return keys; } }
        public ICollection Values { get { return values; } }
        public int Count { get { return keys.Count; } }
        public bool IsReadOnly { get { return false; } }
        public bool IsFixedSize { get { return false; } }
        public object SyncRoot { get { return this; } }
        public bool IsSynchronized { get { return false; } }

        public void Add(object key, object value)
        {
            keys.Add(key);
            values.Add(value);
        }

        public void Insert(int index, object key, object value)
        {
            keys.Insert(index, key);
            values.Insert(index, value);
        }

        public void Remove(object key)
        {
            int index = keys.IndexOf(key);
            if (index >= 0)
            {
                keys.RemoveAt(index);
                values.RemoveAt(index);
            }
        }

        public void RemoveAt(int index)
        {
            keys.RemoveAt(index);
            values.RemoveAt(index);
        }

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        public bool Contains(object key)
        {
            return keys.Contains(key);
        }

        // ОБЯЗАТЕЛЬНО для ICollection
        public void CopyTo(Array array, int index)
        {
            for (int i = 0; i < values.Count; i++)
            {
                array.SetValue(values[i], index + i);
            }
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return new ProductEnumerator(keys, values);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Перечислитель
        private class ProductEnumerator : IDictionaryEnumerator
        {
            private ArrayList keys;
            private ArrayList values;
            private int index = -1;

            public ProductEnumerator(ArrayList k, ArrayList v)
            {
                keys = k;
                values = v;
            }

            public DictionaryEntry Entry
            {
                get { return new DictionaryEntry(keys[index], values[index]); }
            }

            public object Key { get { return keys[index]; } }
            public object Value { get { return values[index]; } }
            public object Current { get { return Entry; } }

            public bool MoveNext()
            {
                index++;
                return index < keys.Count;
            }

            public void Reset()
            {
                index = -1;
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Лабораторная работа 9===\n");

            ProductDictionary dict = new ProductDictionary();

            dict.Add("A", 10);
            dict.Add("B", 20);
            dict.Add("C", 30);
            dict.Add("D", 40);
            dict.Add("E", 50);

            Console.WriteLine("Исходная коллекция:");
            PrintDictionary(dict);

            int n = 2;
            Console.WriteLine("\nУдаляем " + n + " элемента начиная с индекса 1...");

            for (int i = 0; i < n; i++)
                dict.RemoveAt(1);

            Console.WriteLine("После удаления:");
            PrintDictionary(dict);

            dict.Add("X", 99);
            dict.Insert(0, "Y", 77);

            Console.WriteLine("\nПосле добавления:");
            PrintDictionary(dict);

            ConcurrentBag<int> bag = new ConcurrentBag<int>();

            Console.WriteLine("\nПереносим данные во вторую коллекцию...");

            foreach (DictionaryEntry entry in dict)
                bag.Add((int)entry.Value);

            Console.WriteLine("\nConcurrentBag<int>:");
            foreach (int item in bag)
                Console.Write(item + " ");

            int searchValue = 30;
            Console.WriteLine("\n\nИщем значение " + searchValue + "...");

            bool found = bag.Contains(searchValue);
            Console.WriteLine(found ? "Найдено!" : "Не найдено.");

            Console.WriteLine("\n=== ObservableCollection<Product> ===");

            ObservableCollection<Product> observable = new ObservableCollection<Product>();
            observable.CollectionChanged += OnCollectionChanged;

            observable.Add(new Product("Телефон", 15000));
            observable.Add(new Product("Ноутбук", 50000));
            observable.RemoveAt(0);

            Console.ReadLine();
        }

        static void PrintDictionary(IOrderedDictionary dict)
        {
            foreach (DictionaryEntry entry in dict)
                Console.WriteLine(entry.Key + " : " + entry.Value);
        }

        static void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("Событие: " + e.Action);

            if (e.NewItems != null)
                foreach (object item in e.NewItems)
                    Console.WriteLine("Добавлено: " + item);

            if (e.OldItems != null)
                foreach (object item in e.OldItems)
                    Console.WriteLine("Удалено: " + item);
        }
    }
}
