using KS;

public class CWinWaitBase : CWindowBase {
	public	const uint	windowId	=	4194304010;	// 250_000_00010
	static public CWinWait create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinWait>(windowId,cParent);
	}
	public	const uint	RICHTEXT_MESSAGE	=	65546;	// 000_001_00010
	public	const uint	RICHTEXT_TIPS	=	65556;	// 000_001_00020
	public	const uint	TEXTURE_Wait	=	131092;	// 000_002_00020
};
