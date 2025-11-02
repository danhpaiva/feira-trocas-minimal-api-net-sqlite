using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeiraDeTrocaApi.Models;

public class Item
{
    [Key]
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public StatusItem Status { get; set; } = StatusItem.Disponivel;
    public DateTimeOffset DataCadastro { get; set; } = DateTimeOffset.Now;
    [ForeignKey("Aluno")]
    public int AlunoId { get; set; }
}
