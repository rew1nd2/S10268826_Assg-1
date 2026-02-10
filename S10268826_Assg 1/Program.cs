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
        Console.WriteLine("7. Bulk process pending orders");
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
                string itemsStr = data[9]; // ← THE ITEMS FIELD

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

                    // ✅ ADD THIS: Parse and add the ordered items
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

    //==========================================================
    // FEATURE 4: Process an order
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

        // Process each order in the queue
        // We need to convert queue to list to iterate without removing
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
    //Feature 7
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

        // display order info (match screenshot)
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

            // if increase, prompt payment
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
        else // choice == 3
        {
            // only change time part
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
        // Simple PRG2 style:
        // Choose existing item number, change qty, or 0 to stop.
        // (This matches “basic features” and is easy to demo.)

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

        // build items field like your loader expects: "Name,Qty|Name,Qty"
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
                // rebuild line with same columns layout (at least first 10)
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



}

