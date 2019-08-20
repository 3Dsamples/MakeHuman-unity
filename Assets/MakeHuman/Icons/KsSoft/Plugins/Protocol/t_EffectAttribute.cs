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
	@brief	t_EffectAttribute
*/
public class t_EffectAttribute : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_mId;
	public uint	m_mNoneId;
	public uint	m_mFireId;
	public uint	m_mWaterId;
	public uint	m_mEarthId;
	public uint	m_mLightId;
	public uint	m_mDarkId;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_EffectAttribute() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_mId = 0;
		m_mNoneId = 0;
		m_mFireId = 0;
		m_mWaterId = 0;
		m_mEarthId = 0;
		m_mLightId = 0;
		m_mDarkId = 0;
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_mId = cVariable.getU32();
			m_mNoneId = cVariable.getU32();
			m_mFireId = cVariable.getU32();
			m_mWaterId = cVariable.getU32();
			m_mEarthId = cVariable.getU32();
			m_mLightId = cVariable.getU32();
			m_mDarkId = cVariable.getU32();
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_mId);
		cVariable.put(m_mNoneId);
		cVariable.put(m_mFireId);
		cVariable.put(m_mWaterId);
		cVariable.put(m_mEarthId);
		cVariable.put(m_mLightId);
		cVariable.put(m_mDarkId);
		return true;
	}
};

