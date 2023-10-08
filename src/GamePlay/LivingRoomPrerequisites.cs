using System.Runtime.CompilerServices;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal static class LivingRoomPrerequisites
{
    internal static Location Get(EventProvider eventProvider)
    {
        var livingRoom = new Location()
        {
            Key = Keys.LIVINGROOM,
            Name = Locations.LIVINGROOM,
            Description = Descriptions.LIVINGROOM,
            SmellDescription = Descriptions.LIVINGROOM_SMELL
        };

        livingRoom.AddOptionalVerb(VerbKey.USE, OptionalVerbs.POOR, string.Empty);
        livingRoom.AddOptionalVerb(VerbKey.USE, OptionalVerbs.HOLD, Descriptions.NOTHING_TO_HOLD);
        livingRoom.AddOptionalVerb(VerbKey.USE, OptionalVerbs.RUB, string.Empty);
        livingRoom.AddOptionalVerb(VerbKey.SWITCHOFF, OptionalVerbs.BLOW, Descriptions.BLOW_OUT_NOT_POSSIBLE);
        livingRoom.AddOptionalVerb(VerbKey.SWITCHOFF, OptionalVerbs.UNLIGHT, Descriptions.UNLIGHT_NOT_POSSIBLE);

        livingRoom.Items.Add(GetTable(eventProvider));
        livingRoom.Items.Add(GetStove(eventProvider));
        livingRoom.Items.Add(GetKitchenCabinet(eventProvider));
        livingRoom.Items.Add(GetBookShelf(eventProvider));
        livingRoom.Items.Add(GetChest(eventProvider));
        livingRoom.Items.Add(GetDoor(eventProvider));
        livingRoom.Items.Add(GetWindow(eventProvider));

        AddChangeLocationEvents(livingRoom, eventProvider);
        
        AddSurroundings(livingRoom);

        return livingRoom;
    }

    private static Item GetKitchenCabinet(EventProvider eventProvider)
    {
        var cabinet = new Item
        {
            Key = Keys.KITCHEN_CABINET,
            Name = Items.KITCHEN_CABINET,
            Description = Descriptions.KITCHEN_CABINET,
            ContainmentDescription = Descriptions.KITCHEN_CABINET_CONTAINMENT,
            IsPickable = false,
            IsContainer = true,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };
        
        cabinet.Items.Add(GetSausage(eventProvider));
        cabinet.Items.Add(GetLampOilBucket(eventProvider));

        return cabinet;
    }

    private static Item GetSausage(EventProvider eventProvider)
    {
        var sausage = new Item
        {
            Key = Keys.SAUSAGE,
            Name = Items.SAUSAGE,
            Description = Descriptions.SAUSAGE,
            ContainmentDescription = Descriptions.SAUSAGE_CONTAINMENT,
            SmellDescription = Descriptions.SMELL_SAUSAGE,
            TasteDescription = Descriptions.TASTE_SAUSAGE,
            IsHidden = true,
            IsEatable = true,
            Grammar = new IndividualObjectGrammar()
        };
        
        AddEatEvents(sausage, eventProvider);
        
        return sausage;
    }

    private static Item GetLampOilBucket(EventProvider eventProvider)
    {
        var lampOilBucket = new Item
        {
            Key = Keys.LAMP_OIL_BUCKET,
            Name = Items.LAMP_OIL_BUCKET,
            Description = Descriptions.LAMP_OIL_BUCKET,
            Adjectives = Adjectives.LAMP_OIL_BUCKET,
            FirstLookDescription = Descriptions.LAMP_OIL_BUCKET_FIRSTLOOK,
            ContainmentDescription = Descriptions.LAMP_OIL_BUCKET_CONTAINMENT,
            SmellDescription = Descriptions.LAMP_OIL_BUCKET_FIRSTLOOK,
            IsHidden = true
        };
        
        lampOilBucket.Items.Add(GetPetroleum(eventProvider));

        return lampOilBucket;
    }

    private static Item GetPetroleum(EventProvider eventProvider)
    {
        var petroleum = new Item
        {
            Key = Keys.PETROLEUM,
            Name = Items.PETROLEUM,
            Description = Descriptions.PETROLEUM,
            IsHidden = true,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum, isAbstract: true)
        };

        AddTakeEvents(petroleum, eventProvider);
        AddPoorEvents(petroleum, eventProvider);
        AddPoorInLampEvents(petroleum, eventProvider);
        return petroleum;
    }

    private static void AddTakeEvents(Item petroleum, EventProvider eventProvider)
    {
        petroleum.BeforeTake += eventProvider.CantTakePetroleum;
    }

    private static void AddEatEvents(AHereticObject sausage, EventProvider eventProvider)
    {
        sausage.BeforeEat += eventProvider.TryToEatSausage;
    }

    private static Item GetTable(EventProvider eventProvider)
    {
        var table = new Item()
        {
            Key = Keys.TABLE,
            Name = Items.TABLE,
            Description = Descriptions.TABLE,
            ContainmentDescription = Descriptions.TABLE_CONTAINMENT,
            IsPickable = false,
            IsContainer = true,
            IsSurfaceContainer = true,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };

        table.Items.Add(GetCandle(eventProvider));
        table.Items.Add(GetNote(eventProvider));
        
        return table;
    }

    private static Item GetCandle(EventProvider eventProvider)
    {
        var candle = new Item()
        {
            Key = Keys.CANDLE,
            Name = Items.CANDLE,
            Description = Descriptions.CANDLE,
            ContainmentDescription = Descriptions.CANDLE_CONTAINMENT,
            IsLighter = true,
            IsLighterSwitchedOn = true,
            IsOnSurface = true,
            LighterSwitchedOnDescription = Descriptions.LIGHTER_ON
        };
        
        candle.Items.Add(GetIronKey(eventProvider));

        AddCandleTakeEvents(candle, eventProvider);
        AddCandleDropEvents(candle, eventProvider);

        return candle;
    }

    private static Item GetNote(EventProvider eventProvider)
    {
        var note = new Item
        {
            Key = Keys.NOTE,
            Name = Items.NOTE,
            Description = Descriptions.NOTE,
            IsHidden = true,
            IsReadable = true,
            IsOnSurface = true,
            LetterContentDescription = Descriptions.NOTE_LETTER_CONTENT
        };
        
        AddReadEvents(note, eventProvider);
        AddDropEvents(note, eventProvider);
        AddKindleEventsForNote(note, eventProvider);
        
        return note;
    }

    private static Item GetIronKey(EventProvider eventProvider)
    {
        var ironKey = new Item()
        {
            Key = Keys.IRON_KEY,
            Name = Items.IRON_KEY,
            Description = Descriptions.IRON_KEY,
            IsHidden = true,
            IsUnveilable = false,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };
        
        AddTakeEventsForKey(ironKey, eventProvider);

        return ironKey;
    }
    
    private static void AddTakeEventsForKey(Item ironKey, EventProvider eventProvider)
    {
        ironKey.BeforeTake += eventProvider.BeforeTakeIronKey;
    }

    private static Item GetWindow(EventProvider eventProvider)
    {
        var livingRoomWindows = new Item()
        {
            Key = Keys.LIVINGROOM_WINDOW,
            Name = Items.LIVINGROOM_WINDOW,
            Description = Descriptions.LIVINGROOM_WINDOW,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        
        livingRoomWindows.Items.Add(GetSucculent(eventProvider));

        return livingRoomWindows;
    }

    private static Item GetSucculent(EventProvider eventProvider)
    {
        var succulent = new Item()
        {
            Key = Keys.SUCCULENT,
            Name = Items.SUCCULENT,
            Description = Descriptions.SUCCULENT,
            ContainmentDescription = Descriptions.SUCCULENT_CONTAINMENT,
            IsPickable = false,
            IsHidden = true
        };
        
        AddRubEvents(succulent, eventProvider);

        return succulent;
    }
    
    private static void AddRubEvents(Item succulent, EventProvider eventProvider)
    {
        succulent.Use += eventProvider.RubOnSucculent;
        eventProvider.RegisterScore(nameof(eventProvider.RubOnSucculent), 1);
    }
    
    private static Item GetDoor(EventProvider eventProvider)
    {
        var door = new Item()
        {
            Key = Keys.DOOR,
            Name = Items.DOOR,
            Description = Descriptions.DOOR,
            FirstLookDescription = Descriptions.DOOR_FIRSTLOOK,
            IsPickable = false,
            IsLocked = true,
            IsClosed = true,
            IsCloseable = true,
            IsSurrounding = true
        };
        
        AddLookEvents(door, eventProvider);

        return door;
    }

    private static void AddLookEvents(Item door, EventProvider eventProvider)
    {
        door.Look += eventProvider.UnhideMainEntrance;
    }

    private static Item GetChest(EventProvider eventProvider)
    {
        var chest = new Item()
        {
            Key = Keys.CHEST,
            Name = Items.CHEST,
            Description = Descriptions.CHEST,
            Adjectives = Adjectives.CHEST,
            LockDescription = Descriptions.CHEST_LOCKED,
            ContainmentDescription = Descriptions.CHEST_CONTAINMENT,
            CloseDescription = Descriptions.CHEST_CLOSED,
            IsLocked = true,
            IsLockable = true,
            UnlockWithKey = Keys.IRON_KEY,
            IsPickable = false,
            IsSeatable = true,
            IsClosed = true,
            IsCloseable = true,
            IsContainer = true,
            IsSurfaceContainer = true
        };
        
        chest.Items.Add(GetMatchBox(eventProvider));

        return chest;
    }
    
    private static Item GetMatchBox(EventProvider eventProvider)
    {
        var matchBox = new Item()
        {
            Key = Keys.MATCHBOX,
            Name = Items.MATCHBOX,
            ContainmentDescription = Descriptions.MATCHBOX_CONTAINMENT,
            IsContainer = true,
            IsClosed = false
        };
        
        matchBox.Spare.Add("CountOfMatchesInBox", 25);
        matchBox.Description = GetMatchBoxDescription(matchBox);
        matchBox.ContainerEmptyDescription = GetMatchBoxEmptyDescription(matchBox);
        
        matchBox.Items.Add(GetMatch(eventProvider));

        return matchBox;
    }

    private static Func<string> GetMatchBoxDescription(Item matchBox)
    {
        return () => string.Format(Descriptions.MATCHBOX, (int)matchBox.Spare["CountOfMatchesInBox"]);
    }
    
    private static Func<string> GetMatchBoxEmptyDescription(Item matchBox)
    {
        return () => (int)matchBox.Spare["CountOfMatchesInBox"] >= 1
            ? string.Empty
            : Descriptions.MATCH_CONTAINMENT_EMPTY;
    }
    
    private static Item GetMatch(EventProvider eventProvider)
    {
        var match = new Item()
        {
            Key = Keys.MATCH,
            Name = Items.MATCH,
            Description = Descriptions.MATCH,
            IsLighter = true,
            IsLighterSwitchedOn = false,
            LighterSwitchedOnDescription = Descriptions.MATCH_BURNING,
            IsShownInObjectList = false,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        
        match.Spare.Add("NumberOfGameLoopBeforeDying", (int)10);
        
        AddTakeEventsForMatch(match, eventProvider);
        AddAfterDropEventsForMatch(match, eventProvider);
        AddKindleEventsForMatch(match, eventProvider);
        AddGameLoopEventsForMatch(match, eventProvider);

        return match;
    }

    private static void AddTakeEventsForMatch(Item match, EventProvider eventProvider)
    {
        match.BeforeTake += eventProvider.GetNextMatchFromMatchBox;
    }
    
    private static void AddAfterDropEventsForMatch(Item match, EventProvider eventProvider)
    {
        match.BeforeDrop += eventProvider.BeforeDropMatchInMatchBox;
        match.AfterDrop += eventProvider.AfterDropMatchInMatchBox;
    }

    private static void AddKindleEventsForMatch(Item match, EventProvider eventProvider)
    {
        match.Kindle += eventProvider.KindleMatch;
    }
    
    private static void AddGameLoopEventsForMatch(Item match, EventProvider eventProvider)
    {
        match.NextGameLoop += eventProvider.GetNextMatchFromMatchBoxAfterGameLoop;
    }

    private static Item GetStove(EventProvider eventProvider)
    {
        var stove = new Item()
        {
            Key = Keys.STOVE,
            Name = Items.STOVE,
            Description = Descriptions.STOVE,
            FirstLookDescription = Descriptions.STOVE_FIRSTLOOK,
            CloseDescription = Descriptions.STOVE_CLOSED,
            OpenDescription = Descriptions.STOVE_OPEN,
            ContainmentDescription = Descriptions.STOVE_CONTAINMENT,
            IsPickable = false,
            IsClosed = true,
            IsCloseable = true,
            IsContainer = true,
            IsSurfaceContainer = true,
            IsLighter = true,
            LighterSwitchedOnDescription = Descriptions.STOVE_CLOSED_ON_FIRE,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };

        AddPoorEvents(stove, eventProvider);
        AddAfterOpenAndCloseEvents(stove, eventProvider);
        
        var pileOfWood = GetPileOfWood(eventProvider);
        var combustionChamber = GetCombustionChamber(eventProvider, pileOfWood);
        stove.Items.Add(combustionChamber);
        stove.Items.Add(pileOfWood);
        
        return stove;
    }

    private static Item GetCombustionChamber(EventProvider eventProvider, Item pileOfWood)
    {
        var combustionChamber = new Item()
        {
            Key = Keys.COMBUSTION_CHAMBER,
            Name = Items.COMBUSTION_CHAMBER,
            Description = Descriptions.COMBUSTION_CHAMBER,
            CloseDescription = Descriptions.STOVE_CLOSED,
            OpenDescription = Descriptions.STOVE_OPEN,
            IsSurrounding = true,
            IsPickable = false,
            IsCloseable = true,
            IsClosed = true,
            HideOnContainerClose = false,
            IsContainer = true,
            Grammar = new IndividualObjectGrammar()
        };
        
        combustionChamber.Items.Add(pileOfWood);
        
        AddAfterOpenAndCloseEvents(combustionChamber, eventProvider);

        return combustionChamber;
    }

    private static Item GetPileOfWood(EventProvider eventProvider)
    {
        var wood = new Item()
        {
            Key = Keys.PILE_OF_WOOD,
            Name = Items.PILE_OF_WOOD,
            Description = Descriptions.PILE_OF_WOOD,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum, isAbstract: true)
        };
        
        AddPoorEvents(wood, eventProvider);
        AddKindleEventsForWood(wood, eventProvider);

        return wood;
    }

    private static Item GetBooks(EventProvider eventProvider)
    {
        var books = new Item()
        {
            Key = Keys.BOOKS,
            Name = Items.BOOKS,
            Description = eventProvider.GetBookTitle(),
            IsSurrounding = true,
            IsPickable = false,
            IsReadable = true,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        
        AddReadBookEvents(books, eventProvider);
        
        return books;
    }

    private static Item GetBookShelf(EventProvider eventProvider)
    {
        var bookShelf = new Item()
        {
            Key = Keys.BOOKSHELF,
            Name = Items.BOOKSHELF,
            Description = Descriptions.BOOKSHELF,
            ContainmentDescription = Descriptions.BOOKSHELF_CONTAINMENT,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        
        bookShelf.Items.Add(GetBooks(eventProvider));
        
        return bookShelf;
    }

    private static void AddReadBookEvents(Item item, EventProvider eventProvider)
    {
        item.BeforeRead += eventProvider.ReadBooks;
    }

    private static void AddCandleTakeEvents(AHereticObject candle, EventProvider eventProvider)
    {
        candle.BeforeTake += eventProvider.BeforeTakeCandle;
        candle.Take += eventProvider.TakeCandle;
        eventProvider.RegisterScore(nameof(eventProvider.TakeCandle), 1);
    }

    private static void AddChangeLocationEvents(Location room, EventProvider eventProvider)
    {
        room.BeforeEnterLocation += eventProvider.EnterRoomWithoutLight;
    }
    
    private static void AddKindleEventsForWood(Item item, EventProvider eventProvider)
    {
        item.Kindle += eventProvider.UseCandleOrLampOnPileOfWood;
        eventProvider.RegisterScore(nameof(eventProvider.StoveStarted), 1);
    }
    
    private static void AddKindleEventsForNote(Item item, EventProvider eventProvider)
    {
        item.Kindle += eventProvider.UseCandleOrLampOnNote;
        eventProvider.RegisterScore(nameof(eventProvider.StoveStarted), 1);
    }
    
    private static void AddPoorEvents(Item item, EventProvider eventProvider)
    {
        item.Use += eventProvider.PoorPetroleumInStove;
    }
    
    private static void AddPoorInLampEvents(Item item, EventProvider eventProvider)
    {
        item.Use += eventProvider.PoorPetroleumInPetroleumLamp;
    }
    
    private static void AddAfterOpenAndCloseEvents(Item item, EventProvider eventProvider)
    {
        item.AfterOpen += eventProvider.OpenStoveOrCombustionChamber;
        item.AfterClose += eventProvider.CloseStoveOrCombustionChamber;
    }
    
    private static void AddCandleDropEvents(Item candle, EventProvider eventProvider)
    {
        candle.BeforeDrop += eventProvider.CantDropCandleInStove;
        
        candle.AfterDrop += eventProvider.DropCandle;
        candle.AfterPutOn+= eventProvider.DropCandle;
    }
    
    private static void AddReadEvents(Item note, EventProvider eventProvider)
    {
        note.AfterRead += eventProvider.ReadNote;
        eventProvider.RegisterScore(nameof(eventProvider.ReadNote), 1);
    }
    
    private static void AddDropEvents(Item note, EventProvider eventProvider)
    {
        note.BeforeDrop += eventProvider.BeforeDropNoteInStove;
        note.Drop += eventProvider.PutNoteInStove;
    }

    private static void AddSurroundings(Location livingRoom)
    {
        var plank = new Item()
        {
            Key = Keys.PLANK,
            Name = Items.PLANK,
            Description = Descriptions.PLANK,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Female, false)
        };
        livingRoom.Items.Add(plank);

        var keyHole = new Item()
        {
            Key = Keys.KEY_HOLE,
            Name = Items.KEY_HOLE,
            Description = Descriptions.KEY_HOLE,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        livingRoom.Items.Add(keyHole);
        
        var keyHoleShield = new Item()
        {
            Key = Keys.KEY_HOLE_SHIELD,
            Name = Items.KEY_HOLE_SHIELD,
            Description = Descriptions.KEY_HOLE_SHIELD,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        livingRoom.Items.Add(keyHoleShield);
        
        var chestLock = new Item()
        {
            Key = Keys.CHEST_LOCK,
            Name = Items.CHEST_LOCK,
            Description = Descriptions.CHEST_LOCK,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        livingRoom.Items.Add(chestLock);
        
        var wall = new Item()
        {
            Key = Keys.WALL,
            Name = Items.WALL,
            Description = Descriptions.WALL,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar()
        };
        livingRoom.Items.Add(wall);
        
        var floor = new Item()
        {
            Key = Keys.FLOOR,
            Name = Items.FLOOR,
            Description = Descriptions.FLOOR,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };
        livingRoom.Items.Add(floor);
        
        var ceiling = new Item()
        {
            Key = Keys.CEILING,
            Name = Items.CEILING,
            Description = Descriptions.CEILING,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar()
        };
        livingRoom.Items.Add(ceiling);
        
        var shutter = new Item()
        {
            Key = Keys.SHUTTER,
            Name = Items.SHUTTER,
            Description = Descriptions.SHUTTER,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(isSingular:false)
        };
        livingRoom.Items.Add(shutter);
        
        var inspectionWindows = new Item()
        {
            Key = Keys.INSPECTION_WINDOW,
            Name = Items.INSPECTION_WINDOW,
            Description = Descriptions.INSPECTION_WINDOW,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        livingRoom.Items.Add(inspectionWindows);

        var chimney = new Item()
        {
            Key = Keys.CHIMNEY,
            Name = Items.CHIMNEY,
            Description = Descriptions.CHIMNEY,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };
        livingRoom.Items.Add(chimney);
    }
}