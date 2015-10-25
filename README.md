# BotBits.Commands
A BotBits extension that makes adding commands to your bots super easy.

Here is an example:

```csharp
[Command(0, "hi")]
void HiCommand(IInvokeSource source, ParsedRequest request)
{
    source.Reply("Hello world!");
}
```
With BotBits.Commands, this is all you have to write to define a new Hello world command!

## Features

- Commands work in Chat, PM and Console. Write once, run anywhere!
- It is easy to hook up the commands system to external channels, ex. IRC


## Installation

BotBits.Commands is available on [NuGet](https://www.nuget.org/packages/BotBits.Commands/). To install, run the following command: 
```PM> Install-Package BotBits.Commands```

Before you can use the API, you must load the extension by adding this line of code before using any parts of the api:

```csharp
CommandsExtension.LoadInto(bot, '!', '.', ':');
```
This means that the bot will now listen for chats and PMs starting with "!" or "." or ":".

If, for example, you don't want the bot to answer in PMs, you can change this behavior:

```csharp
CommandsExtension.LoadInto(bot, ListeningBehavior.Chat, '!');
```

In order to accept commands from the console, add this at the end of your `Main` method:

```csharp
while (true)
	CommandManager.Of(bot).ReadNextConsoleCommand();
```

## Loading Command Handlers

The command API is very similar to the Events API.

To load all command handlers in a class:

```csharp
CommandLoader
    .Of(bot)
    .Load(this);
```

If the command handlers are defined ```static```:

```csharp
CommandLoader
    .Of(bot)
    .LoadStatic<Program>();
```

## Defining Command handlers

A command handler is a method that has the `CommandAttribute`, and two arguments: `IInvokeSource` and `ParsedRequest`. These will be provided by BotBits.Command.

`IInvokeSource` contains information about the origin of a command.
- The `Name` property of the source contains the `player.ChatName` property or `console` in case of a console command.
- The `Reply` method replies to the person. If the command was called in public chat, this is equivalent of `Chat.Of(bot).Say`, if the command was called in PM, this is equivalent of `Chat.Of(bot).PrivateMessage`. If the command was called in console, this is equivalent of `Console.WriteLine`.

There are two `IInvokeSource` implementations available out of the box: `ConsoleInvokeSource` and `PlayerInvokeSource`. 

`ParsedRequest` includes information about the command itself. 
- The `Type` contains the case sensitive name of the command requested to run.  
- The `Args` array contains the arguments that were separated by spaces.  
- The `GetTrail(start)` method joins all args from the start index with spaces. This is useful if you want to accept strings that contain spaces as an argument.  
- The `GetInt(index)` tries to convert a given argument to a number.  

```csharp
[Command(1, "echo", Usage = "text")]
void HiCommand(IInvokeSource source, ParsedRequest request)
{
    source.Reply(request.GetTrail(0));
}
```
This command echos whatever you give to it as arguments. Ex: "!echo hello world" results in "hello world"
It requires a minimum of one argument. (Defined in the CommandAttribute) If this is not provided, the following error message appears: "Too few arguments. Correct usage: !echo text".


There are two extension methods, `ToPlayerInvokeSource` and `ToConsoleInvokeSource` are available to cast a given InvokeSource to one of the two default implementations. If these methods fail, they throw a `InvalidInvokeSourceCommandException`
This is useful if you want to have commands that are only callable in game or in console.
```csharp
[Command(0, "hi")]
static void HiCommand(IInvokeSource source, ParsedRequest request)
{
    var player = source.ToPlayerInvokeSource().Player;
    source.Reply("Hello world {0}!", player.Username);
}
```
If this command is run in console, an exception is thrown. This exception will be caught by BotBits.Commands (see below) and its message will be outputted to console: "You must call this command as a player."

### Async Support
To be able to use the ```await``` operator in command handlers, use the following syntax:
```csharp
[Command(...)]
static async Task CommandName(IInvokeSource source, ParsedRequest request)
{
    // Command handler code
}
```
__Note:__ ```async void``` is *NOT* supported.

## CommandExceptions

`CommandException` is an exception type that is automatically caught by BotBits.Commands. The error message will be outputted by calling `source.Reply`.

There are two built-in command exceptions:

- `SyntaxCommandException`: Thrown when the command is not used properly. (Ex. not enough arguments, argument must have been integer, etc.)
- `InvalidInvokeSourceCommandException`: The command was not called in the right context.
