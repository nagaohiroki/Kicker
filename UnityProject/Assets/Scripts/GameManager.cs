using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
public class GameManager : MonoBehaviourPunCallbacks
{
	[SerializeField]
	PlayerKicker mPlayerPrefab;
	[SerializeField]
	Transform mCameraHandle;
	[SerializeField]
	Text mUserName;
	[SerializeField]
	GameObject mLogin;
	//ログインボタンを押したときに実行される
	public void Connect()
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
		LoginPlayer();
	}
	void LoginPlayer()
	{
		if(mPlayerPrefab == null)
		{
			return;
		}
		var go = PhotonNetwork.Instantiate(mPlayerPrefab.name, new Vector3(0f, 10.0f, 0f), Quaternion.identity, 0);
		var player =  go.GetComponent<PlayerKicker>();
		player.CameraHandle = mCameraHandle;
		mLogin.SetActive(false);
	}
}
