//==============================================================================================
/*!
	@file  MainSystem
*/
//==============================================================================================
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Object=UnityEngine.Object;
using KS;

//==========================================================================
/*!class
	@brief	Recieve class
*/
public class CMainSystem : CMainSystemBase {
	//==========================================================================
	/*!Awake
	 * @brief	Unity Callback
	*/
	new void Awake() {
		Application.targetFrameRate = 60;
		// スリープしないように設定.
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		base.Awake();

		if (m_instance != null) {
			Debug.LogError("already exist CMainSystem");
			return;
		}
		m_instance = this;

		// Add Component
		gameObject.AddComponent<CInput>();
	    gameObject.AddComponent<CAssetBundleMgr>();
		gameObject.AddComponent<CSpriteFontMgr>();
		gameObject.AddComponent<CTextureResourceMgr>();
		gameObject.AddComponent<CWindowMgr>();
		gameObject.AddComponent<CBgmResourceMgr>();
		gameObject.AddComponent<CSeResourceMgr>();

		gameObject.AddComponent<CSpriteDataMgr>();
		gameObject.AddComponent<CSoundEffectMgr>();

		// 初期化の必要なマネージャ.
		addManager(new CMessageDataSheetMgr());

		CWindowMgr.Instance.load ("windows");
	}
	//==========================================================================
	/*!初期化を行う.
	 * @brief	initialize
	*/
	override protected void initialize() {
		base.initialize();
		//--------------------------------------------
		// WindowMgr初期化.
		//--------------------------------------------
		CWindowMgr	cWindowMgr = CWindowMgr.Instance;
		// caption取得インターフェース初期化
		cWindowMgr.captiondata = CMessageDataSheetMgr.Instance.find(new FiveCC("WNDW"));
		// 標準SE割り当て.
		cWindowMgr.soundeffect = CSeResourceMgr.Instance.reference(new MulId(52,0,0),true);
		cWindowMgr.clickSE = new MulId(52,0,20);
		cWindowMgr.scrollSE = new MulId(52,0,110);
	}
	//==========================================================================
	/*!指定したフォルダ以下の該当するアセットを全て取得する.
		@brief	CollectAll 
	*/
#if UNITY_EDITOR
	public static List<T> CollectAll<T>(string path,bool bDeep) where T : Object {
		List<T> result = new List<T>();
		CollectAll<T>(result,path,bDeep);
		return result;
	}
	static void CollectAll<T>(List<T> result,string path,bool bDeep) where T : Object {
		string[] files = Directory.GetFiles(path);
		
		foreach (string file in files) {
			if (file.Contains(".meta")) continue;
			if (file.Contains(".bin")) continue;
			if (file.Contains(".h")) continue;
			T asset = (T) AssetDatabase.LoadAssetAtPath(file, typeof(T));
			if (asset == null) {
				continue;
			}
			result.Add(asset);
		}
		
		if (bDeep) {
			foreach (string dir in Directory.GetDirectories(path)) {
				if (dir.Contains(".svn")) continue;
				CollectAll<T>(result,dir,bDeep);
			}
		}
	}
#endif
	//==========================================================================
	/*!Instance.
		@brief	Instance.
	*/
	static new private CMainSystem	m_instance = null;
	new public static CMainSystem Instance {
        get {
            return m_instance;
        }
    }
}

