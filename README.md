# Mzinga #

Mzinga is an open-source AI to play the board game Hive. It is architected similar to various chess-playing AIs, consisting of:

* A set of standardized commands similar to the Universal Chess Interface
* An command-line reference engine, which accepts the standardized commands
* A general board UI which can interface with any engine that implements the standardized commands

The goal is not to simply implement Hive in code, but to establish a standard way for programmers to create their own AI players. The hope is that this will encourage explorations around Hive AI.

## Components ##

Mzinga is developed in C# for .NET 4.5.

### Mzinga.Core ###

Mzinga.Core implements all of the rules of the core Hive game (no expansion pieces). In that respect it is complete. It also contains the game AI, which is currently very weak. It looks at the set of valid moves, and if it sees a move that will cause it to win the game (completely surrounding the enemy Queen) it takes it, otherwise it chooses a valid move at random.

Most future investment in Mzinga.Core will be to improve the AI.

### Mzinga.CoreTest ###

Mzinga.CoreTest contains unit tests for Mzinga.Core.

Future investment includes increasing code coverage with more tests, in particular tests to validate the more complicated game rules.

### Mzinga.Engine ###

Mzinga.Engine is a command-line interface which can read input commands, send them to Mzinga.Core and output the results. Through it you can actually play a game of Hive. It's very thin (all of the real logic is in Mzinga.Core) and is therefore complete.

### Mzinga.Viewer ###

Mzinga.Viewer provides a GUI with which to interface with Mzinga.Engine. Currently it provides a graphically rendered gameboard and allows users to to play games with Mzinga.Engine.

Future investment includes exposing game options and improving the usability for players.

Mzinga Copyright © 2015, 2016 Jon Thysell.

Hive Copyright © 2010 Gen42 Games. Mzinga is in no way associated with or endorsed by Gen42 Games. To learn more about Hive, see http://www.hivegame.com.
