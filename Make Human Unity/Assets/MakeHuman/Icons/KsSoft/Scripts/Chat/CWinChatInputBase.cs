using KS;

public class CWinChatInputBase : CWindowBase {
	public	const uint	windowId	=	167772169;	// 010_000_00009
	static public CWinChatInput create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinChatInput>(windowId,cParent);
	}
	public	const uint	RADIO_Private	=	6553610;	// 000_100_00010
	public	const uint	TEXTURE_Private	=	6553620;	// 000_100_00020
	public	const uint	RADIO_Party	=	6553630;	// 000_100_00030
	public	const uint	TEXTURE_Party	=	6553640;	// 000_100_00040
	public	const uint	RADIO_Guild	=	6553650;	// 000_100_00050
	public	const uint	TEXTURE_Guild	=	6553660;	// 000_100_00060
	public	const uint	RADIO_Quest	=	6553670;	// 000_100_00070
	public	const uint	TEXTURE_Quest	=	6553680;	// 000_100_00080
	public	const uint	BUTTON_Phrase	=	6553800;	// 000_100_00200
	public	const uint	TEXT_Phrase	=	6553810;	// 000_100_00210
	public	const uint	TEXTURE_Arrow	=	6553820;	// 000_100_00220
	public	const uint	FRAME_Target	=	6553830;	// 000_100_00230
	public	const uint	TEXT_Target	=	6553840;	// 000_100_00240
	public	const uint	EDITBOX_Chat	=	6553850;	// 000_100_00250
};
