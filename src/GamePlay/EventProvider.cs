using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
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
            printingSubsystem.Resource(Descriptions.CANDLE_PICKUP);
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

    internal void OpenCombustionChamber(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Location { Key: Keys.LIVINGROOM } && eventArgs.ExternalItemKey == Keys.COMBUSTION_CHAMBER)
        {
            var item = this.universe.GetObjectFromWorldByKey(Keys.STOVE);
            if (!item.IsClosed)
            {
                printingSubsystem.ItemAlreadyOpen(item);
            }
            else
            {
                item.IsClosed = false;
                this.universe.UnveilFirstLevelObjects(item);
                var result = printingSubsystem.ItemOpen(item);
            }
        }
    }
    
    internal void CloseCombustionChamber(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Location { Key: Keys.LIVINGROOM } && eventArgs.ExternalItemKey == Keys.COMBUSTION_CHAMBER)
        {
            var item = this.universe.GetObjectFromWorldByKey(Keys.STOVE);
            if (item.IsClosed)
            {
                printingSubsystem.ItemAlreadyClosed(item);
            }
            else
            {
                item.IsClosed = true;
                this.HideItemsOnClose(item);
                printingSubsystem.ItemClosed(item);
            }
        }
    }
    
    private void HideItemsOnClose(AHereticObject item)
    {
        if (item.IsClosed)
        {
            foreach (var child in item.Items.Where(x => x.HideOnContainerClose))
            {
                child.IsHidden = true;
            }
        }
    }

    internal void UseCandleWithPileOfWood(object sender, UseItemEventArgs eventArgs)
    {
        if (sender is Item itemOne && eventArgs.ItemToUse is Item itemTwo && itemOne.Key != itemTwo.Key)
        {
            var itemList = new List<Item> { itemOne, itemTwo };
            var candle = itemList.SingleOrDefault(i => i.Key == Keys.CANDLE);
            var wood = itemList.SingleOrDefault(i => i.Key == Keys.PILE_OF_WOOD);

            if (candle != default && wood != default)
            {
                if (!this.universe.ActivePlayer.Items.Contains(candle))
                {
                    throw new UseException(BaseDescriptions.ITEM_NOT_OWNED);     
                }
                
                var stove = this.universe.ActiveLocation.GetItemByKey(Keys.STOVE);
                if (stove is { IsClosed: true })
                {
                    throw new UseException(Descriptions.STOVE_MUST_BE_OPEN);
                }
                
                candle.Use -= UseCandleWithPileOfWood;
                wood.Use -= UseCandleWithPileOfWood;

                this.universe.Score += this.universe.ScoreBoard[nameof(UseCandleWithPileOfWood)];
                this.universe.SolveQuest(MetaData.QUEST_II);
            }
            else
            {
                throw new UseException(BaseDescriptions.DOES_NOT_WORK);
            }
        }
    }
    
}