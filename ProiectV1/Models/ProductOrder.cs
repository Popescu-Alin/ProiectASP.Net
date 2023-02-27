namespace ProiectV1.Models
{
    public class ProductOrder
    {

        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int Quantity { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
    }
}
