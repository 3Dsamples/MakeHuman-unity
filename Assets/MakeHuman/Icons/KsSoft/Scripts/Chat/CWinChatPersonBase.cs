using KS;

public class CWinChatPersonBase : CWindowBase {
	public	const uint	windowId	=	167772200;	// 010_000_00040
	static public CWinChatPerson create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinChatPerson>(windowId,cParent);
	}
	public	const uint	FRAME_Profile	=	10;	// 000_000_00010
	public	const uint	BUTTON_Close	=	30;	// 000_000_00030
	public	const uint	TEXT_Title	=	40;	// 000_000_00040
	public	const uint	RADIO_Party	=	100;	// 000_000_00100
	public	const uint	RADIO_Friend	=	110;	// 000_000_00110
	public	const uint	RADIO_Guild	=	120;	// 000_000_00120
	public	const uint	LISTBOX_Party	=	200;	// 000_000_00200
	public	const uint	SCROLLBAR_Party	=	205;	// 000_000_00205
	public	const uint	ICON_Party	=	210;	// 000_000_00210
	public	const uint	TEXTURE_PartyConnect	=	220;	// 000_000_00220
	public	const uint	TEXT_PartyName	=	230;	// 000_000_00230
	public	const uint	TEXT_PartyLevel	=	250;	// 000_000_00250
	public	const uint	BUTTON_Party	=	260;	// 000_000_00260
	public	const uint	LISTBOX_Friend	=	300;	// 000_000_00300
	public	const uint	SCROLLBAR_Friend	=	305;	// 000_000_00305
	public	const uint	ICON_Friend	=	310;	// 000_000_00310
	public	const uint	TEXTURE_FriendConnect	=	320;	// 000_000_00320
	public	const uint	TEXT_FriendName	=	330;	// 000_000_00330
	public	const uint	TEXT_FriendLevel	=	350;	// 000_000_00350
	public	const uint	BUTTON_Friend	=	360;	// 000_000_00360
	public	const uint	LISTBOX_Guild	=	400;	// 000_000_00400
	public	const uint	SCROLLBAR_Guild	=	405;	// 000_000_00405
	public	const uint	ICON_Guild	=	410;	// 000_000_00410
	public	const uint	TEXTURE_GuildConnect	=	420;	// 000_000_00420
	public	const uint	TEXT_GuildName	=	430;	// 000_000_00430
	public	const uint	TEXT_GuildLevel	=	450;	// 000_000_00450
	public	const uint	BUTTON_Guild	=	460;	// 000_000_00460
};
