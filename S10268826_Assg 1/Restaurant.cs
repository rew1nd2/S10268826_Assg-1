using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10268826_Assg_1
{
    internal class Restaurant
    {
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantEmail { get; set; }

        public Restaurant() { }
        public Restaurant(string restaurantId, string restaurantName, string restaurantEmail)
        {
            RestaurantId = restaurantId;
            RestaurantName = restaurantName;
            RestaurantEmail = restaurantEmail;
            menus = new List<Menu>();
            specialOffers = new List<SpecialOffer>();
            orderQueue = new Queue<Order>();
        }

        public void AddMenu(Menu menu)
        {
            menus.Add(menu);
        }

        public bool RemoveMenu(Menu menu)
        {
            return menus.Remove(menu);
        }

        public void DisplayMenus()
        {
            foreach (Menu menu in menus)
            {
                Console.WriteLine(menu);
                menu.DisplayFoodItems();
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
