using LMS.Assessment.Api.Abstractions;
using LMS.Assessment.Api.Entities;
using LMS.Assessment.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IDocumentRepository<IDocument>, InMemoryDocumentRepository<IDocument>>();
builder.Services.AddSingleton<IDocumentRepository<LawFirm>, InMemoryDocumentRepository<LawFirm>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
