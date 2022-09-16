// See https://aka.ms/new-console-template for more information

using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;
using LogCabin.GamePlay;
using LogCabin.Printing;

IResourceProvider resourceProvider = new ResourceProvider();
IPrintingSubsystem printingSubsystem = new ConsolePrinting();
var universe = new Universe(printingSubsystem, resourceProvider);
var eventProvider = new EventProvider(universe, printingSubsystem);
IGamePrerequisitesAssembler gamePrerequisitesAssembler = new GamePrerequisitesAssembler(eventProvider);
IGrammar grammar = new GermanGrammar();
var gameLoop = new GameLoop(printingSubsystem, universe, gamePrerequisitesAssembler, grammar);

gameLoop.Run();