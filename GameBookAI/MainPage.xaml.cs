namespace GameBookAI;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void ButtonHra1_Clicked(object sender, EventArgs e)
    {
        var endpoint = new Uri("https://trnko-mk84yeeu-eastus2.cognitiveservices.azure.com/");
        var deploymentName = "gpt-5.2-chat";
        var apiKey = "";

        var systemPrompt =
            "Jsi vypravěč interaktivního fantasy příběhu. " +
            "Vyprávěj příběh po částech. " +
            "Na konci každé části vždy vypiš přesně 4 možnosti A), B), C), D). " +
            "Neuváděj žádný jiný text mimo příběh a možnosti. " +
            "Hráč odpovídá pouze textem „Vybírám možnost X“.";

        var gameContext = new GameContext(
         systemPrompt,
         endpoint,
         deploymentName,
         apiKey);
                
        // Registrace stránky do Shellu (dočasná)
        Routing.RegisterRoute(nameof(GamePage), typeof(GamePage));

        // Otevření herní stránky
        await Shell.Current.GoToAsync(
            nameof(GamePage),
            new Dictionary<string, object>
            {
            { "GameContext", gameContext }
            });

    }

    private void ButtonHra2_Clicked(object sender, EventArgs e)
    {

    }

    private void ButtonHra3_Clicked(object sender, EventArgs e)
    {

    }

    private void ButtonHra4_Clicked(object sender, EventArgs e)
    {

    }

    private void ButtonHra5_Clicked(object sender, EventArgs e)
    {

    }

    private void ButtonHra6_Clicked(object sender, EventArgs e)
    {

    }

    private void ButtonHra7_Clicked(object sender, EventArgs e)
    {

    }

    private void ButtonHra8_Clicked(object sender, EventArgs e)
    {

    }

    private void ButtonHra9_Clicked(object sender, EventArgs e)
    {

    }

    private void ButtonHra10_Clicked(object sender, EventArgs e)
    {

    }
}
