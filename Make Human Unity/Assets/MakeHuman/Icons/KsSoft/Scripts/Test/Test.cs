using UnityEngine;
using System.Collections;
using KS;

public class Test : MonoBehaviour {

	//==========================================================================
	/*!Start
		@brief Unity Callback
	*/
	IEnumerator Start() {
		// メインシステム作成.
		CMainSystem	cMainSystem = CMainSystem.Instance;
		
		GameObject goMS = new GameObject("MainSystem");
		cMainSystem = goMS.AddComponent<CMainSystem>();

		yield return 2;

		while (!cMainSystem.isInitialized) {
			yield return 0;
		}
		CWinTest.create ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
