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
	@brief	t_MessageData
*/
public class t_MessageData : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public string	m_locale;
	public t_MessageDataSheet[]	m_aSheet;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_MessageData() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_locale = "";
		m_aSheet = new t_MessageDataSheet[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			cVariable.getString(ref m_locale,32);
			{
				int n = (int) cVariable.getU8();
				if (n > 255) {
					cVariable.error("array size error in m_aSheet");
					return false;
				}
				m_aSheet = new t_MessageDataSheet [n];
				for(int i = 0;i < n;++i) {
					m_aSheet[i] = new t_MessageDataSheet();
					if (!m_aSheet[i].read(cVariable)) return false;
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(ref m_locale,32);
		{	
				if (m_aSheet.Length > 255) {
					cVariable.error("array size error in m_aSheet");
					return false;
				}
			int n = m_aSheet.Length;
			cVariable.put((byte) n);
			for(int i = 0;i < n;++i) {
				if (!m_aSheet[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

