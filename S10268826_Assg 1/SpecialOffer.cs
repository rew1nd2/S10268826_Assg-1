using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace S10268826_Assg_1
{
    internal class SpecialOffer
    {
        public string OfferCode { get; set; }
        public string OfferDesc { get; set; }
        public double Discount { get; set; }
        public SpecialOffer() { }

        public SpecialOffer(string offerCode, string offerDesc, double discount)
        {
            OfferCode = offerCode;
            OfferDesc = offerDesc;
            Discount = discount;
        }
        public double ApplyDiscount(double price)
        {
            if (Discount > 0)
            {
                return price - (price * Discount / 100);
            }
            return price;
        }

        public override string ToString()
        {
            if (Discount > 0)
            {
                return $"{OfferCode}: {OfferDesc} - {Discount}% off";
            }
            else
            {
                return $"{OfferCode}: {OfferDesc}";
            }
        }
    }
}
