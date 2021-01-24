using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class LauncherPUN : MonoBehaviourPunCallbacks
{
    public static LauncherPUN Instance;
    [SerializeField] TMP_Text username = null;
    [SerializeField] TMP_Text pingText = null;
    [Space]
    [SerializeField] Transform roomListContent = null;
    [SerializeField] GameObject roomListItemPrefab = null;
    [Space]
    [SerializeField] Transform playerListContent = null;
    [SerializeField] GameObject playerListItemPrefab = null;
    [Space]
    [SerializeField] GameObject startButton = null;

    // default nickname if none were selected
    string currentNickname = "Player";

    // Allow for local play
    bool isOnline = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(isOnline)
            pingText.text = "ping: " + PhotonNetwork.GetPing().ToString();
    }

    #region Custom Functions

    /// <summary>
    /// Attempt to connect to the master server.
    /// </summary>
    public void ConnectToServers()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// Creates a custom room with the user being the host/master.
    /// </summary>
    public void CreateRoom()
    {   
        UpdateUsername();
        MenuManager.Instance.OpenMenu("loading");
        PhotonNetwork.CreateRoom("room_dev"); // callback will be either OnJoinedRoom or OnCreateRoomFailed
    }

    /// <summary>
    /// If a room is selected in the room browser, the player attempts to connect to that room
    /// </summary>
    /// <param name="info">Information about the room we're attempting to join.</param>
    public void JoinRoom(RoomInfo info)
    {
        UpdateUsername();

        MenuManager.Instance.OpenMenu("loading");
        PhotonNetwork.JoinRoom(info.Name);
    }

    /// <summary>
    /// Leave's the current room and notifies the server.
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// Currently just brings all players to the dev room.
    /// </summary>
    public void StartGame()
    {
        if (isOnline)
            PhotonNetwork.LoadLevel(1);
        else
            SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Function updates the players username, if a new one has been entered.
    /// </summary>
    void UpdateUsername()
    {
        if (string.IsNullOrEmpty(username.text))
        {
            Debug.Log("was null");
            int num = Random.Range(0, 999);
            PhotonNetwork.NickName = "Player " + num;
            currentNickname = "Player " + num;
        }else{
            Debug.Log("was full: " + username.text.Length + " ~ " + username.text);
            PhotonNetwork.NickName = username.text;
            currentNickname = username.text;
        }
    }

    /// <summary>
    /// Currently just exit's the application. Possible to be extended in the future to save data before closing at this stage
    /// </summary>
    public void ExitClient()
    {
        // save data possible here?
        Application.Quit();
    }
    #endregion

    #region Callback Functions

    /*
     * Callback functions from Photon servers.
     */

    public override void OnConnectedToMaster()
    {
        Debug.Log("Joined Master");
        isOnline = true;
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("main");
        Debug.Log("Joined Lobby");
        
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        Debug.Log("Joined New Room");
        Player[] playerListCount = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < playerListCount.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(playerListCount[i]);
        }

        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed To Create Room");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // clear the current list for future updates.
        foreach (Transform t in roomListContent)
        {
            Destroy(t.gameObject);
        }

        // update list.
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().Setup(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(newPlayer);
    }

    #endregion
}