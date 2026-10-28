using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace AeroAndes.Data.Entities;

public partial class AeroAndesDbContext : DbContext
{
    public AeroAndesDbContext()
    {
    }

    public AeroAndesDbContext(DbContextOptions<AeroAndesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administradore> Administradores { get; set; }

    public virtual DbSet<Contacto> Contactos { get; set; }

    public virtual DbSet<Destino> Destinos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Vuelo> Vuelos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Administradore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("administradores");

            entity.HasIndex(e => e.Correo, "uq_administradores_correo").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
        });

        modelBuilder.Entity<Contacto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("contactos");

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Asunto)
                .HasMaxLength(150)
                .HasColumnName("asunto");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'pendiente'")
                .HasColumnType("enum('pendiente','atendido','cerrado')")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.Mensaje)
                .HasColumnType("text")
                .HasColumnName("mensaje");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Destino>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("destinos");

            entity.HasIndex(e => e.CodigoAeropuerto, "uq_destinos_codigo").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.Ciudad)
                .HasMaxLength(80)
                .HasColumnName("ciudad");
            entity.Property(e => e.CodigoAeropuerto)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("codigo_aeropuerto");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .HasColumnName("descripcion");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(255)
                .HasColumnName("imagen_url");
            entity.Property(e => e.Pais)
                .HasMaxLength(60)
                .HasColumnName("pais");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("reservas");

            entity.HasIndex(e => e.Correo, "ix_reservas_correo");

            entity.HasIndex(e => e.VueloId, "ix_reservas_vuelo");

            entity.HasIndex(e => e.CodigoReserva, "uq_reservas_codigo").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CantidadPasajeros)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(3) unsigned")
                .HasColumnName("cantidad_pasajeros");
            entity.Property(e => e.CodigoReserva)
                .HasMaxLength(10)
                .HasColumnName("codigo_reserva");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'pendiente'")
                .HasColumnType("enum('pendiente','confirmada','cancelada')")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.NombrePasajero)
                .HasMaxLength(120)
                .HasColumnName("nombre_pasajero");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
            entity.Property(e => e.VueloId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("vuelo_id");

            entity.HasOne(d => d.Vuelo).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.VueloId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reservas_vuelo");
        });

        modelBuilder.Entity<Vuelo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("vuelos");

            entity.HasIndex(e => e.DestinoId, "fk_vuelos_destino");

            entity.HasIndex(e => new { e.OrigenId, e.DestinoId, e.FechaSalida }, "ix_vuelos_ruta_fecha");

            entity.HasIndex(e => e.CodigoVuelo, "uq_vuelos_codigo").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("activo");
            entity.Property(e => e.AsientosDisponibles)
                .HasColumnType("smallint(5) unsigned")
                .HasColumnName("asientos_disponibles");
            entity.Property(e => e.CodigoVuelo)
                .HasMaxLength(10)
                .HasColumnName("codigo_vuelo");
            entity.Property(e => e.DestinoId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("destino_id");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'programado'")
                .HasColumnType("enum('programado','retrasado','cancelado','finalizado')")
                .HasColumnName("estado");
            entity.Property(e => e.FechaLlegada)
                .HasColumnType("datetime")
                .HasColumnName("fecha_llegada");
            entity.Property(e => e.FechaSalida)
                .HasColumnType("datetime")
                .HasColumnName("fecha_salida");
            entity.Property(e => e.OrigenId)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("origen_id");
            entity.Property(e => e.PrecioDesde)
                .HasPrecision(10, 2)
                .HasColumnName("precio_desde");

            entity.HasOne(d => d.Destino).WithMany(p => p.VueloDestinos)
                .HasForeignKey(d => d.DestinoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vuelos_destino");

            entity.HasOne(d => d.Origen).WithMany(p => p.VueloOrigens)
                .HasForeignKey(d => d.OrigenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vuelos_origen");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
