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
	@brief	t_WinPropertyString
*/
public class t_WinPropertyString : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public string	m_string;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_WinPropertyString() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_string = "";
	}
	public bool read(CReadVariable cVariable) {
		try {
			cVariable.getString(ref m_string,65535);
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(ref m_string,65535);
		return true;
	}
};

