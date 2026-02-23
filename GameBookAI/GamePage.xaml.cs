using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace GameBookAI;

/// <summary>
/// Herní stránka.
/// </summary>
[QueryProperty(nameof(GameContext), "GameContext")]
public partial class GamePage : ContentPage
{
    /// <summary>
    /// Herní kontext předaný ze Shellu.
    /// </summary>
    public GameContext GameContext
    {
        set
        {
            _gameContext = value;
            // Spuštění hry
            StartGame();
        }
    }

    private GameContext _gameContext;

    public GamePage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Spustí hru a načte úvodní text příběhu.
    /// Pokud se načtení nepovede a uživatel to vzdá, vrátí se zpět.
    /// </summary>
    private async void StartGame()
    {
        // Spustíme akci pro začátek hry s možností opakování při chybě
        bool success = await ExecuteWithRetryAsync(async () =>
        {
            return await _gameContext.StartGameAsync();
        });

        // Pokud se nepodařilo hru nastartovat (uživatel dal Zrušit), vrátíme se do menu
        if (!success)
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    /// <summary>
    /// Odešle volbu hráče a načte pokračování.
    /// </summary>
    private async Task Continue(char choice)
    {
        // Spustíme akci pro pokračování hry
        await ExecuteWithRetryAsync(async () =>
        {
            return await _gameContext.ContinueGameAsync(choice);
        });
    }

    /// <summary>
    /// Univerzální metoda pro volání asynchronních herních akcí s ošetřením chyb a možností opakování.
    /// </summary>
    /// <param name="action">Funkce vracející text příběhu (např. volání AI).</param>
    /// <returns>True pokud akce proběhla úspěšně, False pokud byla zrušena uživatelem.</returns>
    private async Task<bool> ExecuteWithRetryAsync(Func<Task<string>> action)
    {
        while (true)
        {
            // Zablokování UI a zobrazení indikátoru
            SetLoadingState(true);

            try
            {
                // Pokus o provedení akce
                string storyText = await action();

                // Úspěch - zobrazíme text
                StoryLabel.Text = storyText;

                // Odemkneme UI a vracíme úspěch
                SetLoadingState(false);
                return true;
            }
            catch (Exception ex)
            {
                // Při chybě musíme odblokovat UI, aby šlo kliknout na Alert
                SetLoadingState(false);

                // Zobrazení dotazu uživateli pomocí moderního API
                bool retry = await DisplayAlertAsync(
                    "Chyba připojení",
                    $"Nepodařilo se komunikovat s AI.\n{ex.Message}",
                    "Zkusit znovu",
                    "Zrušit");

                // Pokud uživatel nechce opakovat, končíme s neúspěchem
                if (!retry)
                {
                    return false;
                }

                // Pokud chce opakovat, cyklus while jede znovu od začátku
            }
        }
    }

    /// <summary>
    /// Přepíná viditelnost indikátoru načítání a aktivitu tlačítek.
    /// </summary>
    private void SetLoadingState(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        ChoicesGrid.IsEnabled = !isLoading;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnChoiceA(object sender, EventArgs e) => await Continue('A');
    private async void OnChoiceB(object sender, EventArgs e) => await Continue('B');
    private async void OnChoiceC(object sender, EventArgs e) => await Continue('C');
    private async void OnChoiceD(object sender, EventArgs e) => await Continue('D');

    private void OnGameSave(object sender, EventArgs e)
    {
        
    }

    private void OnGameLoad(object sender, EventArgs e)
    {
        
    }
}