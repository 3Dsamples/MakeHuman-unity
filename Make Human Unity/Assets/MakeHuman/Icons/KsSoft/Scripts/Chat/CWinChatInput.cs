using UnityEngine;
using System.Collections;
using KS;

public class CWinChatInput : CWinChatInputBase {
	// mgr.
	CChatMgr			m_cChatMgr;
	CProfileMgr			m_cProfileMgr;
	// ctrl.
	CWinCtrlEditbox		m_ctrlChatInput;
	bool				m_bFlag;
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	public override void onCreate() {
		m_cChatMgr		= CChatMgr.Instance;
		m_cProfileMgr	= CProfileMgr.Instance;
		
		m_ctrlChatInput		= find (EDITBOX_Chat)	as CWinCtrlEditbox;

		// TODO:
		find (RADIO_Quest).hide = find (TEXTURE_Quest).hide = true;
	}
	//==========================================================================
	/*!onClickEnter
		@brief	Window Callback
	*/
	public override void onClickEnter(CWinCtrlBase cCtrl) {
		string	sSentence = m_ctrlChatInput.caption;
#if !(UNITY_IPHONE || UNITY_ANDROID)
		// focus.
		CWindowMgr.Instance.focusObject = m_ctrlChatInput;
#endif
		// clear input box before seinding.
		m_ctrlChatInput.caption = "";

		CProfile	profile;
		if (m_bFlag) {
			profile = m_cProfileMgr.own;
			m_bFlag = false;
		} else {
			profile = m_cProfileMgr.getProfile(6);
			m_bFlag = true;
		}
		m_cChatMgr.append (m_cChatMgr.ChatMode,1,profile.AUID,profile.Name,profile.iconId,sSentence);

	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl) {
		switch (cCtrl.id) {
		case BUTTON_Phrase:
			break;
		case RADIO_Private:
			m_ctrlChatInput.caption = "";
			break;
		case RADIO_Party:
			break;
		case RADIO_Guild:
			break;
		case RADIO_Quest:
			//m_cChatMgr.ChatMode = e_ChatMode.Quest;
			break;
		case FRAME_Target:
			break;
		}
	}
	public void openPrivateChat(ulong uAUID) {
		m_cChatMgr.privateAUID = uAUID;
		setPrivateChatMode();
		focusOn();
		CWinBottomPart cWinBottomPart = CWindowMgr.Instance.find (CWinBottomPart.windowId) as CWinBottomPart;
		if (cWinBottomPart != null) {
			cWinBottomPart.toggleToChatMode();
		}
	}
	void setPrivateChatMode() {
		m_cChatMgr.ChatMode = e_ChatMode.Private;
		m_cChatMgr.isUnread[(int)e_ChatMode.Private] = false;
		CWinCtrlRadio ctrl = find (RADIO_Private) as CWinCtrlRadio;
		ctrl.color = new WinColor(127,127,127,255);
		ctrl.check = true;
	}
	public void focusOn() {
		m_ctrlChatInput.caption = "";
		CWindowMgr.Instance.focusObject = m_ctrlChatInput;
	}
}
