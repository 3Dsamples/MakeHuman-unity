using KS;
public class CWinFriendListBase : CWindowBase {
	public	const uint	windowId	=	169082881;	// 010_020_00001
	static public CWinFriendList create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinFriendList>(windowId,cParent);
	}
	public	const uint	BUTTON_Sort	=	169082890;	// 010_020_00010
	public	const uint	TEXT_Sort	=	169082891;	// 010_020_00011
	public	const uint	CHECKBOX_Edit	=	169082895;	// 010_020_00015
	public	const uint	TEXT_FriendNotFound	=	18092946;	// 001_020_05010
	public	const uint	RICHTEXT_FriendNum	=	18088036;	// 001_020_00100
	public	const uint	LISTBOX_FriendList	=	18088136;	// 001_020_00200
	public	const uint	SCROLLBAR_FriendList	=	18088141;	// 001_020_00205
	public	const uint	FRAME_FriendFrame	=	18088146;	// 001_020_00210
	public	const uint	ICON_FriendIcon	=	18088156;	// 001_020_00220
	public	const uint	TEXTURE_FriendConnect	=	18088159;	// 001_020_00223
	public	const uint	TEXT_FriendName	=	18088158;	// 001_020_00222
	public	const uint	TEXTURE_FriendParty	=	18088162;	// 001_020_00226
	public	const uint	TEXTURE_FriendPlace	=	18088160;	// 001_020_00224
	public	const uint	TEXT_FriendQuest	=	18088161;	// 001_020_00225
	public	const uint	TEXTURE_FriendGuild	=	18088163;	// 001_020_00227
	public	const uint	TEXT_FriendGuild	=	18088164;	// 001_020_00228
	public	const uint	BUTTON_FriendParty	=	18088226;	// 001_020_00290
	public	const uint	BUTTON_FriendChat	=	18088236;	// 001_020_00300
	public	const uint	BUTTON_FriendReply	=	18088216;	// 001_020_00280
	public	const uint	BUTTON_FriendTrash	=	18088246;	// 001_020_00310
};
