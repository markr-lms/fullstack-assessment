using Bogus;
using LMS.Assessment.Api.Abstractions;
using LMS.Assessment.Api.Entities;
using LMS.Assessment.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IRepository<LawFirm>, InMemoryRepository<LawFirm>>();
builder.Services.AddSingleton<IRepository<Document>, InMemoryRepository<Document>>();

var app = builder.Build();

await SeedLawFirms(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task SeedLawFirms(WebApplication app)
{
    var lawFirmRepo = app.Services.GetRequiredService<IRepository<LawFirm>>();

    var lawFirmFaker = new Faker<LawFirm>("en_GB")
        .CustomInstantiator(f => new LawFirm(
            Guid.NewGuid(),
            f.Company.CompanyName(),
            f.Address.FullAddress(),
            f.Phone.PhoneNumber(),
            f.Internet.Email(),
            Guid.NewGuid()));

    foreach (var lawFirm in lawFirmFaker.Generate(50))
    {
        await lawFirmRepo.CreateAsync(lawFirm);
    }
}