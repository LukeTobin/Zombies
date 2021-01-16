using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerModelServerUpdates : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] Camera camera;
    [Space]
    [SerializeField] Quaternion currentRotation;
    [SerializeField] Quaternion readRotation;


    public void OnPhotonSerializeView(PhotonStream p_stream, PhotonMessageInfo p_message){
        if(p_stream.IsWriting){
            p_stream.SendNext((Quaternion)currentRotation);
        }else{
            readRotation = (Quaternion)p_stream.ReceiveNext();
        }
    }

    void Update(){
        if(photonView.IsMine){
            currentRotation = camera.transform.localRotation;
        }

        currentRotation = camera.transform.localRotation;
        ShowRotationGlobal();
        
        //photonView.RPC("ShowRotationGlobal", RpcTarget.All);
    }

    [PunRPC]
    void ShowRotationGlobal(){
        Quaternion cachedRotation = readRotation;
        cachedRotation.x = 0;
        cachedRotation.z = 0;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, cachedRotation, 8f);
        transform.localRotation = cachedRotation;
    }
}
