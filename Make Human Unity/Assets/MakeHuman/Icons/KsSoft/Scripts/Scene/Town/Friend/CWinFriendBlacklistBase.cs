using KS;
public class CWinFriendBlacklistBase : CWindowBase {
	public	const uint	windowId	=	169082884;	// 010_020_00004
	static public CWinFriendBlacklist create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinFriendBlacklist>(windowId,cParent);
	}
	public	const uint	FRAME_BlacklistSearchBackgroundFrame	=	18088336;	// 001_020_00400
	public	const uint	TEXT_BlacklistSearchLabel	=	18088346;	// 001_020_00410
	public	const uint	EDITBOX_BlacklistSearchBox	=	18088356;	// 001_020_00420
	public	const uint	TEXT_BlacklistLabel	=	18088446;	// 001_020_00510
	public	const uint	BUTTON_Sort	=	169082890;	// 010_020_00010
	public	const uint	TEXT_Sort	=	169082891;	// 010_020_00011
	public	const uint	LISTBOX_Blacklist	=	18088456;	// 001_020_00520
	public	const uint	SCROLLBAR_Blacklist	=	18088461;	// 001_020_00525
	public	const uint	FRAME_BlacklistFrame	=	18088466;	// 001_020_00530
	public	const uint	ICON_BlacklistIcon	=	18088476;	// 001_020_00540
	public	const uint	TEXTURE_BlacklistConnect	=	18088479;	// 001_020_00543
	public	const uint	TEXT_BlacklistName	=	18088478;	// 001_020_00542
	public	const uint	TEXTURE_BlacklistParty	=	18088482;	// 001_020_00546
	public	const uint	TEXTURE_BlacklistPlace	=	18088480;	// 001_020_00544
	public	const uint	TEXT_BlacklistQuest	=	18088481;	// 001_020_00545
	public	const uint	TEXTURE_BlacklistGuild	=	18088483;	// 001_020_00547
	public	const uint	TEXT_BlacklistGuild	=	18088484;	// 001_020_00548
	public	const uint	BUTTON_BlacklistCancel	=	18088486;	// 001_020_00550
};
