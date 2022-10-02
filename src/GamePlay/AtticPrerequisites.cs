using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal static class AtticPrerequisites
{
    internal static Location Get(EventProvider eventProvider)
    {
        var attic = new Location()
        {
            Key = Keys.ATTIC,
            Name = Locations.ATTIC,
            Description = Descriptions.ATTIC,
            Grammar = new Grammars(Genders.Male)
        };
        
        AddChangeLocationEvents(attic, eventProvider);
        
        return attic;
    }   
    
    private static void AddChangeLocationEvents(Location room, EventProvider eventProvider)
    {
        room.BeforeChangeLocation += eventProvider.ChangeRoomWithoutLight;
    }
}