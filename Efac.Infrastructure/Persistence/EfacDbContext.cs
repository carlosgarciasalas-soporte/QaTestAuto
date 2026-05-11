using Efac.Domain.Entities;
using Efac.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Efac.Infrastructure.Persistence;

public sealed class EfacDbContext : DbContext
{
    public EfacDbContext(DbContextOptions<EfacDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.HasKey(cliente => cliente.Id);

            entity.Property(cliente => cliente.Id).ValueGeneratedNever();
            entity.Property(cliente => cliente.TipoPersona).HasConversion<string>().IsRequired();
            entity.Property(cliente => cliente.Nit).HasMaxLength(15).IsRequired();
            entity.Property(cliente => cliente.Dv).IsRequired();
            entity.Property(cliente => cliente.Nombres).HasMaxLength(120);
            entity.Property(cliente => cliente.Apellidos).HasMaxLength(120);
            entity.Property(cliente => cliente.RazonSocial).HasMaxLength(180);
            entity.Property(cliente => cliente.Email).HasMaxLength(180).IsRequired();
            entity.Property(cliente => cliente.Telefono).HasMaxLength(40).IsRequired();
            entity.Property(cliente => cliente.Direccion).HasMaxLength(180).IsRequired();
            entity.Property(cliente => cliente.CiudadCodigoMunicipio).HasMaxLength(8).IsRequired();
            entity.Property(cliente => cliente.ResponsabilidadFiscal).HasConversion<string>().IsRequired();

            entity.HasIndex(cliente => cliente.Nit).IsUnique();
            entity.Ignore(cliente => cliente.NombreCompleto);
        });

        SeedClientes(modelBuilder);
    }

    private static void SeedClientes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>().HasData(
            new
            {
                Id = Guid.Parse("0c595236-1562-4359-9ef7-85b5de30a688"),
                TipoPersona = TipoPersona.Juridica,
                Nit = "900373913",
                Dv = 4,
                Nombres = (string?)null,
                Apellidos = (string?)null,
                RazonSocial = "Efac Demo S.A.S.",
                FechaNacimiento = (DateOnly?)null,
                Email = "contacto@efacdemo.com",
                Telefono = "6015551234",
                Direccion = "Calle 100 # 15-20",
                CiudadCodigoMunicipio = "11001",
                ResponsabilidadFiscal = ResponsabilidadFiscal.ResponsableIva
            },
            new
            {
                Id = Guid.Parse("44698a9e-8618-401a-9d29-5fca86edb8cd"),
                TipoPersona = TipoPersona.Natural,
                Nit = "1020304050",
                Dv = 8,
                Nombres = "Ana Maria",
                Apellidos = "Gomez Ruiz",
                RazonSocial = (string?)null,
                FechaNacimiento = new DateOnly(1990, 4, 12),
                Email = "ana.gomez@example.com",
                Telefono = "3005557788",
                Direccion = "Carrera 7 # 45-10",
                CiudadCodigoMunicipio = "05001",
                ResponsabilidadFiscal = ResponsabilidadFiscal.NoResponsableIva
            });
    }
}
