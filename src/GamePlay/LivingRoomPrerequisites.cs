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
        table.Items.Add(GetNote());
        
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

        AddAfterTakeEvents(candle, eventProvider);
        AddUseEvents(candle, eventProvider);

        return candle;
    }

    private static Item GetNote()
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
        
        return note;
    }

    private static Item GetIronKey()
    {
        var ironKey = new Item()
        {
            Key = Keys.IRON_KEY,
            Name = Items.IRON_KEY,
            Description = Descriptions.IRON_KEY,
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
    
    private static void AddOpenEvents(Location room, EventProvider eventProvider)
    {
        room.Open += eventProvider.OpenCombustionChamber;
    }
    
    private static void AddCloseEvents(Location room, EventProvider eventProvider)
    {
        room.Close += eventProvider.CloseCombustionChamber;
    }

    private static void AddSurroundings(Location livingRoom)
    {
        livingRoom.Surroundings.Add(Keys.PLANK, () => Descriptions.PLANK);
        livingRoom.Surroundings.Add(Keys.KEY_HOLE, () => Descriptions.KEY_HOLE);
        livingRoom.Surroundings.Add(Keys.KEY_HOLE_SHIELD, () => Descriptions.KEY_HOLE_SHIELD);
        livingRoom.Surroundings.Add(Keys.CHEST_LOG, () => Descriptions.CHEST_LOG);
        livingRoom.Surroundings.Add(Keys.WALL, () => Descriptions.WALL);
        livingRoom.Surroundings.Add(Keys.FLOOR, () => Descriptions.FLOOR);
        livingRoom.Surroundings.Add(Keys.CEILING, () => Descriptions.CEILING);
        livingRoom.Surroundings.Add(Keys.LIVINGROOM_WINDOW, () => Descriptions.LIVINGROOM_WINDOW);
        livingRoom.Surroundings.Add(Keys.SHUTTER, () => Descriptions.SHUTTER);
        livingRoom.Surroundings.Add(Keys.INSPECTION_WINDOW, () => Descriptions.INSPECTION_WINDOW);
        livingRoom.Surroundings.Add(Keys.COMBUSTION_CHAMBER, () => Descriptions.COMBUSTION_CHAMBER);
    }
}