using Heretic.InteractiveFiction.Objects;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal static class LivingRoomPrerequisites
{
    internal static Location Get()
    {
        var livingRoom = new Location()
        {
            Key = Keys.LIVINGROOM,
            Name = Locations.LIVINGROOM,
            Description = Descriptions.LIVINGROOM
        };
        
        livingRoom.Items.Add(GetChest());
        livingRoom.Items.Add(GetCandle());
        livingRoom.Items.Add(GetDoor());
        
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
            IsPickAble = false,
            IsSeatAble = true,
            IsLocked = true,
            IsClosed = true,
            IsCloseAble = true
        };

        return chest;
    }

    private static Item GetCandle()
    {
        var candle = new Item()
        {
            Key = Keys.CANDLE,
            Name = Items.CANDLE,
            Description = Descriptions.CANDLE,
            ContainmentDescription = Descriptions.CANDLE_CONTAINMENT
        };
        
        return candle;
    }

    private static void AddSurroundings(Location livingRoom)
    {
        livingRoom.Surroundings.Add(Keys.PLANK, () => Descriptions.PLANK);
        livingRoom.Surroundings.Add(Keys.KEY_HOLE, () => Descriptions.KEY_HOLE);
        livingRoom.Surroundings.Add(Keys.TABLE, () => Descriptions.TABLE);
    }
}