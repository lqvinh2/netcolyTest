

using Assets.C_;
using System.Reflection;

public class Player2  {

	public bool isRoomMater = false;

	public bool isLocalPlayer = false;

	public string room_name = "";
    public string room_id = "";
    
    public string sessionId = "";

	public string name = "";

	public short seat = 0;

	public short health = 0;

    public string timeJoined = "";

    public float[] position ;
    public float[] rotation;

    public Player2 Clone()
    {
        Player2 p2 = new Player2();
        FieldInfo[] fData = this.GetType().GetFields();
        FieldInfo[] fDes = p2.GetType().GetFields();

        foreach (FieldInfo fieldData in fData)
        {
            foreach (FieldInfo fieldDes in fDes)
            {
                if (fieldData.Name == fieldDes.Name)
                {
                    fieldDes.SetValue(p2, fieldData.GetValue(this));
                    break;
                }
            }
        }



        return p2;
    }

 
}

