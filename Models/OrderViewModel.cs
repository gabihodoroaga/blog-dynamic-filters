using System;

namespace DynamicFilters.Models
{
    public class OrderViewModel
    {
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCity { get; set; }
    }
}