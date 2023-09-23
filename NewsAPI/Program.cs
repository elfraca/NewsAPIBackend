using Domain.Services.Item;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IItemService, ItemService>();
builder.Services.AddSingleton<HttpClient, HttpClient>();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Obtiene una instancia de ItemService desde el contenedor de servicios
var itemService = app.Services.GetRequiredService<IItemService>();

// Llama a GetNewestStoriesAsync
await itemService.GetNewestStoriesAsync();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


