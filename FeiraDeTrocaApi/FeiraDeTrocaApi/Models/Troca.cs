using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeiraDeTrocaApi.Models;

public class Troca
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("Item")]
    public int ItemOfertadoId { get; set; }
    [ForeignKey("Item")]
    public int ItemRecebidoId { get; set; }
    [ForeignKey("Aluno")]
    public int AlunoOfertanteId { get; set; }
    [ForeignKey("Aluno")]
    public int AlunoRecebedorId { get; set; }
    public StatusTroca Status { get; set; } = StatusTroca.Pendente;
    public DateTimeOffset DataProposta { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset? DataResposta { get; set; } // Pode ser nula se estiver pendente
}
