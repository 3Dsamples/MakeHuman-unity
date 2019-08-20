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
	@brief	t_Message
*/
public class t_Message : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_mSheetId;
	public uint	m_mId;
	public t_MessageArg[]	m_aArg;
	public string[]	m_aString;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_Message() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_mSheetId = 0;
		m_mId = 0;
		m_aArg = new t_MessageArg[0];
		m_aString = new string[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_mSheetId = cVariable.getU32();
			m_mId = cVariable.getU32();
			{
				int n = (int) cVariable.getU8();
				if (n > 255) {
					cVariable.error("array size error in m_aArg");
					return false;
				}
				m_aArg = new t_MessageArg [n];
				for(int i = 0;i < n;++i) {
					m_aArg[i] = new t_MessageArg();
					if (!m_aArg[i].read(cVariable)) return false;
				}
			}
			{
				int n = (int) cVariable.getU8();
				if (n > 255) {
					cVariable.error("array size error in m_aString");
					return false;
				}
				m_aString = new string [n];
				for(int i = 0;i < n;++i) {
					cVariable.getString(ref m_aString[i],255);
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_mSheetId);
		cVariable.put(m_mId);
		{	
				if (m_aArg.Length > 255) {
					cVariable.error("array size error in m_aArg");
					return false;
				}
			int n = m_aArg.Length;
			cVariable.put((byte) n);
			for(int i = 0;i < n;++i) {
				if (!m_aArg[i].write(cVariable)) return false;

			}
		}
		{	
				if (m_aString.Length > 255) {
					cVariable.error("array size error in m_aString");
					return false;
				}
			int n = m_aString.Length;
			cVariable.put((byte) n);
			for(int i = 0;i < n;++i) {
				cVariable.put(ref m_aString[i],255);

			}
		}
		return true;
	}
};

