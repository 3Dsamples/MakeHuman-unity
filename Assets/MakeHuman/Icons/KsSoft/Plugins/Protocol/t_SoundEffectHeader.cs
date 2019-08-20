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
	@brief	t_SoundEffectHeader
*/
public class t_SoundEffectHeader : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public  const uint	MAGIC_NO = 4276949008;	// 4276949008
	public uint	m_magicNo;
	public t_SoundEffect[]	m_aData;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_SoundEffectHeader() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_magicNo = 4276949008;	// MAGIC_NO;
		m_aData = new t_SoundEffect[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_magicNo = cVariable.getU32();
			{
				int n = (int) cVariable.getU16();
				if (n > 65535) {
					cVariable.error("array size error in m_aData");
					return false;
				}
				m_aData = new t_SoundEffect [n];
				for(int i = 0;i < n;++i) {
					m_aData[i] = new t_SoundEffect();
					if (!m_aData[i].read(cVariable)) return false;
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_magicNo);
		{	
				if (m_aData.Length > 65535) {
					cVariable.error("array size error in m_aData");
					return false;
				}
			int n = m_aData.Length;
			cVariable.put((ushort) n);
			for(int i = 0;i < n;++i) {
				if (!m_aData[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

