using KS;
public class CWinChatBase : CWindowBase {
	public	const uint	windowId	=	167772175;	// 010_000_00015
	static public CWinChat create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinChat>(windowId,cParent);
	}
	public	const uint	BUTTON_Resize	=	10;	// 000_000_00010
	public	const uint	FRAME_Chat	=	20;	// 000_000_00020
	public	const uint	LOG_Chat	=	30;	// 000_000_00030
	public	const uint	SCROLLBAR_Chat	=	65537;	// 000_001_00001
	public	const uint	ICON_Chat	=	65536;	// 000_001_00000
	public	const uint	FRAME_Balloon	=	65546;	// 000_001_00010
	public	const uint	LOGTEXT_Chat	=	65636;	// 000_001_00100
	public	const uint	TEXT_Name	=	65646;	// 000_001_00110
};
