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
	@brief	t_WinPropertyArray
*/
public class t_WinPropertyArray : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint[]	m_aValue;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_WinPropertyArray() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_aValue = new uint[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			{
				int n = (int) cVariable.getU8();
				if (n > 255) {
					cVariable.error("array size error in m_aValue");
					return false;
				}
				m_aValue = new uint [n];
				for(int i = 0;i < n;++i) {
					m_aValue[i] = cVariable.getU32();
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		{	
				if (m_aValue.Length > 255) {
					cVariable.error("array size error in m_aValue");
					return false;
				}
			int n = m_aValue.Length;
			cVariable.put((byte) n);
			for(int i = 0;i < n;++i) {
				cVariable.put(m_aValue[i]);

			}
		}
		return true;
	}
};

