using System.Collections;
using System.Reflection.PortableExecutable;

namespace ProductManager.Glue.Interfaces.Models
{
    public interface IFullProduct :IProduct
    {
        IEnumerable<IProductCharacteristic> Characteristics { get; set; }
        IEnumerable<IProductOption> Options { get; set; }
        IEnumerable<IProductSell> Sells { get; set; }
    }
}