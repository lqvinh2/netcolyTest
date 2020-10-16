// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.41
// 

using Colyseus.Schema;

public class State : Schema {
	[Type(0, "map", typeof(MapSchema<PositionJSON>))]
	public MapSchema<PositionJSON> position = new MapSchema<PositionJSON>();

	[Type(1, "map", typeof(MapSchema<PositionJSON>))]
	public MapSchema<PositionJSON> rotation = new MapSchema<PositionJSON>();

	[Type(2, "string")]
	public string phase = "";

	[Type(3, "int16")]
	public short playerTurn = 0;

	[Type(4, "int16")]
	public short winningPlayer = 0;

	[Type(5, "map", typeof(MapSchema<Player>))]
	public MapSchema<Player> players = new MapSchema<Player>();

	[Type(6, "array", "int16")]
	public ArraySchema<short> player1Shots = new ArraySchema<short>();

	[Type(7, "array", "int16")]
	public ArraySchema<short> player2Shots = new ArraySchema<short>();
}

