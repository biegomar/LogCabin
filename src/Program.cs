// See https://aka.ms/new-console-template for more information

using LogCabin.Cli;
using PowerArgs;

try
{
    Args.InvokeMain<CommandHandler>(args);
}
catch (ArgException ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<CommandHandler>());
}