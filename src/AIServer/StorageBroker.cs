using EFxceptions;
using Microsoft.EntityFrameworkCore;

public class ProductsDbContext : EFxceptionsContext
{
    // Define DbSets for your entities here
    public DbSet<Product> Products { get; set; }
}

public class ProductsBroker : IProductsBroker
{
    private readonly IDbContextFactory<ProductsDbContext> productsDbContextFactory;

    public ProductsBroker(IDbContextFactory<ProductsDbContext> productsDbContextFactory) =>
        this.productsDbContextFactory = productsDbContextFactory;



    public async ValueTask<Product> InsertProdcutAsync(Product product)
    {
        using var context = productsDbContextFactory.CreateDbContext();
        context.Entry(product).State = EntityState.Added;
        await context.SaveChangesAsync();

        return product;
    }

    public IQueryable<Product> SelectAllProducts()
    {
        var context = productsDbContextFactory.CreateDbContext();
        return context.Products;
    }

    public async ValueTask<Product> SelectProdcutAsync<T>(params Guid[] productIds)
    {
        using var context = productsDbContextFactory.CreateDbContext();

        return await context.FindAsync<Product>(productIds);
    }

    public async ValueTask<Product> UpdateProductAsync(Product product)
    {
        using var context = productsDbContextFactory.CreateDbContext();
        context.Entry(product).State |= EntityState.Modified;
        await context.SaveChangesAsync();

        return product;
    }

    public async ValueTask<Product> DeleteProductAsync(Product product)
    {
        using var context = productsDbContextFactory.CreateDbContext();
        context.Entry(product).State = EntityState.Deleted;
        await context.SaveChangesAsync();

        return product;
    }
}

public class Product
{ 
    public Guid Id { get; set; }
    public string Name { get; set; }  
}