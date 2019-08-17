using KS;
public class CWinQuestStoryBase : CWindowBase {
	public	const uint	windowId	=	168427600;	// 010_010_00080
	static public CWinQuestStory create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinQuestStory>(windowId,cParent);
	}
	public	const uint	TEXTURE_TitleBarBGTitle	=	1000;	// 000_000_01000
	public	const uint	TEXT_TitleTitle	=	1001;	// 000_000_01001
	public	const uint	CONTAINER_Map	=	100;	// 000_000_00100
	public	const uint	BUTTON_Prev	=	2000;	// 000_000_02000
	public	const uint	BUTTON_Next	=	2010;	// 000_000_02010
	public	const uint	TEXTURE_Page	=	3020;	// 000_000_03020
	public	const uint	TEXT_Page	=	3030;	// 000_000_03030
	public	const uint	BUTTON_BackBack	=	3000;	// 000_000_03000
	public	const uint	TEXT_BackBack	=	3001;	// 000_000_03001
	public	const uint	BUTTON_Camp0	=	150994944;	// 009_000_00000
	public	const uint	TEXT_Camp0	=	150994945;	// 009_000_00001
	public	const uint	TEXTURE_Camp0Star0	=	150994947;	// 009_000_00003
	public	const uint	TEXTURE_Camp0Star1	=	150994948;	// 009_000_00004
	public	const uint	TEXTURE_Camp0Star2	=	150994949;	// 009_000_00005
	public	const uint	BUTTON_Camp1	=	150994954;	// 009_000_00010
	public	const uint	TEXT_Camp1	=	150994955;	// 009_000_00011
	public	const uint	TEXTURE_Camp1Star0	=	150994957;	// 009_000_00013
	public	const uint	TEXTURE_Camp1Star1	=	150994958;	// 009_000_00014
	public	const uint	TEXTURE_Camp1Star2	=	150994959;	// 009_000_00015
	public	const uint	LINE_Camp1	=	150994960;	// 009_000_00016
	public	const uint	BUTTON_Camp2	=	151004954;	// 009_000_10010
	public	const uint	TEXT_Camp2	=	151004955;	// 009_000_10011
	public	const uint	TEXTURE_Camp2Star0	=	151004957;	// 009_000_10013
	public	const uint	TEXTURE_Camp2Star1	=	151004958;	// 009_000_10014
	public	const uint	TEXTURE_Camp2Star2	=	151004959;	// 009_000_10015
	public	const uint	LINE_Camp2	=	151004960;	// 009_000_10016
};
