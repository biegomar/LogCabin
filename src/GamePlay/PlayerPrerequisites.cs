using Heretic.InteractiveFiction.Objects;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal static class PlayerPrerequisites
{
    internal static Player Get()
    {
        var player = new Player()
        {
            Key = Keys.PLAYER,
            Name = "",
            Description = Descriptions.PLAYER,
            Grammar = new Grammars(Genders.Male, isPlayer:true)
        };

        return player;
    }
}