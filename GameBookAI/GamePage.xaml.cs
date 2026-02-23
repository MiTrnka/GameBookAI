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
        set => _gameContext = value;
    }

    /// <summary>
    /// Zahajuje komunikaci s AI nebo načtení UI po úspěšném dokončení navigace na tuto stránku.
    /// </summary>
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (!_isInitialized && _gameContext != null)
        {
            _isInitialized = true;

            if (_gameContext.IsLoadedGame)
            {
                _gameContext.RebuildMessages(_gameContext.SavedSummary, _gameContext.SavedLastMessage);
                StartLoadedGame();
            }
            else
            {
                StartGame();
            }
        }
    }

    /// <summary>
    /// Udržuje aktuální informaci, zda byla hra od posledního posunu uložena.
    /// </summary>
    private bool _hraJeUlozena;

    /// <summary>
    /// Zabraňuje vícenásobnému spuštění úvodní inicializace a komunikace s AI při navigaci na stránku.
    /// </summary>
    private bool _isInitialized;

    /// <summary>
    /// Změní vnitřní stav uložení a podle něj aktivuje nebo deaktivuje tlačítko uložení.
    /// </summary>
    private void NastavStavUlozeni(bool ulozena)
    {
        _hraJeUlozena = ulozena;
        if (SaveButton != null)
        {
            SaveButton.IsEnabled = !ulozena;
            LoadButton.IsEnabled = !ulozena;
        }
    }

    /// <summary>
    /// Zobrazí okamžitě načtenou hru bez nového volání AI.
    /// </summary>
    private void StartLoadedGame()
    {
        // Zobrazení načteného textu v UI
        StoryLabel.Text = _gameContext.SavedLastMessage;
        SetLoadingState(false);

        // Indikace čerstvě načtené (a tím pádem aktuálně uložené) hry
        NastavStavUlozeni(true);
    }

    private GameContext? _gameContext;

    public GamePage()
    {
        InitializeComponent();

        // Přepsání chování výchozí systémové šipky Zpět v navigační liště
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            Command = new Command(async () =>
            {
                // Pokud je uloženo, přeskakujeme pokus o automatické ukládání
                if (!_hraJeUlozena)
                {
                    bool ulozeno = await UlozitHruAsync(false);

                    // Pokud selhalo uložení a uživatel to vzdal, ověříme, zda chce i tak odejít
                    if (!ulozeno)
                    {
                        bool odejit = await DisplayAlertAsync("Upozornění", "Hra není uložena. Opravdu chcete odejít?", "Ano", "Ne");
                        if (!odejit) return;
                    }
                }

                await Shell.Current.GoToAsync("..");
            })
        });
    }

    /// <summary>
    /// Zachytí hardwarové tlačítko Zpět nebo gesto zpět (např. na Androidu).
    /// </summary>
    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            // Pokud je uloženo, rovnou odcházíme zpět
            if (!_hraJeUlozena)
            {
                bool ulozeno = await UlozitHruAsync(false);

                if (!ulozeno)
                {
                    bool odejit = await DisplayAlertAsync("Upozornění", "Hra není uložena. Opravdu chcete odejít?", "Ano", "Ne");
                    if (!odejit) return;
                }
            }

            await Shell.Current.GoToAsync("..");
        });

        return true;
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

                // Indikace, že se příběh posunul a stav není uložen
                NastavStavUlozeni(false);

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


    private async void OnChoiceA(object sender, EventArgs e) => await Continue('A');
    private async void OnChoiceB(object sender, EventArgs e) => await Continue('B');
    private async void OnChoiceC(object sender, EventArgs e) => await Continue('C');
    private async void OnChoiceD(object sender, EventArgs e) => await Continue('D');

    /// <summary>
    /// Pokusí se vytvořit souhrn a uložit hru. Při chybě AI nabídne opakování.
    /// </summary>
    /// <returns>True, pokud se uložení podařilo, jinak False (pokud uživatel zrušil opakování).</returns>
    private async Task<bool> UlozitHruAsync(bool zobrazitUpozorneni)
    {
        string saveKey = $"Story_{_gameContext.cisloPribehu}";

        while (true)
        {
            SetLoadingState(true);
            try
            {
                // Vytvoření souhrnu dosavadního postupu pomocí AI
                string souhrnPribehu = await _gameContext.GenerateSummaryAsync();
                string? posledniZprava = _gameContext.GetLastMessage();

                // Uložení obou částí potřebných pro pozdější obnovení stavu
                Preferences.Set(saveKey, souhrnPribehu);
                Preferences.Set($"{saveKey}_LastMsg", posledniZprava);

                if (zobrazitUpozorneni)
                {
                    await DisplayAlertAsync("Uloženo", "Hra byla úspěšně uložena.", "OK");
                }

                // Přeskládání vnitřní paměti AI na nový komprimovaný stav
                _gameContext.RebuildMessages(souhrnPribehu, posledniZprava);

                SetLoadingState(false);

                // Indikace, že aktuální stav je zapsán v paměti
                NastavStavUlozeni(true);

                return true;
            }
            catch (Exception ex)
            {
                SetLoadingState(false);

                // Dotaz na opakování při chybě s AI
                bool retry = await DisplayAlertAsync(
                    "Chyba při ukládání",
                    $"Nepodařilo se komunikovat s AI při vytváření souhrnu.\n{ex.Message}",
                    "Zkusit znovu",
                    "Zrušit");

                if (!retry)
                {
                    return false;
                }
            }
        }
    }

    private async void OnGameSave(object sender, EventArgs e)
    {
        await UlozitHruAsync(true);
    }

    private async void OnGameLoad(object sender, EventArgs e)
    {
        string saveKey = $"Story_{_gameContext.cisloPribehu}";

        // Ověření existence klíče
        if (!Preferences.ContainsKey(saveKey))
        {
            await DisplayAlertAsync("Chyba", "Pro tento příběh neexistuje žádná uložená pozice.", "OK");
            return;
        }

        // Dotaz na potvrzení nahrání pozice a ztráty aktuálního postupu
        bool potvrdit = await DisplayAlertAsync(
            "Nahrát uloženou hru?",
            "Opravdu chceš nahrát dříve uloženou pozici? Tvůj aktuální neuložený postup bude ztracen.",
            "Ano, nahrát",
            "Zrušit");

        if (!potvrdit)
        {
            return;
        }

        // Extrakce uložení z paměti telefonu
        string souhrnPribehu = Preferences.Get(saveKey, string.Empty);
        string posledniZprava = Preferences.Get($"{saveKey}_LastMsg", string.Empty);

        // Aplikace stavu do herní třídy
        _gameContext.RebuildMessages(souhrnPribehu, posledniZprava);

        // Okamžité zobrazení poslední zprávy
        StoryLabel.Text = posledniZprava;

        // Indikace čerstvě načtené (a tím pádem aktuálně uložené) hry
        NastavStavUlozeni(true);
    }
}