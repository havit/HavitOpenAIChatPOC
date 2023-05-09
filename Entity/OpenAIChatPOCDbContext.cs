using Havit.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Havit.OpenAIChatPOC.Entity;

public class OpenAIChatPOCDbContext : Havit.Data.EntityFrameworkCore.DbContext
{
	/// <summary>
	/// Konstruktor.
	/// Pro použití v unit testech, jiné použití nemá.
	/// </summary>
	internal OpenAIChatPOCDbContext()
	{
		// NOOP
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public OpenAIChatPOCDbContext(DbContextOptions options) : base(options)
	{
		// NOOP
	}

	/// <inheritdoc />
	protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
	{
		base.CustomizeModelCreating(modelBuilder);

		// modelBuilder.HasSequence<int>("XySequence");

		modelBuilder.RegisterModelFromAssembly(typeof(Havit.OpenAIChatPOC.Model.Localizations.Language).Assembly);
		modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
	}
}
