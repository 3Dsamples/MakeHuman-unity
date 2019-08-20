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
	@brief	t_SpriteData
*/
public class t_SpriteData : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_id;
	public string	m_sShader;
	public t_SpriteDataOne[]	m_aData;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_SpriteData() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_id = 0;
		m_sShader = "";
		m_aData = new t_SpriteDataOne[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_id = cVariable.getU32();
			cVariable.getString(ref m_sShader,255);
			{
				int n = (int) cVariable.getU32();
				if (n > 65536) {
					cVariable.error("array size error in m_aData");
					return false;
				}
				m_aData = new t_SpriteDataOne [n];
				for(int i = 0;i < n;++i) {
					m_aData[i] = new t_SpriteDataOne();
					if (!m_aData[i].read(cVariable)) return false;
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_id);
		cVariable.put(ref m_sShader,255);
		{	
				if (m_aData.Length > 65536) {
					cVariable.error("array size error in m_aData");
					return false;
				}
			int n = m_aData.Length;
			cVariable.put((uint) n);
			for(int i = 0;i < n;++i) {
				if (!m_aData[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

