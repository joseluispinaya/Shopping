using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Models;

namespace Shopping.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _context;
        public ConverterHelper(DataContext context)
        {
            _context = context;
        }
        public Task<Product> ToProductAsync(CreateProductViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
