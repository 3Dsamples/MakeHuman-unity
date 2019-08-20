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
	@brief	t_AssetVersion
*/
public class t_AssetVersion : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_verionAsset;
	public uint	m_versionClient;
	public ulong	m_uMd5a;
	public ulong	m_uMd5b;
	public t_AssetVersionOne[]	m_aVersion;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_AssetVersion() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_verionAsset = 0;
		m_versionClient = 0;
		m_uMd5a = 0UL;
		m_uMd5b = 0UL;
		m_aVersion = new t_AssetVersionOne[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_verionAsset = cVariable.getU32();
			m_versionClient = cVariable.getU32();
			m_uMd5a = cVariable.getU64();
			m_uMd5b = cVariable.getU64();
			{
				int n = (int) cVariable.getU16();
				if (n > 65535) {
					cVariable.error("array size error in m_aVersion");
					return false;
				}
				m_aVersion = new t_AssetVersionOne [n];
				for(int i = 0;i < n;++i) {
					m_aVersion[i] = new t_AssetVersionOne();
					if (!m_aVersion[i].read(cVariable)) return false;
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_verionAsset);
		cVariable.put(m_versionClient);
		cVariable.put(m_uMd5a);
		cVariable.put(m_uMd5b);
		{	
				if (m_aVersion.Length > 65535) {
					cVariable.error("array size error in m_aVersion");
					return false;
				}
			int n = m_aVersion.Length;
			cVariable.put((ushort) n);
			for(int i = 0;i < n;++i) {
				if (!m_aVersion[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

