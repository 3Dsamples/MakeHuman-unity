//==============================================================================================
/*!CSeParserHelper.cs
	@file  CSeParserHelper.cs
*/
//==============================================================================================
using UnityEngine;
using System.Collections.Generic;

public enum e_SeProperty {
	NONE,
	VOLUME,		//ボリューム.
	PRIORITY,	//発声プライオリティ.
	GROUP,		//グループ.
	POLYPHONY,	//グループ内同時発生最大数.
	DISTANCE,	//距離.
};

//==============================================================================================
/*!CSeParserProperty
	@brief	CSeParserProperty
	@note	
*/
//==============================================================================================
public class CSeParserProperty {
	CSeParserProperty	m_cNext = null;
	e_SeProperty		m_eProperty = e_SeProperty.NONE;
	byte				m_value;
	Vector2				m_vector2;
	public CSeParserProperty() {
	}
	public CSeParserProperty(e_SeProperty eProperty,byte value) {
		m_eProperty = eProperty;
		m_value = value;
	}
	public CSeParserProperty(e_SeProperty eProperty,double min,double max) {
		m_eProperty = eProperty;
		m_vector2.x = (float) min;
		m_vector2.y = (float) max;
	}
	public CSeParserProperty next {
		get {
			return m_cNext;
		}
		set {
			CSeParserProperty	cLast = this;
			for (;cLast.next != null;cLast = cLast.next);
			cLast.m_cNext = value;
		}
	}
	public e_SeProperty	property {
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
	public int Value {
		get {
			return m_value;
		}
	}
	public int size {
		get {
			int iAdd = (property == e_SeProperty.NONE)? 0:1;
			if (m_cNext == null) {
				return iAdd;
			}
			return m_cNext.size + iAdd;
		}
	}
}
//==============================================================================================
/*!CSeParserDef
	@brief	CSeParserDef
	@note	
*/
//==============================================================================================
public class CSeParserDef {
	CSeParserDef		m_cNext = null;
	uint				m_id = 0;
	uint				m_idbase;
	CSeParserProperty	m_cProperty = null;

	public CSeParserDef() {
		m_cNext = null;
		m_id = 0;
		m_idbase = 0;
		m_cProperty = null;
	}
	public CSeParserDef(uint id,CSeParserProperty cProperty) {
		m_id = id;
		m_idbase = id;
		m_cProperty = cProperty;
	}
	public CSeParserDef(uint id,uint idbase,CSeParserProperty cProperty) {
		m_id = id;
		m_idbase = idbase;
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
	public uint	idbase {
		get {
			return m_idbase;
		}
	}
	public CSeParserProperty	property {
		get {
			return m_cProperty;
		}
	}
	public CSeParserDef next {
		get {
			return m_cNext;
		}
		set {
			CSeParserDef	cLast = this;
			for (;cLast.next != null;cLast = cLast.next);
			cLast.m_cNext = value;
		}
	}
}
