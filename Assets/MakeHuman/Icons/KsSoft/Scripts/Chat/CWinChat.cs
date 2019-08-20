//==============================================================================================
/*!Chatウィンドウ.
	@file	CWinChat
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using KS;

public class CWinChat : CWinChatBase {
	const int MAX_CHAT_FRAME_H = 600;
	const int MIN_CHAT_FRAME_H	= 48;
	const int MIN_BATTLE_CHAT_FRAME_H = 24;
	const int CHAT_ONE_COLUMN_SIZE = 80;
	const int CHAT_OPEN_CLOSE_SPEED = 14;
	const int BOTTOM_PART_FRAME_H = 64;
	const int OFFSET_Y = 112;
	// mgr.
	CChatMgr			m_cChatMgr;
	CProfileMgr			m_cProfileMgr;
	// ctrl.
	CWinCtrlLog			m_ctrlLog;
	CWinCtrlFrame		m_ctrlFrame;
	CWinCtrlButton		m_ctrlButton;
	// battle mode.
	bool				m_bBattleMode = false;

#if UNITY_IPHONE
	float			m_fChatLogOffsetY	= 0f;
	float			m_fChatLogHeight	= 0f;
#endif

	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	public override void onCreate() {
		m_cChatMgr		= CChatMgr.Instance;
		m_cProfileMgr	= CProfileMgr.Instance;

		// chat log resize.
		m_ctrlLog = find(LOG_Chat) as CWinCtrlLog;
		m_ctrlLog.resize(m_cChatMgr.logMax);
		
		m_ctrlFrame		= find (FRAME_Chat) as CWinCtrlFrame;
		m_ctrlButton	= find (BUTTON_Resize) as CWinCtrlButton;

		// initialize chat log
		m_cChatMgr.window = this;
		initializeLog();

		logHeight = CHAT_ONE_COLUMN_SIZE;
				
#if UNITY_IPHONE
		m_fChatLogHeight	= m_ctrlLog.size.y;
		m_fChatLogOffsetY	= m_ctrlLog.position.y;
#endif
	}
	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	public override void onUpdate() {
		if (m_bBattleMode) {
			if (logHeight < CHAT_ONE_COLUMN_SIZE) {
				if (position.y > -OFFSET_Y) {
					position = new Vector2(position.x,position.y-CHAT_OPEN_CLOSE_SPEED);
				}
			} else {
				if (position.y < 0) {
					position = new Vector2(position.x,position.y+CHAT_OPEN_CLOSE_SPEED);
				}
			}
		}
		return;
#if UNITY_IPHONE
		CWinChatInput cWinChatInput = CWindowMgr.Instance.find (CWinChatInput.windowId) as CWinChatInput;
		if (cWinChatInput != null && cWinChatInput.mode == CWinChatInput.e_mode.Minimize) {
			float scale = (float)Screen.width / TouchScreenKeyboard.area.width;
			float y = scale * TouchScreenKeyboard.area.y;
			if (TouchScreenKeyboard.visible) {
				float h = System.Math.Abs(y-m_fChatLogOffsetY) - 150;
				m_ctrlLog.size = new Vector2(m_ctrlLog.size.x, h);
				m_ctrlLog.offset = new Vector2(0f,m_ctrlLog.screensize.y - m_ctrlLog.viewsize.y);
				Debug.Log(TouchScreenKeyboard.area);
			} else {
				m_ctrlLog.size = new Vector2(m_ctrlLog.size.x, m_fChatLogHeight);
			}
		}
#endif
	}
	//==========================================================================
	/*!チャット一文を挿入する.
		@brief	IChat::append
	*/
	public void append(CChatSentence cSentence) {
		// check.
		if (cSentence.mode != m_cChatMgr.ChatMode) {
			m_cChatMgr.isUnread[(int)cSentence.mode] = true;
			return;
		}

		// sentence.
		CWinCtrlLogText ctrlLogText = m_ctrlLog.add (cSentence.sentence,WinColor.black);

		// contents.
		CWinContents contents = ctrlLogText.parent;
		contents.id = cSentence.sequence;
		// icon.
		CWinCtrlIcon icon = contents.find (ICON_Chat) as CWinCtrlIcon;
		// TODO:
/*
		CItemSrcDataOne	cISDO = cSentence.isdo;
		if (cISDO == null) {
			CItemSrcDataOne.clearIconCtrl(icon);
		} else {
			cISDO.setIconCtrl(icon);
		}
*/
		CProfile profile = m_cProfileMgr.getProfile(cSentence.AUID);
		if (profile != null) {
			profile.setAvatarIcon(icon);
		}
		// name.
		CWinCtrlBase name = contents.find (TEXT_Name);
		if (!profile.isOwnPlayer) {
			name.caption = cSentence.name;
		}
		// balloon.
		CWinCtrlBase baloon = contents.find (FRAME_Balloon);
		baloon.height = ctrlLogText.height + 20f;
		// own or not
		if (profile.isOwnPlayer) {
			// name
			if (m_cChatMgr.ChatMode == e_ChatMode.Private) {
				CProfile toProfile = m_cProfileMgr.getProfile(cSentence.toAUID);
				if (toProfile != null) {
					name.caption = "To:" + toProfile.Name;
				}
				name.anchor = e_Anchor.RightTop;
				name.textAnchor = e_Anchor.RightTop;
				name.position = new Vector2(-180,name.position.y);
			} else {
				name.hide = true;
			}
			// icon
			icon.anchor = e_Anchor.RightTop;
			icon.position = new Vector2(-64f,icon.position.y);
			// balloon
			baloon.position = new Vector2(32,baloon.position.y);
			baloon.partId = FiveCC.Id("CHTFR");
			// sentence
			ctrlLogText.position = new Vector2(40,ctrlLogText.position.y);
		}

	}
	//==========================================================================
	/*!チャットログを初期化する.
		@brief	IChat::initializeLog
	*/
	public void initializeLog() {
		m_ctrlLog.clearLog ();
		m_ctrlLog.resize(m_cChatMgr.logMax);
		List<CChatSentence> lstSentence = m_cChatMgr.getChatList();
		foreach (CChatSentence cSentence in lstSentence) {
			append (cSentence);
		}
	}
	//==========================================================================
	/*!onDrag
		@brief	Window Callback
	*/
	public override void onDrag (CWinCtrlBase cCtrl,Vector2 pos,Vector2 dragVelocity) {
		switch (cCtrl.id) {
		case BUTTON_Resize:
		{
			Vector2	delta = cCtrl.absPosition - pos;
			logHeight += (int) delta.y;
		}
			break;
		}
	}
	//==========================================================================
	/*!onDragRender
		@brief	Window Callback
		@retval	true:	ドラッグされているコントロールをレンダリングする.
				false:	ドラッグされているコントロールをレンダリングしない.
	*/
	public override bool onDragRender(CWinCtrlBase cCtrl,Transform transform) {
		return false;
	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl) {
		CChatMgr	cChatMgr = CChatMgr.Instance;
		if (cChatMgr == null) {
			return;
		}
		switch (cCtrl.id) {
		  case BUTTON_Resize:
			autoresize();
			break;
		  case ICON_Chat:
			break;
		  case FRAME_Balloon:
			goto case ICON_Chat;
		  case LOGTEXT_Chat:
			goto case ICON_Chat;
		  case TEXT_Name:
			goto case ICON_Chat;
		}
	}
	//==========================================================================
	/*!onHold
		@brief	Window Callback
	*/
	public override void onHold(CWinCtrlBase cCtrl) {
		CChatMgr	cChatMgr = CChatMgr.Instance;
		if (cChatMgr == null) {
			return;
		}
		switch (cCtrl.id) {
		case ICON_Chat:
		{
			CWinContents	cContents = cCtrl.parent;
			if (cContents == null) {
				break;
			}
			CChatSentence cSentence = cChatMgr.find(cContents.id);
			if (cSentence == null) {
				break;
			}
			CWinProfile.create(cSentence.AUID, this);
		}
			break;
		}
	}
	//==========================================================================
	/*!サイズを変更する.
		@brief	autoresize
	*/
	void autoresize() {
		int	h = logHeight;
		if (h == MAX_CHAT_FRAME_H) {
			if (m_bBattleMode) {
				logHeight = MIN_BATTLE_CHAT_FRAME_H;
			} else {
				logHeight = MIN_CHAT_FRAME_H;
			}
			m_ctrlButton.partId = FiveCC.Id("CHSZ?");
		} else if ((MIN_CHAT_FRAME_H + MAX_CHAT_FRAME_H)/2 <= h) {
			logHeight = MAX_CHAT_FRAME_H;
			m_ctrlButton.partId = FiveCC.Id("CHSD?");
		} else if (CHAT_ONE_COLUMN_SIZE <= h) {
			logHeight = (MIN_CHAT_FRAME_H + MAX_CHAT_FRAME_H)/2;
			m_ctrlButton.partId = FiveCC.Id("CHSZ?");
		} else {
			logHeight = CHAT_ONE_COLUMN_SIZE;
			m_ctrlButton.partId = FiveCC.Id("CHSZ?");
		}
	}
	public int logHeight {
		get {
			return (int) m_ctrlFrame.size.y;
		}
		set {
			if (value > MAX_CHAT_FRAME_H) {
				value = MAX_CHAT_FRAME_H;
				m_ctrlButton.partId = FiveCC.Id("CHSD?");
			} else if (value < MIN_CHAT_FRAME_H) {
				if (m_bBattleMode) {
					value = MIN_BATTLE_CHAT_FRAME_H;
				} else {
					value = MIN_CHAT_FRAME_H;
				}
				m_ctrlButton.partId = FiveCC.Id("CHSZ?");
			} else {
				m_ctrlButton.partId = FiveCC.Id("CHSZ?");
			}
			if (value == m_ctrlFrame.size.y) {
				return;
			}
			float	d = value - m_ctrlFrame.size.y;

			m_ctrlFrame.size	= new Vector2(m_ctrlFrame.size.x,value);
			m_ctrlLog.size		= new Vector2(m_ctrlLog.size.x,value);
			m_ctrlLog.contentsSize = m_ctrlLog.size;
			height = (float) value + BOTTOM_PART_FRAME_H;

			if (d < 0f) {
				Vector2	offset = m_ctrlLog.offset;
				offset.y -= d;
				m_ctrlLog.offset = offset;
			}
		}
	}
	public void minimize() {
		logHeight = CHAT_ONE_COLUMN_SIZE;
	}

	public bool BattleMode {
		get {
			return m_bBattleMode;
		}
		set {
			m_bBattleMode = value;
			logHeight = MIN_BATTLE_CHAT_FRAME_H;
			position = new Vector2(position.x,0);
		}
	}
}
