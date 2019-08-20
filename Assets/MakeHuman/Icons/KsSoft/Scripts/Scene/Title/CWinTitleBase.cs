using KS;

public class CWinTitleBase : CWindowBase {
	public	const uint	windowId	=	16777221;	// 001_000_00005
	static public CWinTitle create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinTitle>(windowId,cParent);
	}
	public	const uint	TEXTURE_TitleMark	=	16777246;	// 001_000_00030
	public	const uint	TEXTURE_Title	=	16777226;	// 001_000_00010
	public	const uint	TEXTURE_TitleFlush	=	16777236;	// 001_000_00020
};
