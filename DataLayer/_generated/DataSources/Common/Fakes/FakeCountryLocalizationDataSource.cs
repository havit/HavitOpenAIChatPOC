﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.Attributes;

namespace Havit.OpenAIChatPOC.DataLayer.DataSources.Common.Fakes;

[Fake]
[System.CodeDom.Compiler.GeneratedCode("Havit.Data.EntityFrameworkCore.CodeGenerator", "1.0")]
public class FakeCountryLocalizationDataSource : FakeDataSource<Havit.OpenAIChatPOC.Model.Common.CountryLocalization>, Havit.OpenAIChatPOC.DataLayer.DataSources.Common.ICountryLocalizationDataSource
{
	public FakeCountryLocalizationDataSource(params Havit.OpenAIChatPOC.Model.Common.CountryLocalization[] data)
		: this((IEnumerable<Havit.OpenAIChatPOC.Model.Common.CountryLocalization>)data)
	{			
	}

	public FakeCountryLocalizationDataSource(IEnumerable<Havit.OpenAIChatPOC.Model.Common.CountryLocalization> data, ISoftDeleteManager softDeleteManager = null)
		: base(data, softDeleteManager)
	{
	}
}