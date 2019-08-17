//==============================================================================================
/*!?テキストコントロール.
	@file  CWinCtrlLogText
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlLogText : CWinCtrlTextBase {
        CWinLogTextOne m_cLogTextOne;
        int m_line;
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlLogText(CWindowBase cWindowBase) : base(cWindowBase, e_WinCtrlKind.LOGTEXT) {
            m_cLogTextOne = new CWinLogTextOne();
            m_cTextOne = m_cLogTextOne;
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlLogText(CWinCtrlLogText src, CWinContents parent) : base(src, parent) {
            m_cLogTextOne = new CWinLogTextOne(src.m_cLogTextOne);
            m_cTextOne = m_cLogTextOne;
        }
        public int setLog(string sentence, WinColor color) {
            m_aTexture[0].color = color;
            caption = sentence;
            m_cLogTextOne.setText(sentence, color, 1f, lineSpace, lineFeedWidth);
            size = new Vector2(m_cLogTextOne.width, m_cLogTextOne.height);
            m_line = m_cLogTextOne.line;
            return m_line;
        }
        //==========================================================================
        /*!設定されているログを初期化する.
            @brief	clearLog
        */
        public void clearLog() {
            caption = "";
            m_cLogTextOne.clear();
            m_line = 0;
        }
        //==========================================================================
        /*!キャプションパーツを再更新する必要があるとき呼ばれる.
            @brief	onResetCaption
        */
        override public void onResetCaption() {
            m_eUpdateFlag &= ~e_UpdateFlag.CAPTION;
        }
    }
}
