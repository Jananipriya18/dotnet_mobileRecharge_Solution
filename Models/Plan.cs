// Plan model
using dotnetapp.Data;

namespace dotnetapp.Models
{
    public class Plan
    {
        public long PlanId { get; set; }
        public string PlanType { get; set; }
        public string PlanName { get; set; }
        public string PlanValidity { get; set; }
        public string PlanOffer { get; set; }
        public string PlanDescription { get; set; }
        public double PlanPrice { get; set; }
    }
}