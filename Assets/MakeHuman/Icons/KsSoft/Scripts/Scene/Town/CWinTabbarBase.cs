using KS;
public class CWinTabbarBase : CWindowBase {
	public	const uint	windowId	=	167772160;	// 010_000_00000
	static public CWinTabbar create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinTabbar>(windowId,cParent);
	}
	public	const uint	RADIO_Home	=	167772170;	// 010_000_00010
	public	const uint	RADIO_Quest	=	167772180;	// 010_000_00020
	public	const uint	RADIO_Item	=	167772200;	// 010_000_00040
	public	const uint	RADIO_Gatcha	=	167772210;	// 010_000_00050
	public	const uint	RADIO_Shop	=	167772220;	// 010_000_00060
};
