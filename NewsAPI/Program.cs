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
builder.Services.AddCors(options => {
    options.AddPolicy("AllowOrigin",
    builder => builder.WithOrigins("http://localhost:4200")
    .AllowAnyMethod()
    .AllowAnyHeader());
});

var app = builder.Build();

// Obtiene una instancia de ItemService desde el contenedor de servicios
var itemService = app.Services.GetRequiredService<IItemService>();

// Llama a GetNewestStoriesAsync
await itemService.GetNewestStoriesAsync(new Models.SearchRequest());


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


