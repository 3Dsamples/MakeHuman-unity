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
	@brief	t_WinProperty
*/
public class t_WinProperty : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public int	m_value;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_WinProperty() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_value = 0;
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_value = cVariable.getS32();
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_value);
		return true;
	}
};

