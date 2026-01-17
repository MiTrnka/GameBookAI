namespace GameBookAI;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private Uri endpoint = new Uri("https://trnko-mk84yeeu-eastus2.cognitiveservices.azure.com/");
    private string deploymentName = "gpt-5.2-chat";
    private string apiKey = "";

    private async void HratPribeh(string popisHry)
    {
        string popisPravidel =
                "Jsi vypravěč interaktivního fantasy příběhu. " +
                "Vyprávěj příběh po částech. " +
                "Na konci každé části vždy vypiš přesně 4 možnosti A), B), C), D). " +
                "Neuváděj žádný jiný text mimo příběh a možnosti. " +
                "Hráč odpovídá pouze textem „Vybírám možnost X“." +
                "Popis příběhu: ";

        var gameContext = new GameContext(
         popisPravidel + popisHry,
         endpoint,
         deploymentName,
         apiKey);

        await Hrat(gameContext);
    }
    private async void HratKviz(string popisHry)
    {
        string popisPravidel =
                "Jsi moderátor kvízu. " +
                "Dávej vždy jen jednu doopravdy náhodnou otázku z požadovaného tématu. " +
                "Na konci otázky vždy vypiš přesně 4 možnosti A), B), C), D). " +
                "Neuváděj žádný jiný text mimo otázky a možností. " +
                "Hráč odpovídá pouze textem „Vybírám možnost X“." +
                "Až hráč odpoví, vtipně okomentuj správnost jeho odpovědi a polož jednu novou otázku." + // ZDE byla chyba (původně "nové 4 otázky")
                "Požadované téma kvízu: ";

        var gameContext = new GameContext(
          popisPravidel + popisHry,
          endpoint,
          deploymentName,
          apiKey);

        await Hrat(gameContext);
    }

    private async Task Hrat(GameContext gameContext)
    {
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

    private async void ButtonHra1_Clicked(object sender, EventArgs e)
    {
        HratPribeh("Jsi Markéta Trnková, bydlíš ve městě Karlovy Vary v ulici Zámecký vrch 411 ve starším domě v posledním 5. patře pod půdou. Na patře jste jedinný byt. S tebou v bytě bydlí tvůj tvůj syn Tobiáš (2 měsíce) a manžel Michal (programátor). Máte ještě 2 černobílé kočky (tlustší se jmenuje Blecha a hubenější Kapina). Příběh bude trochu strašidelný. Zjisti si mapu okolí, ať tvůj popis sedí s realitou. Příběh se bude točit okolo Tobiáše, u kterého se postupem příběhu dozvíme, že je tajuplně geniální a to v tajuplném a dobrém smyslu. V příběhu se může objevit postava Jiří Král, zmatený zedník, který naslibuje kde co, ale pak nestíhá. Naproti domu je lékárna, kterou vlastní místní mafián jménem Osipov. Má syna. Vlastní i dva malé byty v domě v přízemí.");
    }

    private void ButtonHra2_Clicked(object sender, EventArgs e)
    {
        HratPribeh("Jsi Markéta Trnková, bydlíš ve městě Karlovy Vary v ulici Zámecký vrch 411 ve starším domě v posledním 5. patře pod půdou. Na patře jste jedinný byt. S tebou v bytě bydlí tvůj tvůj syn Tobiáš (2 měsíce) a manžel Michal (programátor). Máte ještě 2 černobílé kočky. Vlastníte starší dům v obci Nejdek asi 15 minut autem od Karlových Varů. Jelikož se tam právě rekonstruuje (rekonstrukci provádí soused Patrik Ňorba bydlící kousek od toho domu), tak bydlíte v bytě ve Varech. Původně rekonstrukci prováděl Jiří Král, ale byl hrozně nespolehlivý a líný, tak rekonstrukce postupovala pomalu a po pár měsících jste ho vihodili. Dům má sklep se 3 místnostma, momentálně se celý rekonstruuje. Přízemí se 3 místnostma, 1. patro s kychyní a dvěma místnostma a podkroví s jednou místností jsou vymalovány. V přízemí je koupelna se záchodem a v 1. patře jen záchod s umyvadlem. Dům je kousek od náměstí na mírném kopečku s hezkým výhledem na obě strany na město. Z jedné strany máme sousedy Kučerovi, starší pár, který tam bydlí již ´50 let. Z druhé strany je rodinný dům ubytovan, kde bydlí asi 4 postarší zaměstnanci místní firmy Metalis. Příběh by se měl točit okolo toho, že Markéta vyhrála 2 miliony korun. Michal o tom neví a Markéta chce tajně rozjet rekonstrukci trochu více. Zjisti si mapu okolí, ať tvůj popis sedí s realitou");
    }

    private void ButtonHra3_Clicked(object sender, EventArgs e)
    {
        HratPribeh("Jmenuješ se Markéta Cambelová. Je ti 10 let a bydlíš ve vesnici Miletice ve starším domě s trochu bláznivějšíma rodičema. Máma věří různým čarodějnickým věce, i když se považuje za katlolíka. Táta je také katolík a většinu času něco kutí ve své dílně, kde vyrábí různé elektronické vychytávky, jako například mikrofony pro tajné odposlouchávání. Máš ještě 7 dalších starších sourozenců. Nejvíce si ale rozumíš s o 2 roky starší sestrou Hanou. Také máš o 12 let starší sestru Milenu, která je trochu nemehlo, ale nevidí to a je tím tragikomická. Například si myslí, že je hezká, ale není. Příběh bude ze světa Harryho Pottera. Příběh začne ve chvíli, kdy ti dorazí zvací dopis do Bradavic. Ve škole působí kromě známých učitelů i mladý učitel Michal Trnka, který má velkou moc jako Brumbál, ale spíše to skrývá. S hlavní hrdinkou bude mít přátelský vztah, kde kdyby hlavní hrdinka byla trochu starší, tak by třeba mezi nimi i proběhl néjaký románek. Ve třídě bude mít hlavní hrdinka spolužáka jménem Dominik Ďurovec, který bude spíše otravný, bude se snažit s hlavní hrdinkou kamarádit, ale často si bude vymýšlet, svádět svoji vinu na druhé, ale přitom si bude myslet, že je v právu. Také tam bude spolužák Štěpán Mejšner, který bude dobrý ve famfrpálu, bude mít bohatší rodiče a bude trošku namyšlený, ale bude mít i sem tam dobré vlastnosti. Další spolužák bude Zeněk Korbel zvaný Zdenda, často bude používat omamné látky a prodávat je tajně spolužákům. Bude hodný a bude skrývat to, že je homoseksuál. Další spolužačka bude Káťa Olivíková, která bude velmi hodná a bude se snažit s Markétou hodně kamarádit, i když bude trošičku hloupější, ale Markéta se na její přátelství bude moci spolehnout. V příběhu bude mít důležitou roli duch protáhlé kočky s nataženými krátkými packami, jako kdyby se vzdávala jménem Červokoč, který bude sice hodný, ale zlomyslný a bude mluvit takovým pisklavě protáhlým hlasem (bez háčků a čárek), jako ahooooj, ty maaas ale legracniii oblecek");
    }

    private void ButtonHra4_Clicked(object sender, EventArgs e)
    {
        HratPribeh("Jsi statečný dobrodruh v království Eldoria, kde se šíří temnota a zlo. " +
            "Tvým úkolem je najít a zničit prastarý artefakt, který ohrožuje celý svět. " +
            "Na své cestě potkáš různé postavy, které ti mohou pomoci nebo tě zradit. " +
            "Musíš se rozhodovat moudře, protože každý tvůj krok ovlivní osud Eldorie.");
    }

    private void ButtonHra5_Clicked(object sender, EventArgs e)
    {
        HratKviz("Svět z Harryho Pottera");
    }

    private void ButtonHra6_Clicked(object sender, EventArgs e)
    {
        HratKviz("Hlavní města");
    }

    private void ButtonHra7_Clicked(object sender, EventArgs e)
    {
        HratKviz("Správné zacházení s miminkem");
    }

    private void ButtonHra8_Clicked(object sender, EventArgs e)
    {
        HratKviz("Svět ze Stmívání");
    }

    private void ButtonHra9_Clicked(object sender, EventArgs e)
    {
        HratKviz("Bible");
    }

    private void ButtonHra10_Clicked(object sender, EventArgs e)
    {
        HratKviz("Různé");
    }
}
