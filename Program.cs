using ChatApp.Data;
using ChatApp.DataService;
using ChatApp.Hubs;
using ChatApp.Repositories;
using ChatApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultDbConnection")));


// Add Cors policy to allow the React frontend to connect to the backend, Implementation is not very secure but it is good for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Add the MemoryDb to the services using the singleton pattern to only have one instance of it running
builder.Services.AddSingleton<MemoryDb>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = false, // Not validating the issuer
            ValidateAudience = false, // Not validating the audience
        };
    });

// Make sure to call this
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add the Cors policy to the middleware
app.UseCors("ReactFrontend");
app.UseAuthentication();
app.UseAuthorization();

// Add the SignalR hub to the middleware using the /Chat path
app.MapHub<ChatHub>("/Chat");
app.MapControllers();

app.Run();
