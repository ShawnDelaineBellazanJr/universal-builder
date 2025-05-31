using System;
using System.Threading.Tasks;
using AutonomousAI;

// Entry point that delegates to the UniversalBuilder
public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("Universal Builder starting...");
            
            // Delegate to the UniversalBuilder's Main method
            await UniversalBuilder.Main(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Program: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Environment.Exit(1);
        }
    }
} 