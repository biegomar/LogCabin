using System.Runtime.CompilerServices;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal static class BedRoomPrerequisites
{
    internal static Location Get(EventProvider eventProvider)
    {
        var bedRoom = new Location()
        {
            Key = Keys.BEDROOM,
            Name = Locations.BEDROOM,
            Description = Descriptions.BEDROOM,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        
        bedRoom.AddOptionalVerb(VerbKey.USE, OptionalVerbs.POOR, string.Empty);
        bedRoom.AddOptionalVerb(VerbKey.USE, OptionalVerbs.HOLD, Descriptions.NOTHING_TO_HOLD);

        bedRoom.Items.Add(GetPetroleumLamp(eventProvider));
        
        AddChangeLocationEvents(bedRoom, eventProvider);
        
        return bedRoom;
    }
    
    private static Item GetPetroleumLamp(EventProvider eventProvider)
    {
        var lamp = new Item()
        {
            Key = Keys.PETROLEUM_LAMP,
            Name = Items.PETROLEUM_LAMP,
            Description = Descriptions.PETROLEUM_LAMP,
            FirstLookDescription = Descriptions.PETROLEUM_LAMP_FIRSTLOOK,
            IsLighter = true,
            LighterSwitchedOffDescription = Descriptions.LIGHTER_OFF,
            LighterSwitchedOnDescription = Descriptions.LIGHTER_ON
        };

        AddKindleEvents(lamp, eventProvider);
        AddPoorEvents(lamp, eventProvider);
        
        return lamp;
    }
    
    private static void AddChangeLocationEvents(Location room, EventProvider eventProvider)
    {
        room.BeforeEnterLocation += eventProvider.EnterRoomWithoutLight;
    }
    
    private static void AddKindleEvents(Item lamp, EventProvider eventProvider)
    {
        lamp.Kindle += eventProvider.UseCandleOrMatchOnLamp;
        eventProvider.RegisterScore(nameof(eventProvider.StartPetroleumLamp), 1);
    }
    
    private static void AddPoorEvents(Item item, EventProvider eventProvider)
    {
        item.Use += eventProvider.PoorPetroleumInPetroleumLamp;
        eventProvider.RegisterScore(nameof(eventProvider.PoorPetroleumInPetroleumLamp), 1);
    }
}