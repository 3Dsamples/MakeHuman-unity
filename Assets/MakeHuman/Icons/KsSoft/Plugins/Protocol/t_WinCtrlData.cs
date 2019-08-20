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
	@brief	t_WinCtrlData
*/
public class t_WinCtrlData : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_kind;
	public uint	m_propertyNum;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_WinCtrlData() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_kind = 0;
		m_propertyNum = 0;
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_kind = cVariable.getU32();
			m_propertyNum = cVariable.getU32();
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_kind);
		cVariable.put(m_propertyNum);
		return true;
	}
};

