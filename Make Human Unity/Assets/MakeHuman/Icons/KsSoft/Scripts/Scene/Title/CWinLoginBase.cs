using KS;
public class CWinLoginBase : CWindowBase {
	public	const uint	windowId	=	16777218;	// 001_000_00002
	static public CWinLogin create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinLogin>(windowId,cParent);
	}
	public	const uint	EDITBOX_LoginID	=	16777226;	// 001_000_00010
	public	const uint	BUTTON_Login	=	16777256;	// 001_000_00040
	public	const uint	LISTBOX_List	=	23330816;	// 001_100_00000
	public	const uint	SCROLLBAR_List	=	23330826;	// 001_100_00010
	public	const uint	CHECKBOX_IP	=	16777236;	// 001_000_00020
};
