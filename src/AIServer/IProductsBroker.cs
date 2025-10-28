
public interface IProductsBroker
{
    ValueTask<Product> DeleteProductAsync(Product product);
    ValueTask<Product> InsertProdcutAsync(Product product);
    IQueryable<Product> SelectAllProducts();
    ValueTask<Product> SelectProdcutAsync<T>(params Guid[] productIds);
    ValueTask<Product> UpdateProductAsync(Product product);
}