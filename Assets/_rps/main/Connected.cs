using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Connected : MainApplicationReference
{
    MainApplication application;
    bool joining = false;
    public override void Init(MainApplication application)
    {
        this.application = application;
    }
    public override void OnJoinedRoom()
    {
        application.UpdateState(MainApplication.State.kEnteredRoom);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MainApplication.kMaxPlayersPerRoom });
    }
    void OnGUI()
    {
        int y = 10;
        if (joining)
        {
            GUI.Label(new Rect(10, y += 25, 200, 20), "Finding opponent...");
        }
        else
        {
            GUI.Label(new Rect(10, y += 25, 200, 20), "Connected!");
            GUI.Label(new Rect(10, y += 25, 200, 20), "Welcome! " + PhotonNetwork.NickName);
            GUI.enabled = !joining;
            if (GUI.Button(new Rect(10, y += 25, 200, 20), "Quick play"))
            {
                PhotonNetwork.JoinRandomRoom();
                joining = true;
            }
            GUI.enabled = true;

        }
    }
}
