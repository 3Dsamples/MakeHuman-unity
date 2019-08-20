using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using KS;
public class CFriendMgr {
	/*!FriendCache.
		@brief	Instance.
	*/
	Dictionary<ulong, CFriend>	m_dicFriendCache= new Dictionary<ulong, CFriend>();
	public enum e_State {
		Applying,
		Accepted,
		Blacklist
	};
	e_State m_eState = e_State.Accepted;
	//==========================================================================
	/*!Constructor
	 * @brief	Constructor
	*/
	public CFriendMgr() {
		if (m_instance != null) {
			Debug.Log("rebuild CFriendMgr");
		}
		m_instance = this;

		for (int i = 0;i < 10;++i) {
			append ((ulong) (i + 3));
		}
	}
	//==========================================================================
	/*!Release
		@brief	release
	*/
	public void release() {
		m_dicFriendCache.Clear ();
		m_instance = null;
	}
	/*!removeFriendCache.
		@brief	remove.
	*/
	public bool remove(ulong auid)
	{
		foreach (ulong x in m_dicFriendCache.Keys) {
			Debug.Log(x);
		}
		if (!m_dicFriendCache.ContainsKey(auid)) {
			Debug.LogError ("can't find in cache:" + auid);
			return false;
		}
		m_dicFriendCache.Remove(auid);
		return true;
	}
	//==========================================================================
	/*!getFriends
	 * @brief	getFriends
	*/
	public bool append(ulong AUID) {
		if (m_dicFriendCache.ContainsKey(AUID)) {
			return false;
		}
		m_dicFriendCache[AUID] = new CFriend(AUID);
		return true;
	}
	//==========================================================================
	/*!getFriends
	 * @brief	getFriends
	*/
	public CFriend[] getFriends() {
		List<CFriend> lstFriend = new List<CFriend>(m_dicFriendCache.Count);
		foreach (CFriend friend in m_dicFriendCache.Values) {
			lstFriend.Add(friend);
		}
		return lstFriend.ToArray();
	}
	//==========================================================================
	/*!フレンド関係かどうかチェックする.
	 * @brief	isFriend
	*/
	public bool isFriend(ulong uAUID) {
		CFriend friend;
		if (m_dicFriendCache.TryGetValue(uAUID,out friend)) {
			return true;
		}
		return false;
	}
	public e_State	state {
		get {
			return m_eState;
		}
		set {
			m_eState = value;
		}
	}
	/*!Instance.
		@brief	Instance.
	*/
	static private CFriendMgr	m_instance = null;
	public static CFriendMgr Instance
    {
        get { return m_instance; }
    }
}
