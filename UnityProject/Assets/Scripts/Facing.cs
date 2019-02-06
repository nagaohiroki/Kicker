using UnityEngine;

public class Facing : MonoBehaviour
{
	void Update()
	{
		var cam = Camera.main;
		transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
	}
}
