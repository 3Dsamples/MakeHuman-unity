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
	@brief	t_EffectAttributeHeader
*/
public class t_EffectAttributeHeader : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public  const uint	MAGIC_NO = 4276944900;	// 4276944900
	public uint	m_magicNo;
	public t_EffectAttribute[]	m_aEffectAttribute;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_EffectAttributeHeader() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_magicNo = 4276944900;	// MAGIC_NO;
		m_aEffectAttribute = new t_EffectAttribute[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_magicNo = cVariable.getU32();
			{
				int n = (int) cVariable.getU16();
				if (n > 65535) {
					cVariable.error("array size error in m_aEffectAttribute");
					return false;
				}
				m_aEffectAttribute = new t_EffectAttribute [n];
				for(int i = 0;i < n;++i) {
					m_aEffectAttribute[i] = new t_EffectAttribute();
					if (!m_aEffectAttribute[i].read(cVariable)) return false;
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
				if (m_aEffectAttribute.Length > 65535) {
					cVariable.error("array size error in m_aEffectAttribute");
					return false;
				}
			int n = m_aEffectAttribute.Length;
			cVariable.put((ushort) n);
			for(int i = 0;i < n;++i) {
				if (!m_aEffectAttribute[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

