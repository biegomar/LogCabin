using Heretic.InteractiveFiction.Objects;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal static class BedRoomPrerequisites
{
    internal static Location Get()
    {
        var bedRoom = new Location()
        {
            Key = Keys.BEDROOM,
            Name = Locations.BEDROOM,
            Description = Descriptions.BEDROOM,
            Grammar = new Grammars(Genders.Neutrum)
        };
        
        return bedRoom;
    }
}