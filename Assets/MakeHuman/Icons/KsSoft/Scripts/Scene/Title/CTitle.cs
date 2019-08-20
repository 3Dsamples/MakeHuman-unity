//==============================================================================================
/*!タイトルシーン用スクリプト.
	@file	CTitle
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using System.IO;
using KS;

public class CTitle : MonoBehaviour {
	//==========================================================================
	/*!Awake
		@brief	Unity Callback
	*/
	void Awake() {
		if (m_instance != null) {
			Debug.LogError ("already exist instance");
		}
		m_instance = this;
	}
	//==========================================================================
	/*!Start
		@brief	Unity Callback
	*/
	void Start() {
		CFadeScreen cFS = CMainSystem.Instance.fadeScreen;
		CSoundEffectMgr.Instance.playBgm(new MulId(51,0,0));
		cFS.startFadeIn();
		CWinTitle.create();
		CWinLogin.create();
	}
	static CTitle	m_instance = null;
	static public CTitle Instance {
		get {
			return m_instance;
		}
	}
	
}
