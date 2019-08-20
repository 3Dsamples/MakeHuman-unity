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
	@brief	t_MessageArg
*/
public class t_MessageArg : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public sbyte	m_eType;
	public int	m_value;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_MessageArg() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_eType = 0;
		m_value = 0;
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_eType = cVariable.getS8();
			m_value = cVariable.getS32();
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_eType);
		cVariable.put(m_value);
		return true;
	}
};

