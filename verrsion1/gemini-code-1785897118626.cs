var builder = WebApplication.CreateBuilder(args);

// Registro de servicios según lifetime
builder.Services.AddTransient<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<ICacheService, RedisCacheService>();