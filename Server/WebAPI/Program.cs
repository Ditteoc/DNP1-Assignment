using Entities;
using FileRepositories;
using RepositoryContracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepository<User>>(provider => 
    new FileRepository<User>("users.json", u => u.Id, (u, id) => u.Id = id));

builder.Services.AddScoped<IRepository<Post>>(provider => 
    new FileRepository<Post>("posts.json", p => p.Id, (p, id) => p.Id = id));

builder.Services.AddScoped<IRepository<Comment>>(provider => 
    new FileRepository<Comment>("comments.json", c => c.Id, (c, id) => c.Id = id));

var app = builder.Build();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();