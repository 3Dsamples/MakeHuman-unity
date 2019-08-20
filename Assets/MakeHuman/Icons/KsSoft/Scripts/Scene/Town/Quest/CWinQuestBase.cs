using KS;

public class CWinQuestBase : CWindowBase {
	public	const uint	windowId	=	168427710;	// 010_010_00190
	static public CWinQuest create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinQuest>(windowId,cParent);
	}
	public	const uint	TEXTURE_TitleBarBGTitle	=	1000;	// 000_000_01000
	public	const uint	TEXT_TitleTitle	=	1001;	// 000_000_01001
	public	const uint	RADIO_Solo	=	10;	// 000_000_00010
	public	const uint	RADIO_Multi	=	20;	// 000_000_00020
	public	const uint	RADIO_Event	=	30;	// 000_000_00030
	public	const uint	BUTTON_Solo	=	1010;	// 000_000_01010
	public	const uint	BUTTON_Multi	=	2000;	// 000_000_02000
	public	const uint	BUTTON_Raid	=	2010;	// 000_000_02010
	public	const uint	BUTTON_Event	=	3000;	// 000_000_03000
};
