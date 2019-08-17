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
	@brief	t_MessageDataOne
*/
public class t_MessageDataOne : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_id;
	public string	m_value;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_MessageDataOne() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_id = 0;
		m_value = "";
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_id = cVariable.getU32();
			cVariable.getString(ref m_value,16384);
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_id);
		cVariable.put(ref m_value,16384);
		return true;
	}
};

