using UnityEngine;
using System.Collections;
using KS;

public class CWinBottomPart : CWinBottomPartBase {
	public enum e_mode {
		Menu,
		Chat,
		Num
	};
	e_mode	m_eMode = e_mode.Menu;
	int m_angle;

	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	public override void onCreate() {
	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl) {
		switch (cCtrl.id) {
		case BUTTON_Toggle:
			switch (m_eMode) {
			case e_mode.Menu:
				m_eMode = e_mode.Chat;
				refresh();
				break;
			case e_mode.Chat:
				m_eMode = e_mode.Menu;
				refresh();
				break;
			}
			break;
		}
	}
	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	public override void onUpdate() {
		// unread
		CChatMgr cChatMgr = CChatMgr.Instance;
		if (cChatMgr == null) {
			return;
		}
		bool isUnread = false;
		if (m_eMode == e_mode.Menu) {
			for (int i = 0; i < (int)e_ChatMode.Num; i++) {
				if (cChatMgr.isUnread[i]) {
					isUnread = true;
					m_angle += Angle.Deg2Ang(500f * Time.deltaTime);
					float	k = Angle.Sin (m_angle);
					byte c = (byte) Mathf.Min ((k + 3f) * 64f,255f);
					find (BUTTON_Toggle).color = new WinColor(c,c,c,255);
					break;
				}
			}
		}
		if (!isUnread) {
			find (BUTTON_Toggle).color = new WinColor(127,127,127,255);
		}
	}
	public void refresh() {
		CWinTabbar		cWinTabbar		= CWindowMgr.Instance.find<CWinTabbar>(CWinTabbar.windowId);
		CWinChatInput	cWinChatInput	= CWindowMgr.Instance.find<CWinChatInput>(CWinChatInput.windowId);
		CWinCtrlButton	cToggleButton	= find (BUTTON_Toggle) as CWinCtrlButton;
		switch (m_eMode) {
		case e_mode.Menu:
			cToggleButton.partId = new FiveCC("MBCT?");
			cToggleButton.caption = getCaption(MulId.Id(10,20,40));
			if (cWinChatInput) {
				cWinChatInput.close ();
			}
			if (cWinTabbar == null) {
				cWinTabbar = CWinTabbar.create();
			}
			break;
		case e_mode.Chat:
			cToggleButton.partId = new FiveCC("MBMN?");
			cToggleButton.caption = getCaption(MulId.Id(0,0,290));
			if (cWinTabbar) {
				cWinTabbar.close ();
			}
			if (cWinChatInput == null) {
				cWinChatInput = CWinChatInput.create ();
			}
			break;
		}
	}
	public void toggleToChatMode() {
		m_eMode = e_mode.Chat;
		refresh ();
	}
}
