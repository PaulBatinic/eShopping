using System.Text.Json;
using Catalog.Core.Entities;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Data;

public static class BrandContextSeed
{
  public static void SeedData(IMongoCollection<ProductBrand> brandCollection, bool isNotDockerized)
  {
    string directory = Directory.GetCurrentDirectory();
    string path = Path.Combine("Data", "SeedData", "brands.json");  // For dockerized run
    if (!File.Exists(path))
    {
      path = Path.Combine("bin", "Debug", "net8.0", "Data", "SeedData", "brands.json");  // For VS dockerized Catalog.API debugging
    }

    if (isNotDockerized)
    {
      path = "../Catalog.Infrastructure/Data/SeedData/brands.json";  // For VS Catalog.API debugging
    }

    bool checkBrands = brandCollection.Find(b => true).Any();
    if (!checkBrands)
    {
      var brandsData = File.ReadAllText(path);
      var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
      if (brands != null)
      {
        foreach (var item in brands)
        {
          brandCollection.InsertOneAsync(item);
        }
      }
    }
  }
}