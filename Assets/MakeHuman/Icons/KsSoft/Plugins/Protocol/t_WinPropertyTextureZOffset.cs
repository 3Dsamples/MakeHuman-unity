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
	@brief	t_WinPropertyTextureZOffset
*/
public class t_WinPropertyTextureZOffset : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_texId;
	public int	m_zoffset;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_WinPropertyTextureZOffset() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_texId = 0;
		m_zoffset = 0;
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_texId = cVariable.getU32();
			m_zoffset = cVariable.getS32();
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_texId);
		cVariable.put(m_zoffset);
		return true;
	}
};

