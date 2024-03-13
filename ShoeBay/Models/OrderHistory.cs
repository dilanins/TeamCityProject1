using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeBay.Models
{
    public class OrderHistory
    {

        [Key]
        public int Id { get; set; }

        public string Email { get; set; }


        [ForeignKey("Shoes")]
        public int ShoeId { get; set; }
        public virtual Shoe Shoes { get; set; }

        public int CartOrderId { get; set; }


        public decimal Amount { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
