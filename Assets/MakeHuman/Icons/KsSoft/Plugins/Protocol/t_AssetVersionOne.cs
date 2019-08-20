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
	@brief	t_AssetVersionOne
*/
public class t_AssetVersionOne : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_mId;
	public string	m_path;
	public int	m_iVersion;
	public uint	m_uFlag;
	public ushort	m_width;
	public ushort	m_height;
	public ulong	m_uMd5a;
	public ulong	m_uMd5b;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_AssetVersionOne() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_mId = 0;
		m_path = "";
		m_iVersion = 0;
		m_uFlag = 0;
		m_width = 0;
		m_height = 0;
		m_uMd5a = 0UL;
		m_uMd5b = 0UL;
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_mId = cVariable.getU32();
			cVariable.getString(ref m_path,255);
			m_iVersion = cVariable.getS32();
			m_uFlag = cVariable.getU32();
			m_width = cVariable.getU16();
			m_height = cVariable.getU16();
			m_uMd5a = cVariable.getU64();
			m_uMd5b = cVariable.getU64();
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_mId);
		cVariable.put(ref m_path,255);
		cVariable.put(m_iVersion);
		cVariable.put(m_uFlag);
		cVariable.put(m_width);
		cVariable.put(m_height);
		cVariable.put(m_uMd5a);
		cVariable.put(m_uMd5b);
		return true;
	}
};

