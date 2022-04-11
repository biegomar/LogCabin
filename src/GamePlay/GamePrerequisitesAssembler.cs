using Heretic.InteractiveFiction.Comparer;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Objects;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal sealed class GamePrerequisitesAssembler: IGamePrerequisitesAssembler
{
    public GamePrerequisites AssembleGame()
    {
        var livingRoom = LivingRoomPrerequisites.Get();
        var bedRoom = BedRoomPrerequisites.Get();
        
        var map = new LocationMap(new LocationComparer()) { { livingRoom, LivingRoomLocationMap(bedRoom) } };

        var activeLocation = livingRoom;
        var activePlayer = PlayerPrerequisites.Get();
        var actualQuests = GetQuests();
        
        return new GamePrerequisites(map, activeLocation, activePlayer, null, actualQuests);
    }

    private static ICollection<string> GetQuests()
    {
        var result = new List<string>
        {
            Descriptions.QUEST_I
        };

        return result;
    }
    
    private static IEnumerable<DestinationNode> LivingRoomLocationMap(Location bedRoom)
    {
        var locationMap = new List<DestinationNode>
        {
            new() {Direction = Directions.N, Location = bedRoom},
        };
        return locationMap;
    }
}