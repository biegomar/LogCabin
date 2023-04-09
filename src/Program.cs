// See https://aka.ms/new-console-template for more information

using Heretic.InteractiveFiction.GamePlay;
using LogCabin.GamePlay;

IGamePrerequisitesAssembler gamePrerequisitesAssembler = new GamePrerequisitesAssembler();
var gameLoop = new GameLoop(gamePrerequisitesAssembler);

gameLoop.Run();