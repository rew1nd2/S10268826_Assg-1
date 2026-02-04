//==========================================================
// Student Number : S10268826F
// Student Name : Cyrus Tan
// Partner Name : Kiefer Wang
//==========================================================

//Basic feature 1
using S10268826_Assg_1;
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
        string[] lines = File.ReadAllLines("C:\\Users\\space\\OneDrive\\Documents\\Year 1 Sem (2)\\Programming II\\S10268826_Assg 1\\S10268826_Assg 1\\restaurants.csv");
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
        string[] lines = File.ReadAllLines("C:\\Users\\space\\OneDrive\\Documents\\Year 1 Sem (2)\\Programming II\\S10268826_Assg 1\\S10268826_Assg 1\\fooditems - Copy.csv");
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
}
//Basic feature 2
//Basic feature 3
//Basic feature 4
//Basic feature 5
//Basic feature 6
//Basic feature 7
//Basic feature 8