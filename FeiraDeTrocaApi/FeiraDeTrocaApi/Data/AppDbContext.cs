using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FeiraDeTrocaApi.Models;

namespace FeiraDeTrocaApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<FeiraDeTrocaApi.Models.Aluno> Aluno { get; set; } = default!;
        public DbSet<FeiraDeTrocaApi.Models.Item> Item { get; set; } = default!;
        public DbSet<FeiraDeTrocaApi.Models.Troca> Troca { get; set; } = default!;
    }
}
