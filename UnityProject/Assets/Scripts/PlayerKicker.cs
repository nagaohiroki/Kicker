using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
public class PlayerKicker : MonoBehaviour
{
	public Transform CameraHandle{private get; set;}
	public Rigidbody Ball{private get; set;}
	public GameManager.Team Team{get; set;}
	[SerializeField]
	Rigidbody mRigidbody;
	[SerializeField]
	PhotonView myPV;
	[SerializeField]
	PhotonTransformView mPVTransform;
	[SerializeField]
	TextMesh mUserName;
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
		const float minSpeed = 0.5f;
		vec.x = Input.GetAxis("Horizontal");
		vec.z = Input.GetAxis("Vertical");
		mRigidbody.MovePosition(transform.position + vec * minSpeed);
		if(vec.magnitude > 0.0f)
		{
			mRigidbody.MoveRotation(Quaternion.Euler(0.0f, Mathf.Rad2Deg * Mathf.Atan2(vec.x, vec.z), 0.0f));
		}
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Kick();
		}
	}
	// ------------------------------------------------------------------------
	/// @brief ボールをける
	// ------------------------------------------------------------------------
	void Kick()
	{
		if(!Ball)
		{
			return;
		}
		var vec = Ball.position - mRigidbody.position;
		if(vec.magnitude > 2.0f)
		{
			return;
		}
		Ball.AddForce(vec.normalized * 10.0f, ForceMode.VelocityChange);
	}
	// ------------------------------------------------------------------------
	/// @brief 初回更新
	// ------------------------------------------------------------------------
	void Start()
	{
		if(myPV == null || mUserName == null || mRigidbody == null)
		{
			return;
		}
		if(myPV.Owner != null)
		{
			mUserName.text = myPV.Owner.NickName;
		}
		mRigidbody.isKinematic = !myPV.IsMine;
	}
	// ------------------------------------------------------------------------
	/// @brief 更新
	// ------------------------------------------------------------------------
	void Update()
	{
		if(myPV != null && myPV.IsMine)
		{
			UpdateMine();
			return;
		}
	}
}

