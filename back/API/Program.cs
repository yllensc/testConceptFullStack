using System.Reflection;
using API.Extensions;
using API.Helpers;
using Domine.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.ConfigureCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddAutoMapper(Assembly.GetEntryAssembly());

//Authorization
builder.Services.AddAuthorization(opts =>{
    opts.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new GlobalVerbRoleRequirement())
        .Build();
});
//Comunicación
builder.Services.AddDbContext<TestFullStackDbContext>(options =>{
string ConnectionStrings = builder.Configuration.GetConnectionString("ConexMySQLCampus");
options.UseMySql(ConnectionStrings,ServerVersion.AutoDetect(ConnectionStrings));
});
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
