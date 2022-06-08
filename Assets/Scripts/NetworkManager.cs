
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Status")]
    public Text connectionStatusText;

    [Header("Login Panel")]
    public GameObject loginUI_Panel;



    [Header("Registration Panel")]
    public GameObject registrationUI_Panel;



    [Header("EnterGame UI Panel")]
    public string playerName;
    public GameObject enterGameUI_Panel;

    [Header("Game Option UI Panel")]
    public GameObject gameOptionUI_Panel;

    [Header("Create Room UI Panel")]
    public GameObject createRoomUI_Panel;
    public InputField roomNameInput;

    public InputField maxPlayerInput;

    [Header("Inside Room UI Panel")]
    public GameObject insideRoomUI_Panel;
    public Text roomInfoText;
    public GameObject playerListPrefab;
    public GameObject playerListPrefabParent;
    public GameObject startButton;

    [Header("Room List UI Panel")]
    public GameObject roomListUI_Panel;
    public GameObject roomListEntryPrefab;
    public GameObject roomListPrefabParent;


    [Header("Join Random Room UI Panel")]
    public GameObject joinRandomRoomUI_Panel;


    Dictionary<string, RoomInfo> cachedRoomList;
    Dictionary<string, GameObject> roomListGameObjects;
    Dictionary<int, GameObject> playerListGameObjectsList; 



    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(loginUI_Panel.name);

        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListGameObjects = new Dictionary<string, GameObject>();

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        connectionStatusText.text = "Connection Status : " + PhotonNetwork.NetworkClientState;
    }


    #endregion

    #region UI Call backs


    public void OnRegisterationButtonClick()
    {
        ActivatePanel(registrationUI_Panel.name);
    }



    //public void OnLoginButtonClick()
    //{

    //    if (playerName == null) 
    //    {
            
    //        PhotonNetwork.ConnectUsingSettings();
    //    }
    //    else
    //    {
    //        Debug.Log("Player Name is Invalid ");
    //    }

    //    ActivatePanel(enterGameUI_Panel.name);
    //}





    
    public void OnEnterGameButtonClick()
    {
        PhotonNetwork.NickName = playerName;
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log(PhotonNetwork.NickName);
    }

    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInput.text;

        if (!string.IsNullOrEmpty(roomName))
        {
            roomName = roomName + Random.Range(100, 10000);

        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = (byte)int.Parse(maxPlayerInput.text);

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }


    public void OnCancelButtonClicked()
    {
        ActivatePanel(gameOptionUI_Panel.name);
    }


    public void OnShowRoomButtonClicked()
    {
        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        ActivatePanel(roomListUI_Panel.name);
    }


    public void OnBackButtonClicked()
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        ActivatePanel(gameOptionUI_Panel.name);
    }


    public void OnLeaveButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        ActivatePanel(joinRandomRoomUI_Panel.name);
        PhotonNetwork.JoinRandomRoom();
    }


    public void OnStartButtonClicked()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }


    #endregion


    #region Photon Callbacks

    public override void OnConnected()
    {
        Debug.Log("Connected to the internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to server");
        ActivatePanel(gameOptionUI_Panel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created =" + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined the room " + PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(insideRoomUI_Panel.name);

        //activating start button only for the room owner 
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }

        roomInfoText.text = "Room Name :" + PhotonNetwork.CurrentRoom.Name +
                        "Players / Max. Players" + PhotonNetwork.CurrentRoom.PlayerCount +
                        "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        if(playerListGameObjectsList == null)
        {
            playerListGameObjectsList = new Dictionary<int, GameObject>();
        }


        // instiating player list game object for each player
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListGameobject = Instantiate(playerListPrefab);
            playerListGameobject.transform.SetParent(playerListPrefabParent.transform);
            playerListGameobject.transform.localScale = Vector3.one;


            playerListGameobject.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;

            if(player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerListGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(true);
            }
            else
            {
                playerListGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(false);
            }

            playerListGameObjectsList.Add(player.ActorNumber, playerListGameobject);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //clear the list view of the room list ui panel and setting it again for updation
        ClearRoomListView();

        //checking the room list if any new room is created and removing the room if any properties are changed and adding if room created for first time
        foreach(RoomInfo room in roomList)
        {
            Debug.Log(room.Name);
            if(!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if(cachedRoomList.ContainsKey(room.Name))
                {
                    cachedRoomList.Remove(room.Name);
                }

            }
            else
            {
                //update cached room list
                if(cachedRoomList.ContainsKey(room.Name))
                {
                    cachedRoomList[room.Name] = room;
                }
                else
                {
                    // add new room list
                    cachedRoomList.Add(room.Name, room);
                }
            }
            
        }

        //adding values to room prefab
        foreach(RoomInfo room in cachedRoomList.Values)
        {
            GameObject roomListEntryGameobject = Instantiate(roomListEntryPrefab);
            roomListEntryGameobject.transform.SetParent(roomListPrefabParent.transform);
            roomListEntryGameobject.transform.localScale = Vector3.one;

            roomListEntryGameobject.transform.Find("RoomNameText").GetComponent<Text>().text = room.Name;
            roomListEntryGameobject.transform.Find("RoomPlayersText").GetComponent<Text>().text = room.PlayerCount + " / " + room.MaxPlayers;
            roomListEntryGameobject.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnJoinRoomButtonClicked(room.Name));

            //adding the room prefab Game Object to the room list dictionary
            roomListGameObjects.Add(room.Name, roomListEntryGameobject);
        }
    }



    public override void OnLeftLobby()
    {
        ClearRoomListView();
        cachedRoomList.Clear();
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //update room info text
        roomInfoText.text = "Room Name :" + PhotonNetwork.CurrentRoom.Name +
                       "Players / Max. Players" + PhotonNetwork.CurrentRoom.PlayerCount +
                       "/" + PhotonNetwork.CurrentRoom.MaxPlayers;


        GameObject playerListGameobject = Instantiate(playerListPrefab);
        playerListGameobject.transform.SetParent(playerListPrefabParent.transform);
        playerListGameobject.transform.localScale = Vector3.one;


        playerListGameobject.transform.Find("PlayerNameText").GetComponent<Text>().text = newPlayer.NickName;

        if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerListGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(true);
        }
        else
        {
            playerListGameobject.transform.Find("PlayerIndicator").gameObject.SetActive(false);
        }

        playerListGameObjectsList.Add(newPlayer.ActorNumber, playerListGameobject);
    }



    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //update room info text
        roomInfoText.text = "Room Name :" + PhotonNetwork.CurrentRoom.Name +
                       "Players / Max. Players" + PhotonNetwork.CurrentRoom.PlayerCount +
                       "/" + PhotonNetwork.CurrentRoom.MaxPlayers;


        Destroy(playerListGameObjectsList[otherPlayer.ActorNumber].gameObject);
        playerListGameObjectsList.Remove(otherPlayer.ActorNumber);

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startButton.SetActive(true);
        }    
    }


    public override void OnLeftRoom()
    {
        ActivatePanel(gameOptionUI_Panel.name);

        foreach(GameObject playerListObjects in playerListGameObjectsList.Values)
        {
            Destroy(playerListObjects.gameObject);
        }

        playerListGameObjectsList.Clear();
        playerListGameObjectsList = null;
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);

        string roomName = "Room " + Random.Range(100, 1000);
        RoomOptions roomOptions = new RoomOptions();

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    #endregion


    #region Private Methods

    void OnJoinRoomButtonClicked(string roomName)
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(roomName);
    }


    void ClearRoomListView()
    {
        foreach(var roomListGameobject in roomListGameObjects.Values)
        {
            Destroy(roomListGameobject);
        }

        roomListGameObjects.Clear();
    }


    #endregion


    #region Public Methods

    public void ActivatePanel(string panelToBeActivated)
    {
        loginUI_Panel.SetActive(panelToBeActivated.Equals(loginUI_Panel.name));
        registrationUI_Panel.SetActive(panelToBeActivated.Equals(registrationUI_Panel.name));
        enterGameUI_Panel.SetActive(panelToBeActivated.Equals(enterGameUI_Panel.name));
        gameOptionUI_Panel.SetActive(panelToBeActivated.Equals(gameOptionUI_Panel.name));
        createRoomUI_Panel.SetActive(panelToBeActivated.Equals(createRoomUI_Panel.name));
        insideRoomUI_Panel.SetActive(panelToBeActivated.Equals(insideRoomUI_Panel.name));
        roomListUI_Panel.SetActive(panelToBeActivated.Equals(roomListUI_Panel.name));
        joinRandomRoomUI_Panel.SetActive(panelToBeActivated.Equals(joinRandomRoomUI_Panel.name));
    }


    

    #endregion



}
