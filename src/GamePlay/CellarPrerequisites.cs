using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal static class CellarPrerequisites
{
    internal static Location Get(EventProvider eventProvider)
    {
        var bedRoom = new Location()
        {
            Key = Keys.CELLAR,
            Name = Locations.CELLAR,
            Description = Descriptions.CELLAR,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };
        
        AddChangeLocationEvents(bedRoom, eventProvider);
        
        return bedRoom;
    }
    
    private static void AddChangeLocationEvents(Location room, EventProvider eventProvider)
    {
        room.BeforeEnterLocation += eventProvider.EnterRoomWithoutLight;
    }
}