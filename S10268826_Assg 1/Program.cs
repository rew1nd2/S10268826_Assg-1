using System;
using System.IO;
using System.Collections.Generic;
using S10268826_Assg_1;
//==========================================================
// Student Number : S10268826F
// Student Name : Cyrus Tan
// Partner Name : Kiefer Wang
//==========================================================
//Basic feature 1

class Program
{
    static List<Restaurant> restaurants = new List<Restaurant>();
    static List<Customer> customers = new List<Customer>();
    static Stack<Order> refundStack = new Stack<Order>();

    static void Main(string[] args)
    {
        Console.WriteLine("BaseDirectory: " + AppContext.BaseDirectory);
        Console.WriteLine("CurrentDirectory: " + Directory.GetCurrentDirectory());
        Console.WriteLine("restaurants exists? " + File.Exists(Path.Combine(AppContext.BaseDirectory, "restaurants.csv")));
        Console.WriteLine("fooditems exists? " + File.Exists(Path.Combine(AppContext.BaseDirectory, "fooditems - Copy.csv")));
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
                    LoadOrders();
                    // Feature 2
                    break;
                case "3":
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

    static void LoadOrders()
    {
        string[] lines = File.ReadAllLines("orders - Copy.csv");
        int count = 0;

        for (int i = 1; i < lines.Length; i++) // skip header
        {
            string[] data = lines[i].Split(',');

            // ASSUMED CSV FORMAT (BASIC):
            // OrderID,CustomerEmail,RestaurantID,DeliveryDateTime,DeliveryAddress,TotalAmount,Status

            if (data.Length >= 7)
            {
                int orderId = int.Parse(data[0].Trim());
                string customerEmail = data[1].Trim();
                string restaurantId = data[2].Trim();
                DateTime deliveryDT = DateTime.Parse(data[3].Trim());
                string address = data[4].Trim();
                double total = double.Parse(data[5].Trim());
                string status = data[6].Trim();

                Customer cust = FindCustomer(customerEmail);
                Restaurant rest = FindRestaurant(restaurantId);

                if (cust != null && rest != null)
                {
                    // Order constructor you ACTUALLY HAVE
                    Order o = new Order(orderId);

                    // set properties (basic)
                    o.DeliveryDateTime = deliveryDT;
                    o.DeliveryAddress = address;
                    o.OrderTotal = total;
                    o.OrderStatus = status;

                    // add to customer + restaurant
                    cust.AddOrder(o);
                    rest.EnqueueOrder(o);   

                    count++;
                }
            }
        }

        Console.WriteLine($"{count} orders loaded!");
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
}







    //Basic feature 3
    //Basic feature 4
    //Basic feature 5
    //Basic feature 6
    //Basic feature 7
    //Basic feature 8
