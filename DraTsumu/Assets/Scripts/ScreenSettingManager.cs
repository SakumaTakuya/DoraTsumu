#if UNITY_STANDALONE
using UnityEngine;

public class GameInitial //: MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod]
	private static void OnRuntimeMethodLoad()
	{
		Screen.SetResolution(1024, 1536, true, 60);

	}
}
#endif
