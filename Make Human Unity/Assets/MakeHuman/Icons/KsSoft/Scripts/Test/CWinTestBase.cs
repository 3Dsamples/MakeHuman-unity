using KS;
public class CWinTestBase : CWindowBase {
	public	const uint	windowId	=	4278190080;	// 255_000_00000
	static public CWinTest create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinTest>(windowId,cParent);
	}
	public	const uint	TEXT_Test	=	42282;	// 000_000_42282
	public	const uint	RICHTEXT_Test	=	674666;	// 000_010_19306
	public	const uint	LOG_Test	=	18986;	// 000_000_18986
	public	const uint	TEXTBOX_Test	=	337194;	// 000_005_09514
	public	const uint	EDITBOX_Test	=	334730;	// 000_005_07050
	public	const uint	BUTTON_Test	=	162538;	// 000_002_31466
	public	const uint	CHECKBOX_Test	=	663370;	// 000_010_08010
	public	const uint	RADIO_A	=	9485;	// 000_000_09485
	public	const uint	RADIO_B	=	9486;	// 000_000_09486
	public	const uint	RADIO_C	=	9487;	// 000_000_09487
	public	const uint	LINE_Test	=	37162;	// 000_000_37162
	public	const uint	RENDER_Test	=	156138;	// 000_002_25066
	public	const uint	ICON_Test	=	39114;	// 000_000_39114
	public	const uint	RECASTICON_Test	=	2512234;	// 000_038_21866
	public	const uint	RENDERICON_Test	=	2505194;	// 000_038_14826
	public	const uint	METER_Test	=	79690;	// 000_001_14154
	public	const uint	CONTAINER_Test	=	1263242;	// 000_019_18058
	public	const uint	LISTBOX_Test	=	336554;	// 000_005_08874
	public	const uint	SCROLLBAR_Test	=	1229962;	// 000_018_50314
	public	const uint	LISTBOXEX_Test	=	1340074;	// 000_020_29354
	public	const uint	FRAME_Test	=	73258;	// 000_001_07722
	public	const uint	LABEL_Test	=	74026;	// 000_001_08490
	public	const uint	BAR_Test	=	19178;	// 000_000_19178
	public	const uint	LOGTEXT_Test	=	337962;	// 000_005_10282
	public	const uint	TEXTURE_RadioA	=	1243004;	// 000_018_63356
	public	const uint	TEXTURE_RadioB	=	1243036;	// 000_018_63388
	public	const uint	TEXTURE_RadioC	=	1243068;	// 000_018_63420
	public	const uint	TEXTURE_A	=	38741;	// 000_000_38741
	public	const uint	TEXTURE_B	=	38742;	// 000_000_38742
	public	const uint	TEXTURE_C	=	38743;	// 000_000_38743
	public	const uint	TEXTURE_D	=	38744;	// 000_000_38744
	public	const uint	BUTTON_Listbox	=	1301386;	// 000_019_56202
	public	const uint	BUTTON_ListboxEx	=	5201674;	// 000_079_24330
};
