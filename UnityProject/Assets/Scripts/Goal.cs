using UnityEngine;
public class Goal : MonoBehaviour
{
	[SerializeField]
	GameManager.Team mTeam;
	[SerializeField]
	GameManager mGameManager;
	void OnTriggerEnter(Collider inColl)
	{
		if (mGameManager == null)
		{
			return;
		}
		mGameManager.Goal(mTeam);
	}
}
