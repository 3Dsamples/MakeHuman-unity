//==============================================================================================
/*!
	@file  KsSoft Protocol
	
	(counter SJIS string 京)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using KS;
//==========================================================================
/*!
	@brief	GmsvMessage
*/
public class GmsvMessage : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public sbyte	m_eChatMode;
	public t_Message	m_tMessage;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public GmsvMessage() {
		m_tMessage = new t_Message();
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_eChatMode = 0;
		m_tMessage.clear();
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_eChatMode = cVariable.getS8();
			if (!m_tMessage.read(cVariable)) return false;
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_eChatMode);
		if (!m_tMessage.write(cVariable)) return false;
		return true;
	}
};

