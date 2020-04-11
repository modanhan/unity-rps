using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
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
        if (playerNames.Count == 1)
        {
            game_started = true;
        }
        else if (playerNames.Count > 1)
        {
            Debug.LogError("More than 2 players? how?");
        }
    }

    #endregion

    bool entered_name = false;
    public string playerName;
    void OnGUI()
    {
        if (game_started)
        {
            int y = 10;
            if (GUI.Button(new Rect(10, y += 25, 200, 20), "Rock"))
            {

            }

            if (GUI.Button(new Rect(10, y += 25, 200, 20), "Paper"))
            {

            }

            if (GUI.Button(new Rect(10, y += 25, 200, 20), "Scissor"))
            {

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
}
