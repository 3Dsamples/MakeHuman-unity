//==============================================================================================
/*!町スクリプト.
	@file	CTown
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using KS;

public class CTown : MonoBehaviour {
	CBG				m_cBG;
	public enum e_mode {
		None,
		Home,
		Friend,
		Quest,
		Item,
		Gatcha,
		Shop,
		Guild,
		Num
	};
	e_mode m_eMode = e_mode.None;
	
	static uint[]	m_aChildWindow = new uint[] {
		CWinHome.windowId,
		CWinFriend.windowId,
		CWinQuest.windowId,
		CWinQuestStory.windowId,
		CWinQuestMulti.windowId,
/*
		CWinGuildInit.windowId,
		CWinGuild.windowId,
		CWinGuildCreate.windowId,
		CWinGuildBoard.windowId,
		CWinInventory.windowId,
		CWinShop.windowId,
		CWinGatcha.windowId,
		CWinQuestList.windowId,
		CWinQuestColloseum.windowId,
		CWinFriendMail.windowId,
		CWinGuildBoard.windowId
*/
	};
	//==========================================================================
	/*!Awake
		@brief	Unity Callback
	*/
	void Awake() {
		if (m_instance != null) {
			Debug.LogError ("already exist town instance");
		}
		m_instance = this;
		m_cBG = null;
		GameObject	go = GameObject.Find("BG");
		if (go != null) {
			m_cBG = go.GetComponent<CBG>();
		}
	}
	//==========================================================================
	/*!OnDestroy
		@brief	Unity Callback
	*/
	void OnDestroy() {
		if (m_instance == null) {
			Debug.LogError ("town instance is already null");
		}
		m_instance = null;
	}
	//==========================================================================
	/*!Start
		@brief	Unity Callback
	*/
	void Start() {
		CMainSystem	cMainSystem = CMainSystem.Instance;
		CFadeScreen cFS = cMainSystem.fadeScreen;
		// BGM再生.
		CSoundEffectMgr.Instance.playBgm(new MulId(51,10,0));
		// ウィンドウオープン.	
		CWinTopPart.create();
		CWinBottomPart cWinBottomPart = CWinBottomPart.create();
		cWinBottomPart.refresh();

		cFS.startFadeIn();

		if (Mode == e_mode.None) {
			Mode = e_mode.Home;
		}
	}
	//==========================================================================
	/*!closeChildWindows
		@brief	close windows
	*/
	private void closeChildWindows() {
		CWindowMgr	cWindowMgr = CWindowMgr.Instance;
		
		foreach (uint winId in m_aChildWindow) {
			CWindowBase cWin = cWindowMgr.find(winId);
			if (cWin != null) {
				if (!cWin.isClose) {
					cWin.close();
				}
			}
		}
	}
	//==========================================================================
	/*!町のモードを切り替える.
		@brief	Mode
	*/
	public e_mode Mode {
		get {
			return m_eMode; 
		}
		set {
			if (m_eMode == value) {
				return;
			}
			m_eMode = value;
			closeChildWindows ();
			switch (m_eMode) {
			  case e_mode.Home:
				CWinHome.create ();
				break;
			  case e_mode.Friend:
				CWinFriend.create();
				break;
			  case e_mode.Quest:
				CWinQuest.create();
				break;
			  case e_mode.Guild:
//				CWinGuild.create();
				CMessageBox.create("Sorry...",CMessageBox.e_Type.Ok,0);
				Mode =e_mode.Home;
				break;
			  case e_mode.Item:
				CMessageBox.create("Sorry...",CMessageBox.e_Type.Ok,0);
				Mode =e_mode.Home;
				break;
			  case e_mode.Gatcha:
				// TODO:
				//CWinGatcha.create ();
				CMessageBox.create("Sorry...",CMessageBox.e_Type.Ok,0);
				Mode =e_mode.Home;
				break;
			  case e_mode.Shop:
				// TODO:
				//CWinShop.create ();
				CMessageBox.create("Sorry...",CMessageBox.e_Type.Ok,0);
				Mode =e_mode.Home;
				break;
			}
		}
	}
	//==========================================================================
	/*!背景取得.
		@brief	background.
	*/
	public CBG	background {
		get {
			return m_cBG;
		}
	}
	//==========================================================================
	/*!Instance.
		@brief	Instance.
	*/
	static protected CTown	m_instance;
	public static CTown Instance {
        get {
            return m_instance;
        }
    }
}
