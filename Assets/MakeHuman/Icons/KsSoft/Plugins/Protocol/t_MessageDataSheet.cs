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
	@brief	t_MessageDataSheet
*/
public class t_MessageDataSheet : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_type;
	public t_MessageDataOne[]	m_aContents;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_MessageDataSheet() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_type = 0;
		m_aContents = new t_MessageDataOne[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_type = cVariable.getU32();
			{
				int n = (int) cVariable.getU16();
				if (n > 65535) {
					cVariable.error("array size error in m_aContents");
					return false;
				}
				m_aContents = new t_MessageDataOne [n];
				for(int i = 0;i < n;++i) {
					m_aContents[i] = new t_MessageDataOne();
					if (!m_aContents[i].read(cVariable)) return false;
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_type);
		{	
				if (m_aContents.Length > 65535) {
					cVariable.error("array size error in m_aContents");
					return false;
				}
			int n = m_aContents.Length;
			cVariable.put((ushort) n);
			for(int i = 0;i < n;++i) {
				if (!m_aContents[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

