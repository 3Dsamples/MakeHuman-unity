using KS;
public class CWinQuestMultiBase : CWindowBase {
	public	const uint	windowId	=	168427610;	// 010_010_00090
	static public CWinQuestMulti create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinQuestMulti>(windowId,cParent);
	}
	public	const uint	TEXTURE_TitleBarBGTitle	=	1000;	// 000_000_01000
	public	const uint	TEXT_TitleTitle	=	1001;	// 000_000_01001
	public	const uint	CONTAINER_Map	=	100;	// 000_000_00100
	public	const uint	BUTTON_JoinWithKey	=	4000;	// 000_000_04000
	public	const uint	BUTTON_Prev	=	2000;	// 000_000_02000
	public	const uint	BUTTON_Next	=	2010;	// 000_000_02010
	public	const uint	TEXTURE_Page	=	3020;	// 000_000_03020
	public	const uint	TEXT_Page	=	3030;	// 000_000_03030
	public	const uint	BUTTON_BackBack	=	3000;	// 000_000_03000
	public	const uint	TEXT_BackBack	=	3001;	// 000_000_03001
	public	const uint	BUTTON_Camp1	=	1996488704;	// 119_000_00000
	public	const uint	TEXT_Camp1	=	1996488705;	// 119_000_00001
	public	const uint	TEXTURE_Camp1Star0	=	1996488707;	// 119_000_00003
	public	const uint	TEXTURE_Camp1Star1	=	1996488708;	// 119_000_00004
	public	const uint	TEXTURE_Camp1Star2	=	1996488709;	// 119_000_00005
	public	const uint	BUTTON_Camp2	=	3674210304;	// 219_000_00000
	public	const uint	TEXT_Camp2	=	3674210305;	// 219_000_00001
	public	const uint	TEXTURE_Camp2Star0	=	3674210307;	// 219_000_00003
	public	const uint	TEXTURE_Camp2Star1	=	3674210308;	// 219_000_00004
	public	const uint	TEXTURE_Camp2Star2	=	3674210309;	// 219_000_00005
};
