using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PizzaOrderingSystem
{
    class Program
    {
        static List<Pizza> pizzas = new List<Pizza>();
        static List<Order> orders = new List<Order>();

        static void Main(string[] args)
        {
            InitializePizzas();

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("Witaj w systemie zamówień pizz!\n");

                List<string> startingOptions = new List<string> { "Dodaj nowe zamówienie", "Wyświetl zamówienia", "Edytuj zamówienie", "Usuń zamówienie", "Zapisz zamówienia do pliku", "Wyjdź" };

                for (int i = 0; i < startingOptions.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {startingOptions[i]}");
                }
                Console.Write("\nWybierz opcję: ");

                int choice;
                while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > startingOptions.Count)
                {
                    Console.Write("Wprowadzono nieprawidłowy numer. Wybierz opcję: ");
                }

                switch (choice)
                {
                    case 1:
                        AddOrder();
                        break;
                    case 2:
                        DisplayOrders();
                        break;
                    case 3:
                        EditOrder();
                        break;
                    case 4:
                        DeleteOrder();
                        break;
                    case 5:
                        SaveOrdersToFile();
                        break;
                    case 6:
                        running = false;
                        break;
                }
            }
        }

        static void InitializePizzas()
        {
            pizzas.Add(new Pizza("Margherita", 20, 25, 30));
            pizzas.Add(new Pizza("Pepperoni", 22, 27, 32));
            pizzas.Add(new Pizza("Hawajska", 24, 29, 34));
            pizzas.Add(new Pizza("Vegetariańska", 21, 26, 31));
        }

        static void AddOrder()
        {
            Console.Clear();
            Console.WriteLine("Dodawanie nowego zamówienia...\n");

            string customerName;
            do
            {
                Console.Write("Imię klienta: ");
                customerName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(customerName) || customerName.Any(c => Char.IsDigit(c)))
                {
                    Console.WriteLine("Imię nie może zawierać cyfr ani być puste. Spróbuj ponownie.");
                }
            } while (string.IsNullOrWhiteSpace(customerName) || customerName.Any(c => Char.IsDigit(c)));

            Console.Clear();

            int id = orders.Count > 0 ? orders.Max(o => o.Id) + 1 : 1;
            bool addingPizza = true;
            List<OrderItem> orderItems = new List<OrderItem>();

            while (addingPizza)
            {
                int pizzaChoice;
                Console.WriteLine("MENU\n");
                string format = "{0,-20} {1,10} {2,10} {3,10}";
                Console.WriteLine(format,
                    "Rodzaj pizzy",
                    "Mała",
                    "Średnia",
                    "Duża");

                for (int i = 0; i < pizzas.Count; i++)
                {
                    var currentPizza = pizzas[i];

                    Console.WriteLine(format,
                        $"{i + 1}. {currentPizza.Name}",
                        currentPizza.SmallPrice.ToString("C"),
                        currentPizza.MediumPrice.ToString("C"),
                        currentPizza.LargePrice.ToString("C"));
                }
                Console.Write("\nPodaj numer: ");
                while (!int.TryParse(Console.ReadLine(), out pizzaChoice) || pizzaChoice < 1 || pizzaChoice > pizzas.Count)
                {
                    Console.Write("Wprowadzono nieprawidłowy numer pizzy.\n");
                    Console.Write("\nPodaj numer: ");
                }

                Pizza pizza = pizzas[pizzaChoice - 1];

                Console.Clear();
                Console.WriteLine($"Wybrana pizza - {pizza.Name}\n");

                List<string> pizzaSizes = new List<string> { "Mała", "Średnia", "Duża" };
                int sizeChoice;
                Console.WriteLine("Dostępne rozmiary:");
                for (int i = 0; i < pizzaSizes.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {pizzaSizes[i]}");
                }
                Console.Write("\nWybierz rozmiar pizzy: ");

                while (!int.TryParse(Console.ReadLine(), out sizeChoice) || sizeChoice < 1 || sizeChoice > pizzaSizes.Count)
                {
                    Console.Write("Wprowadzono nieprawidłowy numer.\n");
                    Console.Write("Wybierz rozmiar pizzy: ");
                }

                Console.Clear();

                string pizzaSize = pizzaSizes[sizeChoice - 1];
                decimal price = pizza.GetPrice(pizzaSize);

                Console.WriteLine($"Wybrana pizza - {pizza.Name} ({pizzaSize})\n\nKoszt za sztukę: {price:C}\n");

                int quantity;
                Console.Write("Ilość: ");
                while (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0)
                {
                    Console.Write("Wprowadzono nieprawidłowy numer.\n");
                    Console.Write("Ilość: ");
                }

                orderItems.Add(new OrderItem
                {
                    PizzaType = pizza.Name,
                    Size = pizzaSize,
                    Quantity = quantity,
                    Price = price,
                    TotalPrice = price * quantity
                });
                Console.Clear();
                Console.WriteLine($"Wybrana pizza - {pizza.Name} ({pizzaSize}) x {quantity}\n");
                Console.WriteLine("Pozycja została dodana do zamówienia!");
                Console.WriteLine("\nCo chcesz dalej zrobić?");
                Console.WriteLine("1. Zapisz zamówienie");
                Console.WriteLine("2. Dodaj kolejną pizzę");
                int nextAction;
                while (!int.TryParse(Console.ReadLine(), out nextAction) || (nextAction != 1 && nextAction != 2))
                {
                    Console.WriteLine("Nieprawidłowy wybór. Wybierz 1 lub 2.");
                }

                if (nextAction == 1)
                {
                    orders.Add(new Order
                    {
                        Id = id,
                        CustomerName = customerName,
                        OrderItems = orderItems,
                        TotalPrice = orderItems.Sum(o => o.TotalPrice)
                    });
                    Console.Clear();
                    Console.WriteLine("Zamówienie zostało zapisane!");
                    addingPizza = false;
                }
                Console.Clear();
            }
        }

        static void DisplayOrders()
        {
            Console.Clear();
            if (orders.Count == 0)
            {
                Console.WriteLine("Brak zamówień.");
            }
            else
            {
                foreach (var order in orders)
                {
                    Console.WriteLine(order);
                    Console.WriteLine("----------\n");
                }
            }
            Console.WriteLine("\nNaciśnij dowolny klawisz, aby wrócić do menu...");
            Console.ReadKey();
        }

        static void EditOrder()
        {
            Console.Clear();
            Console.WriteLine("Edytuj zamówienie...\n");

            if (orders.Count == 0)
            {
                Console.WriteLine("Brak zamówień do edytowania.");
                Console.WriteLine("\nNaciśnij dowolny klawisz, aby wrócić do menu...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Wybierz zamówienie do edytowania:");
            foreach (var order in orders)
            {
                Console.WriteLine($"ID: {order.Id} - {order.CustomerName}");
            }

            int orderId;
            Console.Write("\nPodaj ID zamówienia do edytowania (lub wpisz 0, by wyjść): ");
            while (!int.TryParse(Console.ReadLine(), out orderId) || orders.All(o => o.Id != orderId))
            {
                if (orderId == 0)
                {
                    Console.WriteLine("Anulowano edycję.");
                    return;
                }
                Console.Write("Nieprawidłowe ID. Podaj ponownie: ");
            }

            Order selectedOrder = orders.First(o => o.Id == orderId);

            Console.Clear();
            Console.WriteLine($"Edytowanie zamówienia ID: {selectedOrder.Id}, Klient: {selectedOrder.CustomerName}\n");

            for (int i = 0; i < selectedOrder.OrderItems.Count; i++)
            {
                var item = selectedOrder.OrderItems[i];
                Console.WriteLine($"{i + 1}. {item.PizzaType} ({item.Size}) x {item.Quantity}\n\n Cena jednostkowa: {item.Price:C}\n Całkowita cena: {item.TotalPrice:C}\n");
            }

            Console.WriteLine("Wybierz pozycję, którą chcesz edytować (lub wpisz 0, aby wyjść):");
            int itemChoice;
            while (!int.TryParse(Console.ReadLine(), out itemChoice) || (itemChoice < 0 || itemChoice > selectedOrder.OrderItems.Count))
            {
                Console.Write("Nieprawidłowy numer. Wybierz ponownie: ");
            }

            if (itemChoice == 0)
            {
                Console.WriteLine("Anulowano edycję.");
                return;
            }

            UpdateOrderItem(selectedOrder, itemChoice - 1);

            Console.WriteLine("\nZamówienie zostało zaktualizowane!");
            Console.WriteLine("\nNaciśnij dowolny klawisz, aby wrócić do menu...");
            Console.ReadKey();
        }

        static void UpdateOrderItem(Order order, int itemIndex)
        {
            var item = order.OrderItems[itemIndex];

            Console.Clear();
            Console.WriteLine($"Edycja pozycji: {item.PizzaType} ({item.Size}) x {item.Quantity}\n\n Cena jednostkowa: {item.Price:C}\n Cena łączna: {item.Price * item.Quantity:C}\n");

            int pizzaChoice;

            Console.WriteLine("Wybierz nową pizzę:\n");
            string format = "{0,-20} {1,10} {2,10} {3,10}";
            Console.WriteLine(format,
                "Rodzaj pizzy",
                "Mała",
                "Średnia",
                "Duża");

            for (int i = 0; i < pizzas.Count; i++)
            {
                var currentPizza = pizzas[i];

                Console.WriteLine(format,
                    $"{i + 1}. {currentPizza.Name}",
                    currentPizza.SmallPrice.ToString("C"),
                    currentPizza.MediumPrice.ToString("C"),
                    currentPizza.LargePrice.ToString("C"));
            }
            Console.Write("\nPodaj numer: ");

            while (!int.TryParse(Console.ReadLine(), out pizzaChoice) || (pizzaChoice < 0 || pizzaChoice > pizzas.Count))
            {
                Console.Write("Nieprawidłowy numer. Wybierz ponownie: ");
            }

            if (pizzaChoice == 0)
            {
                Console.WriteLine("Anulowano edycję pozycji.");
                return;
            }
            Pizza newPizza = pizzas[pizzaChoice - 1];

            Console.Clear();
            Console.WriteLine($"Edycja pozycji: {item.PizzaType} ({item.Size}) x {item.Quantity}\n\n Cena jednostkowa: {item.Price:C}\n Cena łączna: {item.Price * item.Quantity:C}\n");
            Console.WriteLine($"Wybrana pizza: {newPizza.Name}\n");


            List<string> pizzaSizes = new List<string> { "Mała", "Średnia", "Duża" };
            int sizeChoice;
            Console.WriteLine("Dostępne rozmiary:");
            for (int i = 0; i < pizzaSizes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {pizzaSizes[i]}");
            }
            Console.Write("\nWybierz rozmiar pizzy: ");
            while (!int.TryParse(Console.ReadLine(), out sizeChoice) || sizeChoice < 0 || sizeChoice > pizzaSizes.Count)
            {
                Console.Write("Nieprawidłowy wybór. Wybierz ponownie: ");
            }

            if (sizeChoice == 0)
            {
                Console.WriteLine("Anulowano edycję pozycji.");
                return;
            }

            string newSize = pizzaSizes[sizeChoice - 1];
            decimal newPrice = newPizza.GetPrice(newSize);

            Console.Clear();
            Console.WriteLine($"Edycja pozycji: {item.PizzaType} ({item.Size}) x {item.Quantity}\n\n Cena jednostkowa: {item.Price:C}\n Cena łączna: {item.Price * item.Quantity:C}\n");
            Console.WriteLine($"Wybrana pizza: {newPizza.Name} ({newSize})\n\n Cena jednostkowa: {newPrice:C}\n");

            int newQuantity;
            Console.Write("Podaj nową ilość pizzy: ");
            while (!int.TryParse(Console.ReadLine(), out newQuantity) || newQuantity <= 0)
            {
                Console.Write("Nieprawidłowa ilość. Wybierz ponownie: ");
            }

            item.PizzaType = newPizza.Name;
            item.Size = newSize;
            item.Price = newPrice;
            item.Quantity = newQuantity;
            item.TotalPrice = newPrice * newQuantity;

            order.TotalPrice = order.OrderItems.Sum(o => o.TotalPrice);
        }

        static void DeleteOrder()
        {
            Console.Clear();
            Console.WriteLine("Usuń zamówienie...\n");

            if (orders.Count == 0)
            {
                Console.WriteLine("Brak zamówień do usunięcia.");
                Console.WriteLine("\nNaciśnij dowolny klawisz, aby wrócić do menu...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Wybierz zamówienie do usunięcia (lub wpisz 0, aby wyjść):");
            foreach (var order in orders)
            {
                Console.WriteLine($"ID: {order.Id} - {order.CustomerName}");
            }

            int orderId;
            Console.Write("\nPodaj ID zamówienia do usunięcia: ");
            while (!int.TryParse(Console.ReadLine(), out orderId) || orders.All(o => o.Id != orderId))
            {
                Console.Write("Nieprawidłowe ID. Podaj ponownie: ");
            }

            if (orderId == 0)
            {
                Console.WriteLine("Anulowano usuwanie.");
                return;
            }

            orders.RemoveAll(o => o.Id == orderId);

            Console.WriteLine("Zamówienie zostało usunięte!");
            Console.WriteLine("\nNaciśnij dowolny klawisz, aby wrócić do menu...");
            Console.ReadKey();
        }

        static void SaveOrdersToFile()
        {
            Console.Clear();
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string directoryPath = Path.Combine(projectDirectory, "OrdersData");

            directoryPath = Path.GetFullPath(directoryPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string fileName = $"orders_{DateTime.Now.ToString("dd-MM-yyyy_HH-mm")}.txt";
            string filePath = Path.Combine(directoryPath, fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var order in orders)
                {
                    writer.WriteLine(order);
                    writer.WriteLine("----------\n");
                }
            }

            Console.WriteLine($"Zamówienia zostały zapisane do pliku: {filePath}");
            Console.WriteLine("\nNaciśnij dowolny przycisk, aby wrócić do menu...");
            Console.ReadLine();
        }
    }

    class Pizza
    {
        public string Name { get; set; }
        public decimal SmallPrice { get; set; }
        public decimal MediumPrice { get; set; }
        public decimal LargePrice { get; set; }

        public Pizza(string name, decimal smallPrice, decimal mediumPrice, decimal largePrice)
        {
            Name = name;
            SmallPrice = smallPrice;
            MediumPrice = mediumPrice;
            LargePrice = largePrice;
        }

        public decimal GetPrice(string size)
        {
            return size switch
            {
                "Mała" => SmallPrice,
                "Średnia" => MediumPrice,
                "Duża" => LargePrice,
                _ => 0m,
            };
        }
    }

    class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public decimal TotalPrice { get; set; }

        public override string ToString()
        {
            string orderDetails = $"ID: {Id}\nKlient: {CustomerName}\nPizze: \n";
            foreach (var item in OrderItems)
            {
                orderDetails += $" - {item.PizzaType} ({item.Size}), Ilość: {item.Quantity}, Cena jednostkowa: {item.Price:C}, Całkowita cena: {item.TotalPrice:C}\n";
            }
            orderDetails += $"\nCałkowita wartość zamówienia: {TotalPrice:C}\n";
            return orderDetails;
        }
    }

    class OrderItem
    {
        public string PizzaType { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
