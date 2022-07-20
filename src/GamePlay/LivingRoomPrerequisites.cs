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
            FirstLookDescription = Descriptions.CANDLE_CONTAINMENT
        };

        livingRoom.Items.Add(GetTable(eventProvider));
        livingRoom.Items.Add(GetChest());
        livingRoom.Items.Add(GetStove(eventProvider));
        livingRoom.Items.Add(GetKitchenCabinet());
        livingRoom.Items.Add(GetDoor());

        AddChangeLocationEvents(livingRoom, eventProvider);
        AddOpenEvents(livingRoom, eventProvider);
        AddCloseEvents(livingRoom, eventProvider);
        
        AddSurroundings(livingRoom);

        return livingRoom;
    }

    private static Item GetKitchenCabinet()
    {
        var cabinet = new Item
        {
            Key = Keys.KITCHEN_CABINET,
            Name = Items.KITCHEN_CABINET,
            Description = Descriptions.KITCHEN_CABINET,
            ContainmentDescription = Descriptions.KITCHEN_CABINET_CONTAINMENT,
            IsPickAble = false,
            IsContainer = true,
            Grammar = new Grammars(Genders.Male)
        };
        
        cabinet.Items.Add(GetSausage());
        cabinet.Items.Add(GetLampOilBucket());

        return cabinet;
    }

    private static Item GetSausage()
    {
        var sausage = new Item
        {
            Key = Keys.SAUSAGE,
            Name = Items.SAUSAGE,
            Description = Descriptions.SAUSAGE,
            ContainmentDescription = Descriptions.SAUSAGE_CONTAINMENT,
            IsHidden = true,
            Grammar = new Grammars(Genders.Male)
        };
        
        return sausage;
    }

    private static Item GetLampOilBucket()
    {
        var lampOilBucket = new Item
        {
            Key = Keys.LAMP_OIL_BUCKET,
            Name = Items.LAMP_OIL_BUCKET,
            Description = Descriptions.LAMP_OIL_BUCKET,
            ContainmentDescription = Descriptions.LAMP_OIL_BUCKET_CONTAINMENT,
            IsHidden = true
        };
        
        lampOilBucket.Items.Add(GetPetroleum());

        return lampOilBucket;
    }

    private static Item GetPetroleum()
    {
        var petroleum = new Item
        {
            Key = Keys.PETROLEUM,
            Name = Items.PETROLEUM,
            Description = Descriptions.PETROLEUM,
            IsHidden = true,
            IsPickAble = false,
            Grammar = new Grammars(Genders.Neutrum)
        };

        return petroleum;
    }

    private static Item GetTable(EventProvider eventProvider)
    {
        var table = new Item()
        {
            Key = Keys.TABLE,
            Name = Items.TABLE,
            Description = Descriptions.TABLE,
            IsPickAble = false,
            IsContainer = true,
            IsSurfaceContainer = true,
            Grammar = new Grammars(Genders.Male)
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
            ContainmentDescription = Descriptions.CANDLE_CONTAINMENT
        };
        
        candle.Items.Add(GetIronKey());

        AddAfterTakeEvents(candle, eventProvider);
        AddUseEvents(candle, eventProvider);
        AddBeforeDropEvents(candle, eventProvider);

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
            LetterContentDescription = Descriptions.NOTE_LETTER_CONTENT,
            Grammar = new Grammars(Genders.Neutrum)
        };
        
        AddReadEvents(note, eventProvider);
        
        return note;
    }

    private static Item GetIronKey()
    {
        var ironKey = new Item()
        {
            Key = Keys.IRON_KEY,
            Name = Items.IRON_KEY,
            Description = Descriptions.IRON_KEY,
            IsHidden = true,
            IsUnveilAble = false,
            Grammar = new Grammars(Genders.Male)
        };

        return ironKey;
    }

    private static Item GetDoor()
    {
        var door = new Item()
        {
            Key = Keys.DOOR,
            Name = Items.DOOR,
            Description = Descriptions.DOOR,
            FirstLookDescription = Descriptions.DOOR_FIRSTLOOK,
            IsPickAble = false,
            IsLocked = true,
            IsClosed = true,
            IsCloseAble = true
        };

        return door;
    }

    private static Item GetChest()
    {
        var chest = new Item()
        {
            Key = Keys.CHEST,
            Name = Items.CHEST,
            Description = Descriptions.CHEST,
            LockDescription = Descriptions.CHEST_LOCKED,
            IsPickAble = false,
            IsSeatAble = true,
            IsLocked = true,
            IsClosed = true,
            IsCloseAble = true
        };

        return chest;
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
            IsPickAble = false,
            IsClosed = true,
            IsCloseAble = true,
            IsContainer = true,
            Grammar = new Grammars(Genders.Male)
        };

        stove.Items.Add(GetPileOfWood(eventProvider));
        stove.LinkedTo.Add(GetCookTop(eventProvider));

        return stove;
    }

    private static Item GetPileOfWood(EventProvider eventProvider)
    {
        var wood = new Item()
        {
            Key = Keys.PILE_OF_WOOD,
            Name = Items.PILE_OF_WOOD,
            Description = Descriptions.PILE_OF_WOOD,
            IsPickAble = false,
            Grammar = new Grammars(Genders.Neutrum, false)
        };

        AddUseEvents(wood, eventProvider);

        return wood;
    }

    private static Item GetCookTop(EventProvider eventProvider)
    {
        var cookTop = new Item
        {
            Key = Keys.COOKTOP,
            Name = Items.COOKTOP,
            Description = Descriptions.COOKTOP,
            LinkedToDescription = Descriptions.COOCKTOP_LINKEDTO,
            IsPickAble = false,
            Grammar = new Grammars(Genders.Neutrum)
        };

        return cookTop;
    }

    private static void AddAfterTakeEvents(Item item, EventProvider eventProvider)
    {
        item.AfterTake += eventProvider.TakeCandle;
        eventProvider.ScoreBoard.Add(nameof(eventProvider.TakeCandle), 1);
    }

    private static void AddChangeLocationEvents(Location room, EventProvider eventProvider)
    {
        room.BeforeChangeLocation += eventProvider.ChangeRoomWithoutLight;
    }

    private static void AddUseEvents(Item item, EventProvider eventProvider)
    {
        item.Use += eventProvider.UseCandleWithPileOfWood;
        if (!eventProvider.ScoreBoard.ContainsKey(nameof(eventProvider.UseCandleWithPileOfWood)))
        {
            eventProvider.ScoreBoard.Add(nameof(eventProvider.UseCandleWithPileOfWood), 1);
        }
    }
    
    private static void AddBeforeDropEvents(Item candle, EventProvider eventProvider)
    {
        candle.BeforeDrop += eventProvider.CantDropCandleInStove;
    }
    
    private static void AddOpenEvents(Location room, EventProvider eventProvider)
    {
        room.Open += eventProvider.OpenCombustionChamber;
    }
    
    private static void AddCloseEvents(Location room, EventProvider eventProvider)
    {
        room.Close += eventProvider.CloseCombustionChamber;
    }
    
    private static void AddReadEvents(Item note, EventProvider eventProvider)
    {
        note.AfterRead += eventProvider.ReadNote;
        eventProvider.ScoreBoard.Add(nameof(eventProvider.ReadNote), 1);
    }

    private static void AddSurroundings(Location livingRoom)
    {
        var plank = new Item()
        {
            Key = Keys.PLANK,
            Name = Items.PLANK,
            Description = Descriptions.PLANK,
            IsSurrounding = true,
            IsPickAble = false,
            Grammar = new Grammars(Genders.Female, false)
        };
        livingRoom.Items.Add(plank);

        var keyHole = new Item()
        {
            Key = Keys.KEY_HOLE,
            Name = Items.KEY_HOLE,
            Description = Descriptions.KEY_HOLE,
            IsSurrounding = true,
            IsPickAble = false,
            Grammar = new Grammars(Genders.Neutrum)
        };
        livingRoom.Items.Add(keyHole);
        
        var keyHoleShield = new Item()
        {
            Key = Keys.KEY_HOLE_SHIELD,
            Name = Items.KEY_HOLE_SHIELD,
            Description = Descriptions.KEY_HOLE_SHIELD,
            IsSurrounding = true,
            IsPickAble = false,
            Grammar = new Grammars(Genders.Neutrum)
        };
        livingRoom.Items.Add(keyHoleShield);
        
        var chestLock = new Item()
        {
            Key = Keys.CHEST_LOCK,
            Name = Items.CHEST_LOCK,
            Description = Descriptions.CHEST_LOCK,
            IsSurrounding = true,
            IsPickAble = false,
            Grammar = new Grammars(Genders.Neutrum)
        };
        livingRoom.Items.Add(chestLock);
        
        var wall = new Item()
        {
            Key = Keys.WALL,
            Name = Items.WALL,
            Description = Descriptions.WALL,
            IsSurrounding = true,
            IsPickAble = false,
            Grammar = new Grammars()
        };
        livingRoom.Items.Add(wall);
        
        var floor = new Item()
        {
            Key = Keys.FLOOR,
            Name = Items.FLOOR,
            Description = Descriptions.FLOOR,
            IsSurrounding = true,
            IsPickAble = false,
            Grammar = new Grammars(Genders.Male)
        };
        livingRoom.Items.Add(floor);
        
        var ceiling = new Item()
        {
            Key = Keys.CEILING,
            Name = Items.CEILING,
            Description = Descriptions.CEILING,
            IsSurrounding = true,
            IsPickAble = false,
            Grammar = new Grammars()
        };
        livingRoom.Items.Add(ceiling);
        
        var livingRoomWindows = new Item()
        {
            Key = Keys.LIVINGROOM_WINDOW,
            Name = Items.LIVINGROOM_WINDOW,
            Description = Descriptions.LIVINGROOM_WINDOW,
            IsSurrounding = true,
            IsPickAble = false,
            Grammar = new Grammars(Genders.Neutrum)
        };
        livingRoom.Items.Add(livingRoomWindows);
        
        var shutter = new Item()
        {
            Key = Keys.SHUTTER,
            Name = Items.SHUTTER,
            Description = Descriptions.SHUTTER,
            IsSurrounding = true,
            IsPickAble = false,
            Grammar = new Grammars(isSingular:false)
        };
        livingRoom.Items.Add(shutter);
        
        var inspectionWindows = new Item()
        {
            Key = Keys.INSPECTION_WINDOW,
            Name = Items.INSPECTION_WINDOW,
            Description = Descriptions.INSPECTION_WINDOW,
            IsSurrounding = true,
            IsPickAble = false,
            Grammar = new Grammars(Genders.Neutrum)
        };
        livingRoom.Items.Add(inspectionWindows);
        
        var combustionChamber = new Item()
        {
            Key = Keys.COMBUSTION_CHAMBER,
            Name = Items.COMBUSTION_CHAMBER,
            Description = Descriptions.COMBUSTION_CHAMBER,
            IsSurrounding = true,
            IsPickAble = false,
            Grammar = new Grammars()
        };
        livingRoom.Items.Add(combustionChamber);
        
        var bookShelf = new Item()
        {
            Key = Keys.BOOKSHELF,
            Name = Items.BOOKSHELF,
            Description = Descriptions.BOOKSHELF,
            IsSurrounding = true,
            IsPickAble = false,
            Grammar = new Grammars(Genders.Neutrum)
        };
        livingRoom.Items.Add(bookShelf);
        
        var chimney = new Item()
        {
            Key = Keys.CHIMNEY,
            Name = Items.CHIMNEY,
            Description = Descriptions.CHIMNEY,
            IsSurrounding = true,
            IsPickAble = false,
            Grammar = new Grammars(Genders.Male)
        };
        livingRoom.Items.Add(chimney);
        
        var books = new Item()
        {
            Key = Keys.BOOKS,
            Name = Items.BOOKS,
            Description = GetBookTitle(),
            IsSurrounding = true,
            IsPickAble = false,
            Grammar = new Grammars(Genders.Neutrum)
        };
        livingRoom.Items.Add(books);
    }

    
    private static Func<string> GetBookTitle()
    {
        var bookList = new List<string>
        {
            Descriptions.BOOK_I, Descriptions.BOOK_II, Descriptions.BOOK_III,
            Descriptions.BOOK_IV, Descriptions.BOOK_V
        }; 
        
        var rnd = new Random();
        
        return () => string.Format(Descriptions.BOOKS, bookList[rnd.Next(0, bookList.Count)]);
    }
}