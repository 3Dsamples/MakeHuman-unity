using UnityEngine;
using System.Collections;

using KS;
public class CFriend  {
	ulong					m_AUID	= 0;
	CProfile				m_cProfile = null;
	
	//==========================================================================
	/*!Constructor
	 * @brief	Constructor
	*/
	public CFriend(ulong AUID) {
		m_AUID = AUID;
		CProfileMgr	cProfileMgr = CProfileMgr.Instance;
		
		m_cProfile = cProfileMgr.getProfile(AUID);
	}
	public CProfile Profile {
		get {
			return m_cProfile;
		}
	}
	public ulong AUID {
		get { return m_AUID; }
	}
}
