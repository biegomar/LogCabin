using Heretic.InteractiveFiction.Objects;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

public class CellarPrerequisites
{
    internal static Location Get()
    {
        var bedRoom = new Location()
        {
            Key = Keys.CELLAR,
            Name = Locations.CELLAR,
            Description = Descriptions.CELLAR,
            Grammar = new Grammars(Genders.Male)
        };
        
        return bedRoom;
    }
}