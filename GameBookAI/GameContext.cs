using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace GameBookAI;

/// <summary>
/// Reprezentuje jednu rozehranou hru.
/// 
/// - drží historii konverzace (ChatMessage)
/// - stará se o volby hráče
/// - komunikuje s Azure OpenAI
/// 
/// Jedna instance = jedna hra = jedna paměť.
/// </summary>
public class GameContext
{
    /// <summary>
    /// Historie celé konverzace.
    /// To, co je tady, si AI "pamatuje".
    /// </summary>
    private readonly List<ChatMessage> _messages = new();

    /// <summary>
    /// Získá textový obsah poslední zprávy z historie konverzace.
    /// </summary>
    public string? GetLastMessage()
    {
        // Vrátí text první části poslední zprávy, pokud kolekce není prázdná
        return _messages is { Count: > 0 }
            ? _messages[^1].Content[0].Text
            : null;
    }

    /// <summary>
    /// Klient pro komunikaci s Azure OpenAI.
    /// </summary>
    private readonly ChatClient _chatClient;

    /// <summary>
    /// Popis úvodu do příběhu hry
    /// </summary>
    private string PopisUvoduPribehuHry { get; set; }

    /// <summary>
    /// Popis pravidel hry pro AI
    /// </summary>
    private string PopisPravidel { get; init; } =
                "Jsi vypravěč interaktivního fantasy příběhu. " +
                "Vyprávěj příběh po částech. " +
                "Na konci každé části vždy vypiš přesně 4 možnosti A), B), C), D). " +
                "Neuváděj žádný jiný text mimo příběh a možnosti. " +
                "Hráč odpovídá pouze textem „Vybírám možnost X“." +
                "Popis příběhu: ";


    /// <summary>
    /// Indikuje, zda je hra spuštěna z uložené pozice.
    /// </summary>
    public bool IsLoadedGame { get; set; }

    /// <summary>
    /// Číslo příběhu definované v MainPage, které se předává do GameContextu, aby věděl, jaké pravidla a úvodní popis použít.
    /// </summary>
    public int cisloPribehu;

    /// <summary>
    /// Vytvoří nový herní kontext a uloží systémová pravidla hry.
    /// </summary>
    public GameContext(
        string popisUvoduPribehuHry,
        Uri endpoint,
        string deploymentName,
        string apiKey,
        int cisloPribehu)
    {
        // Inicializace Azure OpenAI klienta
        var azureClient = new AzureOpenAIClient(
            endpoint,
            new AzureKeyCredential(apiKey));

        _chatClient = azureClient.GetChatClient(deploymentName);

        // Uložení textu pravidel
        this.PopisUvoduPribehuHry = popisUvoduPribehuHry;

        // SYSTEM prompt – pravidla hry
        _messages.Add(new SystemChatMessage(PopisPravidel));

        // SYSTEM prompt – úvodní popis příběhu
        _messages.Add(new SystemChatMessage(PopisUvoduPribehuHry));

        this.cisloPribehu = cisloPribehu;
    }

    /// <summary>
    /// Spustí hru – pošle AI první zprávu asynchronně.
    /// </summary>
    public async Task<string> StartGameAsync()
    {
        _messages.Add(new UserChatMessage("Začni příběh."));
        // Čekání na odpověď AI
        return await CallAiAsync();
    }

    /// <summary>
    /// Zpracuje volbu hráče (A–D) a získá asynchronně pokračování.
    /// </summary>
    public async Task<string> ContinueGameAsync(char choice)
    {
        _messages.Add(
            new UserChatMessage($"Vybírám možnost {choice}")
        );

        // Odeslání volby a čekání na odpověď
        return await CallAiAsync();
    }

    /// <summary>
    /// Zavolá Azure OpenAI s celou historií konverzace
    /// a uloží odpověď AI do paměti.
    /// </summary>
    private async Task<string> CallAiAsync()
    {
        // Asynchronní volání AI se všemi zprávami (paměť)
        var response = await _chatClient.CompleteChatAsync(_messages);

        // Text odpovědi AI
        var aiText = response.Value.Content[0].Text;

        // Uložení odpovědi do historie
        _messages.Add(new AssistantChatMessage(aiText));

        return aiText;
    }

    /// <summary>
    /// Získá od AI shrnutí dosavadního postupu hrou.
    /// </summary>
    public async Task<string> GenerateSummaryAsync()
    {
        // Zkopírování historie zpráv bez nultého indexu obsahujícího systémová pravidla
        var summaryMessages = _messages.GetRange(1, _messages.Count - 1);

        // Vložení speciálního příkazu pro sumarizaci
        summaryMessages.Add(new UserChatMessage("Vytvoř detailní souhrn dosavadního odehraného příběhu. Zaměř se na důležitá rozhodnutí, získané předměty a aktuální situaci, aby bylo možné na tento stav později navázat."));

        // Dotaz na AI
        var response = await _chatClient.CompleteChatAsync(summaryMessages);
        return response.Value.Content[0].Text;
    }

    /// <summary>
    /// Přrenastaví historii zpráv pro načtenou hru
    /// </summary>
    public void RebuildMessages(string popisDosudOdehraneHry, string? posledniCastPribehu=null)
    {
        _messages.Clear();
        _messages.Add(PopisPravidel);
        _messages.Add(PopisUvoduPribehuHry);
        _messages.Add(new SystemChatMessage(popisDosudOdehraneHry));
        if ( posledniCastPribehu != null)
        {
            _messages.Add(new AssistantChatMessage(posledniCastPribehu));
        }
    }

    /// <summary>
    /// Spustí komunikaci s AI po načtení hry ze zálohy.
    /// </summary>
    public async Task<string> ContinueLoadedGameAsync()
    {
        _messages.Add(new UserChatMessage("Pokračuj v příběhu od posledního uloženého bodu a rovnou nabídni další možnosti."));
        return await CallAiAsync();
    }
}