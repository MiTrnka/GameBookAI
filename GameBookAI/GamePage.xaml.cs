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
    /// Herní kontext pøedaný ze Shellu.
    /// </summary>
    public GameContext GameContext
    {
        set
        {
            _gameContext = value;
            // Spuštìní hry
            StartGame();
        }
    }

    private GameContext _gameContext;

    public GamePage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Spustí hru a naète úvodní text pøíbìhu.
    /// Pokud se naètení nepovede a uživatel to vzdá, vrátí se zpìt.
    /// </summary>
    private async void StartGame()
    {
        // Spustíme akci pro zaèátek hry s možností opakování pøi chybì
        bool success = await ExecuteWithRetryAsync(async () =>
        {
            return await _gameContext.StartGameAsync();
        });

        // Pokud se nepodaøilo hru nastartovat (uživatel dal Zrušit), vrátíme se do menu
        if (!success)
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    /// <summary>
    /// Odešle volbu hráèe a naète pokraèování.
    /// </summary>
    private async Task Continue(char choice)
    {
        // Spustíme akci pro pokraèování hry
        await ExecuteWithRetryAsync(async () =>
        {
            return await _gameContext.ContinueGameAsync(choice);
        });
    }

    /// <summary>
    /// Univerzální metoda pro volání asynchronních herních akcí s ošetøením chyb a možností opakování.
    /// </summary>
    /// <param name="action">Funkce vracející text pøíbìhu (napø. volání AI).</param>
    /// <returns>True pokud akce probìhla úspìšnì, False pokud byla zrušena uživatelem.</returns>
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

                // Úspìch - zobrazíme text
                StoryLabel.Text = storyText;

                // Odemkneme UI a vracíme úspìch
                SetLoadingState(false);
                return true;
            }
            catch (Exception ex)
            {
                // Pøi chybì musíme odblokovat UI, aby šlo kliknout na Alert
                SetLoadingState(false);

                // Zobrazení dotazu uživateli pomocí moderního API
                bool retry = await DisplayAlertAsync(
                    "Chyba pøipojení",
                    $"Nepodaøilo se komunikovat s AI.\n{ex.Message}",
                    "Zkusit znovu",
                    "Zrušit");

                // Pokud uživatel nechce opakovat, konèíme s neúspìchem
                if (!retry)
                {
                    return false;
                }

                // Pokud chce opakovat, cyklus while jede znovu od zaèátku
            }
        }
    }

    /// <summary>
    /// Pøepíná viditelnost indikátoru naèítání a aktivitu tlaèítek.
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
}