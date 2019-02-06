using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
public class Login : MonoBehaviourPunCallbacks
{
	[SerializeField]
	Text mUserName;
	//ログインボタンを押したときに実行される
	void Connect()
	{
		if(!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.GameVersion = "1";
			PhotonNetwork.ConnectUsingSettings();
		}
	}
	public override void OnConnectedToMaster()
	{
		Debug.Log("マスターがルームに入りました。");
		PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions(), TypedLobby.Default);
	}
	public override void OnJoinedRoom()
	{
		Debug.Log("ルームに入りました。");
		if(mUserName != null)
		{
			PhotonNetwork.LocalPlayer.NickName = mUserName.text;
		}
		PhotonNetwork.IsMessageQueueRunning = false;
		PhotonNetwork.LoadLevel("Main");
	}
}
