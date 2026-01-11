using Microsoft.Maui.Controls;

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
            // Spuštìní hry (fire-and-forget, protože setter nemùže být async)
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
    /// </summary>
    private async void StartGame()
    {
        // Nastavení stavu naèítání (zablokování vstupù, zobrazení indikátoru)
        SetLoadingState(true);

        // Asynchronní získání úvodu od AI
        var story = await _gameContext.StartGameAsync();
        StoryLabel.Text = story;

        // Obnovení UI
        SetLoadingState(false);
    }

    /// <summary>
    /// Odešle volbu hráèe a naète pokraèování.
    /// </summary>
    private async Task Continue(char choice)
    {
        // Nastavení stavu naèítání
        SetLoadingState(true);

        // Asynchronní získání pokraèování pøíbìhu
        var story = await _gameContext.ContinueGameAsync(choice);
        StoryLabel.Text = story;

        // Obnovení UI
        SetLoadingState(false);
    }

    /// <summary>
    /// Pøepíná viditelnost indikátoru naèítání a aktivitu tlaèítek.
    /// </summary>
    private void SetLoadingState(bool isLoading)
    {
        // Zobrazí/skryje toèící se koleèko
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;

        // Zakáže/povolí klikání na tlaèítka voleb
        ChoicesGrid.IsEnabled = !isLoading;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    // Handlery tlaèítek - volají asynchronní metodu Continue
    private async void OnChoiceA(object sender, EventArgs e) => await Continue('A');
    private async void OnChoiceB(object sender, EventArgs e) => await Continue('B');
    private async void OnChoiceC(object sender, EventArgs e) => await Continue('C');
    private async void OnChoiceD(object sender, EventArgs e) => await Continue('D');
}