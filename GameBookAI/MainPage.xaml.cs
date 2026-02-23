namespace GameBookAI;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // P O Z O R - N E P O S I L A T  N A  G I T H U B
    private Uri endpoint = new Uri("https://mitrn-mlyxvjfh-eastus2.cognitiveservices.azure.com/");
    private string deploymentName = "gpt-5.2-chat";
    private string apiKey = "";
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    private async void HratPribeh(string popisHry, int cisloPribehu)
    {
        string saveKey = $"Story_{cisloPribehu}";
        bool isLoaded = false;
        string savedSummary = string.Empty;
        string savedLastMessage = string.Empty;

        // Kontrola, zda hráč tuto hru už někdy uložil
        if (Preferences.ContainsKey(saveKey))
        {
            // Dotaz na nahrání poslední pozice
            bool continueGame = await DisplayAlertAsync(
                "Uložená hra",
                "Pro tento příběh existuje uložená pozice. Chcete v ní pokračovat?",
                "Pokračovat",
                "Nová hra");

            if (continueGame)
            {
                // Načtení souhrnu i přesného posledního textu s možnostmi
                savedSummary = Preferences.Get(saveKey, string.Empty);
                savedLastMessage = Preferences.Get($"{saveKey}_LastMsg", string.Empty);
                isLoaded = true;
            }
        }

        var gameContext = new GameContext(
            popisHry,
            endpoint,
            deploymentName,
            apiKey,
            cisloPribehu)
        {
            // Předání dat do kontextu, aby mohl po přesměrování obnovit stav
            IsLoadedGame = isLoaded,
            SavedSummary = savedSummary,
            SavedLastMessage = savedLastMessage
        };

        await Hrat(gameContext);
    }

    /*private async void HratKviz(string popisHry)
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
    }*/

    private async Task Hrat(GameContext gameContext)
    {
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
        HratPribeh("Jsi Markéta Trnková, bydlíš ve městě Karlovy Vary v ulici Zámecký vrch 411 ve starším domě v posledním 5. patře pod půdou. Na patře jste jedinný byt. S tebou v bytě bydlí tvůj tvůj syn Tobiáš (2 měsíce) a manžel Michal (programátor). Máte ještě 2 černobílé kočky (tlustší se jmenuje Blecha a hubenější Kapina). Příběh bude trochu strašidelný. Zjisti si mapu okolí, ať tvůj popis sedí s realitou. Příběh se bude točit okolo Tobiáše, u kterého se postupem příběhu dozvíme, že je tajuplně geniální a to v tajuplném a dobrém smyslu. V příběhu se může objevit postava Jiří Král, zmatený zedník, který naslibuje kde co, ale pak nestíhá. Naproti domu je lékárna, kterou vlastní místní mafián jménem Osipov. Má syna. Vlastní i dva malé byty v domě v přízemí.",1);
    }

    private void ButtonHra2_Clicked(object sender, EventArgs e)
    {
        HratPribeh("Jsi Markéta Trnková, bydlíš ve městě Karlovy Vary v ulici Zámecký vrch 411 ve starším domě v posledním 5. patře pod půdou. Na patře jste jedinný byt. S tebou v bytě bydlí tvůj tvůj syn Tobiáš (2 měsíce) a manžel Michal (programátor). Máte ještě 2 černobílé kočky. Vlastníte starší dům v obci Nejdek asi 15 minut autem od Karlových Varů. Jelikož se tam právě rekonstruuje (rekonstrukci provádí soused Patrik Ňorba bydlící kousek od toho domu), tak bydlíte v bytě ve Varech. Původně rekonstrukci prováděl Jiří Král, ale byl hrozně nespolehlivý a líný, tak rekonstrukce postupovala pomalu a po pár měsících jste ho vihodili. Dům má sklep se 3 místnostma, momentálně se celý rekonstruuje. Přízemí se 3 místnostma, 1. patro s kychyní a dvěma místnostma a podkroví s jednou místností jsou vymalovány. V přízemí je koupelna se záchodem a v 1. patře jen záchod s umyvadlem. Dům je kousek od náměstí na mírném kopečku s hezkým výhledem na obě strany na město. Z jedné strany máme sousedy Kučerovi, starší pár, který tam bydlí již ´50 let. Z druhé strany je rodinný dům ubytovan, kde bydlí asi 4 postarší zaměstnanci místní firmy Metalis. Příběh by se měl točit okolo toho, že Markéta vyhrála 2 miliony korun. Michal o tom neví a Markéta chce tajně rozjet rekonstrukci trochu více. Zjisti si mapu okolí, ať tvůj popis sedí s realitou",2);
    }

    private void ButtonHra3_Clicked(object sender, EventArgs e)
    {
        HratPribeh("Jmenuješ se Markéta Cambelová. Je ti 10 let a bydlíš ve vesnici Miletice ve starším domě s trochu bláznivějšíma rodičema. Máma věří různým čarodějnickým věce, i když se považuje za katlolíka. Táta je také katolík a většinu času něco kutí ve své dílně, kde vyrábí různé elektronické vychytávky, jako například mikrofony pro tajné odposlouchávání. Máš ještě 7 dalších starších sourozenců. Nejvíce si ale rozumíš s o 2 roky starší sestrou Hanou. Také máš o 12 let starší sestru Milenu, která je trochu nemehlo, ale nevidí to a je tím tragikomická. Například si myslí, že je hezká, ale není. Příběh bude ze světa Harryho Pottera. Příběh začne ve chvíli, kdy ti dorazí zvací dopis do Bradavic. Ve škole působí kromě známých učitelů i mladý učitel Michal Trnka, který má velkou moc jako Brumbál, ale spíše to skrývá. S hlavní hrdinkou bude mít přátelský vztah, kde kdyby hlavní hrdinka byla trochu starší, tak by třeba mezi nimi i proběhl néjaký románek. Ve třídě bude mít hlavní hrdinka spolužáka jménem Dominik Ďurovec, který bude spíše otravný, bude se snažit s hlavní hrdinkou kamarádit, ale často si bude vymýšlet, svádět svoji vinu na druhé, ale přitom si bude myslet, že je v právu. Také tam bude spolužák Štěpán Mejšner, který bude dobrý ve famfrpálu, bude mít bohatší rodiče a bude trošku namyšlený, ale bude mít i sem tam dobré vlastnosti. Další spolužák bude Zeněk Korbel zvaný Zdenda, často bude používat omamné látky a prodávat je tajně spolužákům. Bude hodný a bude skrývat to, že je homoseksuál. Další spolužačka bude Káťa Olivíková, která bude velmi hodná a bude se snažit s Markétou hodně kamarádit, i když bude trošičku hloupější, ale Markéta se na její přátelství bude moci spolehnout. V příběhu bude mít důležitou roli duch protáhlé kočky s nataženými krátkými packami, jako kdyby se vzdávala jménem Červokoč, který bude sice hodný, ale zlomyslný a bude mluvit takovým pisklavě protáhlým hlasem (bez háčků a čárek), jako ahooooj, ty maaas ale legracniii oblecek",3);
    }

    private void ButtonHra4_Clicked(object sender, EventArgs e)
    {
        HratPribeh("Jsi statečný dobrodruh v království Eldoria, kde se šíří temnota a zlo. " +
            "Tvým úkolem je najít a zničit prastarý artefakt, který ohrožuje celý svět. " +
            "Na své cestě potkáš různé postavy, které ti mohou pomoci nebo tě zradit. " +
            "Musíš se rozhodovat moudře, protože každý tvůj krok ovlivní osud Eldorie.",4);
    }

    private string uvodProAdrianuvPribeh = "Jmenuješ se Adriano Lo Scalzo, chodíš na střední technickou školu do 4. ročníku a za pár měsíců budeš maturovat.Bydlíš ve vesnici Dobříš kousek od Prahy.Už máš řidičák a máma (Monika Lo Scalzo) ti občas půjčí své auto, ale častěji jezdíš z Dobříče na Zličín autobusem a ze Zličína pak jedeš metrem B kam potřebuješ.Máma Monika pracuje jako učitelka v mateřské škole a s tvým tátou Fabiem, který je z Itálie z Palerma, ale již dlouho žije v Česku, se rozvedla již před mnoha lety.Fabio bydlí se svoji přítelkyní v Praze v bytě na sídlišti Barrandov.Na Barrandově, ale v jiné části, bydlí v bytě sama i jeho 75 letá babička Emilie Trnková.Je věřící evangelička a často lidi nutí do jídla a skáče jim do řeči, ale Adriana a další své příbuzné má ráda, ale často je lehce kritizuje a do něčeho tlačí, o čem si myslí, že je to správné. Také máš strejdu Michala Trnku (46 let), který bydlí momentálně v bytě v Karlových Varech se svoji ženou Markétou a jejich 4 měsíčním synem Tobiášem(Michal bude v pozdější části příběhu mocnější tajemná ale kladná postava). Michal pracuje jako programátor.Markéta před mateřskou dovolenou také. Ještě mají dům v malém městě Nejdek, který rekonstruují a plánují se tam brzo přestěhovat. Máš o 5 let staršího bratra jménem Richi Lo Scalzo.Ten momentálně bydlí na Slovensku v Bratislavě, protože má práci v Rakousku ve Vídni, která je od Bratislavy kousek.Pracuje tam na letišti v kanceláři pro jednu leteckou společnost ze země Tchaj-wan.V rodinném domku v Dobříči, kde bydlíš s mámou, tak tam s vámi ještě je malý pejsek jménem Leo, je to takové hyperaktivní vtipné trdlo.";

    private void ButtonHra5_Clicked(object sender, EventArgs e)
    {
        HratPribeh(uvodProAdrianuvPribeh+" Příběh začne jednoho březnového večera, kdy ti bratr Richi zatelefonuje, že má obrovskou, skoro až neuvěřitelnou informaci, která změní tvůj život úplně od základů. A že potřebuje, abys teď důkladně poslouchal. Následovat bude dobrodružný tajuplný příběh plný zvratů, který se bude odehrávat nejen v Česku, ale různě po světě.", 5);
    }

    private void ButtonHra6_Clicked(object sender, EventArgs e)
    {
        HratPribeh(uvodProAdrianuvPribeh + " Příběh začne jednoho letního dne, kdy ti přijde dopis do Bradavic, do speciální nadstavby Bradavické školy pro extrémně nadané studenty, kteří již vystudovali mudlovskou střední školu, ale bylo rozhodnuto, že by jim měl být otevřen svět čar a kouzel, protože teprve nyní se poznalo, jak velké mají nadání pro čáry a kouzla. U některých dětí se na to nepřijde dříve, ale až když dovrší plnoletost, nebo pár let po tom. V příběhu se Adriano potká se všemi důležitými postavami ze světa Harryho Pottera.", 6);
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
