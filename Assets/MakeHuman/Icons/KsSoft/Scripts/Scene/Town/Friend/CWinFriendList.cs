using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using KS;

public class CWinFriendList : CWinFriendListBase {
	CFriend[]		m_aFriend = new CFriend[0];	
	// Sort state for Friend.
	enum e_SortFriendState {
		NameAsc,
		LevelDesc
	}
	e_SortFriendState m_eSortFriendState = e_SortFriendState.NameAsc;
	// ctrl
	CWinCtrlBase	m_ctrlFriendListBox;
	CWinCtrlBase	m_ctrlFriendNum;
	CWinCtrlBase	m_ctrlFriendNotFoundText;
	int				m_angle = 0;
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	public override void onCreate() {
		// Profile
		m_ctrlFriendNotFoundText	= find (TEXT_FriendNotFound);
		m_ctrlFriendListBox			= find (LISTBOX_FriendList);
		m_ctrlFriendNum				= find (RICHTEXT_FriendNum);
		find (BUTTON_Sort).caption = getCaption(MulId.Id(0,0,110));
		refresh();
	}
	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	public override void onUpdate() {
		// edit checkbox
		CWinCtrlCheckbox edit = find (CHECKBOX_Edit) as CWinCtrlCheckbox;
		
		// not found message
		if (m_aFriend.Length <= 0) {
			m_ctrlFriendNotFoundText.hide = false;
			m_ctrlFriendNum.hide = true;
			m_angle += Angle.Deg2Ang (500*Time.deltaTime);
			m_ctrlFriendNotFoundText.captionColor = new WinColor(127,127,127,(byte)(Angle.Sin(m_angle)*64+80));
			return;
		} else {
			m_ctrlFriendNotFoundText.hide = true;
			m_ctrlFriendNum.hide = false;
		}
		

		m_ctrlFriendNum.caption = getCaption(MulId.Id(10,10,340)) + " " + m_aFriend.Length + "\\c[0]/16";

		// list
		for (int i = 0;i < m_aFriend.Length;i++) {
			CFriend friend = m_aFriend[i];
			if (friend.Profile == null) {
				continue;
			}
			CWinContents contents = m_ctrlFriendListBox.getContentsFromIndex(i);
			friend.Profile.setContents(contents,ICON_FriendIcon,TEXT_FriendName,TEXTURE_FriendConnect,TEXTURE_FriendPlace,0,TEXT_FriendGuild,TEXTURE_FriendGuild,TEXT_FriendQuest,BUTTON_FriendParty,TEXTURE_FriendParty,BUTTON_FriendChat);
			// edit checkbox
			contents.find(BUTTON_FriendTrash).hide = !edit.check;
			contents.find (BUTTON_FriendParty).hide = edit.check;
			contents.find (BUTTON_FriendChat).hide = edit.check;
			contents.find (BUTTON_FriendReply).hide = edit.check;
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
		  case BUTTON_FriendReply:
			break;
		  case BUTTON_FriendParty:
			{
				int index = m_ctrlFriendListBox.getContentsIndex(cCtrl);
				CFriend friend = m_aFriend[index];
				inviteParty(friend);
			}
			break;
		  case BUTTON_FriendChat:
			{
				int index = m_ctrlFriendListBox.getContentsIndex(cCtrl);
				CFriend friend = m_aFriend[index];
			
				// private chat.
				CWinChatInput cWinChatInput = CWindowMgr.Instance.find (CWinChatInput.windowId) as CWinChatInput;
				if (cWinChatInput == null) {
					cWinChatInput = CWinChatInput.create ();
				}
				if (cWinChatInput != null) {
					cWinChatInput.openPrivateChat(friend.AUID);
				}
			}
			break;
		  case BUTTON_FriendTrash:
			break;
		  case ICON_FriendIcon:
			{
				int index = m_ctrlFriendListBox.getContentsIndex(cCtrl);
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
					// level sort
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
		case ICON_FriendIcon:
		{
			int index = m_ctrlFriendListBox.getContentsIndex(cCtrl);
			openFriendProfile(index);
		}
			break;
		}
	}
	//==========================================================================
	/*!openFriendProfile
		@brief	openFriendProfile
	*/
	private void openFriendProfile(int index) {
		CFriend friend = m_aFriend[index];
		CWinProfile.create(friend.AUID,this);
	}
	//==========================================================================
	/*!inviteParty
		@brief	inviteParty
	*/
	private void inviteParty(CFriend cFriend) {
	}
	//==========================================================================
	/*!refresh
		@brief	refresh
	*/
	public void refresh() {
		m_aFriend = CFriendMgr.Instance.getFriends();
		// sort.
		switch (m_eSortFriendState) {
		case e_SortFriendState.LevelDesc:
			Array.Sort (m_aFriend,delegate(CFriend friend0, CFriend friend1) { return (int)(friend1.Profile.Level - friend0.Profile.Level); });
			break;
		case e_SortFriendState.NameAsc:
			Array.Sort (m_aFriend,delegate(CFriend friend0, CFriend friend1) { return string.Compare(friend0.Profile.Name, friend1.Profile.Name); });
			break;
		}
		// resize
		m_ctrlFriendListBox.resize(m_aFriend.Length);
	}
}
