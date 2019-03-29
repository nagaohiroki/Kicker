using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
public class PlayerKicker : MonoBehaviourPunCallbacks, IPunObservable
{
	[SerializeField]
	GameManager.Team mTeam;
	[SerializeField]
	Rigidbody mRigidbody;
	[SerializeField]
	PhotonTransformView mPVTransform;
	[SerializeField]
	TextMesh mUserName;
	Rigidbody mBallRigid;
	Transform mCameraHandle;
	// ------------------------------------------------------------------------
	/// @brief チームを設定
	///
	/// @param inTeam
	// ------------------------------------------------------------------------
	void SetTeam()
	{
		var players = FindObjectsOfType<PlayerKicker>();
		int left = 0;
		int right = 0;
		foreach(var item in players)
		{
			switch(item.mTeam)
			{
			case GameManager.Team.Left:
				++left;
				break;
			case GameManager.Team.Right:
				++right;
				break;
			}
		}
		Debug.LogWarningFormat("{0}:{1}", left, right);
		// mTeam = left < right ? GameManager.Team.Left : GameManager.Team.Right;
		mTeam = Random.Range(0, 2) == 0 ? GameManager.Team.Left : GameManager.Team.Right;
		var dic = new Dictionary<GameManager.Team, Color>
		{
			{GameManager.Team.Left, Color.red},
			{GameManager.Team.Right, Color.blue},
		};
		mUserName.color = dic[mTeam];
	}
	// ------------------------------------------------------------------------
	/// @brief ボール
	///
	/// @param inBall
	// ------------------------------------------------------------------------
	public void Initialize(GameObject inBall, Transform inCameraHandle)
	{
		mCameraHandle = inCameraHandle;
		if(inBall == null)
		{
			return;
		}
		mBallRigid = inBall.GetComponent<Rigidbody>();
		if(PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			return;
		}
		if(mBallRigid == null)
		{
			return;
		}
		mBallRigid.isKinematic = true;
		mBallRigid.useGravity = false;
	}
	// ------------------------------------------------------------------------
	/// @brief 同期
	///
	/// @param stream
	/// @param info
	// ------------------------------------------------------------------------
	void IPunObservable.OnPhotonSerializeView(PhotonStream inStream, PhotonMessageInfo inInfo)
	{
		if(inStream.IsWriting)
		{
			inStream.SendNext(mTeam);
			return;
		}
		mTeam = (GameManager.Team)inStream.ReceiveNext();
	}
	// ------------------------------------------------------------------------
	/// @brief 自分更新
	// ------------------------------------------------------------------------
	void UpdateMine()
	{
		if(mRigidbody == null || mCameraHandle == null || mPVTransform == null)
		{
			return;
		}
		mCameraHandle.transform.position = transform.position;
		var vec = Vector3.zero;
		const float minSpeed = 0.25f;
		vec.x = Input.GetAxis("Horizontal");
		vec.z = Input.GetAxis("Vertical");
		mRigidbody.MovePosition(transform.position + vec * minSpeed);
		if(vec.magnitude > 0.0f)
		{
			mRigidbody.MoveRotation(Quaternion.Euler(0.0f, Mathf.Rad2Deg * Mathf.Atan2(vec.x, vec.z), 0.0f));
		}
		if(Input.GetButtonDown("Fire1"))
		{
			Kick();
		}
	}
	// ------------------------------------------------------------------------
	/// @brief ボールをける
	// ------------------------------------------------------------------------
	void Kick()
	{
		Debug.Log("Kick");
		if(!mBallRigid)
		{
			return;
		}
		var vec = mBallRigid.position - mRigidbody.position;
		if(vec.magnitude > 10.0f)
		{
			return;
		}
		mBallRigid.AddForce(vec.normalized * 10.0f, ForceMode.VelocityChange);
	}
	// ------------------------------------------------------------------------
	/// @brief 初回更新
	// ------------------------------------------------------------------------
	void Start()
	{
		if(photonView == null || mUserName == null || mRigidbody == null)
		{
			return;
		}
		if(photonView.Owner != null)
		{
			mUserName.text = photonView.Owner.NickName;
		}
		//	mRigidbody.isKinematic = !photonView.IsMine;
		SetTeam();
	}
	// ------------------------------------------------------------------------
	/// @brief 更新
	// ------------------------------------------------------------------------
	void Update()
	{
		if(photonView != null && photonView.IsMine)
		{
			UpdateMine();
			return;
		}
	}
	void OnCollisionEnter(Collision inColl)
	{
		if(inColl.gameObject.layer != LayerMask.NameToLayer("Ball"))
		{
			return;
		}
		var rigid = inColl.gameObject.GetComponent<Rigidbody>();
		if(!rigid)
		{
			return;
		}
		var vec = rigid.position - mRigidbody.position;
		if(vec.magnitude > 10.0f)
		{
			return;
		}
		rigid.AddForce(vec.normalized * 10.0f, ForceMode.VelocityChange);
	}
}
