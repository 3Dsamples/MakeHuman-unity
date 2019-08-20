using KS;
public class CNumberPadBase : CWindowBase {
	public	const uint	windowId	=	4194304020;	// 250_000_00020
	static public CNumberPad create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CNumberPad>(windowId,cParent);
	}
	public	const uint	TEXT_Title	=	100;	// 000_000_00100
	public	const uint	FRAME_Input	=	6553610;	// 000_100_00010
	public	const uint	TEXT_Value	=	6553620;	// 000_100_00020
	public	const uint	BUTTON_One	=	1;	// 000_000_00001
	public	const uint	BUTTON_Two	=	2;	// 000_000_00002
	public	const uint	BUTTON_Three	=	3;	// 000_000_00003
	public	const uint	BUTTON_Four	=	4;	// 000_000_00004
	public	const uint	BUTTON_Five	=	5;	// 000_000_00005
	public	const uint	BUTTON_Six	=	6;	// 000_000_00006
	public	const uint	BUTTON_Seven	=	7;	// 000_000_00007
	public	const uint	BUTTON_Eight	=	8;	// 000_000_00008
	public	const uint	BUTTON_Nine	=	9;	// 000_000_00009
	public	const uint	BUTTON_BackSpace	=	111;	// 000_000_00111
	public	const uint	BUTTON_Zero	=	10;	// 000_000_00010
	public	const uint	BUTTON_OK	=	101;	// 000_000_00101
};
