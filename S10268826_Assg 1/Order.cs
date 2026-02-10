//==========================================================
// Student Number : S10268570D
// Student Name : Kiefer Wang
// Partner Name : Cyrus Tan
//==========================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10268826_Assg_1
{
    internal class Order 
    {
        public int OrderId { get; set; }
        public DateTime OrderDateTime { get; set; } 
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public string DeliveryAddress { get; set; }
        public string PaymentMethod { get; set; }
        public string SpecialRequest { get; set; }

        private List<OrderedFoodItem> orderedFoodItems;

        public Order(int orderId)
        {
            OrderId = orderId;
            OrderDateTime = DateTime.Now;
            OrderStatus = "Pending";
            orderedFoodItems = new List<OrderedFoodItem>();
            SpecialRequest = "";
            PaymentMethod = "";
        }
        public void AddOrderedFoodItem(OrderedFoodItem item)
        {
            orderedFoodItems.Add(item);
            CalculateOrderTotal();
        }

        public bool RemoveOrderedFoodItem(OrderedFoodItem item)
        {
            bool removed = orderedFoodItems.Remove(item);
            CalculateOrderTotal();
            return removed;
        }

        public void DisplayOrderedFoodItems()
        {
            foreach (OrderedFoodItem item in orderedFoodItems)
            {
                Console.WriteLine(item);
            }
        }

        public double CalculateOrderTotal()
        {
            double total = 0;
            foreach (OrderedFoodItem item in orderedFoodItems)
            {
                total += item.GetSubtotal();
            }

            OrderTotal = total + 5.00; // delivery fee
            return OrderTotal;
        }

        public override string ToString()
        {
            return $"Order {OrderId} - Status: {OrderStatus} - Total: ${OrderTotal:F2}";
        }

        public List<OrderedFoodItem> GetOrderedItems()
        {
            return orderedFoodItems;
        }

        public void UpdateStatus(string newStatus)
        {
            OrderStatus = newStatus;
        }

        public void UpdateDeliveryTime(DateTime newDateTime)
        {
            DeliveryDateTime = newDateTime;
        }

        public void UpdateDeliveryAddress(string newAddress)
        {
            DeliveryAddress = newAddress;
        }

        public bool CanModify()
        {
            return OrderStatus == "Pending";
        }

        public bool CanCancel()
        {
            return OrderStatus == "Pending";
        }
    }
}
            
