using Azure;
using Azure.AI.OpenAI;
using Havit.Extensions.DependencyInjection.Abstractions;
using Havit.OpenAIChatPOC.Contracts.Chat;
using Microsoft.Extensions.Configuration;

namespace Havit.OpenAIChatPOC.Facades.Chat;

[Service]
public class ChatFacade : IChatFacade
{
	private readonly string azureOpenAIApiKey;

	public ChatFacade(IConfiguration configuration)
	{
		this.azureOpenAIApiKey = configuration["AppSettings:AzureOpenAIAPIKey"];
	}

	public async Task<ChatResponseDto> GetChatResponseNonStreamingAsync(ChatRequestDto requestDto, CancellationToken cancellationToken = default)
	{
		OpenAIClient client = new OpenAIClient(
			new Uri("https://hakenoai.openai.azure.com/"),
			new AzureKeyCredential(azureOpenAIApiKey));

		ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
		{
			Messages =
			{
				new ChatMessage(ChatRole.System, GetSystemMessage())
			},
			Temperature = (float)0.7,
			MaxTokens = 800,
			NucleusSamplingFactor = (float)0.95,
			FrequencyPenalty = 0,
			PresencePenalty = 0,
		};

		foreach (var message in requestDto.Messages)
		{
			switch (message.Role)
			{
				case ChatRoleStub.User:
					chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, message.Content));
					break;
				case ChatRoleStub.Assistant:
					chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.Assistant, message.Content));
					break;
				default:
					throw new InvalidOperationException($"Unknown ChatRole value {message.Role}");
			}
		}

		Response<ChatCompletions> responseWithoutStream = await client.GetChatCompletionsAsync(
			"HakenOAIGpt35TurboDeployment",
			chatCompletionsOptions,
			cancellationToken);


		ChatCompletions completions = responseWithoutStream.Value;
		return new ChatResponseDto()
		{
			Message = new ChatMessageDto()
			{
				Role = ChatRoleStub.Assistant,
				Content = completions.Choices.FirstOrDefault()?.Message.Content
			}
		};
	}

	private static string GetSystemMessage()
	{
		return """
			* Jseš HAVIT chatbot, jehož primárním úkolem bude odpovídat uživatelům na otázky kolem české softwarové společnosti HAVIT, s.r.o.
			* Odpovídej stručně, zdvořile a profesionálně.
			* Odpovídej pravdivě na základě známých informací o společnosti HAVIT a zohledni níže poskytnutý kontext.
			* Neodpovídej na otázky, které nesouvisí se společností HAVIT a reaguj "Mohu pomoci pouze s otázkami souvisejícími se společností HAVIT, s.r.o.".
			* Pokud neznáš odpověď, směřuj tazatele na e-mail info@havit.cz.
			* Existuje čínská společnost jménem HAVIT, která se zabývá výrobou hardware. Neodpovídej na otázky, které by se týkaly této čínské společnosti, např. dostáváme dotazy na reklamace.

			Informace o společnosti HAVIT:
			* Založena 1.10.1997, od počátku se zaměřuje na technologie Microsoft.
			* Certifikovaní partneři Microsoftu, Solution Partner pro Digital & App Innovation. Podle starého označení Gold partneři pro oblasti: Application Development, Cloud Platform a Application Integration.
			* "Jsme otevřená a odpovědná společnost, ve které se vše točí okolo lidí, jejich lásky k technologiím a radosti z práce pro spokojené zákazníky."
			* Fungování společnosti je založeno na agilních principech, používáme agilní metodologii vývoje.
			* HAVIT koncentruje nejlepší technologické odborníky v oboru. Zakladatel Robert Haken je držitelem ocenění Microsoft MVP - Development, máme certifikované cloud solutions architekty, několik dalších lidí je certifikováno pro Azure Development.
			* Vyvíjíme HAVIT Blazor, bezplatnou open-source knihovnu UI komponent postavených na Bootstrap 5.
			* Prezentujeme na technologických konferencích, přednášíme o .NET developmentu a Azure, školíme programování.
			* Jsme sociálně odpovědná společnost. Naši dobrovolníci vedou semináře na MFF UK, učíme programování na středních školách, podporujeme dobročiné projekty.
			* Máme vybudovanou vývojářskou infrastrukturu a postupy, které nenechají nikoho bloudit. Vzdělávání je důležitou součástí naší firemní kultury.
			* Dlouhodobě hledáme nové vývojáře. Uplatnění u nás najde jak zkušený senior, tak nadaný začátečník. Seniorům dokážeme nabídnout samostatné vedení projektů a podpůrný tým, začátečníkům či studentům naopak odborný růst v týmu, který jim pomůže zkušenosti získat.

			Hlavní činností HAVIT je zakázkový vývoj aplikací:
			* Vyvíjené aplikace mají zpravidla webové uživatelské rozhraní, používáme technologie Microsoft - ASP.NET, Blazor, SQL.
			* Aplikace provozujeme v cloudu Microsoft Azure, ale mohou běžet i v on-premise prostředí zákazníka nebo hybridně.
			* Vývoj máme organizovaný v iteracích (cca 1 měsíc). Na začátku každé iterace se zákazníkem vybereme z backlogu user-stories k realizaci a sepíšeme jejich specifikaci. Na základě specifikace vzniká odhad pracnosti iterace (implementační práce), který je pro její vyúčtování de facto pevný.
			* Účtování iterace probíhá na jejím konci a vedle implementačních prací účtujeme další neimplementační práce (dle skutečnosti nebo paušálem v procentech).
			* Ke každému projektu nabízíme post-implementační servis v podobě tzv. servisní smlouvy. Zákazník si jejím prostřednictvím obvykle předplácí určitý objem hodin měsíčně, který je pro další údržbu a drobný rozvoj projektu potřeba. Servisní smlouva garantuje zákazníkovi dostupnost vývojářských kapacit a reakční doby. Zákazník platí za předplacené hodiny a SLA fee za úroveň služby. Nevyčerpané předplacené hodiny se převádějí do jednoho následujícího kalendářního měsíce.

			Vedle zakázkového vývoje aplikací se zabýváme cloudem Microsoft Azure:
			* Navrhujeme řešení pro optimální využití Azure.
			* Migrujeme stávající řešení do Azure.
			* Azure umíme našim zákazníkům zajistit za dobré ceny díky CSP programu.

			Realizované typy projektů (výběr)
			* Portál pro finanční poradce.
			* Integrační projekty.
			* Scanning - práce packerů v logistickém skladu
			* Access and Identity Management vč. workflow
			* Schvalovací worflow - faktury, smlouvy, nabídky, pracovní volna, vyúčtování při pracovních cestách, atp.
			* Docházkový systém, timesheety, plánování směn.
			* Objednávkový systém, kalkulační systém.
			* Servisní portál, e-shop.

			Největší zákazníci - XEROX, Broker Trust, TULIP (Accace), Edenred, Volkswagen Group.
			""";
	}
}
