using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text roomName = null;
    [SerializeField] TMP_Text playerCount = null;

    public RoomInfo info;

    public void Setup(RoomInfo _info)
    {
        info = _info;
        roomName.text = _info.Name;
        playerCount.text = _info.PlayerCount.ToString();
    }

    public void OnClick()
    {
        LauncherPUN.Instance.JoinRoom(info);
    }
}
