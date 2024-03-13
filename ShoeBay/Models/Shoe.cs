using System.ComponentModel.DataAnnotations.Schema;
using static NuGet.Packaging.PackagingConstants;

namespace ShoeBay.Models
{
    public class Shoe
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public int Size { get; set; }
        public string Color { get; set; }
        public decimal Cost { get; set; }
        public string FileUrl { get; set; }
        [NotMapped]
        public IFormFile Images { get; set; }
        public virtual List<ShoeCart> ShoeOrder { get; set; }
    }
}
