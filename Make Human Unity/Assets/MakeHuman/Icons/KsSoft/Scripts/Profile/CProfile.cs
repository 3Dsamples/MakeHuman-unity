using UnityEngine;
using System.Collections;
using KS;

public class CProfile {
	public enum e_PartyState {
		NotJoin,
		Join,
		MaxJoin,
	};
	ulong				m_AUID = 0;
	string				m_name;

	static	uint[]		m_aAvatarIcon = new uint[] {
		new MulId(1,0,10),
		new MulId(1,0,20),
		new MulId(1,0,30),
	};
	static	uint[]		m_aWeaponIcon = new uint[] {
		new MulId(3,1,10),
		new MulId(3,2,10),
		new MulId(3,3,10),
	};

	//==========================================================================
	/*!Constructor
		@brief	Constructor
	*/
	public CProfile(ulong AUID,string name) {
		m_AUID = AUID;
		m_name = name;
	}

	public void setAvatarIcon(CWinCtrlIcon cCtrl) {
		// キャラクタアイコン.
		cCtrl.setPartId(1,iconId);
		// Level.
		if (Level >= 0) {
			cCtrl.caption = "Lv." + (Level+1);
		} else {
			cCtrl.caption = "";
		}
	}
	public bool setContents(CWinContents cContents,uint ctrlIcon,uint ctrlName,uint ctrlSign,uint ctrlPlace,uint ctrlLevel, uint ctrlGuild, uint ctrlGuildIcon,uint ctrlQuest,uint ctrlParty,uint ctrlPartyIcon,uint ctrlChat) {
		// Avatar用Icon表示.
		if (ctrlIcon != 0) {
			CWinCtrlIcon	cIcon = cContents.find<CWinCtrlIcon>(ctrlIcon);
			if (cIcon != null) {
				setAvatarIcon(cIcon);
			} else {
				Debug.LogWarning("this ctrl is not icon:" + new MulId(ctrlIcon));
			}
		}
		// 名前.
		CWinCtrlBase	cCtrl;
		if (ctrlName != 0) {
			cCtrl = cContents.find (ctrlName);
			if (cCtrl != null) {
				cCtrl.caption = Name;
			}
		}
		// サインイン状態.
		if (ctrlSign != 0) {
			cCtrl = cContents.find (ctrlSign);
			if (cCtrl != null) {
				if (isConnect) {
					cCtrl.partId = new FiveCC("CNCT1");
				} else {
					cCtrl.partId = new FiveCC("CNCT0");
				}
			}
		}
		// レベル.
		if (ctrlLevel != 0) {
			cCtrl = cContents.find (ctrlLevel);
			if (cCtrl != null) {
				cCtrl.caption = "Lv" + (Level+1);
			}
		}
		// Guild
		cCtrl = cContents.find (ctrlGuild);
		if (cCtrl != null) {
			if (GuildUID == 0) {
				cCtrl.hide = true;
			} else {
				cCtrl.hide = false;
				cCtrl.caption = GuildName;
			}
		}
		// Guild icon
		cCtrl = cContents.find (ctrlGuildIcon);
		if (cCtrl != null) {
			if (GuildName != null && GuildName != "") {
				cCtrl.partId = new FiveCC("ICNG");
				cCtrl.hide = false;
			} else {
				cCtrl.hide = true;
			}
		}
		//   Place
		if (ctrlPlace != 0) {
			cCtrl = cContents.find (ctrlPlace);
			if (cCtrl != null) {
				cCtrl.partId = new FiveCC("ICNT");
			}
		}
		// クエスト.
		if (ctrlQuest != 0) {
			cCtrl = cContents.find (ctrlQuest);
			if (cCtrl != null) {
				cCtrl.caption = "QuestName";
			}
		}
		// Private chat
		if (ctrlChat != 0) {
			cCtrl = cContents.find (ctrlChat);
			if (cCtrl != null) {
				if (isConnect) {
					cCtrl.disable = false;
				} else {
					cCtrl.disable = true;
				}
			}
		}
		return true;
	}
	public ulong AUID {
		get {
			return m_AUID;
		}
	}
	public string Name {
		get {
			return m_name;
		}
	}
	public uint Level {
		get {
			return (uint) (m_AUID % 100);
		}
	}
	public ulong GuildUID {
		get {
			return 0;
		}
	}
	public string GuildName {
		get {
			return "Guild Name";
		}
	}
	public string Introduction {
		get {
			return "Hellow!\nHow are you?";
		}
	}
	public uint[] avatarIcons {
		get {
			return m_aAvatarIcon;
		}
	}
	public uint[] weaponIcons {
		get {
			return m_aWeaponIcon;
		}
	}
	public uint iconId {
		get {
			return m_aAvatarIcon[m_AUID % 3];
		}
	}
	public bool isConnect {
		get {
			return true;
		}
	}
	public bool isLogin {
		get {
			return isConnect;
		}
	}
	public bool isLogout {
		get {
			return !isLogin;
		}
	}
	public bool isOwnPlayer {
		get {
			if (m_AUID == 1) {
				return true;
			}
			return false;
		}
	}
}
