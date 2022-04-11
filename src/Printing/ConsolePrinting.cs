using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;
using LogCabin.Resources;

namespace LogCabin.Printing;

internal sealed class ConsolePrinting: BaseConsolePrintingSubsystem
{
    public override bool Opening()
    {
        Console.WriteLine($@"{MetaData.TITLE} - {MetaData.VERSION}");
        this.ForegroundColor = TextColor.DarkCyan;
        this.Resource(BaseDescriptions.CHANGE_NAME);
        this.ResetColors();
        Console.Write(WordWrap(BaseDescriptions.HELLO_STRANGER, this.ConsoleWidth));
        Console.Write(WordWrap(Descriptions.OPENING, this.ConsoleWidth));
        Console.WriteLine();

        return true;
    }

    public override bool Closing()
    {
        Console.WriteLine(Descriptions.CLOSING);
        
        return true;
    }

    public override bool TitleAndScore(int score, int maxScore)
    {
        Console.Title = $"{string.Format(BaseDescriptions.SCORE, score, maxScore)}";
        return true;
    }

    public override bool Credits()
    {
        Console.WriteLine(Descriptions.CREDITS);
        
        return true;
    }
}