using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class GameManager : MonoBehaviour
{
	[SerializeField]
	PlayerKicker mPlayerPrefab;
	[SerializeField]
	Transform mCameraHandle;
	void Start()
	{
		PhotonNetwork.IsMessageQueueRunning = true;
		if(!PhotonNetwork.IsConnected)
		{
			SceneManager.LoadScene("Login");
			return;
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
	}
}
