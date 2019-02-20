using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
public class GameManager : MonoBehaviourPunCallbacks
{
	public enum Team
	{
		Left,
		Right,
	}
	[SerializeField]
	PlayerKicker mPlayerPrefab;
	[SerializeField]
	Transform mCameraHandle;
	[SerializeField]
	Text mUserName;
	[SerializeField]
	Text mTitle;
	[SerializeField]
	GameObject mLogin;
	[SerializeField]
	GameObject mBall;
	Dictionary<Team, int> mScore;
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
	public void Goal(Team inTeam)
	{
		if(mScore == null || !mScore.ContainsKey(inTeam))
		{
			return;
		}
		++mScore[inTeam];
		mTitle.gameObject.SetActive(true);
		mTitle.text = string.Format("{0} - {1}", mScore[Team.Left], mScore[Team.Right]);
	}
	void StartGame()
	{
		mScore = new Dictionary<Team, int>
		{
			{Team.Left, 0},
			{Team.Right, 0},
		};
		mTitle.gameObject.SetActive(false);
		mBall.transform.position = Vector3.zero;
	}
	void Start()
	{
		StartGame();
	}
}
