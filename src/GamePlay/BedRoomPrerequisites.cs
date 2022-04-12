using Heretic.InteractiveFiction.Objects;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal static class BedRoomPrerequisites
{
    internal static Location Get(EventProvider eventProvider)
    {
        var bedRoom = new Location()
        {
            Key = Keys.BEDROOM,
            Name = Locations.BEDROOM,
            Description = Descriptions.BEDROOM,
            Grammar = new Grammars(Genders.Neutrum)
        };
        
        AddChangeLocationEvents(bedRoom, eventProvider);
        
        return bedRoom;
    }
    
    private static void AddChangeLocationEvents(Location room, EventProvider eventProvider)
    {
        room.BeforeChangeLocation += eventProvider.ChangeRoomWithoutLight;
    }
}