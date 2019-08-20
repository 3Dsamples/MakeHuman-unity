using KS;
public class CWinHomeBase : CWindowBase {
	public	const uint	windowId	=	168427590;	// 010_010_00070
	static public CWinHome create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinHome>(windowId,cParent);
	}
	public	const uint	TEXTURE_TitleBarBGTitle	=	1000;	// 000_000_01000
	public	const uint	TEXT_TitleTitle	=	1001;	// 000_000_01001
	public	const uint	TEXTURE_BG	=	10;	// 000_000_00010
	public	const uint	BUTTON_Guild	=	167772260;	// 010_000_00100
	public	const uint	BUTTON_Friend	=	167772270;	// 010_000_00110
	public	const uint	BUTTON_Present	=	167772280;	// 010_000_00120
	public	const uint	RENDER_DECK	=	20;	// 000_000_00020
	public	const uint	BUTTON_Info	=	167772360;	// 010_000_00200
	public	const uint	TEXTURE_Info	=	167772370;	// 010_000_00210
	public	const uint	BUTTON_Settings	=	167772380;	// 010_000_00220
	public	const uint	TEXTURE_Settings	=	167772390;	// 010_000_00230
	public	const uint	TEXTURE_Present	=	167772460;	// 010_000_00300
};
