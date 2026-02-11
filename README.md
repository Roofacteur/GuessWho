# Guess Who ?

Digitization of the board game *Guess Who?*

## Prerequisites
- .NET 8.0.X installed  
  https://dotnet.microsoft.com/en-us/download/dotnet/8.0  
- Install the **.NET Runtime** for your operating system

## Launch
1. Clone the project
2. Navigate to:
   `..\GuessWho\Code\GuessWho\bin\Release\net8.0`
3. Run `GuessWho.exe`

Enjoy the game!

## Solving the Limitations of the Physical Game

This digital version overcomes the main constraints of the physical *Guess Who?* game by introducing automated and intelligent mechanics:

- **Random portrait generation without duplicates**, ensured by a comparison algorithm that prevents look-alikes.
- **Fast game restart**, allowing a new match to be launched instantly without manual setup.
- **Random character positioning** at the start of each game to guarantee fairness and replayability.
- **Automatic gender handling**, with dynamic assignment of male and female characters and their corresponding names.
- **Automatic winner detection**, immediately declaring victory when the last remaining portrait matches the target character.

These improvements modernize the experience while preserving the original gameplay logic.
