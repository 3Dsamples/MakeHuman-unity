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
	@brief	t_WindowData
*/
public class t_WindowData : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_mId;
	public uint	m_propertyNum;
	public uint	m_ctrlNum;
	public byte[]	m_aData;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_WindowData() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_mId = 0;
		m_propertyNum = 0;
		m_ctrlNum = 0;
		m_aData = new byte[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_mId = cVariable.getU32();
			m_propertyNum = cVariable.getU32();
			m_ctrlNum = cVariable.getU32();
			{
				int n = (int) cVariable.getU16();
				if (n > 65535) {
					cVariable.error("array size error in m_aData");
					return false;
				}
				m_aData = new byte [n];
				for(int i = 0;i < n;++i) {
					m_aData[i] = cVariable.getU8();
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_mId);
		cVariable.put(m_propertyNum);
		cVariable.put(m_ctrlNum);
		{	
				if (m_aData.Length > 65535) {
					cVariable.error("array size error in m_aData");
					return false;
				}
			int n = m_aData.Length;
			cVariable.put((ushort) n);
			for(int i = 0;i < n;++i) {
				cVariable.put(m_aData[i]);

			}
		}
		return true;
	}
};

