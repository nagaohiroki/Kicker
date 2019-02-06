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
	void Start()
	{
		if(myPV == null || mUserName == null || mRigidbody == null)
		{
			return;
		}
		mUserName.text = myPV.Owner.NickName;
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
		vec.x = Input.GetAxis("Horizontal") * 0.1f;
		vec.z = Input.GetAxis("Vertical") * 0.1f;
		mRigidbody.MovePosition(transform.position + vec);
	}
}
