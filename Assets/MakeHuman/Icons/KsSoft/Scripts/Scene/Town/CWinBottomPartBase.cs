using KS;

public class CWinBottomPartBase : CWindowBase {
	public	const uint	windowId	=	167772168;	// 010_000_00008
	static public CWinBottomPart create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinBottomPart>(windowId,cParent);
	}
	public	const uint	FRAME_Bottom	=	10;	// 000_000_00010
	public	const uint	BUTTON_Toggle	=	20;	// 000_000_00020
};
