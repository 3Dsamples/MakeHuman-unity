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
	@brief	t_Billboard
*/
public class t_Billboard : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_mId;
	public uint	m_mMaterial;
	public uint	m_mLink;
	public uint	m_fcBone;
	public uint	m_origin;
	public uint	m_flag;
	public float	m_yoffset;
	public float	m_width;
	public float	m_height;
	public float	m_rotate;
	public t_BillboardFrame[]	aFrame;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_Billboard() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_mId = 0;
		m_mMaterial = 0;
		m_mLink = 0;
		m_fcBone = 0;
		m_origin = 0;
		m_flag = 0;
		m_yoffset = 0.0f;
		m_width = 0.0f;
		m_height = 0.0f;
		m_rotate = 0.0f;
		aFrame = new t_BillboardFrame[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_mId = cVariable.getU32();
			m_mMaterial = cVariable.getU32();
			m_mLink = cVariable.getU32();
			m_fcBone = cVariable.getU32();
			m_origin = cVariable.getU32();
			m_flag = cVariable.getU32();
			m_yoffset = cVariable.getFloat();
			m_width = cVariable.getFloat();
			m_height = cVariable.getFloat();
			m_rotate = cVariable.getFloat();
			{
				int n = (int) cVariable.getU8();
				if (n > 255) {
					cVariable.error("array size error in aFrame");
					return false;
				}
				aFrame = new t_BillboardFrame [n];
				for(int i = 0;i < n;++i) {
					aFrame[i] = new t_BillboardFrame();
					if (!aFrame[i].read(cVariable)) return false;
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_mId);
		cVariable.put(m_mMaterial);
		cVariable.put(m_mLink);
		cVariable.put(m_fcBone);
		cVariable.put(m_origin);
		cVariable.put(m_flag);
		cVariable.put(m_yoffset);
		cVariable.put(m_width);
		cVariable.put(m_height);
		cVariable.put(m_rotate);
		{	
				if (aFrame.Length > 255) {
					cVariable.error("array size error in aFrame");
					return false;
				}
			int n = aFrame.Length;
			cVariable.put((byte) n);
			for(int i = 0;i < n;++i) {
				if (!aFrame[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

