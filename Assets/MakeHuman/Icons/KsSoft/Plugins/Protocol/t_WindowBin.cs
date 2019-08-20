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
	@brief	t_WindowBin
*/
public class t_WindowBin : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public string	m_name;
	public string	m_exportpath;
	public uint	m_mAssetbundle;
	public t_WindowData	m_data;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_WindowBin() {
		m_data = new t_WindowData();
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_name = "";
		m_exportpath = "";
		m_mAssetbundle = 0;
		m_data.clear();
	}
	public bool read(CReadVariable cVariable) {
		try {
			cVariable.getString(ref m_name,255);
			cVariable.getString(ref m_exportpath,255);
			m_mAssetbundle = cVariable.getU32();
			if (!m_data.read(cVariable)) return false;
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(ref m_name,255);
		cVariable.put(ref m_exportpath,255);
		cVariable.put(m_mAssetbundle);
		if (!m_data.write(cVariable)) return false;
		return true;
	}
};

