#	Chess.NExT

A chess game with an AI player and 2D graphics built on top of SFML.NET.

##	To Do's
*	Copy the other `Piece`'s move history/moves made count into simulated `Piece`s when copy constructing.
*	Remove depth parameter from `searchMovePossibilityTreeForBestMove()`.
*	When searching for the best move sequence, if two moves are equal chose one randomly.
*	Move the code that pulls the first move out of a move sequence (in `searchMovePossibilityTreeForBestMove()`) into its own method.
*	Look into whether there's some way to hint to the debugger which field to show first for an object.
*	`Color` should be an enum.
