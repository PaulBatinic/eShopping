﻿using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using MediatR;
using System.Reflection;
using Catalog.Application.Handlers;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using HealthChecks.UI.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Catalog.API;

public class Startup
{
  public IConfiguration Configuration;

  public Startup(IConfiguration configuration)
  {
    Configuration = configuration;
  }

  public void ConfigureServices(IServiceCollection services)
  {
    //services.AddControllers();
    services.AddApiVersioning();

    services.AddHealthChecks()
        .AddMongoDb(Configuration["DatabaseSettings:ConnectionString"], "Catalog  Mongo Db Health Check",
            HealthStatus.Degraded);
    services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog.API", Version = "v1" }); });
    //DI
    services.AddAutoMapper(typeof(Startup));
    // services.AddMediatR(typeof(CreateProductHandler).GetTypeInfo().Assembly);  // Per depricated MediatR DI NuGet Package
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductHandler).GetTypeInfo().Assembly));
    services.AddScoped<ICatalogContext, CatalogContext>();
    services.AddScoped<IProductRepository, ProductRepository>();
    services.AddScoped<IBrandRepository, ProductRepository>();
    services.AddScoped<ITypesRepository, ProductRepository>();

    // Identity server changes
    var userPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    
    services.AddControllers(config =>
    {
        config.Filters.Add(new AuthorizeFilter(userPolicy));
    });

     services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:9009";
        options.Audience = "Catalog";
    });
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));
    }

    app.UseRouting();
    app.UseAuthentication();
    app.UseStaticFiles();
    app.UseAuthorization();
    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
      endpoints.MapHealthChecks("/health", new HealthCheckOptions()
      {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
      });
    });
  }
}