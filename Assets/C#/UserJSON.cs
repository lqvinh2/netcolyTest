// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.41
// 

using Colyseus.Schema;

public class UserJSON : Schema {
	[Type(0, "string")]
	public string name = "";

	[Type(1, "array", "float32")]
	public ArraySchema<float> position = new ArraySchema<float>();

	[Type(2, "array", "float32")]
	public ArraySchema<float> rotation = new ArraySchema<float>();

	[Type(3, "int32")]
	public int health = 0;
}

