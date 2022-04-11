using Heretic.InteractiveFiction.Objects;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal static class AtticPrerequisites
{
    internal static Location Get()
    {
        var attic = new Location()
        {
            Key = Keys.ATTIC,
            Name = Locations.ATTIC,
            Description = Descriptions.ATTIC,
            Grammar = new Grammars(Genders.Male)
        };
        
        return attic;
    }   
}