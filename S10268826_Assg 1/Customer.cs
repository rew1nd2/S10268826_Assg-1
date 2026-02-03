using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    
namespace S10268826_Assg_1
{
    internal class Customer
    {
        public string EmailAddress { get; set; }
        public string  CustomerName { get; set; }

        public Customer() { }

        private List<Order> orders;

        public Customer(string name,string email)
        {
            CustomerName = name;
            EmailAddress = email;
            orders = new List<Order>();
        }
        public void AddOrder(Order order)
        {
            orders.Add(order);
        }
            
        public bool RemoveOrder(Order order)
        {
            return orders.Remove(order);
        }

        public void DisplayAllOrders()
        {
            foreach (Order order in orders)
            {
                Console.WriteLine(order);
            }
        }
        public List<Order> GetPendingOrders()
{
            List<Order> pendingOrders = new List<Order>();
            foreach (Order order in orders)
            {
                if (order.OrderStatus == "Pending")
                {
                    pendingOrders.Add(order);
                }
            }
            return pendingOrders;
        }

        public Order FindOrder(int orderId)
        {
            foreach (Order order in orders)
            {
                if (order.OrderId == orderId)
                {
                    return order;
                }
            }
            return null;
        }
        public override string ToString()
        {
            return $"{CustomerName} ({EmailAddress})";
        }

        public List<Order> GetOrders()
        {
            return orders;
        }
    }
}

