using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using KS;

public class CProfileMgr {
	Dictionary<ulong, CProfile> m_dicProfileCache = new Dictionary<ulong, CProfile>();
	CProfile	m_ownProfile;
	//==========================================================================
	/*!Constructor
	 * @brief	Constructor
	*/
	public CProfileMgr() {
		if (m_instance != null) {
			Debug.LogError ("already exist profile mgr");
			return;
		}
		m_instance = this;
		CProfile[] aProfile = new CProfile[] {
			new CProfile( 1,"Bindweed "),	//own player!
			new CProfile( 2,"Thistle"),
			new CProfile( 3,"Iris"),
			new CProfile( 4,"Phlox"),
			new CProfile( 5,"Black-Eyed Susan"),
			new CProfile( 6,"Columbine"),
			new CProfile( 7,"Baby's breath"),
			new CProfile( 8,"Columbine"),
			new CProfile( 9,"Chrysanthemum"),
			new CProfile(10,"SnapDragon"),
			new CProfile(11,"Pot Marigold"),
			new CProfile(12,"Buttercup"),
			new CProfile(13,"Nasturtium"),
			new CProfile(14,"Clematis"),
			new CProfile(15,"Cockscomb"),
			new CProfile(16,"Cyclamenn"),
		};
		foreach (CProfile profile in aProfile) {
			if (profile.isOwnPlayer) {
				m_ownProfile = profile;
			}
			m_dicProfileCache[profile.AUID] = profile;
		}
	}
	//==========================================================================
	/*!release
	 * @brief	release
	*/
	public void release() {
		m_dicProfileCache.Clear ();
		m_instance = null;
	}
	//==========================================================================
	/*!プロフィールデータを一つ取得する.
		@brief	getProfile.
	*/
	public CProfile getProfile(ulong AUID) {
		return m_dicProfileCache[AUID];
	}
	public CProfile	own {
		get {
			return m_ownProfile;
		}

	}
	/*!Instance.
		@brief	Instance.
	*/
	static private CProfileMgr	m_instance = null;
	public static CProfileMgr Instance
    {
        get { return m_instance; }
    }
}
