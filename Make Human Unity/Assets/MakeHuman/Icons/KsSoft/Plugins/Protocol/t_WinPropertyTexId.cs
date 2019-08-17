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
	@brief	t_WinPropertyTexId
*/
public class t_WinPropertyTexId : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_texId;
	public uint	m_partId;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_WinPropertyTexId() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_texId = 0;
		m_partId = 0;
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_texId = cVariable.getU32();
			m_partId = cVariable.getU32();
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_texId);
		cVariable.put(m_partId);
		return true;
	}
};

