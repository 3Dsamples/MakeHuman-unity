//==============================================================================================
/*!CTexParserHelper.cs
	@file  CTexParserHelper.cs
*/
//==============================================================================================
using UnityEngine;
using System.Collections.Generic;
using KS;

//==============================================================================================
/*!CTexParserProperty
	@brief	CTexParserProperty
	@note	
*/
//==============================================================================================
public class CTexParserProperty {
	CTexParserProperty	m_cNext = null;
	e_TexProperty		m_eProperty = e_TexProperty.NONE;
	Vector2				m_vector2;
	Vector4				m_vector4;
	int					m_iValue = 0;
	public CTexParserProperty() {
	}
	public CTexParserProperty(e_TexProperty eProperty,Vector2 value) {
		m_eProperty = eProperty;
		m_vector2 = value;
	}
	public CTexParserProperty(e_TexProperty eProperty,Vector4 value) {
		m_eProperty = eProperty;
		m_vector4 = value;
	}
	public CTexParserProperty(e_TexProperty eProperty,int iValue) {
		m_eProperty = eProperty;
		m_iValue = iValue;
	}
	public CTexParserProperty next {
		get {
			return m_cNext;
		}
		set {
			CTexParserProperty	cLast = this;
			for (;cLast.next != null;cLast = cLast.next);
			cLast.m_cNext = value;
		}
	}
	public e_TexProperty	property {
		get {
			return m_eProperty;
		}
		set {
			m_eProperty = value;
		}
	}
	public Vector2 getVector2 {
		get {
			return m_vector2;
		}
	}
	public Vector4 getVector4 {
		get {
			return m_vector4;
		}
	}
	public int Value {
		get {
			return m_iValue;
		}
	}
	public int size {
		get {
			int iAdd = (property == e_TexProperty.NONE)? 0:1;
			if (m_cNext == null) {
				return iAdd;
			}
			return m_cNext.size + iAdd;
		}
	}
}
//==============================================================================================
/*!CTexParserPart
	@brief	CTexParserPart
	@note	
*/
//==============================================================================================
public class CTexParserPart {
	CTexParserPart		m_cNext = null;
	uint				m_id = 0;
	uint				m_texid;
	CTexParserProperty	m_cProperty = null;

	public CTexParserPart() {
		m_cNext = null;
		m_id = 0;
		m_texid = 0;
		m_cProperty = null;
	}
	public CTexParserPart(uint id,CTexParserProperty cProperty) {
		m_id = id;
		m_texid = id;
		m_cProperty = cProperty;
	}
	public CTexParserPart(uint id,uint texid,CTexParserProperty cProperty) {
		m_id = id;
		m_texid = texid;
		m_cProperty = cProperty;
	}
	public int size {
		get {
			int	iAdd = (m_id == 0)? 0:1;
			if (m_cNext == null) {
				return iAdd;
			}
			return m_cNext.size + iAdd;
		}
	}
	public uint	id {
		get {
			return m_id;
		}
	}
	public uint	texid {
		get {
			return m_texid;
		}
	}
	public CTexParserProperty	property {
		get {
			return m_cProperty;
		}
	}
	public CTexParserPart next {
		get {
			return m_cNext;
		}
		set {
			CTexParserPart	cLast = this;
			for (;cLast.next != null;cLast = cLast.next);
			cLast.m_cNext = value;
		}
	}
}
