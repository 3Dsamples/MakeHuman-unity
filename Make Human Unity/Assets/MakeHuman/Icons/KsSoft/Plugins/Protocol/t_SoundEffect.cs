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
	@brief	t_SoundEffect
*/
public class t_SoundEffect : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_id;
	public uint	m_mAssetBundle;
	public uint	m_idClip;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_SoundEffect() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_id = 0;
		m_mAssetBundle = 0;
		m_idClip = 0;
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_id = cVariable.getU32();
			m_mAssetBundle = cVariable.getU32();
			m_idClip = cVariable.getU32();
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_id);
		cVariable.put(m_mAssetBundle);
		cVariable.put(m_idClip);
		return true;
	}
};

