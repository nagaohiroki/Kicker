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
	Text mLog;
	[SerializeField]
	Text mTitle;
	[SerializeField]
	GameObject mLogin;
	[SerializeField]
	GameObject mBall;
	Dictionary<Team, int> mScore;
	float mGoalTime;
	bool mIsGaol;
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
		Debug.Log("ルームに入りました。 Master " + PhotonNetwork.LocalPlayer.IsMasterClient);
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
		var go = PhotonNetwork.Instantiate(mPlayerPrefab.name, new Vector3(0f, 0.5f, 0f), Quaternion.identity, 0);
		var player =  go.GetComponent<PlayerKicker>();
		player.CameraHandle = mCameraHandle;
		var players = FindObjectsOfType<PlayerKicker>();
		int left = 0;
		int right = 0;
		foreach(var item in players)
		{
			switch(item.Team)
			{
			case Team.Left:
				++left;
				break;
			case Team.Right:
				++right;
				break;
			}
		}
		var team = left >= right ? Team.Left : Team.Right;
		player.SetTeam(team, mBall);
		mLogin.SetActive(false);
	}
	public void Goal(Team inTeam)
	{
		if(!PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			return;
		}
		++mScore[inTeam];
		if(photonView != null)
		{
			photonView.RPC("GoalSync", RpcTarget.All, mScore[Team.Left], mScore[Team.Right]);
		}
	}
	[PunRPC]
	void GoalSync(int inL, int inR)
	{
		if(mIsGaol)
		{
			return;
		}
		mTitle.gameObject.SetActive(true);
		mTitle.text = string.Format("{0} - {1}", inL, inR);
		mGoalTime = 5.0f;
		mIsGaol = true;
	}
	void StartGame()
	{
		if(mScore == null)
		{
			mScore = new Dictionary<Team, int>
			{
				{Team.Left, 0},
				{Team.Right, 0},
			};
		}
		mTitle.gameObject.SetActive(false);
		var rigid = mBall.GetComponent<Rigidbody>();
		if(rigid != null)
		{
			rigid.Sleep();
		}
		mBall.transform.position = Vector3.zero;
		mIsGaol = false;
		mGoalTime = 0.0f;
	}
	void UpdateGoalTime()
	{
		if(!mIsGaol)
		{
			return;
		}
		mGoalTime -= Time.deltaTime;
		if(mGoalTime <= 0.0f)
		{
			StartGame();
		}
	}
	void Start()
	{
		StartGame();
	}
	void Update()
	{
		UpdateGoalTime();
	}
}
