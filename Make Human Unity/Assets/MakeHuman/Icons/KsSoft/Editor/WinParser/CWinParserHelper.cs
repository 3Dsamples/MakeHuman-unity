//==============================================================================================
/*!CWinParserHelper.cs
	@file  CWinParserHelper.cs
*/
//==============================================================================================
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using KS;

//==============================================================================================
/*!CWinNumberList
	@brief	CWinNumberList
	@note
*/
//==============================================================================================
public class CWinNumberList {
	protected uint m_uNumber;
	protected string m_id;
	protected CWinNumberList m_cNext;
	protected e_WinCtrlKind	m_eKind = e_WinCtrlKind.NONE;

	public CWinNumberList(uint uNumber) {
		m_uNumber = uNumber;
		m_cNext = null;
		m_id = "";
	}
	public CWinNumberList(e_WinCtrlKind eKind,string id) {
		m_id = id + "." + eKind;
		m_uNumber = 0;
		m_cNext = null;
		m_eKind = eKind;
	}
	public CWinNumberList(CWinParserCtrl cCtrl) {
		m_id = cCtrl.ToString();
		m_uNumber = 0;
		m_cNext = null;
		m_eKind = cCtrl.kind;

	}
	public uint getValue(Dictionary<string,uint> dicCtrlId) {
		uint id = m_uNumber;
		if (m_id == "" || dicCtrlId == null) {
			foreach (uint ctrlId in dicCtrlId.Values) {
				if (ctrlId == id) {
					return id;
				}
			}
			return 0;
		}
		id = 0;
		if (dicCtrlId.TryGetValue(m_id,out id)) {
			return id;
		}
		return 0;
	}
	public CWinNumberList next {
		get {
			return m_cNext;
		}
		set {
			CWinNumberList	cLast = this;
			for (;cLast.next != null;cLast = cLast.next);
			cLast.m_cNext = value;
		}
	}
	public int size {
		get {
			if (m_cNext == null) {
				return 1;
			}
			return m_cNext.size + 1;
		}
	}
	public e_WinCtrlKind	kind {
		get {
			return m_eKind;
		}
	}
	override public string ToString () {
		if (m_id == "") {
			return new MulId(m_uNumber).ToString();
		}
		return m_id;
	}
}
//==============================================================================================
/*!CWinParserProperty
	@brief	CWinParserProperty
	@note
*/
//==============================================================================================
public class CWinParserProperty {
	CWinParserProperty	m_cNext = null;
	e_WinProperty		m_eProperty = e_WinProperty.NONE;
	string				m_string = "";
	int					m_iValue = 0;
	uint				m_uPartsId = 0;
    Vector2             m_vector2;
    Vector4             m_vector4;
    Vector3[]           m_pairratio;
    Vector3[]           m_screen;
    CWinNumberList      m_cNumberList = null;
	CWinParserCtrl		m_cContentsList = null;
	CWinParser.Tokens[]	m_aTokens = null;

	static Encoding m_cUtf8Encode = Encoding.GetEncoding("utf-8");

	public CWinParserProperty() {
	}
	public void set(e_WinProperty eProperty,string value) {
		m_eProperty = eProperty;
		m_string = encoding(value);
	}
    public void set(e_WinProperty eProperty, int value) {
        m_eProperty = eProperty;
        m_iValue = value;
    }
    public void set(e_WinProperty eProperty,Vector2 value) {
		m_eProperty = eProperty;
		m_vector2 = value;
	}
    public void set(e_WinProperty eProperty,Vector4 value) {
        m_eProperty = eProperty;
        m_vector4 = value;
    }
    public void set(e_WinProperty eProperty,Vector3[] value) {
		m_eProperty = eProperty;
        m_pairratio = value;
	}
    public void set(e_WinProperty eProperty,Vector3[] pos,Vector3[] size) {
        m_eProperty = eProperty;
        m_screen = new Vector3[4];
        m_screen[0] = pos[0];
        m_screen[1] = pos[1];
        m_screen[2] = size[0];
        m_screen[3] = size[1];
    }
    public void set(e_WinProperty eProperty,int value,uint uPartsId) {
		m_eProperty = eProperty;
		m_iValue = value;
		m_uPartsId = uPartsId;
	}
    public void set(e_WinProperty eProperty, uint texid,int value) {
        m_eProperty = eProperty;
        m_iValue = value;
        m_uPartsId = texid;
    }
    public void set(e_WinProperty eProperty,CWinNumberList cNumberList) {
		m_eProperty = eProperty;
		m_cNumberList = cNumberList;
	}
	public void set(e_WinProperty eProperty,CWinParserCtrl cContentsList) {
		m_eProperty = eProperty;
		m_cContentsList = cContentsList;
		for (CWinParserCtrl cCtrl = cContentsList;cCtrl != null;cCtrl = cCtrl.next) {
			if (cCtrl.kind == e_WinCtrlKind.NONE) {
				continue;
			}
			if (m_cNumberList == null) {
				m_cNumberList = new CWinNumberList(cCtrl);
			} else {
				m_cNumberList.next = new CWinNumberList(cCtrl);
			}
		}
	}
	internal void setStyle(CWinParser.Tokens[] aTokens) {
		m_eProperty = e_WinProperty.STYLE;
		m_aTokens = aTokens;
	}
	static public string encoding(string src) {
		byte[] buffer = new byte[src.Length];
		int idx = 0;
		for (int i = 0;i < src.Length;++i) {
			char	ch = src[i];
			if (ch == '\\') {
				++i;
				if (i >= src.Length) {
					buffer[idx++] = (byte) '\\';
					break;
				}
				switch (src[i]) {
				  case 'n':
					ch = '\n';
					break;
				  case 'r':
					ch = '\n';
					break;
				  default:
					--i;
					break;
				}
			}
			buffer[idx++] = (byte) ch;
		}
		return m_cUtf8Encode.GetString(buffer,0,idx);
	}
	public CWinParserProperty next {
		get {
			return m_cNext;
		}
		set {
			if (value == null) {
				return;
			}
			CWinParserProperty	cLast = this;
			for (;cLast.next != null;cLast = cLast.next);
			cLast.m_cNext = value;
		}
	}
	public e_WinProperty	property {
		get {
			return m_eProperty;
		}
		set {
			m_eProperty = value;
		}
	}
	public string getString {
		get {
			return m_string;
		}
	}
	public int getValue {
		get {
			return m_iValue;
		}
	}
	public int setValue {
		set {
			m_iValue = value;
		}
	}
	public uint getPartsId {
		get {
			return m_uPartsId;
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
    public Vector3[] getPairRatio {
		get {
			return m_pairratio;
		}
	}
    public Vector3[] getScreen {
        get {
            return m_screen;
        }
    }
    public CWinNumberList getNumberList {
		get {
			return m_cNumberList;
		}
	}
	public CWinParserCtrl getContentsList {
		get {
			return m_cContentsList;
		}
	}
	public int size {
		get {
			int iAdd = (property == e_WinProperty.NONE || property >= e_WinProperty.RESOURCE)? 0:1;
			if (m_cNext == null) {
				return iAdd;
			}
			return m_cNext.size + iAdd;
		}
	}
	internal CWinParser.Tokens[] styles {
		get {
			return m_aTokens;
		}
	}
}
//==============================================================================================
/*!CWinParserCtrl
	@brief	CWinParserCtrl
	@note
*/
//==============================================================================================
public class CWinParserCtrl {
	CWinParserCtrl		m_cNext = null;
	e_WinCtrlKind		m_eKind = e_WinCtrlKind.NONE;
	string				m_id = "";
	CWinParserProperty	m_cProperty = null;

	public CWinParserCtrl() {
		m_cNext = null;
		m_eKind = e_WinCtrlKind.NONE;
		m_id = "";
		m_cProperty = null;
	}
	public CWinParserCtrl(e_WinCtrlKind eKind,string id,CWinParserProperty cProperty) {
		m_eKind = eKind;
		m_id = id;
		m_cProperty = cProperty;
	}
	public int size {
		get {
			int	iAdd = (m_eKind == e_WinCtrlKind.NONE)? 0:1;
			if (m_cNext == null) {
				return iAdd;
			}
			return m_cNext.size + iAdd;
		}
	}
	public e_WinCtrlKind	kind {
		get {
			return m_eKind;
		}
	}
	public string	id {
		get {
			return m_id;
		}
	}
	public CWinParserProperty	property {
		get {
			return m_cProperty;
		}
	}
	public CWinParserCtrl next {
		get {
			return m_cNext;
		}
		set {
			CWinParserCtrl	cLast = this;
			for (;cLast.next != null;cLast = cLast.next);
			cLast.m_cNext = value;
		}
	}
	override public string ToString() {
		return m_id + "." + m_eKind;
	}
}
