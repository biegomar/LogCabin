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
    private bool isPaperInStove;
    private bool isPetroleumInStove;
    private bool isPetroleumInLamp;

    internal IDictionary<string, int> ScoreBoard => this.universe.ScoreBoard;

    public EventProvider(Universe universe, IPrintingSubsystem printingSubsystem)
    {
        this.printingSubsystem = printingSubsystem;
        this.universe = universe;
    }

    internal void UnhideMainEntrance(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item {Key: Keys.DOOR})
        {
            var destination = this.universe.GetDestinationNodeFromActiveLocationByDirection(Directions.S);
            destination.IsHidden = false;
        }
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
            if (!this.universe.ActivePlayer.Items.Any(x => x.IsLighter && x.IsLighterSwitchedOn))
            {
                throw new BeforeChangeLocationException(Descriptions.CANT_LEAVE_ROOM_WITHOUT_LIGHT); 
            }
        }
    }

    internal void WaitInLivingRoom(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Location {Key: Keys.LIVINGROOM})
        {
            var candle = this.universe.GetObjectFromWorld(Keys.CANDLE);
            if (candle is { IsHidden: false })
            {
                throw new WaitException(Descriptions.LIVINGROOM_WAIT);
            }
        }
        
        throw new WaitException(BaseDescriptions.TIME_GOES_BY);
    }
    
    internal void ReadNote(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.NOTE } note)
        {
            this.universe.Score += this.universe.ScoreBoard[nameof(this.ReadNote)];
            note.AfterRead -= this.ReadNote;
        }
    }
    
    internal void PutNoteInStove(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.NOTE })
        {
            if (this.isPetroleumInStove)
            {
                throw new DropException(Descriptions.PETROLEUM_IN_STOVE);
            }
            
            this.isPaperInStove = true;
        }
    }

    internal void PoorPetroleumInStove(object sender, UseItemEventArgs eventArgs)
    {
        if (sender is Item itemOne && eventArgs.ItemToUse is Item itemTwo && itemOne.Key != itemTwo.Key)
        {
            var itemList = new List<Item> { itemOne, itemTwo };
            var petroleum = itemList.SingleOrDefault(i => i.Key == Keys.PETROLEUM);
            var destinationItem = itemList.SingleOrDefault(i => i.Key == Keys.STOVE);

            if (destinationItem == default)
            {
                destinationItem = itemList.SingleOrDefault(i => i.Key == Keys.PILE_OF_WOOD);
            }

            if (petroleum != default && destinationItem != default)
            {
                if (!this.universe.ActivePlayer.OwnsItem(petroleum))
                {
                    throw new UseException(BaseDescriptions.ITEM_NOT_OWNED);     
                }
                
                var stove = this.universe.ActiveLocation.GetItem(Keys.STOVE);
                if (stove is { IsClosed: true })
                {
                    throw new UseException(Descriptions.STOVE_MUST_BE_OPEN);
                }

                if (this.isPetroleumInStove)
                {
                    throw new UseException(Descriptions.PETROLEUM_IN_STOVE);
                }
                
                if (this.isPaperInStove)
                {
                    throw new UseException(Descriptions.PAPER_IN_STOVE);
                }

                PreparePileOfWoodWithPetroleum();
            }
        }
    }

    internal void PoorPetroleumInPetroleumLamp(object sender, UseItemEventArgs eventArgs)
    {
        if (sender is Item itemOne && eventArgs.ItemToUse is Item itemTwo && itemOne.Key != itemTwo.Key)
        {
            var itemList = new List<Item> { itemOne, itemTwo };
            var petroleum = itemList.SingleOrDefault(i => i.Key == Keys.PETROLEUM);
            var destinationItem = itemList.SingleOrDefault(i => i.Key == Keys.PETROLEUM_LAMP);

            if (petroleum != default && destinationItem != default)
            {
                if (!this.universe.ActivePlayer.OwnsItem(petroleum) || !this.universe.ActivePlayer.OwnsItem(petroleum))
                {
                    throw new UseException(BaseDescriptions.ITEM_NOT_OWNED);     
                }

                if (isPetroleumInLamp)
                {
                    throw new UseException(Descriptions.ENOUGH_PETROLEUM_IN_LAMP);
                }

                isPetroleumInLamp = true;
                destinationItem.FirstLookDescription = Descriptions.PETROLEUM_LAMP_FIRSTLOOK_POORED;
                printingSubsystem.Resource(Descriptions.POOR_PETROLEUM_IN_LAMP);
                this.universe.Score += this.universe.ScoreBoard[nameof(this.PoorPetroleumInPetroleumLamp)];
            }
        }
    }

    private void PreparePileOfWoodWithPetroleum()
    {
        var pileOfWood = this.universe.ActiveLocation.GetItem(Keys.PILE_OF_WOOD);
        pileOfWood.Description = Descriptions.PILE_OF_WOOD_WITH_PETROLEUM;
        this.isPetroleumInStove = true;
        printingSubsystem.Resource(Descriptions.POOR_PETROLEUM_OVER_WOOD);
    }

    internal void UseLightersOnThings(object sender, UseItemEventArgs eventArgs)
    {
        if (sender is Item itemOne && eventArgs.ItemToUse is Item itemTwo && itemOne.Key != itemTwo.Key)
        {
            var itemList = new List<Item> { itemOne, itemTwo };
            
            //Candle and...
            var candle = itemList.SingleOrDefault(i => i.Key == Keys.CANDLE);
            if (candle != default)
            {
                if (!this.universe.ActivePlayer.OwnsItem(candle))
                {
                    throw new UseException(string.Format(BaseDescriptions.ITEM_NOT_OWNED_FORMATTED, candle.AccusativeArticleName.LowerFirstChar()));     
                }
                
                //... pile of wood
                var pileOfWood = itemList.SingleOrDefault(i => i.Key == Keys.PILE_OF_WOOD);
                if (pileOfWood != default)
                {
                    StartFireInStoveWithLighterAndWood();
                }
                else
                {
                    //... note
                    var note = itemList.SingleOrDefault(i => i.Key == Keys.NOTE);
                    if (note != default)
                    {
                        StartFireInStoveWithLighterAndNote(candle, note);
                    }
                    else
                    {
                        //... petroleum lamp
                        var lamp = itemList.SingleOrDefault(i => i.Key == Keys.PETROLEUM_LAMP);
                        if (lamp != default)
                        {
                            StartPetroleumLampWithCandle(lamp);
                        }
                    }
                }
            }
            else
            {
                //Petroleum lamp and...
                var petroleumLamp = itemList.SingleOrDefault(i => i.Key == Keys.PETROLEUM_LAMP);
                if (petroleumLamp != default)
                {
                    if (!this.universe.ActivePlayer.OwnsItem(petroleumLamp))
                    {
                        throw new UseException(BaseDescriptions.ITEM_NOT_OWNED);     
                    }
                    
                    if (!petroleumLamp.IsLighterSwitchedOn)
                    {
                        throw new UseException(string.Format(Descriptions.PETROLEUM_LAMP_NOT_BURNING, petroleumLamp.AccusativeArticleName));
                    }
                
                    //... pile of wood
                    var pileOfWood = itemList.SingleOrDefault(i => i.Key == Keys.PILE_OF_WOOD);
                    if (pileOfWood != default)
                    {
                        StartFireInStoveWithLighterAndWood();
                    }
                    else
                    {
                        //... note
                        var note = itemList.SingleOrDefault(i => i.Key == Keys.NOTE);
                        if (note != default)
                        {
                            StartFireInStoveWithLighterAndNote(petroleumLamp, note);
                        }
                    } 
                }
            }
        }

        if (eventArgs.ItemToUse == default)
        {
            printingSubsystem.Resource(Descriptions.HOW_TO_DO);
            return;
        }

        if (sender is Item senderItem && eventArgs.ItemToUse is Item itemToUse && senderItem.Key == itemToUse.Key)
        {
            printingSubsystem.Resource(string.Format(Descriptions.FIRE_FIRE_WITH_FIRE, senderItem.AccusativeArticleName.LowerFirstChar(),
                itemToUse.DativeArticleName.LowerFirstChar()));
        }
    }

    internal void StartPetroleumLampWithCandle(Item lamp)
    {
        if (!this.isPetroleumInLamp)
        {
            throw new UseException(Descriptions.NO_PETROLEUM_IN_LAMP);
        }

        if (lamp.IsLighterSwitchedOn)
        {
            throw new UseException(Descriptions.PETROLEUM_LAMP_IS_BURNING);
        }

        lamp.IsLighterSwitchedOn = true;
        printingSubsystem.Resource(Descriptions.PETROLEUM_LAMP_SWITCH_ON);
        this.universe.Score += this.universe.ScoreBoard[nameof(StartPetroleumLampWithCandle)];
    }

    private void StartFireInStoveWithLighterAndNote(Item lighter, Item note)
    {
        if (this.isPaperInStove)
        {
            CheckIfStoveIsOpen();
            
            RemovePaperFromStove();
            
            printingSubsystem.Resource(Descriptions.NOTE_BURNED);
            printingSubsystem.Resource(Descriptions.FIRE_STARTER);
        
            CloseStoveBecauseItIsToHot();

            AssignEventForCombustionChamber();
        
            this.universe.Score += this.universe.ScoreBoard[nameof(UseLightersOnThings)];
            this.universe.SolveQuest(MetaData.QUEST_II);
        }
        else
        {
            if (!this.universe.ActivePlayer.OwnsItem(note))
            {
                throw new UseException(BaseDescriptions.ITEM_NOT_OWNED);     
            }

            this.universe.ActivePlayer.RemoveItem(note);
            printingSubsystem.FormattedResource(Descriptions.BURN_NOTE, lighter.AccusativeArticleName, true);
        }
    }

    internal void CantDropCandleInStove(object sender, DropItemEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.CANDLE } && eventArgs.ItemContainer is Item {Key: Keys.STOVE})
        {
            throw new DropException(Descriptions.CANT_DROP_CANDLE_IN_STOVE);
        }
    }
    
    internal void CantTakePetroleum(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.PETROLEUM})
        {
            throw new TakeException(Descriptions.CANT_TAKE_PETROLEUM);
        }
    }
    
    private void CantOpenStoveOnFire(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.STOVE or Keys.COMBUSTION_CHAMBER})
        {
            throw new OpenException(Descriptions.CANT_OPEN_STOVE_ON_FIRE);
        }
    }
    
    private void StartFireInStoveWithLighterAndWood()
    {
        CheckIfStoveIsOpen();

        CheckForFireStarters();
                
        if (this.isPaperInStove)
        {
            RemovePaperFromStove();
            printingSubsystem.Resource(Descriptions.NOTE_BURNED);
        }
        else if (this.isPetroleumInStove)
        {
            printingSubsystem.Resource(Descriptions.PETROLEUM_BURNED);
        }
        
        printingSubsystem.Resource(Descriptions.FIRE_STARTER);
        
        CloseStoveBecauseItIsToHot();

        AssignEventForCombustionChamber();
        
        this.universe.Score += this.universe.ScoreBoard[nameof(UseLightersOnThings)];
        this.universe.SolveQuest(MetaData.QUEST_II);
    }

    private void AssignEventForCombustionChamber()
    {
        var combustionChamber = this.universe.GetObjectFromWorld(Keys.COMBUSTION_CHAMBER);
        combustionChamber.BeforeOpen += this.CantOpenStoveOnFire;
    }

    private void CloseStoveBecauseItIsToHot()
    {
        var stove = this.universe.ActiveLocation.GetItem(Keys.STOVE);
        stove.IsClosed = true;
        stove.BeforeOpen += this.CantOpenStoveOnFire;
    }

    private void CheckForFireStarters()
    {
        if (!this.isPetroleumInStove && !this.isPaperInStove)
        {
            throw new UseException(Descriptions.NO_FIRE_ACCELERATOR);
        }
    }

    private void CheckIfStoveIsOpen()
    {
        var stove = this.universe.ActiveLocation.GetItem(Keys.STOVE);
        if (stove is { IsClosed: true })
        {
            throw new UseException(Descriptions.STOVE_MUST_BE_OPEN);
        }
    }

    private void RemovePaperFromStove()
    {
        var stove = this.universe.ActiveLocation.GetItem(Keys.STOVE);
        stove.Items.Remove(stove.Items.Single(i => i.Key is Keys.NOTE));
        this.isPaperInStove = false;
    }
    
}