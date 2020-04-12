using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MainApplication : MonoBehaviourPunCallbacks
{
    public const int kMaxPlayersPerRoom = 1;
    public enum State
    {
        kNone, kNotConnected, kConnected, kEnteredRoom, kGameStarted
    }
    State state = State.kNone;

    public MainApplicationReference currentStateScript = null;

    public void UpdateState(State newState)
    {
        if (state == newState)
        {
            return;
        }
        state = newState;
        GameObject.Destroy(currentStateScript);
        switch (state)
        {
            case State.kNotConnected: currentStateScript = gameObject.AddComponent<NotConnected>(); currentStateScript.Init(this); break;
            case State.kConnected: currentStateScript = gameObject.AddComponent<Connected>(); currentStateScript.Init(this); break;
            case State.kEnteredRoom: currentStateScript = gameObject.AddComponent<EnteredRoom>(); currentStateScript.Init(this); break;
            case State.kGameStarted: currentStateScript = gameObject.AddComponent<GameStarted>(); currentStateScript.Init(this); break;
            default:
                break;
        }
    }
    void Awake() { PhotonNetwork.AutomaticallySyncScene = true; }
    void Start()
    {
        UpdateState(State.kNotConnected);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
        UpdateState(State.kNotConnected);
    }
}


