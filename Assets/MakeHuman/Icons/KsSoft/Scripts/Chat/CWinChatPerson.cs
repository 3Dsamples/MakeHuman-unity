using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using KS;

public class CWinChatPerson : CWinChatPersonBase {
	CWinCtrlListbox		m_ctrlListParty;
	CWinCtrlListbox		m_ctrlListFriend;
	CWinCtrlListbox		m_ctrlListGuild;
	
	CProfile[] m_aProfileParty = null;
	CProfile[] m_aProfileFriend = null;
	CProfile[] m_aProfileGuild = null;
	
	int m_angle = 0;
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	public override void onCreate() {
		m_ctrlListParty		= find (LISTBOX_Party) as CWinCtrlListbox;
		m_ctrlListFriend	= find (LISTBOX_Friend) as CWinCtrlListbox;
		m_ctrlListGuild		= find (LISTBOX_Guild) as CWinCtrlListbox;
		switchActiveRadioButton(RADIO_Party);
		refresh ();
	}
	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	public override void onUpdate() {
		m_angle += Angle.Deg2Ang(500*Time.deltaTime);
		find (TEXT_Title).captionColor = new WinColor(255,255,255,(byte)(Angle.Sin(m_angle)*64+80));
		// party.
		if (m_aProfileParty != null) {
			m_ctrlListParty.resize(m_aProfileParty.Length);
			for (int i = 0; i < m_aProfileParty.Length; i++) {
				CProfile profile = m_aProfileParty[i];
				if (profile != null) {
					CWinContents contents = m_ctrlListParty.getContentsFromIndex(i);
					profile.setContents(contents, ICON_Party, TEXT_PartyName, TEXTURE_PartyConnect, 0, TEXT_PartyLevel,0,0,0,0,0,0);
				}
			}
		}
		// friend.
		if (m_aProfileFriend != null) {
			m_ctrlListFriend.resize(m_aProfileFriend.Length);
			for (int i = 0; i < m_aProfileFriend.Length; i++) {
				CProfile profile = m_aProfileFriend[i];
				if (profile != null) {
					CWinContents contents = m_ctrlListFriend.getContentsFromIndex(i);
					profile.setContents(contents, ICON_Friend, TEXT_FriendName, TEXTURE_FriendConnect, 0, TEXT_FriendLevel,0,0,0,0,0,0);
				}
			}
		}
	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl) {
		switch (cCtrl.id) {
		case BUTTON_Party:
		{
			int index = m_ctrlListParty.getContentsIndex(cCtrl);
			CProfile profile = m_aProfileParty[index];
			setPrivateChat(profile.AUID);
			close ();
		}
			break;
		case BUTTON_Friend:
		{
			int index = m_ctrlListFriend.getContentsIndex(cCtrl);
			CProfile profile = m_aProfileFriend[index];
			setPrivateChat(profile.AUID);
			close ();
		}
			break;
		case BUTTON_Guild:
		{
			int index = m_ctrlListGuild.getContentsIndex(cCtrl);
			CProfile profile = m_aProfileGuild[index];
			setPrivateChat(profile.AUID);
			close ();
		}
			break;
		case ICON_Party:
		{
			int index = m_ctrlListParty.getContentsIndex(cCtrl);
			CProfile profile = m_aProfileParty[index];
			if (profile != null) {
				CWinProfile.create(profile.AUID, this);
			}
		}
			break;
		case ICON_Friend:
		{
			int index = m_ctrlListFriend.getContentsIndex(cCtrl);
			CProfile profile = m_aProfileFriend[index];
			if (profile != null) {
				CWinProfile.create(profile.AUID, this);
			}
		}
			break;
		case ICON_Guild:
		{
			int index = m_ctrlListGuild.getContentsIndex(cCtrl);
			CProfile profile = m_aProfileGuild[index];
			if (profile != null) {
				CWinProfile.create(profile.AUID, this);
			}
		}
			break;
		case RADIO_Friend:
			switchActiveRadioButton(cCtrl.id);
			break;
		case RADIO_Guild:
			goto case RADIO_Friend;
		case RADIO_Party:
			goto case RADIO_Friend;
		case BUTTON_Close:
			close ();
			break;
		}
	}
	private void switchActiveRadioButton(uint ctrlID) {
		find (RADIO_Friend).color = find (RADIO_Guild).color = find (RADIO_Party).color = new WinColor(127,127,127,255);
		find (RADIO_Friend).captionColor	= new WinColor(110,110,110,255);
		find (RADIO_Guild).captionColor		= new WinColor(110,110,110,255);
		find (RADIO_Party).captionColor		= new WinColor(110,110,110,255);
		find (ctrlID).color				= new WinColor(127,127,127,255);
		find (ctrlID).captionColor		= new WinColor(15,15,15,255);
	}
	private void setPrivateChat(ulong uAUID) {
		CChatMgr	cChatMgr = CChatMgr.Instance;
		if (cChatMgr == null) {
			return;
		}
		// chat window.
		CWinChatInput cWinChatInput = CWindowMgr.Instance.find (CWinChatInput.windowId) as CWinChatInput;
		if (cWinChatInput == null) {
			cWinChatInput = CWinChatInput.create ();
		}
		if (cWinChatInput != null) {
			cWinChatInput.focusOn();
			cChatMgr.privateAUID = uAUID;
		}
		if (CChatMgr.Instance != null) {
			cChatMgr.ChatMode = e_ChatMode.Private;
		}
	}
	public void refresh () {
		// friend.
		CFriend[] aFriend = CFriendMgr.Instance.getFriends();
		m_aProfileFriend = new CProfile[aFriend.Length];
		for (int i = 0;i < aFriend.Length;++i) {
			m_aProfileFriend[i] = aFriend[i].Profile;
		}
	}
}
