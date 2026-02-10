using System;
using System.IO;
using System.Collections.Generic;
using S10268826_Assg_1;
//==========================================================
// Student Number : S10268570D (Kiefer Wang)
// Student Number : S10268826F (Cyrus Tan)
// Partner Name : Kiefer Wang
// Student Name : Cyrus Tan
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
        LoadSpecialOffers();
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
                    ProcessOrder();
                    break;
                case "5":
                    ModifyExistingOrder();
                    // Feature 5
                    break;
                case "6":
                    DeleteOrder();
                    break;
                // Feature 6
                case "7":
                    BulkProcessPendingOrders(); // Advanced
                    break;
                case "8":
                    DisplayTotalOrderAmount();  // Advanced
                    break;
                case "9":
                    FavouriteOrdersMenu();     // Additional
                    break;
                case "10":
                    CreateOrderWithSpecialOffer(); // Additional
                    break;

                case "0":
                    SaveQueueAndStack();
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
        Console.WriteLine("7. Bulk process pending orders");
        Console.WriteLine("8. Display total order amount");
        Console.WriteLine("9. Favourite orders");
        Console.WriteLine("10. Create order with special offer");
        Console.WriteLine("0. Exit");
        Console.Write("Enter your choice: ");
    }

    //==========================================================
    // Feature 1: List restaurants & menu
    //==========================================================
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

    //==========================================================
    // Feature 2: List Orders
    //==========================================================
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

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];

            // Split carefully - items field has commas inside quotes
            string[] data = SplitCSVLine(line);

            if (data.Length >= 10)
            {
                int orderId = int.Parse(data[0]);
                string customerEmail = data[1];
                string restaurantId = data[2];
                string deliveryDate = data[3];
                string deliveryTime = data[4];
                string deliveryAddress = data[5];
                double totalAmount = double.Parse(data[7]);
                string status = data[8];
                string itemsStr = data[9]; 

                Customer customer = FindCustomer(customerEmail);
                Restaurant restaurant = FindRestaurant(restaurantId);

                if (customer != null && restaurant != null)
                {
                    Order order = new Order(orderId);

                    // Parse delivery date/time
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

                    ParseAndAddOrderItems(order, restaurant, itemsStr);

                    customer.AddOrder(order);
                    restaurant.EnqueueOrder(order);
                    count++;
                }
            }
        }

        Console.WriteLine($"{count} orders loaded!");
    }

    // Helper to split CSV line (handles quotes)
    static string[] SplitCSVLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string current = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.Trim());
                current = "";
            }
            else
            {
                current += c;
            }
        }
        result.Add(current.Trim());

        return result.ToArray();
    }

    // Helper to parse items and add to order
    static void ParseAndAddOrderItems(Order order, Restaurant restaurant, string itemsStr)
    {
        // Remove quotes
        itemsStr = itemsStr.Trim('"');

        if (string.IsNullOrEmpty(itemsStr))
            return;

        // Split by | to get each item
        string[] items = itemsStr.Split('|');

        foreach (string item in items)
        {
            // Split by comma to get name and quantity
            string[] parts = item.Split(',');

            if (parts.Length >= 2)
            {
                string itemName = parts[0].Trim();
                int quantity = int.Parse(parts[1].Trim());

                // Find the food item in restaurant's menu
                FoodItem foodItem = null;
                foreach (FoodItem f in restaurant.GetMenus())
                {
                    if (f.ItemName == itemName)
                    {
                        foodItem = f;
                        break;
                    }
                }

                if (foodItem != null)
                {
                    OrderedFoodItem orderedItem = new OrderedFoodItem(foodItem, quantity);
                    order.AddOrderedFoodItem(orderedItem);
                }
            }
        }
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
    //==========================================================
    // Feature 3: Create New Order
    //==========================================================
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

        // Date & Time
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

        // Append to CSV (simple)
        string line =
            $"{order.OrderId},{customer.EmailAddress},{restaurant.RestaurantId}," +
            $"{deliveryDateTime:dd/MM/yyyy},{deliveryDateTime:HH:mm}," +
            $"{address},{DateTime.Now},{order.OrderTotal},{order.OrderStatus}";

        File.AppendAllText("orders - Copy.csv", line + "\n");

        Console.WriteLine($"Order {order.OrderId} created successfully! Status: Pending");
    }

    //==========================================================
    // Feature 4: Process Order
    //==========================================================
    static void ProcessOrder()
    {
        Console.WriteLine("\nProcess Order");
        Console.WriteLine("=============");

        // Get restaurant ID
        Restaurant restaurant = null;
        while (restaurant == null)
        {
            Console.Write("Enter Restaurant ID: ");
            string rid = Console.ReadLine();
            restaurant = FindRestaurant(rid);

            if (restaurant == null)
                Console.WriteLine("Restaurant not found. Please try again.");
        }

        // Get the order queue
        Queue<Order> orderQueue = restaurant.GetOrderQueue();

        if (orderQueue.Count == 0)
        {
            Console.WriteLine("No orders in queue for this restaurant.");
            return;
        }

        List<Order> ordersList = new List<Order>(orderQueue);

        foreach (Order order in ordersList)
        {
            // Display order details
            Console.WriteLine($"\nOrder {order.OrderId}:");
            Console.WriteLine($"Customer: {FindCustomerByOrder(order).CustomerName}");
            Console.WriteLine("Ordered Items:");

            int itemNum = 1;
            foreach (OrderedFoodItem item in order.GetOrderedItems())
            {
                Console.WriteLine($"{itemNum}. {item.FoodItem.ItemName} - {item.Quantity}");
                itemNum++;
            }

            Console.WriteLine($"Delivery date/time: {order.DeliveryDateTime:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Total Amount: ${order.OrderTotal:F2}");
            Console.WriteLine($"Order Status: {order.OrderStatus}");

            // Get action
            Console.Write("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
            string action = Console.ReadLine().ToUpper();

            // Process based on action
            if (action == "C")
            {
                if (order.OrderStatus == "Pending")
                {
                    order.UpdateStatus("Preparing");
                    Console.WriteLine($"Order {order.OrderId} confirmed. Status: Preparing");
                }
                else
                {
                    Console.WriteLine($"Cannot confirm. Order status is {order.OrderStatus}. Only Pending orders can be confirmed.");
                }
            }
            else if (action == "R")
            {
                if (order.OrderStatus == "Pending")
                {
                    order.UpdateStatus("Rejected");
                    refundStack.Push(order);
                    Console.WriteLine($"Order {order.OrderId} rejected. Refund of ${order.OrderTotal:F2} processed.");
                }
                else
                {
                    Console.WriteLine($"Cannot reject. Order status is {order.OrderStatus}. Only Pending orders can be rejected.");
                }
            }
            else if (action == "S")
            {
                if (order.OrderStatus == "Cancelled")
                {
                    Console.WriteLine("Order skipped.");
                }
                else
                {
                    Console.WriteLine($"Cannot skip. Order status is {order.OrderStatus}. Only Cancelled orders can be skipped.");
                }
            }
            else if (action == "D")
            {
                if (order.OrderStatus == "Preparing")
                {
                    order.UpdateStatus("Delivered");
                    Console.WriteLine($"Order {order.OrderId} delivered.");
                }
                else
                {
                    Console.WriteLine($"Cannot deliver. Order status is {order.OrderStatus}. Only Preparing orders can be delivered.");
                }
            }
            else
            {
                Console.WriteLine("Invalid action. Skipping this order.");
            }
        }

        Console.WriteLine("\nAll orders processed.");
    }

    // Helper method to find customer by order
    static Customer FindCustomerByOrder(Order order)
    {
        foreach (Customer c in customers)
        {
            foreach (Order o in c.GetOrders())
            {
                if (o.OrderId == order.OrderId)
                    return c;
            }
        }
        return null;
    }

    //==========================================================
    // Feature 5: Modify Order
    //==========================================================
    static void ModifyExistingOrder()
    {
        Console.WriteLine("\nModify Order");
        Console.WriteLine("===========");

        // prompt customer email
        Customer customer = null;
        while (customer == null)
        {
            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine().Trim();
            customer = FindCustomer(email);

            if (customer == null)
                Console.WriteLine("Customer not found.");
        }

        // display pending orders
        List<Order> pendingOrders = customer.GetPendingOrders();

        Console.WriteLine("Pending Orders:");
        if (pendingOrders.Count == 0)
        {
            Console.WriteLine("None");
            return;
        }

        foreach (Order o in pendingOrders)
            Console.WriteLine(o.OrderId);

        // enter order id
        Order selectedOrder = null;
        while (selectedOrder == null)
        {
            Console.Write("Enter Order ID: ");
            int oid;
            if (!int.TryParse(Console.ReadLine(), out oid))
            {
                Console.WriteLine("Invalid Order ID.");
                continue;
            }

            selectedOrder = customer.FindOrder(oid);

            if (selectedOrder == null)
            {
                Console.WriteLine("Order not found.");
            }
            else if (selectedOrder.OrderStatus != "Pending")
            {
                Console.WriteLine("Only Pending orders can be modified.");
                selectedOrder = null;
            }
        }

        // display order info 
        Console.WriteLine("Order Items:");
        List<OrderedFoodItem> orderedItems = selectedOrder.GetOrderedItems();
        for (int i = 0; i < orderedItems.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {orderedItems[i].FoodItem.ItemName} - {orderedItems[i].Quantity}");
        }

        Console.WriteLine("Address:");
        Console.WriteLine(selectedOrder.DeliveryAddress);

        Console.WriteLine("Delivery Date/Time:");
        Console.WriteLine($"{selectedOrder.DeliveryDateTime:dd/M/yyyy}, {selectedOrder.DeliveryDateTime:HH:mm}");
        Console.WriteLine();

        // modification options
        Console.Write("Modify: [1] Items [2] Address [3] Delivery Time: ");
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3)
        {
            Console.Write("Modify: [1] Items [2] Address [3] Delivery Time: ");
        }

        double oldTotal = selectedOrder.OrderTotal;

        if (choice == 1)
        {
            ModifyItems(selectedOrder);
            double newTotal = CalculateTotal(selectedOrder);

            if (newTotal > oldTotal)
            {
                Console.WriteLine($"Order total increased from ${oldTotal:F2} to ${newTotal:F2}");
                Console.Write("Proceed to payment? [Y/N]: ");
                if (Console.ReadLine().Trim().ToUpper() != "Y")
                {
                    Console.WriteLine("Payment not made. Item changes not confirmed.");
                    return;
                }

                Console.Write("[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
                string pm = Console.ReadLine().Trim().ToUpper();
                while (pm != "CC" && pm != "PP" && pm != "CD")
                {
                    Console.Write("[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
                    pm = Console.ReadLine().Trim().ToUpper();
                }

            }

            selectedOrder.OrderTotal = newTotal;

            Console.WriteLine($"Order {selectedOrder.OrderId} updated. New Total: ${selectedOrder.OrderTotal:F2}");
            UpdateOrdersCsvAfterModify(customer, selectedOrder);
        }
        else if (choice == 2)
        {
            Console.Write("Enter new Delivery Address: ");
            string newAddr = Console.ReadLine();
            selectedOrder.DeliveryAddress = newAddr;

            Console.WriteLine($"Order {selectedOrder.OrderId} updated. New Address: {selectedOrder.DeliveryAddress}");
            UpdateOrdersCsvAfterModify(customer, selectedOrder);
        }
        else 
        {
            DateTime newDT = selectedOrder.DeliveryDateTime;

            while (true)
            {
                Console.Write("Enter new Delivery Time (hh:mm): ");
                string timeStr = Console.ReadLine().Trim();

                try
                {
                    DateTime temp = DateTime.Parse("01/01/2000 " + timeStr);

                    newDT = new DateTime(
                        selectedOrder.DeliveryDateTime.Year,
                        selectedOrder.DeliveryDateTime.Month,
                        selectedOrder.DeliveryDateTime.Day,
                        temp.Hour,
                        temp.Minute,
                        0
                    );
                    break;
                }
                catch
                {
                    Console.WriteLine("Invalid time. Example: 14:00");
                }
            }

            selectedOrder.DeliveryDateTime = newDT;

            Console.WriteLine($"Order {selectedOrder.OrderId} updated. New Delivery Time: {selectedOrder.DeliveryDateTime:HH:mm}");
            UpdateOrdersCsvAfterModify(customer, selectedOrder);
        }
    }

    static void ModifyItems(Order order)
    {
        while (true)
        {
            Console.WriteLine("\nOrder Items:");
            List<OrderedFoodItem> items = order.GetOrderedItems();
            for (int i = 0; i < items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {items[i].FoodItem.ItemName} - {items[i].Quantity}");
            }

            Console.Write("Enter item number to change quantity (0 to finish): ");
            int num;
            if (!int.TryParse(Console.ReadLine(), out num))
            {
                Console.WriteLine("Invalid number.");
                continue;
            }

            if (num == 0) break;

            if (num < 1 || num > items.Count)
            {
                Console.WriteLine("Invalid item number.");
                continue;
            }

            Console.Write("Enter new quantity: ");
            int newQty;
            if (!int.TryParse(Console.ReadLine(), out newQty) || newQty <= 0)
            {
                Console.WriteLine("Invalid quantity.");
                continue;
            }

            items[num - 1].Quantity = newQty;
            Console.WriteLine("Quantity updated.");
        }
    }

    static double CalculateTotal(Order order)
    {
        double total = 0;
        foreach (OrderedFoodItem item in order.GetOrderedItems())
        {
            total += item.FoodItem.ItemPrice * item.Quantity;
        }
        total += 5.0; // delivery fee (same as your create)
        return total;
    }

    static Restaurant FindRestaurantByOrderId(int orderId)
    {
        foreach (Restaurant r in restaurants)
        {
            foreach (Order o in r.GetOrderQueue())
            {
                if (o.OrderId == orderId)
                    return r;
            }
        }
        return null;
    }

    // Update orders 
    static void UpdateOrdersCsvAfterModify(Customer customer, Order order)
    {
        string filePath = "orders - Copy.csv";
        if (!File.Exists(filePath)) return;

        Restaurant rest = FindRestaurantByOrderId(order.OrderId);
        if (rest == null) return;

        string[] lines = File.ReadAllLines(filePath);
        List<string> newLines = new List<string>();
        string itemsField = "";
        List<OrderedFoodItem> items = order.GetOrderedItems();
        for (int i = 0; i < items.Count; i++)
        {
            itemsField += items[i].FoodItem.ItemName + "," + items[i].Quantity;
            if (i < items.Count - 1) itemsField += "|";
        }
        itemsField = "\"" + itemsField + "\"";

        for (int i = 0; i < lines.Length; i++)
        {
            // keep header
            if (i == 0)
            {
                newLines.Add(lines[i]);
                continue;
            }

            string[] data = SplitCSVLine(lines[i]);
            if (data.Length < 10)
            {
                newLines.Add(lines[i]);
                continue;
            }

            int oid;
            if (int.TryParse(data[0], out oid) && oid == order.OrderId)
            {
                string deliveryDate = order.DeliveryDateTime.ToString("dd/MM/yyyy");
                string deliveryTime = order.DeliveryDateTime.ToString("HH:mm");

                string newLine =
                    order.OrderId + "," +
                    customer.EmailAddress + "," +
                    rest.RestaurantId + "," +
                    deliveryDate + "," +
                    deliveryTime + "," +
                    order.DeliveryAddress + "," +
                    data[6] + "," +                     
                    order.OrderTotal.ToString("F2") + "," +
                    order.OrderStatus + "," +
                    itemsField;

                newLines.Add(newLine);
            }
            else
            {
                newLines.Add(lines[i]);
            }
        }

        File.WriteAllLines(filePath, newLines.ToArray());
    }

    //==========================================================
    // FEATURE 6: Delete an existing order
    //==========================================================

    static void DeleteOrder()
    {
        Console.WriteLine("\nDelete Order");
        Console.WriteLine("============");

        // Get customer email
        Customer customer = null;
        while (customer == null)
        {
            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine();
            customer = FindCustomer(email);

            if (customer == null)
                Console.WriteLine("Customer not found. Please try again.");
        }

        // Get pending orders
        List<Order> pendingOrders = customer.GetPendingOrders();

        if (pendingOrders.Count == 0)
        {
            Console.WriteLine("No pending orders found for this customer.");
            return;
        }

        // Display pending order IDs
        Console.WriteLine("Pending Orders:");
        foreach (Order o in pendingOrders)
        {
            Console.WriteLine(o.OrderId);
        }

        // Get order ID
        Order selectedOrder = null;
        while (selectedOrder == null)
        {
            Console.Write("Enter Order ID: ");
            int orderId;

            if (!int.TryParse(Console.ReadLine(), out orderId))
            {
                Console.WriteLine("Invalid Order ID. Please enter a number.");
                continue;
            }

            selectedOrder = customer.FindOrder(orderId);

            if (selectedOrder == null)
            {
                Console.WriteLine("Order not found.");
            }
            else if (selectedOrder.OrderStatus != "Pending")
            {
                Console.WriteLine("Only pending orders can be cancelled.");
                selectedOrder = null;
            }
        }

        // Display order details
        Console.WriteLine($"\nCustomer: {customer.CustomerName}");
        Console.WriteLine("Ordered Items:");

        int itemNum = 1;
        foreach (OrderedFoodItem item in selectedOrder.GetOrderedItems())
        {
            Console.WriteLine($"{itemNum}. {item.FoodItem.ItemName} - {item.Quantity}");
            itemNum++;
        }

        Console.WriteLine($"Delivery date/time: {selectedOrder.DeliveryDateTime:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Total Amount: ${selectedOrder.OrderTotal:F2}");
        Console.WriteLine($"Order Status: {selectedOrder.OrderStatus}");

        // Confirm deletion
        Console.Write("\nConfirm deletion? [Y/N]: ");
        string confirm = Console.ReadLine().ToUpper();

        if (confirm == "Y")
        {
            // Update order status to Cancelled
            selectedOrder.UpdateStatus("Cancelled");

            // Add to refund stack
            refundStack.Push(selectedOrder);

            UpdateOrdersCsvAfterModify(customer, selectedOrder);

            // Display confirmation
            Console.WriteLine($"\nOrder {selectedOrder.OrderId} cancelled. Refund of ${selectedOrder.OrderTotal:F2} processed.");
        }
        else
        {
            Console.WriteLine("Deletion cancelled.");
        }
    }

    //==========================================================
    // ADVANCED FEATURE (a): Bulk processing of unprocessed orders
    //==========================================================

    static void BulkProcessPendingOrders()
    {
        Console.WriteLine("\nBulk Process Pending Orders");
        Console.WriteLine("============================");

        // Identify all orders with status "Pending" from all restaurants
        List<Order> allPendingOrders = new List<Order>();

        foreach (Restaurant restaurant in restaurants)
        {
            Queue<Order> queue = restaurant.GetOrderQueue();
            foreach (Order order in queue)
            {
                if (order.OrderStatus == "Pending")
                {
                    allPendingOrders.Add(order);
                }
            }
        }

        // Display total number of pending orders
        Console.WriteLine($"Total Pending Orders: {allPendingOrders.Count}");

        if (allPendingOrders.Count == 0)
        {
            Console.WriteLine("No pending orders to process.");
            return;
        }

        // Counters for statistics
        int processedCount = 0;
        int preparingCount = 0;
        int rejectedCount = 0;

        // Process each pending order
        foreach (Order order in allPendingOrders)
        {
            // Calculate time until delivery
            TimeSpan timeUntilDelivery = order.DeliveryDateTime - DateTime.Now;
            double hoursUntilDelivery = timeUntilDelivery.TotalHours;

            if (hoursUntilDelivery < 1)
            {
                // Reject if delivery time is less than 1 hour
                order.UpdateStatus("Rejected");
                refundStack.Push(order);
                rejectedCount++;
                Console.WriteLine($"Order {order.OrderId}: REJECTED (delivery time < 1 hour)");
            }
            else
            {
                // Set to Preparing otherwise
                order.UpdateStatus("Preparing");
                preparingCount++;
                Console.WriteLine($"Order {order.OrderId}: PREPARING");
            }

            processedCount++;
        }

        // Calculate total orders (from all customers)
        int totalOrders = 0;
        foreach (Customer customer in customers)
        {
            totalOrders += customer.GetOrders().Count;
        }

        // Display summary statistics
        Console.WriteLine("\n===== Summary Statistics =====");
        Console.WriteLine($"Number of orders processed: {processedCount}");
        Console.WriteLine($"Orders set to Preparing: {preparingCount}");
        Console.WriteLine($"Orders set to Rejected: {rejectedCount}");

        if (totalOrders > 0)
        {
            double percentage = (double)processedCount / totalOrders * 100;
            Console.WriteLine($"Percentage of automatically processed orders: {percentage:F2}%");
        }
        else
        {
            Console.WriteLine("Percentage of automatically processed orders: 0.00%");
        }
    }
    //==========================================================
    // ADVANCED FEATURE (b): Display total order amount
    //==========================================================
    static void DisplayTotalOrderAmount()
    {
        Console.WriteLine("\nDisplay Total Order Amount");
        Console.WriteLine("==========================");

        double totalOrderAmount = 0;  // delivered revenue (minus delivery fee)
        double totalRefunds = 0;      // refunded amount (from refundStack)

        foreach (Restaurant r in restaurants)
        {
            foreach (Order o in r.GetOrderQueue())
            {
                if (o.OrderStatus == "Delivered")
                {
                    totalOrderAmount += (o.OrderTotal - 5.0);
                }
            }
        }

        foreach (Order o in refundStack)
        {
            totalRefunds += o.OrderTotal;
        }

        double finalAmount = totalOrderAmount - totalRefunds;

        Console.WriteLine($"Total order amount (less delivery fee): ${totalOrderAmount:F2}");
        Console.WriteLine($"Total refunds: ${totalRefunds:F2}");
        Console.WriteLine($"Final amount Gruberoo earns: ${finalAmount:F2}");
    }

    //==========================================================
    // ADDITIONAL FEATURE: Favourite Orders
    //==========================================================
    static void FavouriteOrdersMenu()
    {
        Console.WriteLine("\nFavourite Orders");
        Console.WriteLine("===============");

        Console.WriteLine("1. Add favourite order");
        Console.WriteLine("2. View favourite orders");
        Console.WriteLine("3. Reorder from favourite");
        Console.WriteLine("0. Back");
        Console.Write("Enter choice: ");
        string choice = Console.ReadLine();

        if (choice == "1") AddFavouriteOrder();
        else if (choice == "2") ViewFavouriteOrdersDetailed();
        else if (choice == "3") ReorderFromFavourite();
    }

    static void ViewFavouriteOrdersDetailed()
    {
        Customer customer = null;
        while (customer == null)
        {
            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine().Trim();
            customer = FindCustomer(email);
            if (customer == null) Console.WriteLine("Customer not found.");
        }

        if (!File.Exists("favourites.csv"))
        {
            Console.WriteLine("No favourites found yet.");
            return;
        }

        string[] lines = File.ReadAllLines("favourites.csv");
        bool any = false;

        Console.WriteLine("Favourite Orders:");

        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            if (parts.Length >= 2 && parts[0].Trim() == customer.EmailAddress)
            {
                int oid;
                if (int.TryParse(parts[1].Trim(), out oid))
                {
                    Order favOrder = customer.FindOrder(oid);
                    if (favOrder != null)
                    {
                        Console.WriteLine("\nOrder ID: " + favOrder.OrderId);
                        Console.WriteLine("Items:");
                        int n = 1;
                        foreach (OrderedFoodItem item in favOrder.GetOrderedItems())
                        {
                            Console.WriteLine(n + ". " + item.FoodItem.ItemName + " - " + item.Quantity);
                            n++;
                        }
                        Console.WriteLine("Total: $" + favOrder.OrderTotal.ToString("F2"));
                        any = true;
                    }
                }
            }
        }

        if (!any) Console.WriteLine("None");
    }

    static void ReorderFromFavourite()
    {
        Customer customer = null;
        while (customer == null)
        {
            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine().Trim();
            customer = FindCustomer(email);
            if (customer == null) Console.WriteLine("Customer not found.");
        }

        if (!File.Exists("favourites.csv"))
        {
            Console.WriteLine("No favourites found yet.");
            return;
        }

        // show favourites 
        string[] lines = File.ReadAllLines("favourites.csv");
        List<int> favIds = new List<int>();

        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            if (parts.Length >= 2 && parts[0].Trim() == customer.EmailAddress)
            {
                int oid;
                if (int.TryParse(parts[1].Trim(), out oid))
                    favIds.Add(oid);
            }
        }

        if (favIds.Count == 0)
        {
            Console.WriteLine("No favourites found.");
            return;
        }

        Console.WriteLine("Favourite Orders:");
        foreach (int id in favIds) Console.WriteLine(id);

        Console.Write("Enter Favourite Order ID to reorder: ");
        int favOid;
        while (!int.TryParse(Console.ReadLine(), out favOid))
        {
            Console.Write("Enter Favourite Order ID to reorder: ");
        }

        Order oldOrder = customer.FindOrder(favOid);
        if (oldOrder == null)
        {
            Console.WriteLine("Order not found.");
            return;
        }

        // Need restaurant for this order 
        Restaurant rest = FindRestaurantByOrderId(oldOrder.OrderId);
        if (rest == null)
        {
            Console.WriteLine("Restaurant not found for this order.");
            return;
        }

        // new delivery info
        Console.Write("Enter new Delivery Address: ");
        string newAddr = Console.ReadLine();

        DateTime newDT;
        while (true)
        {
            try
            {
                Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
                string d = Console.ReadLine();
                Console.Write("Enter Delivery Time (hh:mm): ");
                string t = Console.ReadLine();
                newDT = DateTime.Parse(d + " " + t);
                break;
            }
            catch
            {
                Console.WriteLine("Invalid date/time.");
            }
        }
        static int GetNextOrderIdSimple()
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

        static void AppendOrderToCsv(Customer customer, Restaurant rest, Order order)
        {
            // Build items field: "Name,Qty|Name,Qty"
            string itemsField = "";
            List<OrderedFoodItem> items = order.GetOrderedItems();

            for (int i = 0; i < items.Count; i++)
            {
                itemsField += items[i].FoodItem.ItemName + "," + items[i].Quantity;
                if (i < items.Count - 1)
                    itemsField += "|";
            }

            itemsField = "\"" + itemsField + "\"";

            string line =
                order.OrderId + "," +
                customer.EmailAddress + "," +
                rest.RestaurantId + "," +
                order.DeliveryDateTime.ToString("dd/MM/yyyy") + "," +
                order.DeliveryDateTime.ToString("HH:mm") + "," +
                order.DeliveryAddress + "," +
                DateTime.Now.ToString() + "," +
                order.OrderTotal.ToString("F2") + "," +
                order.OrderStatus + "," +
                itemsField;

            File.AppendAllText("orders - Copy.csv", line + "\n");
        }

        // create new order id 
        int newId = GetNextOrderIdSimple();

        Order newOrder = new Order(newId);
        newOrder.DeliveryAddress = newAddr;
        newOrder.DeliveryDateTime = newDT;
        newOrder.OrderStatus = "Pending";

        // copy items
        foreach (OrderedFoodItem item in oldOrder.GetOrderedItems())
        {
            OrderedFoodItem copy = new OrderedFoodItem(item.FoodItem, item.Quantity);
            newOrder.AddOrderedFoodItem(copy);
        }

        // compute total again
        newOrder.OrderTotal = CalculateTotal(newOrder);

        // add to lists/queue
        customer.AddOrder(newOrder);
        rest.EnqueueOrder(newOrder);

        // append to orders CSV with items
        AppendOrderToCsv(customer, rest, newOrder);

        Console.WriteLine("Reorder successful! New Order ID: " + newOrder.OrderId);
        Console.WriteLine("Status: Pending");
    }

    static void AddFavouriteOrder()
    {
        Customer customer = null;
        while (customer == null)
        {
            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine().Trim();
            customer = FindCustomer(email);

            if (customer == null)
                Console.WriteLine("Customer not found.");
        }

        // Show delivered orders only
        List<Order> delivered = new List<Order>();
        foreach (Order o in customer.GetOrders())
        {
            if (o.OrderStatus == "Delivered")
                delivered.Add(o);
        }

        if (delivered.Count == 0)
        {
            Console.WriteLine("No Delivered orders to favourite.");
            return;
        }

        Console.WriteLine("Delivered Orders:");
        foreach (Order o in delivered)
            Console.WriteLine(o.OrderId);

        Console.Write("Enter Order ID to favourite: ");
        int oid;
        while (!int.TryParse(Console.ReadLine(), out oid))
        {
            Console.Write("Enter Order ID to favourite: ");
        }

        bool found = false;
        foreach (Order o in delivered)
        {
            if (o.OrderId == oid)
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            Console.WriteLine("That Order ID is not in your Delivered orders.");
            return;
        }

        if (IsFavourite(customer.EmailAddress, oid))
        {
            Console.WriteLine("This order is already in favourites.");
            return;
        }

        File.AppendAllText("favourites.csv", customer.EmailAddress + "," + oid + "\n");
        Console.WriteLine("Favourite saved!");
    }
    static bool IsFavourite(string email, int orderId)
    {
        if (!File.Exists("favourites.csv"))
            return false;

        string[] lines = File.ReadAllLines("favourites.csv");
        foreach (string line in lines)
        {
            string[] parts = line.Split(',');
            if (parts.Length >= 2)
            {
                if (parts[0].Trim() == email)
                {
                    int oid;
                    if (int.TryParse(parts[1].Trim(), out oid) && oid == orderId)
                        return true;
                }
            }
        }
        return false;
    }

    //==========================================================
    // ADDITIONAL FEATURE: Create order with special offer
    //==========================================================
    static void LoadSpecialOffers()
    {
        string[] lines = File.ReadAllLines("specialoffers.csv");
        int count = 0;

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            string[] data = lines[i].Split(',');
            if (data.Length >= 4)
            {
                string restaurantName = data[0].Trim();
                string offerCode = data[1].Trim();
                string description = data[2].Trim();
                string discountStr = data[3].Trim();

                // Parse discount (could be number or "-")
                double discountAmount = 0;
                if (discountStr != "-")
                {
                    double.TryParse(discountStr, out discountAmount);
                }

                // Find restaurant by name
                Restaurant restaurant = FindRestaurantByName(restaurantName);
                if (restaurant != null)
                {
                    SpecialOffer offer = new SpecialOffer(offerCode, description, discountAmount);
                    restaurant.AddSpecialOffer(offer);
                    count++;
                }
            }
        }

        Console.WriteLine($"{count} special offers loaded!");
    }

    static Restaurant FindRestaurantByName(string name)
    {
        foreach (Restaurant r in restaurants)
        {
            if (r.RestaurantName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return r;
            }
        }
        return null;
    }

    static void CreateOrderWithSpecialOffer()
    {
        Console.WriteLine("\nCreate Order with Special Offer");
        Console.WriteLine("================================");

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

        // Display available special offers
        List<SpecialOffer> offers = restaurant.GetSpecialOffers();

        SpecialOffer selectedOffer = null;
        if (offers.Count > 0)
        {
            Console.WriteLine("\nAvailable Special Offers:");
            for (int i = 0; i < offers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {offers[i]}");
            }

            Console.Write("Select offer number (0 for no offer): ");
            int offerChoice;
            if (int.TryParse(Console.ReadLine(), out offerChoice) && offerChoice > 0 && offerChoice <= offers.Count)
            {
                selectedOffer = offers[offerChoice - 1];
                Console.WriteLine($"Selected: {selectedOffer.OfferCode}");
            }
        }

        // Date & Time
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
            if (choice == 0) break;

            Console.Write("Enter quantity: ");
            int qty = int.Parse(Console.ReadLine());

            selectedItems.Add(foodList[choice - 1]);
            quantities.Add(qty);
        }

        // Special request
        Console.Write("Add special request? [Y/N]: ");
        string specialRequest = "";
        if (Console.ReadLine().ToUpper() == "Y")
        {
            Console.Write("Enter special request: ");
            specialRequest = Console.ReadLine();
        }

        // Calculate subtotal
        double subtotal = 0;
        for (int i = 0; i < selectedItems.Count; i++)
        {
            subtotal += selectedItems[i].ItemPrice * quantities[i];
        }

        // Apply discount
        double discount = 0;
        double finalSubtotal = subtotal;
        bool freeDelivery = false;

        if (selectedOffer != null)
        {
            if (selectedOffer.Discount > 0)
            {
                discount = subtotal * (selectedOffer.Discount / 100);
                finalSubtotal = subtotal - discount;
                Console.WriteLine($"\nSubtotal: ${subtotal:F2}");
                Console.WriteLine($"Discount ({selectedOffer.Discount}%): -${discount:F2}");
                Console.WriteLine($"After discount: ${finalSubtotal:F2}");
            }
            else if (selectedOffer.OfferDesc.Contains("Free Delivery") && subtotal > 30)
            {
                freeDelivery = true;
                Console.WriteLine($"\nSubtotal: ${subtotal:F2}");
                Console.WriteLine($"Special Offer: {selectedOffer.OfferDesc}");
            }
        }

        double deliveryFee = freeDelivery ? 0.00 : 5.00;
        double total = finalSubtotal + deliveryFee;

        Console.WriteLine($"Delivery fee: ${deliveryFee:F2}");
        Console.WriteLine($"Order Total: ${total:F2}");

        // Payment
        Console.Write("Proceed to payment? [Y/N]: ");
        if (Console.ReadLine().ToUpper() != "Y") return;

        Console.Write("[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
        string payment = Console.ReadLine().ToUpper();

        // Create order
        int newId = GetNextOrderId();
        Order order = new Order(newId);
        order.DeliveryDateTime = deliveryDateTime;
        order.DeliveryAddress = address;
        order.OrderTotal = total;
        order.OrderStatus = "Pending";
        order.SpecialRequest = specialRequest;
        order.PaymentMethod = payment;

        // Add items
        for (int i = 0; i < selectedItems.Count; i++)
        {
            OrderedFoodItem orderedItem = new OrderedFoodItem(selectedItems[i], quantities[i]);
            order.AddOrderedFoodItem(orderedItem);
        }

        customer.AddOrder(order);
        restaurant.EnqueueOrder(order);

        // Build CSV line
        string itemsField = "";
        for (int i = 0; i < selectedItems.Count; i++)
        {
            itemsField += selectedItems[i].ItemName + "," + quantities[i];
            if (i < selectedItems.Count - 1) itemsField += "|";
        }
        itemsField = "\"" + itemsField + "\"";

        string line = $"{order.OrderId},{customer.EmailAddress},{restaurant.RestaurantId}," +
            $"{deliveryDateTime:dd/MM/yyyy},{deliveryDateTime:HH:mm},{address}," +
            $"{DateTime.Now},{order.OrderTotal},{order.OrderStatus},{itemsField}";

        File.AppendAllText("orders - Copy.csv", line + "\n");

        Console.WriteLine($"\nOrder {order.OrderId} created successfully! Status: Pending");
        if (selectedOffer != null)
        {
            Console.WriteLine($"Special Offer Applied: {selectedOffer.OfferCode}");
            if (discount > 0)
                Console.WriteLine($"You saved: ${discount:F2}");
        }
    }

    static void SaveQueueAndStack()
    {
        SaveQueue();
        SaveStack();
        Console.WriteLine("queue.csv and stack.csv saved!");
    }
    // save queue
    static void SaveQueue()
    {
        List<string> lines = new List<string>();

        // simple header
        lines.Add("RestaurantId,OrderId,Status,TotalAmount,DeliveryDateTime,DeliveryAddress");

        foreach (Restaurant r in restaurants)
        {
            foreach (Order o in r.GetOrderQueue())
            {
                string addr = o.DeliveryAddress;
                if (addr == null) addr = "";
                addr = addr.Replace(",", " "); 

                string line =
                    r.RestaurantId + "," +
                    o.OrderId + "," +
                    o.OrderStatus + "," +
                    o.OrderTotal.ToString("F2") + "," +
                    o.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm") + "," +
                    addr;

                lines.Add(line);
            }
        }

        File.WriteAllLines("queue.csv", lines);
    }
    // save stack
    static void SaveStack()
    {
        List<string> lines = new List<string>();

        lines.Add("OrderId,Status,TotalAmount,DeliveryDateTime,DeliveryAddress");

        foreach (Order o in refundStack) 
        {
            string addr = o.DeliveryAddress;
            if (addr == null) addr = "";
            addr = addr.Replace(",", " "); 
            string line =
                o.OrderId + "," +
                o.OrderStatus + "," +
                o.OrderTotal.ToString("F2") + "," +
                o.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm") + "," +
                addr;

            lines.Add(line);
        }

        File.WriteAllLines("stack.csv", lines);
    }

}





