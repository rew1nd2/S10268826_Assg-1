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

namespace S10268826_Assg_1
{
    internal class Menu
    {
        public string Menuid { get; set; }
        public string Menuname { get; set; }

        public Menu() { }

        private List<FoodItem> FoodItems;
        public Menu(string menueID, string menuName)
        {
             Menuid = menueID;
             Menuname = menuName;
            FoodItems = new List<FoodItem>();
        }

        public void AddFoodItem(FoodItem fItem)
        {
            FoodItems.Add(fItem);
        }

        public bool RemoveFoodItem(FoodItem fItem)
        {
            return FoodItems.Remove(fItem);
        }

        public void DisplayFoodItems()
        {
            foreach (FoodItem item in FoodItems)
            {
                Console.WriteLine(item);
            }
        }

        public override string ToString()
        {
            return $"{Menuname} ({Menuid})";
        }

        public List<FoodItem> GetFoodItems()
        {
            return FoodItems;
        }
    }
}
