using Microsoft.EntityFrameworkCore;
using FeiraDeTrocaApi.Data;
using FeiraDeTrocaApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace FeiraDeTrocaApi.Endpoints;

public static class AlunoEndpoints
{
    public static void MapAlunoEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Aluno").WithTags(nameof(Aluno));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Aluno.ToListAsync();
        })
        .WithName("GetAllAlunos")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Aluno>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Aluno.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Aluno model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetAlunoById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Aluno aluno, AppDbContext db) =>
        {
            var affected = await db.Aluno
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, aluno.Id)
                    .SetProperty(m => m.Nome, aluno.Nome)
                    .SetProperty(m => m.Matricula, aluno.Matricula)
                    .SetProperty(m => m.Serie, aluno.Serie)
                    .SetProperty(m => m.Email, aluno.Email)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateAluno")
        .WithOpenApi();

        group.MapPost("/", async (Aluno aluno, AppDbContext db) =>
        {
            db.Aluno.Add(aluno);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Aluno/{aluno.Id}",aluno);
        })
        .WithName("CreateAluno")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Aluno
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteAluno")
        .WithOpenApi();
    }
}
