using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class btn_show_roomID : MonoBehaviour
{
    public Text roomID;

    // Use this for initialization
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() => JoinRoomByID(roomID.text));
    }

    public void JoinRoomByID(string rID)
    {
        netColy.instance.JoinRoomByID(rID);
    }

}



