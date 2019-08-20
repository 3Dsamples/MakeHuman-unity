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
	@brief	t_ModelEffectHeader
*/
public class t_ModelEffectHeader : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public  const uint	MAGIC_NO = 4276944903;	// 4276944903
	public uint	m_magicNo;
	public t_ModelEffect[]	m_aModelEffect;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_ModelEffectHeader() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_magicNo = 4276944903;	// MAGIC_NO;
		m_aModelEffect = new t_ModelEffect[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_magicNo = cVariable.getU32();
			{
				int n = (int) cVariable.getU16();
				if (n > 65535) {
					cVariable.error("array size error in m_aModelEffect");
					return false;
				}
				m_aModelEffect = new t_ModelEffect [n];
				for(int i = 0;i < n;++i) {
					m_aModelEffect[i] = new t_ModelEffect();
					if (!m_aModelEffect[i].read(cVariable)) return false;
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
				if (m_aModelEffect.Length > 65535) {
					cVariable.error("array size error in m_aModelEffect");
					return false;
				}
			int n = m_aModelEffect.Length;
			cVariable.put((ushort) n);
			for(int i = 0;i < n;++i) {
				if (!m_aModelEffect[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

