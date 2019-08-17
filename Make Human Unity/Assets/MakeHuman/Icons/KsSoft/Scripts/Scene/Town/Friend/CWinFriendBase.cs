using KS;
public class CWinFriendBase : CWindowBase {
	public	const uint	windowId	=	169082880;	// 010_020_00000
	static public CWinFriend create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinFriend>(windowId,cParent);
	}
	public	const uint	TEXTURE_TitleBarBGTitle	=	10;	// 000_000_00010
	public	const uint	TEXT_TitleTitle	=	11;	// 000_000_00011
	public	const uint	RADIO_RadioFriendList	=	169083080;	// 010_020_00200
	public	const uint	RADIO_RadioApplying	=	169083180;	// 010_020_00300
	public	const uint	RADIO_RadioBlacklist	=	169083280;	// 010_020_00400
	public	const uint	BUTTON_BackBack	=	3000;	// 000_000_03000
	public	const uint	TEXT_BackBack	=	3001;	// 000_000_03001
};
