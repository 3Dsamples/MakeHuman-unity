//==============================================================================================
/*!チャットマネージャ.
	@file	CChatMgr
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using KS;

public class CChatMgr {
	List<CChatSentence>[]	m_aLstSentence = new List<CChatSentence>[(int) e_ChatMode.Num];
	uint					m_noSeq = 0;
	e_ChatMode				m_eChatMode = e_ChatMode.Party;
	CWinChat				m_cWinChat = null;

	const	int LogMax	= 160;

	bool[] m_isUnread = new bool[(int)e_ChatMode.Num];
	ulong					m_auidPrivate;
	//==========================================================================
	/*!Constructor
		@brief	Constructor
	*/
	public CChatMgr() {
		if (m_instance != null) {
			Debug.LogError("already instance CChatMgr");
		}
		m_instance = this;
		for (int i = 0;i < m_aLstSentence.Length;i++) {
			m_aLstSentence[i] = new List<CChatSentence>();
		}
		m_auidPrivate = 0;
	}
	//==========================================================================
	/*!Release
		@brief	release
	*/
	public void release() {
		for (int i = 0;i < m_aLstSentence.Length;++i) {
			m_aLstSentence[i].Clear ();
		}
		m_noSeq = 0;
		m_eChatMode = e_ChatMode.Party;
		m_cWinChat = null;
		m_instance = null;
		m_auidPrivate = 0;
	}
	//==========================================================================
	/*!チャットを指定した数だけ取得する.
		@brief	getChatList
	*/
	public List<CChatSentence>	getChatList(int n = LogMax) {
		List<CChatSentence>	lstResult = new List<CChatSentence>();
		List<CChatSentence>	lst = m_aLstSentence[(int) m_eChatMode];

		for (int i = lst.Count - 1;i >= 0;--i) {
			lstResult.Add (lst[i]);
			--n;
			if (n == 0) {
				break;
			}
		}
		lstResult.Reverse();
		return lstResult;
	}
	//==========================================================================
	/*!チャットの一文を追加する.
		@brief	append
	*/
	public int append(e_ChatMode eMode,ulong toAUID,ulong frAUID,string frName,uint mIcon,string sSentence) {
		if (eMode >= e_ChatMode.Num) {
			Debug.LogError ("chat kind is illegal:" + eMode);			
			return -1;
		}
		if (sSentence == null || sSentence.Length == 0) {
//			Debug.LogError ("chat sentence is null!!");
			return -2;
		}
		List<CChatSentence>	lstTarget = m_aLstSentence[(int) eMode];
		
		// 文を追加.
		CChatSentence	cSentence = new CChatSentence(m_noSeq++,eMode,toAUID,frAUID,frName,mIcon,sSentence);
		lstTarget.Add(cSentence);
		// 最大ログ数を超えたときは消す.
		if (lstTarget.Count > LogMax) {
			// 先頭が一番古い.
			lstTarget.RemoveAt (0);
		}
		if (m_cWinChat != null) {
			m_cWinChat.append (cSentence);
		}
		return 0;
	}
	//==========================================================================
	/*!チャットの一文をシーケンスIDから取得する.
		@brief	find
	*/
	public CChatSentence find(uint seqId) {
		List<CChatSentence>	lst = m_aLstSentence[(int) m_eChatMode];
		foreach (CChatSentence cSentence in lst) {
			if (cSentence.sequence == seqId) {
				return cSentence;
			}
		}
		return null;
	}
	public e_ChatMode ChatMode {
		get {
			return m_eChatMode; 
		}
		set {
			if (m_eChatMode == value) {
				return;
			}
			m_eChatMode = value; 
			if (m_cWinChat != null) {
				m_cWinChat.initializeLog();
			}
		}
	}
	public	CWinChat	window {
		set {
			m_cWinChat = value;
		}
	}
	public	int logMax {
		get {
			return LogMax;
		}
	}
	public bool[] isUnread {
		get {
			return m_isUnread;
		}
		set {
			m_isUnread = value;
		}
	}
	public ulong privateAUID {
		get {
			return m_auidPrivate;
		}
		set {
			m_auidPrivate = value;
		}
	}
	static protected CChatMgr	m_instance;
	public static CChatMgr Instance {
        get {
            return m_instance;
        }
    }
}
