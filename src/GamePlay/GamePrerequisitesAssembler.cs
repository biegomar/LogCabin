using Heretic.InteractiveFiction.Comparer;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;
using LogCabin.Printing;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal sealed class GamePrerequisitesAssembler: IGamePrerequisitesAssembler
{
    private EventProvider eventProvider = default!;
    private IPrintingSubsystem printingSubsystem = default!;
    private IResourceProvider resourceProvider = default!;
    private IHelpSubsystem helpSubsystem = default!;
    private IGrammar grammar = default!;
    private IVerbHandler verbHandler = default!;
    private ScoreBoard scoreBoard = default!;
    private Universe universe = default!;

    public GamePrerequisitesAssembler()
    {
        InitializeSystem();
    }

    public IPrintingSubsystem PrintingSubsystem
    {
        get => printingSubsystem;
        set => printingSubsystem = value;
    }

    public IResourceProvider ResourceProvider
    {
        get => resourceProvider;
        set => resourceProvider = value;
    }

    public IHelpSubsystem HelpSubsystem
    {
        get => helpSubsystem;
        set => helpSubsystem = value;
    }

    public IGrammar Grammar
    {
        get => grammar;
        set => grammar = value;
    }

    public IVerbHandler VerbHandler
    {
        get => verbHandler;
        set => verbHandler = value;
    }

    public ScoreBoard ScoreBoard
    {
        get => scoreBoard;
        set => scoreBoard = value;
    }

    public Universe Universe
    {
        get => universe;
        set => universe = value;
    }

    public void AssembleGame()
    {
        var livingRoom = LivingRoomPrerequisites.Get(this.eventProvider);
        var bedRoom = BedRoomPrerequisites.Get(this.eventProvider);
        var cellar = CellarPrerequisites.Get(this.eventProvider);
        var attic = AtticPrerequisites.Get(this.eventProvider);
        var freedom = FreedomPrerequisites.Get();
        
        this.eventProvider.AddEventsForUniverse();
        
        var locationMap = new LocationMap(new LocationComparer())
        {
            { livingRoom, LivingRoomLocationMap(bedRoom, cellar, freedom) },
            { bedRoom, BedRoomLocationMap(livingRoom, attic) },
            { cellar, CellarLocationMap(livingRoom) },
            { freedom, FreedomLocationMap(livingRoom) },
            { attic, AtticLocationMap(bedRoom) }
        };

        var activeLocation = livingRoom;
        var activePlayer = PlayerPrerequisites.Get(this.eventProvider);
        var actualQuests = GetQuests();
        
        this.universe.LocationMap = locationMap;
        this.universe.ActiveLocation = activeLocation;
        this.universe.ActivePlayer = activePlayer;
        this.universe.Quests = actualQuests;
    }

    public void Restart()
    {
        InitializeSystem();
    }
    
    private void InitializeSystem()
    {
        this.resourceProvider = new ResourceProvider();
        this.printingSubsystem = new ConsolePrinting();

        this.universe = new Universe(printingSubsystem, resourceProvider);
        this.scoreBoard = new ScoreBoard(printingSubsystem);
        this.eventProvider = new EventProvider(universe, printingSubsystem, scoreBoard);

        this.verbHandler = new GermanVerbHandler(universe, resourceProvider);
        this.AssignNewErrorMessagesToVerbs();
        
        this.grammar = new GermanGrammar(resourceProvider, verbHandler);
        this.helpSubsystem = new BaseHelpSubsystem(grammar, printingSubsystem);
    }

    private void AssignNewErrorMessagesToVerbs()
    {
        var kindle = this.verbHandler.Verbs.First(x => x.Key == VerbKey.KINDLE);
        kindle.ErrorMessage = Descriptions.KINDLE_ERROR_MESSAGE;
    }

    private static ICollection<string> GetQuests()
    {
        var result = new List<string>
        {
            MetaData.QUEST_I,
            MetaData.QUEST_II
        };

        return result;
    }
    
    private static IEnumerable<DestinationNode> LivingRoomLocationMap(Location bedRoom, Location cellar, Location freedom)
    {
        var locationMap = new List<DestinationNode>
        {
            new() {Direction = Directions.N, Location = bedRoom},
            new() {Direction = Directions.DOWN, Location = cellar, IsHidden = true},
            new() {Direction = Directions.S, Location = freedom, IsHidden = true, DestinationDescription = Descriptions.FREEDOM_DESTINATION_DESCRIPTION}
        };
        return locationMap;
    }
    
    private static IEnumerable<DestinationNode> FreedomLocationMap(Location livingRoom)
    {
        var locationMap = new List<DestinationNode>
        {
            new() {Direction = Directions.N, Location = livingRoom},
        };
        return locationMap;
    }
    
    private static IEnumerable<DestinationNode> BedRoomLocationMap(Location livingRoom, Location attic)
    {
        var locationMap = new List<DestinationNode>
        {
            new() {Direction = Directions.S, Location = livingRoom},
            new() {Direction = Directions.UP, Location = attic}
        };
        return locationMap;
    }
    
    private static IEnumerable<DestinationNode> CellarLocationMap(Location livingRoom)
    {
        var locationMap = new List<DestinationNode>
        {
            new() {Direction = Directions.UP, Location = livingRoom},
        };
        return locationMap;
    }
    
    private static IEnumerable<DestinationNode> AtticLocationMap(Location bedRoom)
    {
        var locationMap = new List<DestinationNode>
        {
            new() {Direction = Directions.DOWN, Location = bedRoom},
        };
        return locationMap;
    }
}