using System.ComponentModel.DataAnnotations;

namespace FeiraDeTrocaApi.Models;

public class Aluno
{
    [Key]
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string Serie { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
