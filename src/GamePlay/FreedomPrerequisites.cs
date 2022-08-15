using Heretic.InteractiveFiction.Objects;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal static class FreedomPrerequisites
{
    internal static Location Get()
    {
        var freedom = new Location()
        {
            Key = Keys.FREEDOM,
            Name = Locations.FREEDOM,
            Description = Descriptions.FREEDOM,
            IsLocked = true,
            IsLockAble = true,
            LockDescription = Descriptions.FREEDOM_LOCK_DESCRIPTION
        };
        
        return freedom;
    }
}