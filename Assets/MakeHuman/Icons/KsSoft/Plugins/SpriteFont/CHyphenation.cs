//==============================================================================================
/*!禁則処理対象文字列かどうか判定する.
	@file  CHyphenation

	( counter SJIS string 京.)
*/
//==============================================================================================

namespace KS {
    class CHyphenation {
        static string Nonstart = ",)）]｝、〕〉》」』】〟’”≫" +
                                    "ゝゞーァィゥェォッャュョヮヵヶぁぃぅぇぉっゃゅょゎ々" +
                                    "‐～" +
                                    " ?!？！" +
                                    "・:;/：；／" +
                                    "。.";
        static string Nonend = "([｛〔〈《「『【〝‘“?≪" +
                                     "…‥";
        static public bool isNonstart(char c) {
            if (Nonstart.IndexOf(c) >= 0) {
                return true;
            }
            return false;
        }
        static public bool isNonend(char c) {
            if (Nonend.IndexOf(c) >= 0) {
                return true;
            }
            return false;
        }
    };
}
