using KS;
public class CWinTopPartBase : CWindowBase {
	public	const uint	windowId	=	167772165;	// 010_000_00005
	static public CWinTopPart create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinTopPart>(windowId,cParent);
	}
	public	const uint	TEXTURE_BG	=	10;	// 000_000_00010
	public	const uint	ICON_Icon	=	20;	// 000_000_00020
	public	const uint	TEXTURE_Icon	=	30;	// 000_000_00030
	public	const uint	TEXTURE_Connect	=	100;	// 000_000_00100
	public	const uint	TEXT_Name	=	110;	// 000_000_00110
	public	const uint	METER_Etc	=	140;	// 000_000_00140
	public	const uint	TEXT_Level	=	150;	// 000_000_00150
	public	const uint	METER_Exp	=	170;	// 000_000_00170
	public	const uint	RENDERICON_Party0	=	167772370;	// 010_000_00210
	public	const uint	RENDERICON_Party1	=	167772380;	// 010_000_00220
	public	const uint	RENDERICON_Party2	=	167772390;	// 010_000_00230
};
