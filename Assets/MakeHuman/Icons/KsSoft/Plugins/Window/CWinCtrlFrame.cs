//==============================================================================================
/*!?ウィンドウフレームをレンダリングする.
	@file  CWinCtrlFrame
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public class CWinCtrlFrame : CWinCtrlBase {
        //==========================================================================
        /*!Constructor
            @brief	Constructor
        */
        public CWinCtrlFrame(CWindowBase cWindowBase) : base(cWindowBase, e_WinCtrlKind.FRAME) {
            m_aTexture[0].partId = new FiveCC("FRAME");
        }
        //==========================================================================
        /*!Copy Constructor
            @brief	Copy Constructor
        */
        public CWinCtrlFrame(CWinCtrlFrame src, CWinContents parent) : base(src, parent) {
        }
        //==========================================================================
        /*!Render
            @brief	render
        */
        override public void render(Vector3 pos, CWindowMesh cWinMesh, CWinTextMesh cTextMesh, WinColor color, ClipRect cr) {
            // 背景.
            color = color * this.color;
            cWinMesh.request(parts, pos, color, 0, cr);
        }
    }
}
