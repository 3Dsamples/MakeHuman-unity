using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using KS;

public class CWinFriendApply : CWinFriendApplyBase {
	CFriend[]		m_aApplying = new CFriend[0];
	// Sort state for Friend.
	enum e_SortFriendState {
		NameAsc,
		LevelDesc,
	}
	e_SortFriendState m_eSortFriendState = e_SortFriendState.NameAsc;
	// ctrl
	CWinCtrlBase	m_ctrlApplyingListBox;
	CWinCtrlBase	m_ctrlSearchBox;
	CWinCtrlBase	m_ctrlNotFound;
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	public override void onCreate() {
		m_ctrlApplyingListBox	= find (LISTBOX_ApplyList);
		m_ctrlSearchBox			= find(EDITBOX_ApplySearchBox);
		find (BUTTON_Sort).caption = getCaption(MulId.Id(0,0,110));
		refresh();
	}
	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	public override void onUpdate() {
		// not found
		if (m_aApplying.Length <= 0) {
			return;
		}
		// list
		for (int i = 0;i < m_aApplying.Length;i++) {
			CFriend friend = m_aApplying[i];
			if (friend.Profile == null) {
				continue;
			}
			CWinContents contents = m_ctrlApplyingListBox.getContentsFromIndex(i);
			friend.Profile.setContents(contents,ICON_ApplyIcon,TEXT_ApplyName,TEXTURE_ApplyConnect,TEXTURE_ApplyPlace,0,TEXT_ApplyGuild,TEXTURE_ApplyGuild,TEXT_ApplyQuest,0,0,0);
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
		  case BUTTON_ApplyCancel:
			break;
		  case ICON_ApplyIcon:
			{
				int index = m_ctrlApplyingListBox.getContentsIndex(cCtrl);
				openFriendProfile(index);
			}
			break;
		  case BUTTON_Sort:
			{
				CWinCtrlBase ctrlSort = find (BUTTON_Sort);
				if (m_eSortFriendState == e_SortFriendState.LevelDesc) {
					// name sort
					m_eSortFriendState = e_SortFriendState.NameAsc;
					ctrlSort.caption = getCaption(MulId.Id(0,0,110));
				} else if (m_eSortFriendState == e_SortFriendState.NameAsc) {
					// date sort
					m_eSortFriendState = e_SortFriendState.LevelDesc;
					ctrlSort.caption = getCaption(MulId.Id(0,0,120));
				}
				refresh();
			}
			break;
		}
	}
	public override void onHold(CWinCtrlBase cCtrl) {
		switch (cCtrl.id) {
		case ICON_ApplyIcon:
			{
				int index = m_ctrlApplyingListBox.getContentsIndex(cCtrl);
				CFriend friend = m_aApplying[index];
				CWinProfile.create(friend.AUID,this);
			}
			break;
		}
	}
	//==========================================================================
	/*!openFriendProfile
		@brief	openFriendProfile
	*/
	private void openFriendProfile(int index) {
		CFriend friend = m_aApplying[index];
		CWinProfile.create(friend.AUID,this);
	}
	//==========================================================================
	/*!onClickEnter
		@brief	onClickEnter
	*/
	public override void onClickEnter(CWinCtrlBase cCtrl) {
		if (cCtrl != null && cCtrl == m_ctrlSearchBox) {
			string str = cCtrl.caption.Trim();
			if (KsSoftUtility.isInvalidSQL(str)) {
				// invalid character specified
				CMessageBox.create(MulId.Id(2,0,30), CMessageBox.e_Type.Ok, 0, this);
			}
		}
	}
	//==========================================================================
	/*!refresh
		@brief	refresh
	*/
	public void refresh() {
		m_aApplying = CFriendMgr.Instance.getFriends();
		// sort.
		switch (m_eSortFriendState) {
		case e_SortFriendState.LevelDesc:
			Array.Sort (m_aApplying,delegate(CFriend friend0, CFriend friend1) { return (int)(friend1.Profile.Level - friend0.Profile.Level); });
			break;
		case e_SortFriendState.NameAsc:
			Array.Sort (m_aApplying,delegate(CFriend friend0, CFriend friend1) { return string.Compare(friend0.Profile.Name, friend1.Profile.Name); });
			break;
		}
		// resize
		m_ctrlApplyingListBox.resize(m_aApplying.Length);
	}
}
