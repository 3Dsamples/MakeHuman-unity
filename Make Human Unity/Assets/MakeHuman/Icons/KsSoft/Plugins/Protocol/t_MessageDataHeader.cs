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
	@brief	t_MessageDataHeader
*/
public class t_MessageDataHeader : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_magicNo;
	public t_MessageData[]	m_aData;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_MessageDataHeader() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_magicNo = 4277006336;	// 4277006336;
		m_aData = new t_MessageData[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_magicNo = cVariable.getU32();
			{
				int n = (int) cVariable.getU8();
				if (n > 255) {
					cVariable.error("array size error in m_aData");
					return false;
				}
				m_aData = new t_MessageData [n];
				for(int i = 0;i < n;++i) {
					m_aData[i] = new t_MessageData();
					if (!m_aData[i].read(cVariable)) return false;
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_magicNo);
		{	
				if (m_aData.Length > 255) {
					cVariable.error("array size error in m_aData");
					return false;
				}
			int n = m_aData.Length;
			cVariable.put((byte) n);
			for(int i = 0;i < n;++i) {
				if (!m_aData[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

