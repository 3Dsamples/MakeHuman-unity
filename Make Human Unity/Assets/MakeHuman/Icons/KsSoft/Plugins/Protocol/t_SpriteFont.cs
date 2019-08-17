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
	@brief	t_SpriteFont
*/
public class t_SpriteFont : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_id;
	public string	m_face;
	public string	m_textureName;
	public ushort	m_pxSize;
	public sbyte	m_charSpacing;
	public sbyte	m_lineHeight;
	public sbyte	m_baseHeight;
	public short	m_texWidth;
	public short	m_texHeight;
	public sbyte	m_pages;
	public t_SpriteChar[]	m_aChar;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_SpriteFont() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_id = 0;
		m_face = "";
		m_textureName = "";
		m_pxSize = 0;
		m_charSpacing = 1;	// 1;
		m_lineHeight = 0;
		m_baseHeight = 0;
		m_texWidth = 0;
		m_texHeight = 0;
		m_pages = 0;
		m_aChar = new t_SpriteChar[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_id = cVariable.getU32();
			cVariable.getString(ref m_face,255);
			cVariable.getString(ref m_textureName,255);
			m_pxSize = cVariable.getU16();
			m_charSpacing = cVariable.getS8();
			m_lineHeight = cVariable.getS8();
			m_baseHeight = cVariable.getS8();
			m_texWidth = cVariable.getS16();
			m_texHeight = cVariable.getS16();
			m_pages = cVariable.getS8();
			{
				int n = (int) cVariable.getU32();
				if (n > 65536) {
					cVariable.error("array size error in m_aChar");
					return false;
				}
				m_aChar = new t_SpriteChar [n];
				for(int i = 0;i < n;++i) {
					m_aChar[i] = new t_SpriteChar();
					if (!m_aChar[i].read(cVariable)) return false;
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_id);
		cVariable.put(ref m_face,255);
		cVariable.put(ref m_textureName,255);
		cVariable.put(m_pxSize);
		cVariable.put(m_charSpacing);
		cVariable.put(m_lineHeight);
		cVariable.put(m_baseHeight);
		cVariable.put(m_texWidth);
		cVariable.put(m_texHeight);
		cVariable.put(m_pages);
		{	
				if (m_aChar.Length > 65536) {
					cVariable.error("array size error in m_aChar");
					return false;
				}
			int n = m_aChar.Length;
			cVariable.put((uint) n);
			for(int i = 0;i < n;++i) {
				if (!m_aChar[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

