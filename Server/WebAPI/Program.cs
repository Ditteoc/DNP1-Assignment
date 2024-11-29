using Entities;
using FileRepositories;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using EfcRepositories;
using AppContext = EfcRepositories.AppContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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

// Register DbContext with Sqlite connection
builder.Services.AddDbContext<AppContext>(options =>
    options.UseSqlite(@"Data Source=C:\Users\ditte\OneDrive - ViaUC\3 Semester\DNP 1\app.db"));

// Register generic repository and custom repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register file-based repositories
builder.Services.AddScoped<IRepository<Post>>(provider =>
    new FileRepository<Post>("posts.json", p => p.Id));
builder.Services.AddScoped<IRepository<Comment>>(provider =>
    new FileRepository<Comment>("comments.json", c => c.Id));

var app = builder.Build();

// Enable CORS for all origins
app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();