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
    public Button btn_close_canvas_connect_board;

    protected Client client;
    protected Room<State> room;
    public InputField inputText;

    string roomName = "gameVinh";
    string serverLink = "ws://localhost:2569";

    public GameObject grid_list_room;
    public GameObject bnt_show_roomID;

    public List<CLASS_PlayerConnInfo> ListPlayerConnInfo = new List<CLASS_PlayerConnInfo>();

    public static netColy instance = null;

    public bool isRoomMater = false; // chi la` room master thi` moi tao ENEMY
    public bool isLocalPlayer = false; // khi tham gia room thành công thì chuyển trạng thái set sang TRUE
    

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        PlayerPrefs.SetString("roomName", "");
        PlayerPrefs.SetString("roomId", "");
        PlayerPrefs.SetString("sessionId", "");
        PlayerPrefs.Save();

        btn_move_right.onClick.AddListener(OnMoveRight);
        btn_create_room.onClick.AddListener(CreateRoom);
        btn_refesh_room.onClick.AddListener(RefeshRoom);
        btn_clear_text.onClick.AddListener(ClearText);
        btn_leave_room.onClick.AddListener(OnLeaveRoom);
        btn_close_canvas_connect_board.onClick.AddListener(CloseConnBoard);
        
        ConnectToMater();
        ClientFirstConnToMaster_CreateRoom_And_Join();
    }

    public GameObject ConnBoard;
    bool boardConn_Close = false;
    void CloseConnBoard()
    {
        if (boardConn_Close == true)
        {
            boardConn_Close = false;
            ConnBoard.SetActive(true);
            return;
        }

        if (boardConn_Close == false)
        {
            boardConn_Close = true;
            ConnBoard.SetActive(false);
            return;
        }

    }


    #region NET
    void ConnectToMater()
    {
        string endpoint = serverLink;
        Debug.Log("Connecting to " + endpoint + ".....");
        client = ColyseusManager.Instance.CreateClient(endpoint);
    }

    async void ClientFirstConnToMaster_CreateRoom_And_Join()
    {
        var roomsAvailable = await client.GetAvailableRooms<RoomAvailable>(roomName);

        CreateGrid_Show_Room(roomsAvailable);

        string str = "Available rooms is (" + roomsAvailable.Length + ")";
        Debug.Log(str);
        inputText.text = str;

        if (roomsAvailable.Length <= 0)
            CreateRoom();
    }

    public async void CreateRoom()
    {
        try
        {
            // Create auto join so we need out first
            bool b = await LeavingRoom();

            room = await client.Create<State>(roomName);

            RegisterRecMes();

            Debug.Log($"room {room.Name} CREATED SCC !!");

            isRoomMater = true;

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

    public async void JoinRoom()
    {
        bool b = await LeavingRoom();

        room = await client.Join<State>(roomName, new Dictionary<string, object>() { });
        RegisterRecMes();

        isLocalPlayer = true;
        isRoomMater = false;
    }

    public async void JoinRoomByID(string rID)
    {
        try
        {
            string rNew = rID;
            string rOld = PlayerPrefs.GetString("roomId");

            //if (rNew == rOld) 
            //    return;

            bool b = await LeavingRoom();

            room = await client.JoinById<State>(rNew, new Dictionary<string, object>() { });
            RegisterRecMes();
        }
        catch
        {
        }
    }

    public async void JoinOrCreateRoom()
    {
        bool b = await LeavingRoom();

        room = await client.JoinOrCreate<State>(roomName, new Dictionary<string, object>() { });
        RegisterRecMes();

        isLocalPlayer = true;
        isRoomMater = false;

    }

    public void RegisterRecMes()
    {
        PlayerPrefs.SetString("roomName", room.Name);
        PlayerPrefs.SetString("roomId", room.Id);
        PlayerPrefs.SetString("sessionId", room.SessionId);
        PlayerPrefs.Save();

        //ListPlayerConnInfo.Add(new CLASS_PlayerConnInfo() { roomName = room.Name, roomId = room.Id, sessionId = room.SessionId });



        room.OnMessage<object>("svTestNet", (message) =>
        {
            Debug.Log(message.ToString());
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

        room.OnMessage<float[]>("player move", (pos) =>
        {
            Debug.Log(pos[0]);
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.position = new Vector3(pos[0], pos[1], pos[2]);

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

    /*
     if use Recconect in Server side code fucntion : async onLeave (client:Client) {}
        must call :
           const newClient = await this.allowReconnection(client, 10);
           console.log("reconnected!", newClient.sessionId);
     */
    async void Reconn()
    {
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

    async System.Threading.Tasks.Task<bool> LeavingRoom()
    {
        bool ret = false;
        try
        {
            await room.Leave();

            //PlayerPrefs.SetString("roomId", "");
            //PlayerPrefs.Save();

            ConnectToMater();

            // Chuyển về thái chờ...
            //   nếu tạo room thì là RoomMater, 
            //   join room thành công thì sẽ thành isLocalPlayer
            isLocalPlayer = false;
            isRoomMater = false;

            ret = true;
        }
        catch {
            ret = false;
        }

        return ret;
    }

    #region NET UI -  RefeshRoom  ClearText  CreateGrid_Show_Room
    async void RefeshRoom()
    {
        try
        {
            var roomsAvailable = await client.GetAvailableRooms<RoomAvailable>(roomName);
            CreateGrid_Show_Room(roomsAvailable);
        }
        catch 
        {

           
        }

    }

    void ClearText()
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
    #endregion NET UI -  RefeshRoom  ClearText  CreateGrid_Show_Room




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

        StartCoroutine(SetBtnColor());

        time_refesh_room = time_refesh_room - Time.deltaTime;
        if (time_refesh_room <= 0)
        {
            time_refesh_room = 3F;
            RefeshRoom();
        }


        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearText();
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

        if (!isLocalPlayer)
        {
            return;
        }

    }

    IEnumerator SetBtnColor()
    {
        yield return new WaitForSeconds(1F);
        try
        {
            foreach (Transform child in grid_list_room.transform)
            {
                if (child.gameObject.GetComponentInChildren<Text>().text == room.Id)
                {
                    child.gameObject.GetComponentInChildren<Text>().color = Color.red;
                    break;
                }
            }
        }
        catch 
        {

            
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
        await room.Send("cl_check_before_join");
    }
   
  
    async void OnLeaveRoom()
    {
        bool b = await LeavingRoom();
    }


    // CommandMove(transform.position);


    // Code demo, wirte this function in client
    public async void CommandMove(Vector3 vec3)
    {
        if (room == null)
            return;

        float[] data = { vec3 .x, vec3.y, vec3.z };
 
        await room.Send("player move", data);
    }



    #endregion GAME
}
