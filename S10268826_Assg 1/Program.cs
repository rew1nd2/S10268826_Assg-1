using System;
using System.IO;
using System.Collections.Generic;
using S10268826_Assg_1;
//==========================================================
// Student Number : S10268826F (Cyrus Tan)
// Student Number : S10268570D (Kiefer Wang)
// Student Name : Cyrus Tan
// Partner Name : Kiefer Wang
//==========================================================

class Program
{
    static List<Restaurant> restaurants = new List<Restaurant>();
    static List<Customer> customers = new List<Customer>();
    static Stack<Order> refundStack = new Stack<Order>();

    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
        LoadRestaurants();
        LoadFoodItems();
        LoadCustomers();
        LoadOrders();
        bool exit = false;
        while (!exit)
        {
            DisplayMenu();
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ListAllRestaurantsAndMenuItems();
                    break;
                case "2":
                    ListAllOrders();
                    // Feature 2
                    break;
                case "3":
                    CreateNewOrder();
                    //Feature 3
                    break;
                case "4":
                    // Feature 4
                    break;
                case "5":
                    // Feature 5
                    break;
                case "6":
                    // Feature 6
                    break;
                case "0":
                    exit = true;
                    Console.WriteLine("\nThank you for using Gruberoo!");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    static void DisplayMenu()
    {
        Console.WriteLine("\n===== Gruberoo Food Delivery System =====");
        Console.WriteLine("1. List all restaurants and menu items");
        Console.WriteLine("2. List all orders");
        Console.WriteLine("3. Create a new order");
        Console.WriteLine("4. Process an order");
        Console.WriteLine("5. Modify an existing order");
        Console.WriteLine("6. Delete an existing order");
        Console.WriteLine("0. Exit");
        Console.Write("Enter your choice: ");
    }
    static void LoadRestaurants()
    {
        string[] lines = File.ReadAllLines("restaurants.csv");
        int count = 0;

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            string[] data = lines[i].Split(',');
            if (data.Length >= 3)
            {
                Restaurant r = new Restaurant(data[0].Trim(), data[1].Trim(), data[2].Trim());
                restaurants.Add(r);
                count++;
            }
        }
        Console.WriteLine($"{count} restaurants loaded!");
    }

    static void LoadFoodItems()
    {
        string[] lines = File.ReadAllLines("fooditems - Copy.csv");
        int count = 0;

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            string[] data = lines[i].Split(',');
            if (data.Length >= 4)
            {
                string restaurantId = data[0].Trim();
                string itemName = data[1].Trim();
                string description = data[2].Trim();
                double price = double.Parse(data[3].Trim());

                // Find restaurant and add food item
                Restaurant restaurant = FindRestaurant(restaurantId);
                if (restaurant != null)
                {
                    FoodItem item = new FoodItem(itemName, description, price);
                    restaurant.AddFoodItem(item);
                    count++;
                }
            }
        }
        Console.WriteLine($"{count} food items loaded!");
    }
    static void ListAllRestaurantsAndMenuItems()
    {
        Console.WriteLine("\nAll Restaurants and Menu Items");
        Console.WriteLine("==============================");

        foreach (Restaurant restaurant in restaurants)
        {
            restaurant.DisplayMenu();
        }
    }
    static Restaurant FindRestaurant(string restaurantId)
    {
        foreach (Restaurant r in restaurants)
        {
            if (r.RestaurantId == restaurantId)
                return r;
        }
        return null;
    }

    //Basic feature 2
    static void LoadCustomers()
    {
        string[] lines = File.ReadAllLines("customers.csv");
        int count = 0;

        for (int i = 1; i < lines.Length; i++) // skip header
        {
            string[] data = lines[i].Split(',');

            if (data.Length >= 2)
            {
                string name = data[0].Trim();
                string email = data[1].Trim();

                if (FindCustomer(email) == null)
                {
                    Customer c = new Customer(name, email);
                    customers.Add(c);
                    count++;
                }
            }
        }

        Console.WriteLine($"{count} customers loaded!");
    }
    static Customer FindCustomer(string email)
    {
        foreach (Customer c in customers)
        {
            if (c.EmailAddress == email)
                return c;
        }
        return null;
    }

    static void LoadOrders()
    {
        string[] lines = File.ReadAllLines("orders - Copy.csv");
        int count = 0;

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            string line = lines[i];
            string[] data = line.Split(',');

            if (data.Length >= 9)
            {
                int orderId = int.Parse(data[0]);
                string customerEmail = data[1];
                string restaurantId = data[2];
                string deliveryDate = data[3];
                string deliveryTime = data[4];
                string deliveryAddress = data[5];
                double totalAmount = double.Parse(data[7]);
                string status = data[8];

                // Find the customer and restaurant
                Customer customer = FindCustomer(customerEmail);
                Restaurant restaurant = FindRestaurant(restaurantId);

                if (customer != null && restaurant != null)
                {
                    // Create order
                    Order order = new Order(orderId);

                    // Parse the delivery date/time
                    string[] dateParts = deliveryDate.Split('/');
                    string[] timeParts = deliveryTime.Split(':');
                    int day = int.Parse(dateParts[0]);
                    int month = int.Parse(dateParts[1]);
                    int year = int.Parse(dateParts[2]);
                    int hour = int.Parse(timeParts[0]);
                    int minute = int.Parse(timeParts[1]);

                    order.DeliveryDateTime = new DateTime(year, month, day, hour, minute, 0);
                    order.DeliveryAddress = deliveryAddress;
                    order.OrderTotal = totalAmount;
                    order.OrderStatus = status;

                    // Add order to customer and restaurant
                    customer.AddOrder(order);
                    restaurant.EnqueueOrder(order);
                    count++;
                }
            }
        }

        Console.WriteLine($"{count} orders loaded!");
    }
    static void ListAllOrders()
    {
        Console.WriteLine("\nAll Orders");
        Console.WriteLine("==========");
        Console.WriteLine($"{"Order ID",-10} {"Customer",-15} {"Restaurant",-20} {"Delivery Date/Time",-20} {"Amount",-10} {"Status",-12}");
        Console.WriteLine("-----------------------------------------------------------------------------------------------");

        // Get all orders from all customers
        foreach (Customer customer in customers)
        {
            foreach (Order order in customer.GetOrders())
            {
                // Find which restaurant this order belongs to
                Restaurant rest = null;
                foreach (Restaurant r in restaurants)
                {
                    foreach (Order o in r.GetOrderQueue())
                    {
                        if (o.OrderId == order.OrderId)
                        {
                            rest = r;
                            break;
                        }
                    }
                    if (rest != null) break;
                }

                string restaurantName = rest != null ? rest.RestaurantName : "Unknown";

                Console.WriteLine($"{order.OrderId,-10} {customer.CustomerName,-15} {restaurantName,-20} {order.DeliveryDateTime:dd/MM/yyyy HH:mm} ${order.OrderTotal,-9:F2} {order.OrderStatus,-12}");
            }
        }
    }
    static void CreateNewOrder()
    {
        Console.WriteLine("\nCreate New Order");
        Console.WriteLine("================");

        // Customer
        Customer customer = null;
        while (customer == null)
        {
            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine();
            customer = FindCustomer(email);

            if (customer == null)
                Console.WriteLine("Customer not found.");
        }

        // Restaurant
        Restaurant restaurant = null;
        while (restaurant == null)
        {
            Console.Write("Enter Restaurant ID: ");
            string rid = Console.ReadLine();
            restaurant = FindRestaurant(rid);

            if (restaurant == null)
                Console.WriteLine("Restaurant not found.");
        }

        // Date & Time (BASIC)
        DateTime deliveryDateTime;
        while (true)
        {
            try
            {
                Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
                string date = Console.ReadLine();

                Console.Write("Enter Delivery Time (hh:mm): ");
                string time = Console.ReadLine();

                deliveryDateTime = DateTime.Parse(date + " " + time);
                break;
            }
            catch
            {
                Console.WriteLine("Invalid date/time. Example: 15/02/2026 12:30");
            }
        }

        // Address
        Console.Write("Enter Delivery Address: ");
        string address = Console.ReadLine();

        // Show food items
        List<FoodItem> foodList = restaurant.GetMenus();


        Console.WriteLine("\nAvailable Food Items:");
        for (int i = 0; i < foodList.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {foodList[i].ItemName} - ${foodList[i].ItemPrice:F2}");
        }

        // Select items
        List<FoodItem> selectedItems = new List<FoodItem>();
        List<int> quantities = new List<int>();

        while (true)
        {
            Console.Write("Enter item number (0 to finish): ");
            int choice = int.Parse(Console.ReadLine());

            if (choice == 0)
                break;

            Console.Write("Enter quantity: ");
            int qty = int.Parse(Console.ReadLine());

            selectedItems.Add(foodList[choice - 1]);
            quantities.Add(qty);
        }

        // Special request
        string specialRequest = "";
        Console.Write("Add special request? [Y/N]: ");
        if (Console.ReadLine().ToUpper() == "Y")
        {
            Console.Write("Enter special request: ");
            specialRequest = Console.ReadLine();
        }

        // Calculate total
        double total = 0;
        for (int i = 0; i < selectedItems.Count; i++)
        {
            total += selectedItems[i].ItemPrice * quantities[i];
        }

        total += 5.0; // delivery fee

        Console.WriteLine($"Order Total (incl delivery): ${total:F2}");

        // Payment
        Console.Write("Proceed to payment? [Y/N]: ");
        if (Console.ReadLine().ToUpper() != "Y")
            return;

        Console.Write("[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
        string payment = Console.ReadLine().ToUpper();



        // Create order
        int newId = GetNextOrderId();
        Order order = new Order(newId);
        order.DeliveryDateTime = deliveryDateTime;
        order.DeliveryAddress = address;
        order.OrderTotal = total;
        order.OrderStatus = "Pending";

        customer.AddOrder(order);
        restaurant.EnqueueOrder(order);

        static int GetNextOrderId()
        {
            int maxId = 0;

            foreach (Customer c in customers)
            {
                foreach (Order o in c.GetOrders())
                {
                    if (o.OrderId > maxId)
                        maxId = o.OrderId;
                }
            }

            return maxId + 1;
        }


        // Append to CSV (simple)
        string line =
            $"{order.OrderId},{customer.EmailAddress},{restaurant.RestaurantId}," +
            $"{deliveryDateTime:dd/MM/yyyy},{deliveryDateTime:HH:mm}," +
            $"{address},{DateTime.Now},{order.OrderTotal},{order.OrderStatus}";

        File.AppendAllText("orders - Copy.csv", line + "\n");

        Console.WriteLine($"Order {order.OrderId} created successfully! Status: Pending");
    }


}

//Basic feature 3
//Basic feature 4
//Basic feature 5
//Basic feature 6
//Basic feature 7
//Basic feature 8
