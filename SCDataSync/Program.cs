using SCDataSync.Communication;
using SCDataSync.Memory.Extensions;
using SCDataSync.Memory.Native;

namespace SCDataSync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var modeHandle = Kernel32.GetStdHandle(-10);
            Kernel32.GetConsoleMode(modeHandle, out var consoleMode);
            consoleMode &= ~((uint)0x0040);
            Kernel32.SetConsoleMode(modeHandle, consoleMode);
            Console.Title = $"SCDataSync";

            Winmm.timeBeginPeriod(1);
            try
            {
                var client = args.Length switch
                {
                    1 => new ScDataSyncClient(args[0]),
                    0 => new ScDataSyncClient(),
                    _ => throw new Exception("Invalid number of arguments")
                };

                client.Run();
            }
            catch(Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Thread.Sleep(5000);
            }
            Winmm.timeEndPeriod(1);

            int a = 100;
            a.AsByteSpan();
        }
        
    }
}