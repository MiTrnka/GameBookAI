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
    /// Klient pro komunikaci s Azure OpenAI.
    /// </summary>
    private readonly ChatClient _chatClient;

    /// <summary>
    /// Vytvoří nový herní kontext a uloží systémová pravidla hry.
    /// </summary>
    public GameContext(
        string systemPrompt,
        Uri endpoint,
        string deploymentName,
        string apiKey)
    {
        // Inicializace Azure OpenAI klienta
        var azureClient = new AzureOpenAIClient(
            endpoint,
            new AzureKeyCredential(apiKey));

        _chatClient = azureClient.GetChatClient(deploymentName);

        // SYSTEM prompt – pravidla hry
        _messages.Add(new SystemChatMessage(systemPrompt));
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
}