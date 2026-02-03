using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10268826_Assg_1
{
    internal class OrderedFoodItem //correct
    {
        public FoodItem FoodItem { get; set; }
        public int Quantity { get; set; }
        public OrderedFoodItem() { }

        public OrderedFoodItem(FoodItem foodItem, int quantity)
        {
            FoodItem = foodItem;
            Quantity = quantity;
        }

        public double GetSubtotal()
        {
            return FoodItem.ItemPrice*Quantity;
        }

        public override string ToString()
        {
            return $"{FoodItem.ItemName} - {Quantity}"
            ;
        }

    }
}
