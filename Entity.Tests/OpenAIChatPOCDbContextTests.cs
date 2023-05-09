using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.OpenAIChatPOC.Entity.Tests;

[TestClass]
public class OpenAIChatPOCDbContextTests
{
	[TestMethod]
	public void OpenAIChatPOCDbContext_CheckModelConventions()
	{
		// Arrange
		DbContextOptions<OpenAIChatPOCDbContext> options = new DbContextOptionsBuilder<OpenAIChatPOCDbContext>()
			.UseInMemoryDatabase(nameof(OpenAIChatPOCDbContext))
			.Options;
		OpenAIChatPOCDbContext dbContext = new OpenAIChatPOCDbContext(options);

		// Act
		Havit.Data.EntityFrameworkCore.ModelValidation.ModelValidator modelValidator = new Havit.Data.EntityFrameworkCore.ModelValidation.ModelValidator();
		string errors = modelValidator.Validate(dbContext);

		// Assert
		if (!String.IsNullOrEmpty(errors))
		{
			Assert.Fail(errors);
		}
	}
}
