using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using KS;

public class CWinFriend : CWinFriendBase {
	const int	MsgBox_FriendInvite	= 1;
	
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	public override void onCreate() {
		CFriendMgr	cFriendMgr = CFriendMgr.Instance;

		uint	focusCtrlId = 0;
		switch (cFriendMgr.state) {
		case CFriendMgr.e_State.Accepted:
			{
				focusCtrlId = RADIO_RadioFriendList;
				CWinFriendList.create();
			}
			break;
		case CFriendMgr.e_State.Applying:
			{
				focusCtrlId = RADIO_RadioApplying;
				CWinFriendApply.create();
			}
			break;
		case CFriendMgr.e_State.Blacklist:
			{
				focusCtrlId = RADIO_RadioBlacklist;
				CWinFriendBlacklist.create();
			}
			break;
		}
		if (focusCtrlId != 0) {
			CWinCtrlRadio focus	= find (focusCtrlId) as CWinCtrlRadio;
			focus.check			= true;
		}
	}
	//==========================================================================
	/*!onClose
		@brief	Window Callback
	*/
	public override bool onClose(int iCloseInfo) {
		closeSubWindows();
		return true;
	}
	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	public override void onUpdate() {		
		if (isClose) {
			return;
		}
		// radio button
		switchActiveDentTab();
	}
	//==========================================================================
	/*!switchActiveDentTab
		@brief	switchActiveDentTab
	*/
	private void switchActiveDentTab(CWinCtrlRadio cCtrl = null) {
		if (cCtrl == null) {
			switch (CFriendMgr.Instance.state) {
			case CFriendMgr.e_State.Accepted:
				cCtrl = find (RADIO_RadioFriendList) as CWinCtrlRadio;
				CWinFriendList list = CWindowMgr.Instance.find(CWinFriendList.windowId) as CWinFriendList;
				if (list == null) {
					closeSubWindows();
					list = CWinFriendList.create();
				}
				break;
			case CFriendMgr.e_State.Applying:
				cCtrl = find (RADIO_RadioApplying) as CWinCtrlRadio;
				CWinFriendApply apply = CWindowMgr.Instance.find(CWinFriendApply.windowId) as CWinFriendApply;
				if (apply == null) {
					closeSubWindows();
					apply = CWinFriendApply.create();
				}
				break;
			case CFriendMgr.e_State.Blacklist:
				cCtrl = find (RADIO_RadioBlacklist) as CWinCtrlRadio;
				CWinFriendBlacklist blacklist = CWindowMgr.Instance.find(CWinFriendBlacklist.windowId) as CWinFriendBlacklist;
				if (blacklist == null) {
					closeSubWindows();
					blacklist = CWinFriendBlacklist.create();
				}
				break;
			}
		}
	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl) {
		CFriendMgr	cFriendMgr = CFriendMgr.Instance;
		if (cFriendMgr == null) {
			return;
		}
		switch (cCtrl.id) {
		  // Radio (dent tab)
		  case RADIO_RadioFriendList:
			{
				closeSubWindows();
				CWinFriendList.create();
			}
			break;
		  case RADIO_RadioApplying:
			{
				closeSubWindows();
				CWinFriendApply.create();
			}
			break;
		case RADIO_RadioBlacklist:
			{
				closeSubWindows();
				CWinFriendBlacklist.create();
			}
			break;
		  case BUTTON_BackBack:
			CTown cTown = CTown.Instance;
			if (cTown != null) {
				cTown.Mode = CTown.e_mode.Home;
			}
			break;
		}
	}
	private void closeSubWindows() {
		CWinFriendList cWinFriendList = CWindowMgr.Instance.find (CWinFriendList.windowId) as CWinFriendList;
		if (cWinFriendList != null) {
			cWinFriendList.close();
		}
		CWinFriendApply cWinFriendApply = CWindowMgr.Instance.find (CWinFriendApply.windowId) as CWinFriendApply;
		if (cWinFriendApply != null) {
			cWinFriendApply.close();
		}
		CWinFriendBlacklist cWinFriendBlacklist = CWindowMgr.Instance.find(CWinFriendBlacklist.windowId) as CWinFriendBlacklist;
		if (cWinFriendBlacklist != null) {
			cWinFriendBlacklist.close ();
		}
	}
	public void hideTabs(bool bHide) {
		find (RADIO_RadioFriendList).hide = find (RADIO_RadioApplying).hide = find (RADIO_RadioBlacklist).hide = bHide;
	}
}
