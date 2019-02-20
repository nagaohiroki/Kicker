using UnityEngine;
using Photon.Pun;
public class PlayerKicker : MonoBehaviour
{
	public Transform CameraHandle{private get; set;}
	[SerializeField]
	Rigidbody mRigidbody;
	[SerializeField]
	PhotonView myPV;
	[SerializeField]
	PhotonTransformView mPVTransform;
	[SerializeField]
	TextMesh mUserName;
	float mDashSpeed;
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
	void Update()
	{
		if(myPV != null && myPV.IsMine)
		{
			UpdateMine();
			return;
		}
	}
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
		if (vec.magnitude > 0.0f)
		{
			mRigidbody.MoveRotation(Quaternion.Euler(0.0f, Mathf.Rad2Deg * Mathf.Atan2(vec.x, vec.z), 0.0f));
		}
	}
}
