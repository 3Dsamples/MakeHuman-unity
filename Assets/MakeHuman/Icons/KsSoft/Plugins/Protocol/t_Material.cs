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
	@brief	t_Material
*/
public class t_Material : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public  const byte	PropertyType_Color = 0;	// 0
	public  const byte	PropertyType_Vector = 1;	// 1
	public  const byte	PropertyType_Float = 2;	// 2
	public  const byte	PropertyType_Texture = 3;	// 3
	public uint	m_mId;
	public string	m_shader;
	public ushort	m_nProperty;
	public byte[]	m_aBuffer;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_Material() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_mId = 0;
		m_shader = "";
		m_nProperty = 0;
		m_aBuffer = new byte[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_mId = cVariable.getU32();
			cVariable.getString(ref m_shader,255);
			m_nProperty = cVariable.getU16();
			{
				int n = (int) cVariable.getU16();
				if (n > 65535) {
					cVariable.error("array size error in m_aBuffer");
					return false;
				}
				m_aBuffer = new byte [n];
				for(int i = 0;i < n;++i) {
					m_aBuffer[i] = cVariable.getU8();
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_mId);
		cVariable.put(ref m_shader,255);
		cVariable.put(m_nProperty);
		{	
				if (m_aBuffer.Length > 65535) {
					cVariable.error("array size error in m_aBuffer");
					return false;
				}
			int n = m_aBuffer.Length;
			cVariable.put((ushort) n);
			for(int i = 0;i < n;++i) {
				cVariable.put(m_aBuffer[i]);

			}
		}
		return true;
	}
};

