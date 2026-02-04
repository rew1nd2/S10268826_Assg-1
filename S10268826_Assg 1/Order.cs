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
        public string OrderPaymentMethod { get; set; }
        public string OrderShippingMethod { get; set; }

        private List<OrderedFoodItem> orderedFoodItems;

        public Order(int orderId)
        {
            OrderId = orderId;
            OrderDateTime = DateTime.Now;
            OrderStatus = "Pending";
            orderedFoodItems = new List<OrderedFoodItem>();
        }
    }
 }
