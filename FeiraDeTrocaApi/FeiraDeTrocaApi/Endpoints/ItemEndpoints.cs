using Microsoft.EntityFrameworkCore;
using FeiraDeTrocaApi.Data;
using FeiraDeTrocaApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace FeiraDeTrocaApi.Endpoints;

public static class ItemEndpoints
{
    public static void MapItemEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Item").WithTags(nameof(Item));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Item.ToListAsync();
        })
        .WithName("GetAllItems")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Item>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Item.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Item model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetItemById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Item item, AppDbContext db) =>
        {
            var affected = await db.Item
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, item.Id)
                    .SetProperty(m => m.Nome, item.Nome)
                    .SetProperty(m => m.Descricao, item.Descricao)
                    .SetProperty(m => m.Categoria, item.Categoria)
                    .SetProperty(m => m.Status, item.Status)
                    .SetProperty(m => m.DataCadastro, item.DataCadastro)
                    .SetProperty(m => m.AlunoId, item.AlunoId)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateItem")
        .WithOpenApi();

        group.MapPost("/", async (Item item, AppDbContext db) =>
        {
            db.Item.Add(item);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Item/{item.Id}",item);
        })
        .WithName("CreateItem")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Item
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteItem")
        .WithOpenApi();

        group.MapGet("/disponiveis", async (AppDbContext db) =>
        {
            var itens = await db.Item
                .AsNoTracking()
                .Where(i => i.Status == StatusItem.Disponivel)
                .ToListAsync();

            return TypedResults.Ok(itens);
        })
        .WithName("GetAllItensDisponiveis")
        .WithSummary("Busca todos os itens que estão atualmente disponíveis para troca.")
        .WithOpenApi();

        group.MapGet("/ofertavel/{alunoId}", async Task<Results<Ok<List<Item>>, NotFound>> (int alunoId, AppDbContext db) =>
        {
            var alunoExists = await db.Aluno.AnyAsync(a => a.Id == alunoId);
            if (!alunoExists)
            {
                return TypedResults.NotFound(); // Aluno não encontrado
            }

            var itensOfertavel = await db.Item
                .AsNoTracking()
                .Where(i => i.Status == StatusItem.Disponivel && // Deve estar disponível
                            i.AlunoId != alunoId)                 // NÃO pode ser o item do próprio aluno
                .ToListAsync();

            return TypedResults.Ok(itensOfertavel);
        })
        .WithName("GetItensOfertavelByAlunoId")
        .WithSummary("Lista todos os itens disponíveis para troca, exceto aqueles que pertencem ao aluno especificado.")
        .WithOpenApi();
    }
}
