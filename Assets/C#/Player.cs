// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.41
// 

using Colyseus.Schema;

public class Player : Schema {
	[Type(0, "boolean")]
	public bool isRoomMater = false;

	[Type(1, "boolean")]
	public bool isLocalPlayer = false;

	[Type(2, "string")]
	public string room_name = "";

	[Type(3, "string")]
	public string sessionId = "";

	[Type(4, "string")]
	public string name = "";

	[Type(5, "int16")]
	public short seat = 0;

	[Type(6, "int16")]
	public short health = 0;

	[Type(7, "array", "float32")]
	public ArraySchema<float> position = new ArraySchema<float>();

	[Type(8, "array", "float32")]
	public ArraySchema<float> rotation = new ArraySchema<float>();
}

