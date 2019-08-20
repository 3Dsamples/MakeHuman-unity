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
	@brief	t_WindowDataHeader
*/
public class t_WindowDataHeader : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_uMagicNo;
	public uint	m_uVersion;
	public t_WindowData[]	m_aWindowData;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_WindowDataHeader() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_uMagicNo = 0;
		m_uVersion = 0;
		m_aWindowData = new t_WindowData[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_uMagicNo = cVariable.getU32();
			m_uVersion = cVariable.getU32();
			{
				int n = (int) cVariable.getU8();
				if (n > 255) {
					cVariable.error("array size error in m_aWindowData");
					return false;
				}
				m_aWindowData = new t_WindowData [n];
				for(int i = 0;i < n;++i) {
					m_aWindowData[i] = new t_WindowData();
					if (!m_aWindowData[i].read(cVariable)) return false;
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_uMagicNo);
		cVariable.put(m_uVersion);
		{	
				if (m_aWindowData.Length > 255) {
					cVariable.error("array size error in m_aWindowData");
					return false;
				}
			int n = m_aWindowData.Length;
			cVariable.put((byte) n);
			for(int i = 0;i < n;++i) {
				if (!m_aWindowData[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

