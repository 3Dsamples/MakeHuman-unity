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
	@brief	t_ParticleEffectHeader
*/
public class t_ParticleEffectHeader : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public  const uint	MAGIC_NO = 4276944901;	// 4276944901
	public uint	m_magicNo;
	public t_ParticleEffect[]	m_aParticleEffect;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_ParticleEffectHeader() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_magicNo = 4276944901;	// MAGIC_NO;
		m_aParticleEffect = new t_ParticleEffect[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_magicNo = cVariable.getU32();
			{
				int n = (int) cVariable.getU16();
				if (n > 65535) {
					cVariable.error("array size error in m_aParticleEffect");
					return false;
				}
				m_aParticleEffect = new t_ParticleEffect [n];
				for(int i = 0;i < n;++i) {
					m_aParticleEffect[i] = new t_ParticleEffect();
					if (!m_aParticleEffect[i].read(cVariable)) return false;
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
				if (m_aParticleEffect.Length > 65535) {
					cVariable.error("array size error in m_aParticleEffect");
					return false;
				}
			int n = m_aParticleEffect.Length;
			cVariable.put((ushort) n);
			for(int i = 0;i < n;++i) {
				if (!m_aParticleEffect[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

