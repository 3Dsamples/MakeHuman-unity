using KS;

public class CWinAssetbundleLoadingBase : CWindowBase {
	public	const uint	windowId	=	16777216;	// 001_000_00000
	static public CWinAssetbundleLoading create(CWindowBase cParent = null) {
		return CWindowMgr.Instance.create<CWinAssetbundleLoading>(windowId,cParent);
	}
	public	const uint	METER_ProgressTotal	=	65536;	// 000_001_00000
	public	const uint	METER_ProgressPart	=	65546;	// 000_001_00010
	public	const uint	TEXTURE_Wait	=	131092;	// 000_002_00020
	public	const uint	TEXT_Message	=	131102;	// 000_002_00030
};
