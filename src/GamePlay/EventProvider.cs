using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;
using LogCabin.Resources;

namespace LogCabin.GamePlay;

internal class EventProvider
{
    private readonly Universe universe;
    private readonly ObjectHandler objectHandler;
    private readonly IPrintingSubsystem printingSubsystem;
    private readonly ScoreBoard scoreBoard;
    private bool isPaperInStove;
    private bool isPetroleumInStove;
    private bool isPetroleumInLamp;
    private int waitCounter;
    
    internal EventProvider(Universe universe, IPrintingSubsystem printingSubsystem, ScoreBoard scoreBoard)
    {
        this.printingSubsystem = printingSubsystem;
        this.scoreBoard = scoreBoard;
        this.universe = universe;
        this.objectHandler = new ObjectHandler(this.universe);
        
        InitializeStates();
    }

    private void InitializeStates()
    {
        this.waitCounter = 0;
        this.isPaperInStove = false;
        this.isPetroleumInStove = false;
        this.isPetroleumInLamp = false;
    }

    internal void RegisterScore(string key, int value)
    {
        this.scoreBoard.RegisterScore(key, value);
    }
    
    internal void SetPlayersName(object? sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Player)
        {
            this.universe.ActivePlayer.Name = eventArgs.ExternalItemKey;
            this.universe.ActivePlayer.IsStranger = false;
            printingSubsystem.ActivePlayer(this.universe.ActivePlayer);
        }
    }

    internal void UnhideMainEntrance(object? sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item {Key: Keys.DOOR})
        {
            var destination = this.objectHandler.GetDestinationNodeFromActiveLocationByDirection(Directions.S);
            if (destination != null)
            {
                destination.IsHidden = false;
            }
        }
    }
    
    internal void TakeCandle(object? sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.CANDLE } candle)
        {
            printingSubsystem.Resource(Descriptions.CANDLE_PICKUP);
            this.scoreBoard.WinScore(nameof(TakeCandle));
            candle.Take -= this.TakeCandle;
        }
    }
    
    internal void BeforeTakeCandle(object? sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.CANDLE })
        {
            var table = this.objectHandler.GetObjectFromWorldByKey(Keys.TABLE);
            if (table != default)
            {
                table.ContainmentDescription = Descriptions.TABLE_CONTAINMENT_WITHOUT_CANDLE;
            }
        }
    }
    
    internal void DropCandle(object? sender, UseItemEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.CANDLE })
        {
            if (eventArgs.ItemToUse is Item {Key: Keys.TABLE} table)
            {
                table.ContainmentDescription = Descriptions.TABLE_CONTAINMENT;
            }
        }
    }
    
    internal void GetNextMatchFromMatchBox(object? sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.MATCH } match)
        {
            if (this.universe.ActivePlayer.Items.Contains(match))
            {
                throw new TakeException(Descriptions.MATCH_ALREADY_TAKEN);
            }

            var matchBox = this.objectHandler.GetObjectFromWorldByKey(Keys.MATCHBOX);
            
            if (matchBox != null)
            {
                if ((int)matchBox.Spare["CountOfMatchesInBox"] == 0)
                {
                    throw new TakeException(Descriptions.NO_MATCHES_LEFT);
                }

                if (matchBox.OwnsObject(match))
                {
                    matchBox.Spare["CountOfMatchesInBox"] = (int)matchBox.Spare["CountOfMatchesInBox"] - 1;    
                }
            }
        }
    }
    
    internal void GetNextMatchFromMatchBoxAfterGameLoop(object? sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.MATCH } match)
        {
            var matchBox = this.objectHandler.GetObjectFromWorldByKey(Keys.MATCHBOX);

            if (matchBox != null)
            {
                var countOfMatchesInBox = (int)matchBox.Spare["CountOfMatchesInBox"];
                var numberOfGameLoopBeforeDying = (int)match.Spare["NumberOfGameLoopBeforeDying"];
                
                match.Spare["NumberOfGameLoopBeforeDying"] = (int)match.Spare["NumberOfGameLoopBeforeDying"] - 1;

                if (numberOfGameLoopBeforeDying == 5)
                {
                    this.printingSubsystem.Resource(Descriptions.MATCH_PARTIALY_BURNED);
                } else if (numberOfGameLoopBeforeDying == 0)
                {
                    if (countOfMatchesInBox > 0)
                    {
                        this.printingSubsystem.Resource(Descriptions.MATCH_BURNED);
                        
                        matchBox.Spare["CountOfMatchesInBox"] = (int)matchBox.Spare["CountOfMatchesInBox"] - 1;
                        match.Spare["NumberOfGameLoopBeforeDying"] = (int)10;

                        if (countOfMatchesInBox < 10)
                        {
                            this.printingSubsystem.Resource(Descriptions.MATCHES_RUNNING_LOW);
                        }
                    }    
                }
            }
        }
    }

    internal void AfterDropMatchInMatchBox(object? sender, DropItemEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.MATCH } match)
        {
            if (eventArgs.ItemToUse is Item {Key: Keys.MATCHBOX} matchBox)
            {
                if (matchBox.OwnsObject(match))
                {
                    match.IsShownInObjectList = false;
                    matchBox.Spare["CountOfMatchesInBox"] = (int)matchBox.Spare["CountOfMatchesInBox"] + 1;    
                }
            }
            else
            {
                match.IsShownInObjectList = true;
            }
        }
    }
    
    internal void BeforeDropMatchInMatchBox(object? sender, DropItemEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.MATCH } match)
        {
            if (eventArgs.ItemToUse is Item {Key: Keys.MATCHBOX} matchBox)
            {
                if (!this.universe.ActivePlayer.OwnsObject(matchBox))
                {
                    throw new DropException(string.Format(BaseDescriptions.ITEM_NOT_OWNED_FORMATTED, matchBox.Name));
                }
                else if (match.IsLighterSwitchedOn)
                {
                    if ((int)matchBox.Spare["CountOfMatchesInBox"] == 0)
                    {
                        throw new DropException(Descriptions.CANT_DROP_MATCH);    
                    }
                    
                    throw new DropException(Descriptions.CANT_DROP_MATCH_IN_BOX);
                }
            }
            else
            {
                if (match.IsLighterSwitchedOn)
                {
                    throw new DropException(Descriptions.CANT_DROP_MATCH);
                }
            }
        }
    }

    internal void KindleMatch(object? sender, KindleItemEventArgs eventArgs)
    {
        if (sender is Item {Key: Keys.MATCH} match)
        {
            if (!this.universe.ActivePlayer.OwnsObject(match))
            {
                throw new KindleException(BaseDescriptions.ITEM_NOT_OWNED);
            }
            
            if (match.IsLighterSwitchedOn)
            {
                throw new KindleException(Descriptions.MATCH_ALREADY_BURNING);
            }
            
            if (eventArgs.ItemToUse is Item {Key: Keys.MATCHBOX} matchbox)
            {
                if (!this.universe.ActivePlayer.OwnsObject(matchbox))
                {
                    throw new KindleException(BaseDescriptions.ITEM_NOT_OWNED);
                }

                match.IsLighterSwitchedOn = true;
                this.printingSubsystem.Resource(Descriptions.KINDLE_MATCH_WITH_BOX);
                this.universe.NextGameLoop += match.HandleNextGameLoop;
            }
            else if (eventArgs.ItemToUse is Item {Key: Keys.PETROLEUM_LAMP} lamp)
            {
                if (!this.universe.ActivePlayer.OwnsObject(lamp))
                {
                    throw new KindleException(BaseDescriptions.ITEM_NOT_OWNED);
                }

                if (lamp.IsLighterSwitchedOn)
                {
                    match.IsLighterSwitchedOn = true;
                    this.printingSubsystem.Resource(Descriptions.KINDLE_MATCH_WITH_LAMP);
                    this.universe.NextGameLoop += match.HandleNextGameLoop;
                }
                else
                {
                    throw new KindleException(Descriptions.PETROLEUM_LAMP_NOT_BURNING);
                }
            }
            else if (eventArgs.ItemToUse == default)
            {
                throw new KindleException(BaseDescriptions.HOW_TO_KINDLE);
            }
            else
            {
                throw new KindleException(BaseDescriptions.DOES_NOT_WORK);
            }
        }
    }

    internal void EnterRoomWithoutLight(object? sender, EnterLocationEventArgs eventArgs)
    {
        if (sender is Location)
        {
            if (!this.universe.ActivePlayer.Items.Any(x => x.IsLighter && x.IsLighterSwitchedOn))
            {
                throw new EnterLocationException(Descriptions.CANT_LEAVE_ROOM_WITHOUT_LIGHT); 
            }
        }
    }

    internal void AddEventsForUniverse()
    {
        this.universe.PeriodicEvents += this.WaitForCandleToMelt;
        this.RegisterScore(nameof(WaitForCandleToMelt), 1);
    }

    internal void WaitForCandleToMelt(object? sender, PeriodicEventArgs eventArgs)
    {
        if (sender is Universe)
        {
            var candleObject = this.objectHandler.GetObjectFromWorldByKey(Keys.CANDLE);
            var stoveObject = this.objectHandler.GetObjectFromWorldByKey(Keys.STOVE);

            if (stoveObject is Item { IsLighterSwitchedOn: true } stove && candleObject is Item candle && stove.OwnsObject(candle) && this.universe.ActiveLocation.Key == Keys.LIVINGROOM)
            {
                switch (waitCounter)
                {
                    case 0:
                    {
                        waitCounter++;
                        throw new PeriodicException(Descriptions.MELT_CANDLE_I);
                    }
                    case 1:
                    {
                        waitCounter++;
                        throw new PeriodicException(Descriptions.MELT_CANDLE_II);
                    }
                    case 2:
                    {
                        this.universe.PeriodicEvents -= WaitForCandleToMelt;
                        
                        var ironKey = candle.Items.Single(i => i.Key == Keys.IRON_KEY);
                        ironKey.IsHidden = false;
                        candle.RemoveItem(ironKey);
                        
                        stove.Items.Add(ironKey);
                        ironKey.IsOnSurface = true;
                        
                        stove.RemoveItem(candle);
                        
                        this.scoreBoard.WinScore(nameof(WaitForCandleToMelt));
                        
                        throw new PeriodicException(Descriptions.MELT_CANDLE_III);
                    }
                }
            }
        }
    }

    internal void TryToEatSausage(object? sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item {Key: Keys.SAUSAGE})
        {
            throw new EatException(Descriptions.TRY_TO_EAT_SAUSAGE);
        }
    }

    internal void OpenStoveOrCombustionChamber(object? sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.STOVE or Keys.COMBUSTION_CHAMBER })
        {
            if (this.universe.ActiveLocation.GetItem(Keys.STOVE) is {} stove)
            {
                stove.IsClosed = false;    
            }
            
            if (this.universe.ActiveLocation.GetItem(Keys.COMBUSTION_CHAMBER) is {} combustionChamber)
            {
                combustionChamber.IsClosed = false;    
            }
        }
    }
    
    internal void CloseStoveOrCombustionChamber(object? sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.STOVE or Keys.COMBUSTION_CHAMBER })
        {
            CloseStoveAndCombustionChamber();
        }
    }

    private void CloseStoveAndCombustionChamber(bool hasBeforeOpenEvent = false)
    {
        if (this.universe.ActiveLocation.GetItem(Keys.STOVE) is { } stove &&
            this.universe.ActiveLocation.GetItem(Keys.COMBUSTION_CHAMBER) is { } combustionChamber)
        {
            stove.IsClosed = true;
            combustionChamber.IsClosed = true;

            if (hasBeforeOpenEvent)
            {
                stove.BeforeOpen += this.CantOpenStoveOnFire;
                combustionChamber.BeforeOpen += this.CantOpenStoveOnFire;
            }
        }
    }
    
    internal void ReadNote(object? sender, ReadItemEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.NOTE } note)
        {
            printingSubsystem.ForegroundColor = TextColor.Magenta;
            printingSubsystem.Resource(Descriptions.CENTRAL_MESSAGE_UNDERSTOOD);
            printingSubsystem.ResetColors();
            
            this.scoreBoard.WinScore(nameof(ReadNote));
            note.AfterRead -= ReadNote;
        }
    }
    
    internal void PutNoteInStove(object? sender, DropItemEventArgs dropItemEventArgs)
    {
        if (sender is Item { Key: Keys.NOTE } note)
        {
            if (dropItemEventArgs.ItemToUse is Item)
            {
                if (dropItemEventArgs.ItemToUse is {Key: Keys.STOVE} && this.universe.ActiveLocation.GetItem(Keys.COMBUSTION_CHAMBER) is {} chamber)
                {
                    chamber.Items.Add(note);
                    this.isPaperInStove = true;
                }
            
                if (dropItemEventArgs.ItemToUse is {Key: Keys.COMBUSTION_CHAMBER} && this.universe.ActiveLocation.GetItem(Keys.STOVE) is {} stove)
                {
                    stove.Items.Add(note);
                    this.isPaperInStove = true;
                }
            }
        }
    }
    
    internal void BeforeDropNoteInStove(object? sender, DropItemEventArgs dropItemEventArgs)
    {
        if (sender is Item { Key: Keys.NOTE } note)
        {
            if (this.isPetroleumInStove)
            {
                throw new DropException(Descriptions.PETROLEUM_IN_STOVE);
            }
        }
    }

    internal void PoorPetroleumInStove(object? sender, UseItemEventArgs eventArgs)
    {
        if (sender is Item itemOne && eventArgs.ItemToUse is Item itemTwo && itemOne.Key != itemTwo.Key)
        {
            var itemList = new List<Item> { itemOne, itemTwo };
            var petroleum = itemList.SingleOrDefault(i => i.Key == Keys.PETROLEUM);
            var destinationItem = itemList.SingleOrDefault(i => i.Key == Keys.STOVE);

            if (destinationItem == default)
            {
                destinationItem = itemList.SingleOrDefault(i => i.Key == Keys.PILE_OF_WOOD);
                if (destinationItem == default)
                {
                    destinationItem = itemList.SingleOrDefault(i => i.Key == Keys.COMBUSTION_CHAMBER);
                }
            }

            if (petroleum != default && destinationItem != default)
            {
                if (!this.universe.ActivePlayer.OwnsObject(petroleum))
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

    internal void PoorPetroleumInPetroleumLamp(object? sender, UseItemEventArgs eventArgs)
    {
        if (sender is Item itemOne && eventArgs.ItemToUse is Item itemTwo && itemOne.Key != itemTwo.Key)
        {
            var itemList = new List<Item> { itemOne, itemTwo };
            var petroleum = itemList.SingleOrDefault(i => i.Key == Keys.PETROLEUM);
            var destinationItem = itemList.SingleOrDefault(i => i.Key == Keys.PETROLEUM_LAMP);

            if (petroleum != default && destinationItem != default)
            {
                if (!this.universe.ActivePlayer.OwnsObject(petroleum) || !this.universe.ActivePlayer.OwnsObject(petroleum))
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
                this.scoreBoard.WinScore(nameof(PoorPetroleumInPetroleumLamp));
            }
        }
    }

    private void PreparePileOfWoodWithPetroleum()
    {
        if (this.universe.ActiveLocation.GetItem(Keys.PILE_OF_WOOD) is {} pileOfWood)
        {
            pileOfWood.Description = Descriptions.PILE_OF_WOOD_WITH_PETROLEUM;
            this.isPetroleumInStove = true;
            printingSubsystem.Resource(Descriptions.POOR_PETROLEUM_OVER_WOOD);    
        }
    }

    internal void UseLightersOnThings(object? sender, KindleItemEventArgs eventArgs)
    {
        if (sender is Item itemOne && eventArgs.ItemToUse is Item itemTwo && itemOne.Key != itemTwo.Key)
        {
            var itemList = new List<Item> { itemOne, itemTwo };
            
            //Candle and...
            var candle = itemList.SingleOrDefault(i => i.Key == Keys.CANDLE);
            if (candle != default)
            {
                if (!this.universe.ActivePlayer.OwnsObject(candle))
                {
                    var candleName = ArticleHandler.GetNameWithArticleForObject(candle, GrammarCase.Accusative, lowerFirstCharacter: true);
                    throw new KindleException(string.Format(BaseDescriptions.ITEM_NOT_OWNED_FORMATTED, candleName));     
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
                    if (!this.universe.ActivePlayer.OwnsObject(petroleumLamp))
                    {
                        throw new KindleException(BaseDescriptions.ITEM_NOT_OWNED);     
                    }
                    
                    if (!petroleumLamp.IsLighterSwitchedOn)
                    {
                        var petroleumLampName = ArticleHandler.GetNameWithArticleForObject(petroleumLamp, GrammarCase.Accusative);
                        throw new KindleException(string.Format(Descriptions.PETROLEUM_LAMP_NOT_BURNING, petroleumLampName));
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
            var itemName = ArticleHandler.GetNameWithArticleForObject(itemToUse, GrammarCase.Dative, lowerFirstCharacter: true);
            var senderItemName = ArticleHandler.GetNameWithArticleForObject(senderItem, GrammarCase.Accusative, lowerFirstCharacter: true);
            printingSubsystem.Resource(string.Format(Descriptions.FIRE_FIRE_WITH_FIRE, senderItemName, itemName));
        }
    }

    internal void StartPetroleumLampWithCandle(Item lamp)
    {
        if (!this.isPetroleumInLamp)
        {
            throw new KindleException(Descriptions.NO_PETROLEUM_IN_LAMP);
        }

        if (lamp.IsLighterSwitchedOn)
        {
            throw new KindleException(Descriptions.PETROLEUM_LAMP_IS_BURNING);
        }

        lamp.IsLighterSwitchedOn = true;
        printingSubsystem.Resource(Descriptions.PETROLEUM_LAMP_SWITCH_ON);
        this.scoreBoard.WinScore(nameof(StartPetroleumLampWithCandle));
    }

    private void StartFireInStoveWithLighterAndNote(Item lighter, Item note)
    {
        if (this.isPaperInStove)
        {
            CheckIfStoveIsOpen();
            
            RemovePaperFromStove();
            
            printingSubsystem.Resource(Descriptions.NOTE_BURNED);
            printingSubsystem.Resource(Descriptions.FIRE_STARTER);
        
            CloseStoveAndCombustionChamber(true);

            IndicateThatStoveIsHot();
            
            this.scoreBoard.WinScore(nameof(UseLightersOnThings));
            this.universe.SolveQuest(MetaData.QUEST_II);
        }
        else
        {
            if (!this.universe.ActivePlayer.OwnsObject(note))
            {
                throw new KindleException(BaseDescriptions.ITEM_NOT_OWNED);     
            }

            this.universe.ActivePlayer.RemoveItem(note);
            var lighterName = ArticleHandler.GetNameWithArticleForObject(lighter, GrammarCase.Accusative);
            printingSubsystem.FormattedResource(Descriptions.BURN_NOTE, lighterName, true);
        }
    }

    internal void CantDropCandleInStove(object? sender, DropItemEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.CANDLE })
        {
            if (eventArgs.ItemToUse is Item {Key: Keys.STOVE or Keys.COMBUSTION_CHAMBER, IsClosed: true})
            {
                throw new DropException(Descriptions.STOVE_MUST_BE_OPEN);
            }
            throw new DropException(Descriptions.CANT_DROP_CANDLE_IN_STOVE);
        }
    }
    
    internal void CantTakePetroleum(object? sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.PETROLEUM})
        {
            throw new TakeException(Descriptions.CANT_TAKE_PETROLEUM);
        }
    }

    internal void ReadBooks(object? sender, ReadItemEventArgs eventArgs)
    {
        if (sender is Item {Key: Keys.BOOKS})
        {
            Description desc = this.GetBookTitle();
            throw new ReadException(desc);
        }
    }
    
    internal Func<string> GetBookTitle()
    {
        var bookList = new List<string>
        {
            Descriptions.BOOK_I, Descriptions.BOOK_II, Descriptions.BOOK_III,
            Descriptions.BOOK_IV, Descriptions.BOOK_V
        }; 
        
        var rnd = new Random();
        
        return () => string.Format(Descriptions.BOOKS, bookList[rnd.Next(0, bookList.Count)]);
    }

    private void CantOpenStoveOnFire(object? sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.STOVE or Keys.COMBUSTION_CHAMBER})
        {
            throw new OpenException(Descriptions.CANT_OPEN_STOVE_ON_FIRE);
        }
    }

    internal void SmellInLivingRoom(object? sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Location {Key: Keys.LIVINGROOM})
        {
            throw new SmellException(Descriptions.LIVINGROOM_SMELL);
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
        
        CloseStoveAndCombustionChamber(true);

        IndicateThatStoveIsHot();

        this.scoreBoard.WinScore(nameof(UseLightersOnThings));
        this.universe.SolveQuest(MetaData.QUEST_II);
    }

    private void IndicateThatStoveIsHot()
    {
        if (this.universe.ActiveLocation.GetItem(Keys.STOVE) is {} stove)
        {
            stove.IsLighterSwitchedOn = true;    
        }
    }

    private void CheckForFireStarters()
    {
        if (!this.isPetroleumInStove && !this.isPaperInStove)
        {
            throw new KindleException(Descriptions.NO_FIRE_ACCELERATOR);
        }
    }

    private void CheckIfStoveIsOpen()
    {
        if (this.universe.ActiveLocation.GetItem(Keys.STOVE) is { IsClosed: true })
        {
            throw new KindleException(Descriptions.STOVE_MUST_BE_OPEN);
        }
    }

    private void RemovePaperFromStove()
    {
        var stove = this.universe.ActiveLocation.GetItem(Keys.STOVE);
        stove?.Items.Remove(stove.Items.Single(i => i.Key is Keys.NOTE));

        var chamber = this.universe.ActiveLocation.GetItem(Keys.COMBUSTION_CHAMBER);
        chamber?.Items.Remove(chamber.Items.Single(i => i.Key is Keys.NOTE));
        
        this.isPaperInStove = false;
    }
}