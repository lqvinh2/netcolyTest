using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Colyseus;
using Colyseus.Schema;
using System;
using UnityEngine.UI;

public class netColy : MonoBehaviour
{

    

    public Button btn_move_right;
    public Button btn_create_room;
    public Button btn_refesh_room;
    public Button btn_clear_text;
    public Button btn_leave_room;

    protected Client client;
    protected Room<State> room;
    public InputField inputText;

    string roomName = "gameVinh";
    string serverLink = "ws://localhost:2569";

    public GameObject grid_list_room;
    public GameObject bnt_show_roomID;

    public List<CLASS_PlayerConnInfo> ListPlayerConnInfo = new List<CLASS_PlayerConnInfo>();

    public static netColy instance = null;



    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString("roomName", "");
        PlayerPrefs.SetString("roomId", "");
        PlayerPrefs.SetString("sessionId", "");
        PlayerPrefs.Save();


        instance = this;

        btn_move_right.onClick.AddListener(OnMoveRight);
        btn_create_room.onClick.AddListener(CreateRoom);

        btn_refesh_room.onClick.AddListener(OnRefeshRoom);
        btn_clear_text.onClick.AddListener(OnClearText);

        btn_leave_room.onClick.AddListener(OnLeaveRoom);
        

        ConnectToServer();
    }



    #region NET
    void ConnectToServer()
    {
        string endpoint = serverLink;
        Debug.Log("Connecting to " + endpoint + ".....");
        client = ColyseusManager.Instance.CreateClient(endpoint);
        GetAvailableRooms();
    }


    public async void CreateRoom()
    {
        try
        {
            // Create auto join so we need out first
            LeavingRoom();

            room = await client.Create<State>(roomName);

            RegisterRecMes();

            Debug.Log($"room {room.Name} CREATED SCC !!");

            var roomsAvailable = await client.GetAvailableRooms<RoomAvailable>(roomName);

            string ss = "";
            int cnt = 0;
            if (roomsAvailable.Length > 0)
            {
                foreach (var item in roomsAvailable)
                {
                    cnt++;
                    ss += $"\n + {cnt}. roomID = [{item.roomId}]";
                }
            }
            ss = $"__ CREATED room SCC NOW WE HAVE: {roomsAvailable.Length}" + ss;
            inputText.text = ss;


            CreateGrid_Show_Room(roomsAvailable);


        }

        catch 
        {
            Debug.Log($"room {room.Name} CREATED FAIL !!");
          
        }

    }

    void SaveConnInfo()
    {
        PlayerPrefs.SetString("roomName", room.Name);
        PlayerPrefs.SetString("roomId", room.Id);
        PlayerPrefs.SetString("sessionId", room.SessionId);
        PlayerPrefs.Save();
    }

    public void RegisterRecMes()
    {
        SaveConnInfo();

        ListPlayerConnInfo.Add(new CLASS_PlayerConnInfo() { roomName = room.Name, roomId = room.Id, sessionId = room.SessionId });

        room.OnMessage<Player>("svTestNet", (message) =>
        {
            Debug.Log(message);
            inputText.text += "__ CONN to server SCC !! ROOM_ID :" + room.Id + "ssID:" + room.SessionId;
        });

        room.OnMessage<Player>("sv_close_conn", (player) =>
        {
            inputText.text += "__" + player.sessionId;
        });


        room.OnMessage<Player>("sv_move_right", (player) =>
        {
            inputText.text += "__" + player.sessionId;
        });

        room.OnMessage<string>("sv_check_before_join", (msg) =>
        {
            if (msg != "NONE")
            {
                string sessionIdLocal = PlayerPrefs.GetString("sessionId");
                string roomLocal = PlayerPrefs.GetString("roomId");

                string[] listClientSessID = msg.Split(new string[] { "__" }, options: StringSplitOptions.None);
                bool canJoin = true;
                foreach (var ssIDServer in listClientSessID)
                {
                    if (sessionIdLocal == ssIDServer)
                    {
                        canJoin = false;
                        break;
                    }
                }
                if (canJoin)
                {
                    Reconn();
                }
            }
        });

    }

    async void Reconn()
    {

        // JoinRoomByID(roomIDWillJoin);
        string roomId = PlayerPrefs.GetString("roomId");
        string sessionId = PlayerPrefs.GetString("sessionId");

        string ss = $"Cannot Reconnect roomId: __{roomId}__ and sessionId : __{sessionId}__";
        if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(roomId))
        {
            Debug.Log("empty:" + ss);
            return;
        }

        try
        {
            room = await client.Reconnect<State>(roomId, sessionId);
       
            Debug.Log("Reconnected into room successfully.");

            room = await client.JoinById<State>(room.Id, new Dictionary<string, object>() { });

            RegisterRecMes();
        }
        catch
        {
            Debug.Log("catch:" + ss);
        }

        
    }

    string roomIDWillJoin = "";

    public async void JoinOrCreateRoom()
    {
        room = await client.JoinOrCreate<State>(roomName, new Dictionary<string, object>() { });
        RegisterRecMes();

    }

    public async void JoinRoom()
    {
        room = await client.Join<State>(roomName, new Dictionary<string, object>() { });
        RegisterRecMes();
    }

    public async void JoinRoomByID(string rID)
    {
        try
        {
            string rNew = rID;
            string rOld = PlayerPrefs.GetString("roomId");

            //if (rNew == rOld) 
            //    return;

            LeavingRoom();

            room = await client.JoinById<State>(rNew, new Dictionary<string, object>() { });
            RegisterRecMes();
        }
        catch 
        {
        }
       

    }

    async void LeavingRoom()
    {
        try
        {
            await room.Leave();

            //PlayerPrefs.SetString("roomId", "");
            //PlayerPrefs.Save();

            string endpoint = serverLink;
            Debug.Log("Connecting to " + endpoint + ".....");
            client = ColyseusManager.Instance.CreateClient(endpoint);
        }
        catch {}
    }



    async void OnRefeshRoom()
    {
        var roomsAvailable = await client.GetAvailableRooms<RoomAvailable>(roomName);
        CreateGrid_Show_Room(roomsAvailable);
    }

    void OnClearText()
    {
        inputText.text = "";
    }

    void CreateGrid_Show_Room(RoomAvailable[] roomsAvailable)
    {
        string ss = "";
        int cnt = 0;

        foreach (Transform child in grid_list_room.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        if (roomsAvailable.Length > 0)
        {

            foreach (var item in roomsAvailable)
            {
                cnt++;
                ss += $"{cnt}. roomID = [{item.roomId}]";

                GameObject btn_roomID = Instantiate(bnt_show_roomID, new Vector2(0, 0), Quaternion.identity);
                btn_roomID.transform.SetParent(grid_list_room.transform, false);
                btn_roomID.GetComponentInChildren<Text>().text = item.roomId;

            }

            GameObject[] listBtn = GameObject.FindGameObjectsWithTag("btn_room_name");

            // reset all button to black first
            //foreach(GameObject btn in listBtn)
            //{
            //    btn.GetComponentInChildren<Text>().color = Color.black;
            //}
            //foreach (Transform child in grid_list_room.transform)
            //{
            //    child.gameObject.GetComponentInChildren<Text>().color = Color.black;
            //}


            // set color for connected room
            //foreach (Transform child in grid_list_room.transform)
            //{
            //    if (child.gameObject.GetComponentInChildren<Text>().text == room.Id)
            //    {
            //        child.gameObject.GetComponentInChildren<Text>().color = Color.red;
            //        break;
            //    }
            //}

            //foreach (Transform child in grid_list_room.transform)
            //{
            //    child.gameObject.GetComponentInChildren<Text>().color = Color.red;
            //}


        }
    }


    async void GetAvailableRooms()
    {
        var roomsAvailable = await client.GetAvailableRooms<RoomAvailable>(roomName);

        CreateGrid_Show_Room(roomsAvailable);

        string str = "Available rooms is (" + roomsAvailable.Length + ")";
        Debug.Log(str);
        inputText.text = str;

        if (roomsAvailable.Length <= 0)
        {
            CreateRoom();
        }
        else
        {
            JoinRoom();
        }
    }


    #endregion NET

    #region GAME
    public GameObject spawnPoint;
    public GameObject prefab;
    float spawnTime = 0.3F;
    float time_refesh_room = 3F;
    void Update()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }
        time_refesh_room = time_refesh_room - Time.deltaTime;
        if (time_refesh_room <= 0)
        {
            time_refesh_room = 3F;
            OnRefeshRoom();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            OnClearText();
        }


        // CLOSE CONN
        if (Input.GetKeyUp(KeyCode.D))
        {
            OnCheckBeforeJoin_AndJoinRoom();
        }

        //StartCoroutine(spawnThing());
        spawnTime = spawnTime - Time.deltaTime;

        if (spawnTime <= 0)
        {
            spawnTime = 0.2F;
            StartCoroutine(spawnThing());
        }
    }

    IEnumerator spawnThing()
    {
        yield return new WaitForSeconds(0.01F);

        float x = UnityEngine.Random.Range(-3,3);
        float y = UnityEngine.Random.Range(4,6);

        GameObject prefab1 = Instantiate(prefab, new Vector3(x,y, spawnPoint.transform.position.z), Quaternion.identity);
        Destroy(prefab1, 8F);
    }


    async void OnMoveRight()
    {
        await room.Send("cl_move_right");
    }

    async void OnCheckBeforeJoin_AndJoinRoom()
    {
        #region delete

        //if (ListPlayerConnInfo.Count > 0)
        //{
        //    await room.Leave();
        //    bool bRec = false;
        //    foreach (var item in ListPlayerConnInfo)
        //    {
        //        if (item.sessionId == room.SessionId)
        //        {
        //            ListPlayerConnInfo.Remove(item);
        //            bRec = true;
        //            break;
        //        }
        //    }

        //    if (bRec)
        //    {
        //        if (ListPlayerConnInfo.Count > 0)
        //        {
        //            string roomId = ListPlayerConnInfo[0].roomId;
        //            string sessionId = ListPlayerConnInfo[0].sessionId;

        //            roomId = PlayerPrefs.GetString("roomId");
        //            sessionId = PlayerPrefs.GetString("sessionId");

        //            string ss = $"Cannot Reconnect roomId: __{roomId}__ and sessionId : __{sessionId}__";
        //            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(roomId))
        //            {
        //                Debug.Log("empty:" + ss);
        //                return;
        //            }

        //            try
        //            {
        //                room = await client.Reconnect<State>(roomId, sessionId);

        //                Debug.Log("Reconnected into room successfully.");
        //            }
        //            catch 
        //            {
        //                Debug.Log("catch:" + ss);
        //            }

        //        }

        //    }
        //}

        #endregion delete

        // get check data before join
        // get list all client connected to server
        // then go to room.OnMessage<string>("sv_check_before_join" check list, if list this client (sessionID) ? 
        // if not can join room
        await room.Send("cl_check_before_join");
    }

    async void OnLeaveRoom()
    {
        LeavingRoom();
    }

    #endregion GAME
}
