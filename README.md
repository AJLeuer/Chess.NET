#	Chess.NExT

A chess game with an AI player and 2D graphics built on top of SFML.NET.

##	To Do's
*	Implement game-ending conditions checking
*	Copy the other `Piece`'s move history/moves made count into simulated `Piece`s when copy constructing.
*	Look into whether there's some way to hint to the debugger which field to show first for an object.
*	`Color` should be an enum
*	Test to ensure King moves properly
*	Implement Monte Carlo approach
*	SimpleAI should check for pieces that are in danger (only in the current game state, not by looking ahead obviously)
*	Look into some way to graphically highlight when pieces move (where the moved from, where to)
*	Investigate why the King didn't capture the Pawn in this game state: `6B1/p2p4/P2K4/8/2P4p/8/P3PP1P/R1B3R1 w - - 0 1`
*	King's possible moves can't include any that would put it in check
