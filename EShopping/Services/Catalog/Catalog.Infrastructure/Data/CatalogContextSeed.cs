using System.Text.Json;
using Catalog.Core.Entities;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Data;

public class CatalogContextSeed
{
  public static void SeedData(IMongoCollection<Product> productCollection, bool isNotDockerized)
  {
    string path = Path.Combine("Data", "SeedData", "products.json");
    if (!File.Exists(path))
    {
      path = Path.Combine("bin", "Debug", "net8.0", "Data", "SeedData", "products.json");
    }

    if (isNotDockerized)
    {
      path = "../Catalog.Infrastructure/Data/SeedData/products.json";
    }

    bool checkProducts = productCollection.Find(b => true).Any();
    if (!checkProducts)
    {
      var productsData = File.ReadAllText(path);
      var products = JsonSerializer.Deserialize<List<Product>>(productsData);
      if (products != null)
      {
        foreach (var item in products)
        {
          productCollection.InsertOneAsync(item);
        }
      }
    }
  }
}