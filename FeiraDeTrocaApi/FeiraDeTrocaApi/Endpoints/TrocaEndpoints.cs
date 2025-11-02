using Microsoft.EntityFrameworkCore;
using FeiraDeTrocaApi.Data;
using FeiraDeTrocaApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace FeiraDeTrocaApi.Endpoints;

public static class TrocaEndpoints
{
    public static void MapTrocaEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Troca").WithTags(nameof(Troca));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Troca.ToListAsync();
        })
        .WithName("GetAllTrocas")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Troca>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Troca.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Troca model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetTrocaById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Troca troca, AppDbContext db) =>
        {
            var affected = await db.Troca
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, troca.Id)
                    .SetProperty(m => m.ItemOfertadoId, troca.ItemOfertadoId)
                    .SetProperty(m => m.ItemRecebidoId, troca.ItemRecebidoId)
                    .SetProperty(m => m.AlunoOfertanteId, troca.AlunoOfertanteId)
                    .SetProperty(m => m.AlunoRecebedorId, troca.AlunoRecebedorId)
                    .SetProperty(m => m.Status, troca.Status)
                    .SetProperty(m => m.DataProposta, troca.DataProposta)
                    .SetProperty(m => m.DataResposta, troca.DataResposta)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateTroca")
        .WithOpenApi();

        group.MapPost("/", async (Troca troca, AppDbContext db) =>
        {
            db.Troca.Add(troca);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Troca/{troca.Id}",troca);
        })
        .WithName("CreateTroca")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Troca
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteTroca")
        .WithOpenApi();
    }
}
