using DiscordRPC;
using DiscordRPC.Events;
using DiscordRPC.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Linq;

#nullable enable
namespace ResolveDRPC {
    internal class Program {
        private static DiscordRpcClient client;
        private static bool bgMode = false;

       private static void Main(string[] args) {
           if (args.Contains("--help") || args.Contains("-h")) {
               ShowHelp();
               return;
           }

           if (args.Contains("--background") || args.Contains("-b")) {
               bgMode = true;
           }

           bool flag = false;
           string processName = "Resolve";

           if (Process.GetProcessesByName(processName).Length == 0) {
               Console.WriteLine(processName + " is Not Running...");
               Thread.Sleep(500);
           } else {
               Console.WriteLine(processName + " is Running...");
               Thread.Sleep(500);
               flag = true;
           }

           if (flag) {
               Console.WriteLine("DRPC is Starting...");
               Thread.Sleep(1000);
               Program.GetTitleAndUpdate();
           } else {
               Console.WriteLine("DRPC failed to start...");
               Thread.Sleep(500);
               Console.WriteLine("Exiting...");
               Thread.Sleep(500);
           }
       }

       private static void ShowHelp() {
           Console.WriteLine("Resolve Discord Rich Presence (DRPC) CLI");
           Console.WriteLine("Usage: ResolveDRPC [options]");
           Console.WriteLine("\nOptions:");
           Console.WriteLine("  -h, --help      Show this help message");
           Console.WriteLine("  -b, --background  Run in background mode");
       }

        public static void Initialize() {
            Program.client = new DiscordRpcClient("1131090005872349215");
            Program.client.Logger = (ILogger)new ConsoleLogger() { Level = LogLevel.Warning };

            Program.client.OnReady += (OnReadyEvent)((sender, e) => {
                Console.WriteLine("DRPC Connected to user {0}", (object)e.User.Username);
                
                if (!bgMode) {
                    Thread.Sleep(500);
                    Console.WriteLine("________________________________________");
                    Thread.Sleep(500);
                    Console.WriteLine("   Keep this window running");
                    Console.WriteLine(new string('\n', 7));
                    Thread.Sleep(500);
                    Console.WriteLine("Ctrl + C to close this program");
                    Console.ReadLine();
                }
            });

            Program.client.Initialize();
            DiscordRpcClient client = Program.client;
            RichPresence richPresence = new RichPresence();
            richPresence.State = "Starting...";
            richPresence.Timestamps = Timestamps.Now;
            richPresence.Buttons = new Button[1] {
                new Button() {
                    Label = "\u200E ",
                    Url = "https://github.com/thebearodactyl/RPC"
                }
            };

            richPresence.Assets = new Assets() {
                LargeImageKey = "main",
                LargeImageText = "DaVinci Resolve"
            };

            RichPresence presence = richPresence;
            client.SetPresence(presence);
        }

        public static void GetTitleAndUpdate() {
            Program.Initialize();

            while (true) {
                foreach (Process process in Process.GetProcessesByName("Resolve")) {
                    if (process.MainWindowHandle != IntPtr.Zero) {
                        MyGlobals.Title = process.MainWindowTitle;

                        if (MyGlobals.Title.Length >= 17) {
                            MyGlobals.Title.Substring(0, 17);
                            MyGlobals.Title = MyGlobals.Title.Remove(0, 17);
                        }

                        if (!string.IsNullOrEmpty(MyGlobals.Title)) {
                            Program.client.UpdateState("In " + MyGlobals.Title);
                        }
                    }

                    Thread.Sleep(5000);
                }
            }
        }
    }
}