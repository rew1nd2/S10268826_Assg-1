using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10268826_Assg_1
{
    internal class FoodItem
    {
        public string ItemName { get; set; }
        public string ItemDesc { get; set; }
        public double ItemPrice { get; set; }
        public string Customise { get; set; }

        public FoodItem() { }

        public FoodItem(string itemName, string itemDesc, double itemPrice)
        {
            ItemName = itemName;
            ItemDesc = itemDesc;
            ItemPrice = itemPrice;
        }

        public override string ToString()
        {
            return $"{ItemName}: {ItemDesc} - ${ItemPrice:F2}";
        }

    }
}
