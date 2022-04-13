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
            Description = Descriptions.LIVINGROOM
        };
        
        livingRoom.Items.Add(GetChest());
        livingRoom.Items.Add(GetCandle(eventProvider));
        livingRoom.Items.Add(GetStove(eventProvider));
        livingRoom.Items.Add(GetDoor());
        
        AddChangeLocationEvents(livingRoom, eventProvider);

        AddSurroundings(livingRoom);

        return livingRoom;
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
    
    private static Item GetStove(EventProvider eventProvider)
    {
        var stove = new Item()
        {
            Key = Keys.STOVE,
            Name = Items.STOVE,
            Description = Descriptions.STOVE,
            FirstLookDescription = Descriptions.STOVE_FIRSTLOOK,
            CloseDescription = Descriptions.STOVE_CLOSED,
            IsPickAble = false,
            IsClosed = true,
            IsCloseAble = true,
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
            Grammar = new Grammars(Genders.Neutrum)
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

    private static void AddSurroundings(Location livingRoom)
    {
        livingRoom.Surroundings.Add(Keys.PLANK, () => Descriptions.PLANK);
        livingRoom.Surroundings.Add(Keys.KEY_HOLE, () => Descriptions.KEY_HOLE);
        livingRoom.Surroundings.Add(Keys.TABLE, () => Descriptions.TABLE);
    }
}