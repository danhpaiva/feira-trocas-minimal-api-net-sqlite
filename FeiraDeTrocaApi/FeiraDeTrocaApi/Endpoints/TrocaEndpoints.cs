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

        group.MapPut("/{id}/Aceitar", async Task<Results<Ok, NotFound, BadRequest>> (int id, AppDbContext db) =>
        {
            var troca = await db.Troca
                .FirstOrDefaultAsync(t => t.Id == id);

            if (troca is null)
            {
                return TypedResults.NotFound();
            }

            if (troca.Status != StatusTroca.Pendente)
            {
                return TypedResults.BadRequest();
            }

            await using var transaction = await db.Database.BeginTransactionAsync();
            
            try
            {
                var affectedTroca = await db.Troca
                    .Where(t => t.Id == id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(t => t.Status, StatusTroca.Aceita)
                        .SetProperty(t => t.DataResposta, DateTimeOffset.Now));

                if (affectedTroca == 0) throw new Exception("Falha ao atualizar o status da Troca.");

                var affectedItemOfertado = await db.Item
                    .Where(i => i.Id == troca.ItemOfertadoId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(i => i.Status, StatusItem.Trocado));

                if (affectedItemOfertado == 0) throw new Exception("Falha ao atualizar o status do Item Ofertado.");

                var affectedItemRecebido = await db.Item
                    .Where(i => i.Id == troca.ItemRecebidoId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(i => i.Status, StatusItem.Trocado));

                if (affectedItemRecebido == 0) throw new Exception("Falha ao atualizar o status do Item Recebido.");

                await transaction.CommitAsync();

                return TypedResults.Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return TypedResults.BadRequest();
            }
        })
        .WithName("AceitarTroca")
        .WithSummary("Muda o status de uma troca para Aceita e atualiza o status dos itens envolvidos para Trocado.")
        .WithOpenApi();
    }
}
