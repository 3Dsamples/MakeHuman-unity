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
	@brief	t_SoundParamHeader
*/
public class t_SoundParamHeader : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public  const uint	MAGIC_NO = 4276948992;	// 4276948992
	public uint	m_magicNo;
	public t_SoundParam[]	m_aSoundParam;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_SoundParamHeader() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_magicNo = 4276948992;	// MAGIC_NO;
		m_aSoundParam = new t_SoundParam[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_magicNo = cVariable.getU32();
			{
				int n = (int) cVariable.getU16();
				if (n > 65535) {
					cVariable.error("array size error in m_aSoundParam");
					return false;
				}
				m_aSoundParam = new t_SoundParam [n];
				for(int i = 0;i < n;++i) {
					m_aSoundParam[i] = new t_SoundParam();
					if (!m_aSoundParam[i].read(cVariable)) return false;
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
				if (m_aSoundParam.Length > 65535) {
					cVariable.error("array size error in m_aSoundParam");
					return false;
				}
			int n = m_aSoundParam.Length;
			cVariable.put((ushort) n);
			for(int i = 0;i < n;++i) {
				if (!m_aSoundParam[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

