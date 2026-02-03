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

        public override string ToString()
        {
            return $"{OfferCode}: {OfferDesc} ({Discount}% off)";
        }
    }
}
