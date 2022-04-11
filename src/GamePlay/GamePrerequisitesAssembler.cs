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
        var cellar = CellarPrerequisites.Get();
        
        var map = new LocationMap(new LocationComparer())
        {
            { livingRoom, LivingRoomLocationMap(bedRoom, cellar) },
            { bedRoom, BedRoomLocationMap(livingRoom) },
            { cellar, CellarLocationMap(livingRoom) }
        };

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
    
    private static IEnumerable<DestinationNode> LivingRoomLocationMap(Location bedRoom, Location cellar)
    {
        var locationMap = new List<DestinationNode>
        {
            new() {Direction = Directions.N, Location = bedRoom},
            new() {Direction = Directions.DOWN, Location = cellar}
        };
        return locationMap;
    }
    
    private static IEnumerable<DestinationNode> BedRoomLocationMap(Location livingRoom)
    {
        var locationMap = new List<DestinationNode>
        {
            new() {Direction = Directions.S, Location = livingRoom},
        };
        return locationMap;
    }
    
    private static IEnumerable<DestinationNode> CellarLocationMap(Location livingRoom)
    {
        var locationMap = new List<DestinationNode>
        {
            new() {Direction = Directions.UP, Location = livingRoom},
        };
        return locationMap;
    }
}