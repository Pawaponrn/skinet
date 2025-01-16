using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ProductRepository(StoreContext context) : IProductRepository
{
    public async Task<IReadOnlyList<string>> GetBrandAsync()
    {
        return await context.Products.Select(p => p.Brand)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetTypeAsync()
    {
        return await context.Products.Select(p => p.Type)
            .Distinct()
            .ToListAsync();
    }

    void IProductRepository.AddProduct(Product product)
    {
        context.Products.Add(product);
    }

    void IProductRepository.DeleteProduct(Product product)
    {
        context.Products.Remove(product);
    }

    async Task<Product?> IProductRepository.GetProductByIdAsync(int id)
    {
        return await context.Products.FindAsync(id);
    }

    async Task<IReadOnlyList<Product>> IProductRepository.GetProductsAsync(string? brand, string? type, string? sort)
    {
        var query = context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(p => p.Brand == brand);
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(p => p.Type == type);
        }

        query = sort switch
        {
            "priceAsc" => query.OrderBy(p => p.Price),
            "priceDesc" => query.OrderByDescending(p => p.Price),
            _ => query.OrderBy(p => p.Name)
        };

        return await query.ToListAsync();
    }

    bool IProductRepository.ProductExists(int id)
    {
        return context.Products.Any(e => e.Id == id);
    }

    async Task<bool> IProductRepository.SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    void IProductRepository.UpdateProduct(Product product)
    {
        context.Entry(product).State = EntityState.Modified;
    }
}
