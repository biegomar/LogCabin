using Heretic.InteractiveFiction.Objects;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal static class PlayerPrerequisites
{
    internal static Player Get(EventProvider eventProvider)
    {
        var player = new Player()
        {
            Key = Keys.PLAYER,
            Name = "",
            Description = Descriptions.PLAYER,
            Grammar = new Grammars(Genders.Male, isPlayer:true)
        };
        
        AddToBeEvents(player, eventProvider);

        return player;
    }
    
    private static void AddToBeEvents(Player you, EventProvider eventProvider)
    {
        you.ToBe += eventProvider.SetPlayersName;
    }
}