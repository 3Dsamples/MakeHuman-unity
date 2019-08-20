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
	@brief	t_BillboardFrame
*/
public class t_BillboardFrame : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_texid;
	public uint	m_color;
	public float	m_frame;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_BillboardFrame() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_texid = 0;
		m_color = 0;
		m_frame = 0.0f;
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_texid = cVariable.getU32();
			m_color = cVariable.getU32();
			m_frame = cVariable.getFloat();
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_texid);
		cVariable.put(m_color);
		cVariable.put(m_frame);
		return true;
	}
};

