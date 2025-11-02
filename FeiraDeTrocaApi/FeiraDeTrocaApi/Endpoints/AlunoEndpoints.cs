using FeiraDeTrocaApi.Data;
using FeiraDeTrocaApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
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

        group.MapGet("/{id}/Itens", async Task<Results<Ok<List<Item>>, NotFound>> (int id, AppDbContext db) =>
        {
            var alunoExists = await db.Aluno.AnyAsync(a => a.Id == id);
            if (!alunoExists)
            {
                return TypedResults.NotFound();
            }

            var itens = await db.Item
                .Where(i => i.AlunoId == id)
                .AsNoTracking()
                .ToListAsync();

            return TypedResults.Ok(itens);
        })
        .WithName("GetItensByAluno")
        .WithOpenApi()
        .WithSummary("Listar todos os itens de um aluno");

        group.MapGet("/{id}/TrocasOfertadas", async Task<Results<Ok<List<Troca>>, NotFound>> (int id, AppDbContext db) =>
        {
            var alunoExists = await db.Aluno.AnyAsync(a => a.Id == id);
            if (!alunoExists)
            {
                return TypedResults.NotFound();
            }

            var trocas = await db.Troca
                .Where(t => t.AlunoOfertanteId == id)
                .AsNoTracking()
                .ToListAsync();

            return TypedResults.Ok(trocas);
        })
        .WithName("GetTrocasOfertadasByAluno")
        .WithOpenApi()
        .WithSummary("Listar trocas Ofertadas pelo aluno (AlunoOfertanteId)"); ;

        group.MapGet("/{id}/TrocasRecebidas", async Task<Results<Ok<List<Troca>>, NotFound>> (int id, AppDbContext db) =>
        {
            var alunoExists = await db.Aluno.AnyAsync(a => a.Id == id);
            if (!alunoExists)
            {
                return TypedResults.NotFound();
            }

            var trocas = await db.Troca
                .Where(t => t.AlunoRecebedorId == id)
                .AsNoTracking()
                .ToListAsync();

            return TypedResults.Ok(trocas);
        })
        .WithName("GetTrocasRecebidasByAluno")
        .WithOpenApi()
        .WithSummary("Listar trocas Recebidas pelo aluno (AlunoRecebedorId)");

    }
}
