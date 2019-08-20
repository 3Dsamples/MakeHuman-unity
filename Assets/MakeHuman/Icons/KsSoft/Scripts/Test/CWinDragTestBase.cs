using KS;
public class CWinDragTestBase : CWindowBase {
	public	const uint	windowId	=	4278190090;	// 255_000_00010
	static public CWinDragTest create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinDragTest>(windowId,cParent);
	}
};
