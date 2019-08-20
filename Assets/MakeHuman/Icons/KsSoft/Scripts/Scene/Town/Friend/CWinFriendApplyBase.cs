using KS;
public class CWinFriendApplyBase : CWindowBase {
	public	const uint	windowId	=	169082883;	// 010_020_00003
	static public CWinFriendApply create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinFriendApply>(windowId,cParent);
	}
	public	const uint	FRAME_ApplySearchBackgroundFrame	=	18088336;	// 001_020_00400
	public	const uint	TEXT_ApplySearchLabel	=	18088346;	// 001_020_00410
	public	const uint	EDITBOX_ApplySearchBox	=	18088356;	// 001_020_00420
	public	const uint	TEXT_ApplyLabel	=	18088446;	// 001_020_00510
	public	const uint	BUTTON_Sort	=	169082890;	// 010_020_00010
	public	const uint	TEXT_Sort	=	169082891;	// 010_020_00011
	public	const uint	LISTBOX_ApplyList	=	18088456;	// 001_020_00520
	public	const uint	SCROLLBAR_ApplyList	=	18088461;	// 001_020_00525
	public	const uint	FRAME_ApplyFrame	=	18088466;	// 001_020_00530
	public	const uint	ICON_ApplyIcon	=	18088476;	// 001_020_00540
	public	const uint	TEXTURE_ApplyConnect	=	18088479;	// 001_020_00543
	public	const uint	TEXT_ApplyName	=	18088478;	// 001_020_00542
	public	const uint	TEXTURE_ApplyParty	=	18088482;	// 001_020_00546
	public	const uint	TEXTURE_ApplyPlace	=	18088480;	// 001_020_00544
	public	const uint	TEXT_ApplyQuest	=	18088481;	// 001_020_00545
	public	const uint	TEXTURE_ApplyGuild	=	18088483;	// 001_020_00547
	public	const uint	TEXT_ApplyGuild	=	18088484;	// 001_020_00548
	public	const uint	BUTTON_ApplyCancel	=	18088486;	// 001_020_00550
};
