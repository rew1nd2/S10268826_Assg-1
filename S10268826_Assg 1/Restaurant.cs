//==========================================================
// Student Number : S10268826F
// Student Name : Cyrus Tan
// Partner Name : Kiefer Wang
//==========================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace S10268826_Assg_1
{
    internal class Restaurant
    {
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantEmail { get; set; }

        public Restaurant() { }

        private List<FoodItem> menus;
        private List<SpecialOffer> specialOffers;
        private Queue<Order> orderQueue;
        public Restaurant(string restaurantId, string restaurantName, string restaurantEmail)
        {
            RestaurantId = restaurantId;
            RestaurantName = restaurantName;
            RestaurantEmail = restaurantEmail;
            menus = new List<FoodItem>();
            specialOffers = new List<SpecialOffer>();
            orderQueue = new Queue<Order>();
        }

        public void AddFoodItem(FoodItem item)
        {
            menus.Add(item);
        }
        public void AddMenu(FoodItem menu)
        {
            menus.Add(menu);
        }

        public bool RemoveMenu(FoodItem menu)
        {
            return menus.Remove(menu);
        }

        public void DisplayMenu()
        {
            Console.WriteLine($"\nRestaurant: {RestaurantName} ({RestaurantId})");
            foreach (FoodItem item in menus)
            {
                Console.WriteLine($" - {item}");
            }
        }

        public void DisplayOrders()
        {
            foreach (Order order in orderQueue)
            {
                Console.WriteLine(order);
            }
        }

        public void DisplaySpecialOffers()
        {
            foreach (SpecialOffer offer in specialOffers)
            {
                Console.WriteLine(offer);
            }
        }

        public void AddSpecialOffer(SpecialOffer offer)
        {
            specialOffers.Add(offer);
        }

        public void EnqueueOrder(Order order)
        {
            orderQueue.Enqueue(order);
        }

        public Queue<Order> GetOrderQueue()
        {
            return orderQueue;
        }

        public override string ToString()
        {
            return $"{RestaurantName} ({RestaurantId})";
        }


    }
}
