using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal class EventProvider
{
    private readonly Universe universe;
    private readonly IPrintingSubsystem printingSubsystem;

    internal IDictionary<string, int> ScoreBoard => this.universe.ScoreBoard;

    public EventProvider(Universe universe, IPrintingSubsystem printingSubsystem)
    {
        this.printingSubsystem = printingSubsystem;
        this.universe = universe;
    }
    
    internal void TakeCandle(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.CANDLE } candle)
        {
            this.universe.Score += this.universe.ScoreBoard[nameof(this.TakeCandle)];
            candle.AfterTake -= this.TakeCandle;
        }
    }
    
    internal void ChangeRoomWithoutLight(object sender, ChangeLocationEventArgs eventArgs)
    {
        if (sender is Location)
        {
            if (this.universe.ActivePlayer.Items.All(x => x.Key != Keys.CANDLE))
            {
                throw new BeforeChangeLocationException(Descriptions.CANT_LEAVE_ROOM_WITHOUT_LIGHT); 
            }
        }
    }
    
}