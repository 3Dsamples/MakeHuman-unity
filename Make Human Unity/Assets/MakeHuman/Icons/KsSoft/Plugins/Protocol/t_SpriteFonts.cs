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
	@brief	t_SpriteFonts
*/
public class t_SpriteFonts : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public t_SpriteFont[]	m_aFont;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_SpriteFonts() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_aFont = new t_SpriteFont[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			{
				int n = (int) cVariable.getU8();
				if (n > 255) {
					cVariable.error("array size error in m_aFont");
					return false;
				}
				m_aFont = new t_SpriteFont [n];
				for(int i = 0;i < n;++i) {
					m_aFont[i] = new t_SpriteFont();
					if (!m_aFont[i].read(cVariable)) return false;
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		{	
				if (m_aFont.Length > 255) {
					cVariable.error("array size error in m_aFont");
					return false;
				}
			int n = m_aFont.Length;
			cVariable.put((byte) n);
			for(int i = 0;i < n;++i) {
				if (!m_aFont[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

