using System.Text.Json;
using Catalog.Core.Entities;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Data;

public class TypeContextSeed
{
  public static void SeedData(IMongoCollection<ProductType> typeCollection, bool isNotDockerized)
  {
    string path = Path.Combine("Data", "SeedData", "types.json");
    if (!File.Exists(path))
    {
      path = Path.Combine("bin", "Debug", "net8.0", "Data", "SeedData", "types.json");
    }

    if (isNotDockerized)
    {
      path = "../Catalog.Infrastructure/Data/SeedData/types.json";
    }

    bool checkTypes = typeCollection.Find(b => true).Any();
    if (!checkTypes)
    {
      var typesData = File.ReadAllText(path);
      var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);
      if (types != null)
      {
        foreach (var item in types)
        {
          typeCollection.InsertOneAsync(item);
        }
      }
    }
  }
}