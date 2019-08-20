//==============================================================================================
/*!
	@file  e_WinProperty.h

	(counter SJIS string 京.)
*/
//==============================================================================================

namespace KS {
    public enum e_WinProperty {
        NONE = -1,
        ID,
        CAPTION,
        CAPTION_STR,
        CAPTION_OFFSET,
        POSITION,
        POSITION2,
        CLOSE_POSITION,
        CLOSE_SCALE,
        STYLE,
        SIZE,
        SAFEAREA,
        SE_ID,
        CAPTION_COLOR,
        COLOR0,
        COLOR1,
        COLOR2,
        COLOR3,
        COLOR4,
        COLOR5,
        COLOR6,
        COLOR7,
        TEX_ID0,
        TEX_ID1,
        TEX_ID2,
        TEX_ID3,
        TEX_ID4,
        TEX_ID5,
        TEX_ID6,
        TEX_ID7,
        TEXTURE_OFFSET0,
        TEXTURE_OFFSET1,
        TEXTURE_OFFSET2,
        TEXTURE_OFFSET3,
        TEXTURE_OFFSET4,
        TEXTURE_OFFSET5,
        TEXTURE_OFFSET6,
        TEXTURE_OFFSET7,
        TEXTURE_SIZE1,
        TEXTURE_SIZE2,
        TEXTURE_SIZE3,
        TEXTURE_SIZE4,
        TEXTURE_SIZE5,
        TEXTURE_SIZE6,
        TEXTURE_SIZE7,
        EDIT,
        RELATION_ID,
        HELP_ID,
        TOOLTIP,
        FONT_KIND,
        GROUP,
        CONTENTS,
        SLIDEMAX,
        PRIORITY,
        CONTENTS_SIZE,
        LINE_SPACE,
        TEXTURE_ZOFFSET,
        SCREEN,
        LINE_FEED_OFFSET,
        RESOURCE = 65536,
        PATH,
        BASECLASS,
        Max,
    };
    public enum e_TexProperty {
        NONE,
        NODIVIDE,
        DIVIDE3H,
        DIVIDE3V,
        DIVIDE9,
        DITHER,
        COLOR,
    };
    public class WinPropertyChunk {
        public static uint ID = new FiveCC("ID");
        public static uint CAPTION = new FiveCC("CAPTN");
        public static uint CAPTION_STR = new FiveCC("CAPTS");
        public static uint CAPTION_OFFSET = new FiveCC("CPOFS");
        public static uint POSITION = new FiveCC("POSIT");
        public static uint POSITION2 = new FiveCC("POSI2");
        public static uint SCREEN = new FiveCC("SCREN");
        public static uint CLOSE_POSITION = new FiveCC("CLPOS");
        public static uint CLOSE_SCALE = new FiveCC("CLSCL");
        public static uint SIZE = new FiveCC("SIZE");
        public static uint SAFEAREA = new FiveCC("SFAR");
        public static uint STYLE = new FiveCC("STYLE");
        public static uint SE_ID = new FiveCC("SEID");
        public static uint CAPTION_COLOR = new FiveCC("CPCOL");
        public static uint COLOR0 = new FiveCC("COLR0");
        public static uint COLOR1 = new FiveCC("COLR1");
        public static uint COLOR2 = new FiveCC("COLR2");
        public static uint COLOR3 = new FiveCC("COLR3");
        public static uint COLOR4 = new FiveCC("COLR4");
        public static uint COLOR5 = new FiveCC("COLR5");
        public static uint COLOR6 = new FiveCC("COLR6");
        public static uint COLOR7 = new FiveCC("COLR7");
        public static uint TEX_ID0 = new FiveCC("TXID0");
        public static uint TEX_ID1 = new FiveCC("TXID1");
        public static uint TEX_ID2 = new FiveCC("TXID2");
        public static uint TEX_ID3 = new FiveCC("TXID3");
        public static uint TEX_ID4 = new FiveCC("TXID4");
        public static uint TEX_ID5 = new FiveCC("TXID5");
        public static uint TEX_ID6 = new FiveCC("TXID6");
        public static uint TEX_ID7 = new FiveCC("TXID7");
        public static uint TEXTURE_OFFSET0 = new FiveCC("TXOF0");
        public static uint TEXTURE_OFFSET1 = new FiveCC("TXOF1");
        public static uint TEXTURE_OFFSET2 = new FiveCC("TXOF2");
        public static uint TEXTURE_OFFSET3 = new FiveCC("TXOF3");
        public static uint TEXTURE_OFFSET4 = new FiveCC("TXOF4");
        public static uint TEXTURE_OFFSET5 = new FiveCC("TXOF5");
        public static uint TEXTURE_OFFSET6 = new FiveCC("TXOF6");
        public static uint TEXTURE_OFFSET7 = new FiveCC("TXOF7");
        public static uint TEXTURE_SIZE1 = new FiveCC("TXSZ1");
        public static uint TEXTURE_SIZE2 = new FiveCC("TXSZ2");
        public static uint TEXTURE_SIZE3 = new FiveCC("TXSZ3");
        public static uint TEXTURE_SIZE4 = new FiveCC("TXSZ4");
        public static uint TEXTURE_SIZE5 = new FiveCC("TXSZ5");
        public static uint TEXTURE_SIZE6 = new FiveCC("TXSZ6");
        public static uint TEXTURE_SIZE7 = new FiveCC("TXSZ7");
        public static uint EDIT = new FiveCC("EDIT");
        public static uint RELATION_ID = new FiveCC("RLNID");
        public static uint HELP_ID = new FiveCC("HELP");
        public static uint TOOLTIP = new FiveCC("TLTIP");
        public static uint FONT_KIND = new FiveCC("FONTK");
        public static uint GROUP = new FiveCC("GROUP");
        public static uint CONTENTS = new FiveCC("CONTT");
        public static uint SLIDEMAX = new FiveCC("SLDMX");
        public static uint PRIORITY = new FiveCC("PRIOR");
        public static uint CONTENTS_SIZE = new FiveCC("CNTSZ");
        public static uint TEXTURE_ZOFFSET = new FiveCC("TXZOF");
        public static uint LINE_SPACE = new FiveCC("LNSPC");
        public static uint LINE_FEED_OFFSET = new FiveCC("LNFDO");
    };

    public class WinSystemCtrl {
        public static uint BAR = new MulId(255, 0, 0);
        public static uint FRAME = new MulId(255, 0, 1);
        public static uint CLOSEBUTTON = new MulId(255, 0, 2);
        public static uint HELPBUTTON = new MulId(255, 0, 3);
    };
}
