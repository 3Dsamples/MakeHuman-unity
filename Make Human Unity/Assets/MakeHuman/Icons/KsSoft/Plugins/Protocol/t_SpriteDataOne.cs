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
	@brief	t_SpriteDataOne
*/
public class t_SpriteDataOne : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_id;
	public uint	m_color;
	public sbyte	m_ePatch;
	public Vector4[]	m_aUV;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_SpriteDataOne() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_id = 0;
		m_color = 0;
		m_ePatch = 0;
		m_aUV = new Vector4[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_id = cVariable.getU32();
			m_color = cVariable.getU32();
			m_ePatch = cVariable.getS8();
			{
				int n = (int) cVariable.getU8();
				if (n > 9) {
					cVariable.error("array size error in m_aUV");
					return false;
				}
				m_aUV = new Vector4 [n];
				for(int i = 0;i < n;++i) {
					m_aUV[i] = cVariable.getVector4();
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_id);
		cVariable.put(m_color);
		cVariable.put(m_ePatch);
		{	
				if (m_aUV.Length > 9) {
					cVariable.error("array size error in m_aUV");
					return false;
				}
			int n = m_aUV.Length;
			cVariable.put((byte) n);
			for(int i = 0;i < n;++i) {
				cVariable.put(m_aUV[i]);

			}
		}
		return true;
	}
};

