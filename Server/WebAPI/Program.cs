using Entities;
using FileRepositories;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using EfcRepositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS to allow requests from any origin (for development purposes)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddScoped<IRepository<User>>(provider => 
    new FileRepository<User>("users.json", u => u.Id));

builder.Services.AddScoped<IRepository<Post>>(provider => 
    new FileRepository<Post>("posts.json", p => p.Id));

builder.Services.AddScoped<IRepository<Comment>>(provider => 
    new FileRepository<Comment>("comments.json", c => c.Id));

builder.Services.AddDbContext<EfcRepositories.AppContext>(options =>
    options.UseSqlite("Data Source=C:\\Users\\ditte\\OneDrive - ViaUC\\3 Semester\\DNP 1\\app.db"));


var app = builder.Build();

// Enable CORS for all origins
app.UseCors("AllowAllOrigins");

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();