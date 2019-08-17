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
	@brief	t_ModelEffect
*/
public class t_ModelEffect : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_mId;
	public uint	m_mMesh;
	public uint	m_fcAnim;
	public uint	m_mLink;
	public uint	m_fcBone;
	public uint	m_flag;
	public float	m_radLight;
	public Vector3	m_offset;
	public Vector3	m_rotate;
	public Vector3	m_scale;
	public uint	m_color;
	public t_ModelEffectFrame[]	m_aFrame;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_ModelEffect() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_mId = 0;
		m_mMesh = 0;
		m_fcAnim = 0;
		m_mLink = 0;
		m_fcBone = 0;
		m_flag = 0;
		m_radLight = 0.0f;
		m_color = 0;
		m_aFrame = new t_ModelEffectFrame[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_mId = cVariable.getU32();
			m_mMesh = cVariable.getU32();
			m_fcAnim = cVariable.getU32();
			m_mLink = cVariable.getU32();
			m_fcBone = cVariable.getU32();
			m_flag = cVariable.getU32();
			m_radLight = cVariable.getFloat();
			m_offset = cVariable.getVector3();
			m_rotate = cVariable.getVector3();
			m_scale = cVariable.getVector3();
			m_color = cVariable.getU32();
			{
				int n = (int) cVariable.getU8();
				if (n > 255) {
					cVariable.error("array size error in m_aFrame");
					return false;
				}
				m_aFrame = new t_ModelEffectFrame [n];
				for(int i = 0;i < n;++i) {
					m_aFrame[i] = new t_ModelEffectFrame();
					if (!m_aFrame[i].read(cVariable)) return false;
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_mId);
		cVariable.put(m_mMesh);
		cVariable.put(m_fcAnim);
		cVariable.put(m_mLink);
		cVariable.put(m_fcBone);
		cVariable.put(m_flag);
		cVariable.put(m_radLight);
		cVariable.put(m_offset);
		cVariable.put(m_rotate);
		cVariable.put(m_scale);
		cVariable.put(m_color);
		{	
				if (m_aFrame.Length > 255) {
					cVariable.error("array size error in m_aFrame");
					return false;
				}
			int n = m_aFrame.Length;
			cVariable.put((byte) n);
			for(int i = 0;i < n;++i) {
				if (!m_aFrame[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

