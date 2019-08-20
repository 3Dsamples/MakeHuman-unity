using KS;

public class CMessageBoxBase : CWindowBase {
	public	const uint	windowId	=	4194304000;	// 250_000_00000
	static public CMessageBox create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CMessageBox>(windowId,cParent);
	}
	public	const uint	TEXTURE_BG	=	10;	// 000_000_00010
	public	const uint	TEXTURE_TitleBG	=	20;	// 000_000_00020
	public	const uint	TEXT_Title	=	65541;	// 000_001_00005
	public	const uint	RICHTEXT_MESSAGE	=	65546;	// 000_001_00010
	public	const uint	BUTTON_YES	=	131092;	// 000_002_00020
	public	const uint	BUTTON_NO	=	1310750;	// 000_020_00030
	public	const uint	BUTTON_CANCEL	=	131112;	// 000_002_00040
	public	const uint	BUTTON_OK	=	131122;	// 000_002_00050
};
