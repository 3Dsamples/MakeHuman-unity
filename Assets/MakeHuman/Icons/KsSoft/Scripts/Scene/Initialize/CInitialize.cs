//==============================================================================================
/*!初期化シーン.
	@file  CInitialize
	@brief	初期化が終わったら次のシーンに移行する.
	
	(counter SJIS string 京.)
*/
//==============================================================================================
// 自動で接続を確立するときは、AUTO_CONNECTを有効にする.
//#define	AUTO_CONNECT
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using KS;

public class CInitialize : MonoBehaviour {
	public GameObject	m_goFadeObject;
	bool				m_bConnect;
	//==========================================================================
	/*!Start
		@brief Unity Callback
	*/
	IEnumerator Start() {
		// メインシステム作成.
		CMainSystem	cMainSystem = CMainSystem.Instance;

		GameObject goMS = new GameObject("MainSystem");
		cMainSystem = goMS.AddComponent<CMainSystem>();
		// フェードオブジェクトの適用.
		cMainSystem.fadeObject = m_goFadeObject;

		// 初期化読み込みプログレスバーを生成.
		while (CSpriteFontMgr.Instance == null) {
			yield return 0;
		}
		CAssetBundleMgr	cABMgr = CAssetBundleMgr.Instance;

		yield return 2;
        // 初期化待ち.
        CWinAssetbundleLoading cLoading = CWinAssetbundleLoading.create ();
        while (!cMainSystem.isInitialized) {
			if (cABMgr == null) {
				yield return 0;
				continue;
			}
			switch (cABMgr.initializeState) {
			  case CAssetBundleMgr.e_InitializeState.CheckVersion:
				cLoading.message = "Checking Version...";
				break;
			case CAssetBundleMgr.e_InitializeState.CheckCacheFile:
				cLoading.message = "Checking Cached file...";
				break;
			case CAssetBundleMgr.e_InitializeState.Finish:
				cLoading.message = "Now Loading...";
				break;
			}
			yield return 0;
		}
		cMainSystem.changeScene("Title");
	}
}
