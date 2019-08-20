//==============================================================================================
/*!チャット一文を保持.
	@file	CChatMgr
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections.Generic;
using KS;

public class CChatSentence {
	uint			m_noSeq;
	e_ChatMode		m_eMode;
	ulong			m_toAUID;
	ulong			m_AUID;
	uint			m_mIcon;
	string			m_name;
	string			m_sSentence;
	static	WinColor[]	m_aColor = new WinColor[(int) e_ChatMode.Num];
	//==========================================================================
	/*!class Constructor
		@brief	class Constructor
	*/
	static CChatSentence() {
		m_aColor[(int) e_ChatMode.Private] = WinColor.white;
		m_aColor[(int) e_ChatMode.Party] = WinColor.green;
		m_aColor[(int) e_ChatMode.Guild] = WinColor.yellow;
		m_aColor[(int) e_ChatMode.Info] = new WinColor(32,32,127,255);
		m_aColor[(int) e_ChatMode.System] = new WinColor(64,64,127,255);
	}
	//==========================================================================
	/*!Constructor
		@brief	Constructor
	*/
	public CChatSentence(uint noSeq,e_ChatMode eMode,ulong toAUID,ulong AUID,string name,uint mIcon,string sSentence) {
		m_noSeq = noSeq;		//受信した順番.
		m_eMode = eMode;
		m_toAUID = toAUID;
		m_AUID = AUID;
		m_name = name;
		m_mIcon = mIcon;
		m_sSentence = sSentence;
	}
	public e_ChatMode	mode {
		get {
			return m_eMode;
		}
	}
	public ulong toAUID {
		get {
			return m_toAUID;
		}
	}
	public ulong AUID {
		get {
			return m_AUID;
		}
	}
	public string name {
		get {
			return m_name;
		}
	}
	public string sentence {
		get {
			return m_sSentence;
		}
	}
	public uint sequence {
		get {
			return m_noSeq;
		}
	}
	public uint icon {
		get {
			return m_mIcon;
		}
	}
	public WinColor color {
		get {
			return m_aColor[(int) m_eMode];
		}
	}
}
