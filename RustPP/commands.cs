using Fougerite;
using RustPP;
using System;
using RustPP.Commands;

public class rustpp : ConsoleSystem
{
    [Admin]
    public static void day(ref ConsoleSystem.Arg arg)
    {
        World.GetWorld().Time = 6f;
        arg.ReplyWith("Sun is rising.");
    }

    [Admin]
    public static void night(ref ConsoleSystem.Arg arg)
    {
        World.GetWorld().Time = 18f;
        arg.ReplyWith("It's Night!");
    }

    [Admin]
    public static void savealldata(ref ConsoleSystem.Arg arg)
    {
        TimedEvents.savealldata();
        arg.ReplyWith("Saved Rust++ Data.");
    }

    [Admin]
    public static void shutdown(ref ConsoleSystem.Arg arg)
    {
        ShutDownCommand.StartShutdown();
        arg.ReplyWith("Initiating Server Shutdown in " + ShutDownCommand.ShutdownTime + " seconds.");
        //TimedEvents.shutdown();
    }
}