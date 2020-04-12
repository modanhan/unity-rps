using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Launcher : MonoBehaviourPunCallbacks, IOnEventCallback
{

    /// <summary>
    /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
    /// </summary>
    string gameVersion = "1";
    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>

    /// <summary>
    /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
    /// </summary>
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 2;

    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Connect();
        playerNames = new List<string>();
        play_history = new List<player_choice>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    #region MonoBehaviourPunCallbacks Callbacks


    public override void OnConnectedToMaster()
    {

    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    bool joined_room = false;

    bool game_started = false;

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        foreach (var v in PhotonNetwork.PlayerList)
        {
            playerNames.Add(v.NickName);
        }
        updateGameStarted();
        joined_room = true;
    }

    List<string> playerNames;
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerNames.Add(newPlayer.NickName);
        updateGameStarted();
    }

    void updateGameStarted()
    {
        Debug.Log(playerNames.Count);
        if (playerNames.Count == maxPlayersPerRoom)
        {
            game_started = true;
        }
        else if (playerNames.Count > maxPlayersPerRoom)
        {
            Debug.LogError("More than 2 players? how?");
        }
    }

    #endregion

    bool entered_name = false;
    public string playerName;

    struct player_choice
    {
        public string n;
        public string s;
    }
    List<player_choice> play_history;

    void Play(string p, string s)
    {
        object[] content = new object[] { p, s };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(0, content, raiseEventOptions, sendOptions);
    }

    void OnGUI()
    {
        if (game_started)
        {
            int y = 10;
            if (GUI.Button(new Rect(10, y += 25, 200, 20), "Rock"))
            {
                Play(playerName, "Rock");
            }

            if (GUI.Button(new Rect(10, y += 25, 200, 20), "Paper"))
            {

                Play(playerName, "Paper");
            }

            if (GUI.Button(new Rect(10, y += 25, 200, 20), "Scissor"))
            {

                Play(playerName, "Scissor");
            }

            foreach (var v in play_history)
            {
                GUI.Label(new Rect(10, y += 25, 200, 20), v.n + " played " + v.s);
            }

            return;
        }

        if (joined_room)
        {
            int y = 10;
            GUI.Label(new Rect(10, y += 25, 200, 20), "Players:");
            foreach (var v in playerNames)
            {
                GUI.Label(new Rect(10, y += 25, 200, 20), v);
            }
            return;
        }

        if (!entered_name)
        {
            int y = 10;
            playerName = GUI.TextField(new Rect(10, y += 25, 200, 20), playerName, 25);
            if (GUI.Button(new Rect(10, y += 25, 200, 20), "Enter"))
            {
                entered_name = true;
                PhotonNetwork.NickName = playerName;

                Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
            }
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        Debug.Log(photonEvent.Code);
        if (photonEvent.Code == 0)
        {
            // player played a card
            object[] data = (object[])photonEvent.CustomData;
            string player = (string)data[0];
            string choice = (string)data[1];

            player_choice choice1 = new player_choice();
            choice1.n = player;
            choice1.s = choice;

            play_history.Add(choice1);
        }
    }
}
