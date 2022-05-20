using Shopping.Data.Entities;
using Shopping.Models;

namespace Shopping.Helpers
{
    public interface IConverterHelper
    {
        Task<Product> ToProductAsync(CreateProductViewModel model);

        //ProductViewModel ToProductViewModel(Product product);
    }
}
