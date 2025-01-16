using AIProductify.Application.DTO.Product;
using AIProductify.Application.Interfaces;
using AIProductify.Core.Entities;
using AIProductify.Infrastructure.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AIProductify.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task SaveProductAsync(ProductDto productDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var product = _mapper.Map<Product>(productDto);

                var existingProduct = await _context.Products
                    .Include(p => p.Attributes)
                    .FirstOrDefaultAsync(p => p.Sku == product.Sku);

                if (existingProduct != null)
                {
                    _mapper.Map(productDto, existingProduct);

                    existingProduct.UpdatedDate = DateTime.Now;
                    _context.Products.Update(existingProduct);
                }
                else
                {
                    product.IsActive = true;
                    product.UploadDate = DateTime.Now;
                    await _context.Products.AddAsync(product);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error occurred while saving product data", ex);
            }
        }




    }
}
