﻿using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
public class PlayerKicker : MonoBehaviourPunCallbacks, IPunObservable
{
	public Transform CameraHandle{private get; set;}
	public GameManager.Team Team{get; set;}
	[SerializeField]
	Rigidbody mRigidbody;
	[SerializeField]
	PhotonTransformView mPVTransform;
	[SerializeField]
	TextMesh mUserName;
	Rigidbody mBallRigid;
	// ------------------------------------------------------------------------
	/// @brief チームを設定
	///
	/// @param inTeam
	// ------------------------------------------------------------------------
	public void SetTeam(GameManager.Team inTeam)
	{
		Team = inTeam;
		var dic = new Dictionary<GameManager.Team, Color>
		{
			{GameManager.Team.Left, Color.red},
			{GameManager.Team.Right, Color.blue},
		};
		mUserName.color = dic[inTeam];
	}
	// ------------------------------------------------------------------------
	/// @brief ボール
	///
	/// @param inBall
	// ------------------------------------------------------------------------
	public void SetBall(GameObject inBall)
	{
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
		if (inStream.IsWriting)
		{
			inStream.SendNext(Team);
			return;
		}
		SetTeam((GameManager.Team)inStream.ReceiveNext());
	}
	// ------------------------------------------------------------------------
	/// @brief 自分更新
	// ------------------------------------------------------------------------
	void UpdateMine()
	{
		if(mRigidbody == null || CameraHandle == null || mPVTransform == null)
		{
			return;
		}
		CameraHandle.transform.position = transform.position;
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
