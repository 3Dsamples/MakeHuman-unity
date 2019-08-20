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
	@brief	t_SoundParam
*/
public class t_SoundParam : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_id;
	public uint	m_mClip;
	public byte	m_volume;
	public byte	m_priority;
	public byte	m_group;
	public byte	m_maxPolyphony;
	public float	m_minDistance;
	public float	m_maxDistance;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_SoundParam() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_id = 0;
		m_mClip = 0;
		m_volume = 0;
		m_priority = 0;
		m_group = 0;
		m_maxPolyphony = 0;
		m_minDistance = 0.0f;
		m_maxDistance = 0.0f;
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_id = cVariable.getU32();
			m_mClip = cVariable.getU32();
			m_volume = cVariable.getU8();
			m_priority = cVariable.getU8();
			m_group = cVariable.getU8();
			m_maxPolyphony = cVariable.getU8();
			m_minDistance = cVariable.getFloat();
			m_maxDistance = cVariable.getFloat();
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_id);
		cVariable.put(m_mClip);
		cVariable.put(m_volume);
		cVariable.put(m_priority);
		cVariable.put(m_group);
		cVariable.put(m_maxPolyphony);
		cVariable.put(m_minDistance);
		cVariable.put(m_maxDistance);
		return true;
	}
};

