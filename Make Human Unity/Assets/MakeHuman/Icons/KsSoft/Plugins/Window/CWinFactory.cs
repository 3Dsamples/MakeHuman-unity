//==============================================================================================
/*!
	@file  CWinFactory.h

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace KS {
    public class CWinFactory {
        class CPropertyTable {
            public CPropertyTable(uint chunk, e_WinProperty eProperty) {
                m_chunk = chunk;
                m_eWinProperty = eProperty;
            }
            public uint m_chunk;
            public e_WinProperty m_eWinProperty;
        };
        CPropertyTable[] m_aPropertyTable = new CPropertyTable[] {
        new CPropertyTable(WinPropertyChunk.ID,         e_WinProperty.ID        ),
        new CPropertyTable(WinPropertyChunk.CAPTION,    e_WinProperty.CAPTION   ),
        new CPropertyTable(WinPropertyChunk.CAPTION_STR,e_WinProperty.CAPTION_STR   ),
        new CPropertyTable(WinPropertyChunk.CAPTION_OFFSET, e_WinProperty.CAPTION_OFFSET    ),
        new CPropertyTable(WinPropertyChunk.CAPTION_COLOR,  e_WinProperty.CAPTION_COLOR ),
        new CPropertyTable(WinPropertyChunk.POSITION,   e_WinProperty.POSITION  ),
        new CPropertyTable(WinPropertyChunk.POSITION2,  e_WinProperty.POSITION2 ),
        new CPropertyTable(WinPropertyChunk.SCREEN,     e_WinProperty.SCREEN    ),
        new CPropertyTable(WinPropertyChunk.SAFEAREA,   e_WinProperty.SAFEAREA  ),
        new CPropertyTable(WinPropertyChunk.CLOSE_POSITION, e_WinProperty.CLOSE_POSITION    ),
        new CPropertyTable(WinPropertyChunk.CLOSE_SCALE,    e_WinProperty.CLOSE_SCALE   ),
        new CPropertyTable(WinPropertyChunk.SIZE,       e_WinProperty.SIZE      ),
        new CPropertyTable(WinPropertyChunk.STYLE,      e_WinProperty.STYLE     ),
        new CPropertyTable(WinPropertyChunk.SE_ID,      e_WinProperty.SE_ID     ),
        new CPropertyTable(WinPropertyChunk.COLOR0,     e_WinProperty.COLOR0    ),
        new CPropertyTable(WinPropertyChunk.COLOR1,     e_WinProperty.COLOR1    ),
        new CPropertyTable(WinPropertyChunk.COLOR2,     e_WinProperty.COLOR2    ),
        new CPropertyTable(WinPropertyChunk.COLOR3,     e_WinProperty.COLOR3    ),
        new CPropertyTable(WinPropertyChunk.COLOR4,     e_WinProperty.COLOR4    ),
        new CPropertyTable(WinPropertyChunk.COLOR5,     e_WinProperty.COLOR5    ),
        new CPropertyTable(WinPropertyChunk.COLOR6,     e_WinProperty.COLOR6    ),
        new CPropertyTable(WinPropertyChunk.COLOR7,     e_WinProperty.COLOR7    ),
        new CPropertyTable(WinPropertyChunk.TEX_ID0,    e_WinProperty.TEX_ID0   ),
        new CPropertyTable(WinPropertyChunk.TEX_ID1,    e_WinProperty.TEX_ID1   ),
        new CPropertyTable(WinPropertyChunk.TEX_ID2,    e_WinProperty.TEX_ID2   ),
        new CPropertyTable(WinPropertyChunk.TEX_ID3,    e_WinProperty.TEX_ID3   ),
        new CPropertyTable(WinPropertyChunk.TEX_ID4,    e_WinProperty.TEX_ID4   ),
        new CPropertyTable(WinPropertyChunk.TEX_ID5,    e_WinProperty.TEX_ID5   ),
        new CPropertyTable(WinPropertyChunk.TEX_ID6,    e_WinProperty.TEX_ID6   ),
        new CPropertyTable(WinPropertyChunk.TEX_ID7,    e_WinProperty.TEX_ID7   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET0,e_WinProperty.TEXTURE_OFFSET0   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET1,e_WinProperty.TEXTURE_OFFSET1   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET2,e_WinProperty.TEXTURE_OFFSET2   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET3,e_WinProperty.TEXTURE_OFFSET3   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET4,e_WinProperty.TEXTURE_OFFSET4   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET5,e_WinProperty.TEXTURE_OFFSET5   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET6,e_WinProperty.TEXTURE_OFFSET6   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_OFFSET7,e_WinProperty.TEXTURE_OFFSET7   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_SIZE1,e_WinProperty.TEXTURE_SIZE1   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_SIZE2,e_WinProperty.TEXTURE_SIZE2   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_SIZE3,e_WinProperty.TEXTURE_SIZE3   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_SIZE4,e_WinProperty.TEXTURE_SIZE4   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_SIZE5,e_WinProperty.TEXTURE_SIZE5   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_SIZE6,e_WinProperty.TEXTURE_SIZE6   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_SIZE7,e_WinProperty.TEXTURE_SIZE7   ),
        new CPropertyTable(WinPropertyChunk.TEXTURE_ZOFFSET,e_WinProperty.TEXTURE_ZOFFSET   ),
        new CPropertyTable(WinPropertyChunk.RELATION_ID,e_WinProperty.RELATION_ID),
        new CPropertyTable(WinPropertyChunk.EDIT,       e_WinProperty.EDIT),
        new CPropertyTable(WinPropertyChunk.HELP_ID,    e_WinProperty.HELP_ID   ),
        new CPropertyTable(WinPropertyChunk.TOOLTIP,    e_WinProperty.TOOLTIP   ),
        new CPropertyTable(WinPropertyChunk.FONT_KIND,  e_WinProperty.FONT_KIND ),
        new CPropertyTable(WinPropertyChunk.GROUP,      e_WinProperty.GROUP     ),
        new CPropertyTable(WinPropertyChunk.CONTENTS,   e_WinProperty.CONTENTS  ),
        new CPropertyTable(WinPropertyChunk.SLIDEMAX,   e_WinProperty.SLIDEMAX  ),
        new CPropertyTable(WinPropertyChunk.PRIORITY,   e_WinProperty.PRIORITY  ),
        new CPropertyTable(WinPropertyChunk.CONTENTS_SIZE,  e_WinProperty.CONTENTS_SIZE),
        new CPropertyTable(WinPropertyChunk.LINE_SPACE, e_WinProperty.LINE_SPACE),
        new CPropertyTable(WinPropertyChunk.LINE_FEED_OFFSET, e_WinProperty.LINE_FEED_OFFSET),
    };
        Dictionary<e_WinProperty, CPropertyTable> m_dicProperty = new Dictionary<e_WinProperty, CPropertyTable>();
        Dictionary<uint, CPropertyTable> m_dicChunk = new Dictionary<uint, CPropertyTable>();
        //==============================================================================================
        /*!class Constructor
            @brief	class Constructor
            @note
        */
        //==============================================================================================
        static CWinFactory() {
            if (m_instance != null) {
                Debug.LogError("already exist instance");
                return;
            }
            m_instance = new CWinFactory();
        }
        //==============================================================================================
        /*!Constructor
            @brief	Constructor
            @note
        */
        //==============================================================================================
        public CWinFactory() {
            foreach (CPropertyTable cPT in m_aPropertyTable) {
                m_dicProperty[cPT.m_eWinProperty] = cPT;
                m_dicChunk[cPT.m_chunk] = cPT;
            }
        }
        //==============================================================================================
        /*!e_WinPropertyから、チャンクIDを取得する.
            @brief	getChunk
        */
        //==============================================================================================
        public uint getChunk(e_WinProperty eProperty) {
            CPropertyTable cPT;
            if (m_dicProperty.TryGetValue(eProperty, out cPT)) {
                return cPT.m_chunk;
            }
            return 0;
        }
        //==============================================================================================
        /*!ウィンドウコントロールを複製する.
            @brief	copyCtrl
        */
        //==============================================================================================
        static public CWinCtrlBase copyCtrl(CWinCtrlBase src, CWinContents parent) {
            switch (src.kind) {
            case e_WinCtrlKind.TEXT:
                return new CWinCtrlText(src as CWinCtrlText, parent);
            case e_WinCtrlKind.RICHTEXT:
                return new CWinCtrlRichText(src as CWinCtrlRichText, parent);
            case e_WinCtrlKind.LOGTEXT:
                return new CWinCtrlLogText(src as CWinCtrlLogText, parent);
            case e_WinCtrlKind.LOG:
                return new CWinCtrlLog(src as CWinCtrlLog, parent);
            case e_WinCtrlKind.EDITBOX:
                return new CWinCtrlEditbox(src as CWinCtrlEditbox, parent);
            case e_WinCtrlKind.TEXTBOX:
                return new CWinCtrlTextbox(src as CWinCtrlTextbox, parent);
            case e_WinCtrlKind.RICHTEXTBOX:
                return new CWinCtrlRichTextbox(src as CWinCtrlRichTextbox, parent);
            case e_WinCtrlKind.BUTTON:
                return new CWinCtrlButton(src as CWinCtrlButton, parent);
            case e_WinCtrlKind.CHECKBOX:
                return new CWinCtrlCheckbox(src as CWinCtrlCheckbox, parent);
            case e_WinCtrlKind.COMBOBOX:
                break;
            case e_WinCtrlKind.WINDOWCLOSEBUTTON:
                return new CWinCtrlCloseButton(src as CWinCtrlCloseButton, parent);
            case e_WinCtrlKind.WINDOWCAPTION:
                break;
            case e_WinCtrlKind.WINDOWMINIMIZATION:
                break;
            case e_WinCtrlKind.HELPBUTTON:
                break;
            case e_WinCtrlKind.RADIO:
                return new CWinCtrlRadio(src as CWinCtrlRadio, parent);
            case e_WinCtrlKind.RENDER:
                return new CWinCtrlRender(src as CWinCtrlRender, parent);
            case e_WinCtrlKind.CANVAS:
                return new CWinCtrlCanvas(src as CWinCtrlCanvas, parent);
            case e_WinCtrlKind.ICON:
                return new CWinCtrlIcon(src as CWinCtrlIcon, parent);
            case e_WinCtrlKind.RENDERICON:
                return new CWinCtrlRenderIcon(src as CWinCtrlRenderIcon, parent);
            case e_WinCtrlKind.RECASTICON:
                return new CWinCtrlRecastIcon(src as CWinCtrlRecastIcon, parent);
            case e_WinCtrlKind.METER:
                return new CWinCtrlMeter(src as CWinCtrlMeter, parent);
            case e_WinCtrlKind.SCROLLBAR:
                return new CWinCtrlScrollbar(src as CWinCtrlScrollbar, parent);
            case e_WinCtrlKind.SLIDEBAR:
                break;
            case e_WinCtrlKind.TEXTURE:
                return new CWinCtrlTexture(src as CWinCtrlTexture, parent);
            case e_WinCtrlKind.LINE:
                return new CWinCtrlLine(src as CWinCtrlLine, parent);
            case e_WinCtrlKind.FRAME:
                return new CWinCtrlFrame(src as CWinCtrlFrame, parent);
            case e_WinCtrlKind.LABEL:
                return new CWinCtrlLabel(src as CWinCtrlLabel, parent);
            case e_WinCtrlKind.BAR:
                return new CWinCtrlBar(src as CWinCtrlBar, parent);
            case e_WinCtrlKind.LISTBOX:
                return new CWinCtrlListbox(src as CWinCtrlListbox, parent);
            case e_WinCtrlKind.LISTBOXEX:
                return new CWinCtrlListboxEx(src as CWinCtrlListboxEx, parent);
            case e_WinCtrlKind.CONTAINER:
                return new CWinCtrlContainer(src as CWinCtrlContainer, parent);
            }
            return null;
        }
        //==============================================================================================
        /*!ウィンドウコントロールを生成する.
            @brief	create
        */
        //==============================================================================================
        static public CWinCtrlBase create(CWindowBase cWindow, e_WinCtrlKind eKind) {
            switch (eKind) {
            case e_WinCtrlKind.TEXT:
                return new CWinCtrlText(cWindow);
            case e_WinCtrlKind.LOGTEXT:
                return new CWinCtrlLogText(cWindow);
            case e_WinCtrlKind.RICHTEXT:
                return new CWinCtrlRichText(cWindow);
            case e_WinCtrlKind.LOG:
                return new CWinCtrlLog(cWindow);
            case e_WinCtrlKind.EDITBOX:
                return new CWinCtrlEditbox(cWindow);
            case e_WinCtrlKind.TEXTBOX:
                return new CWinCtrlTextbox(cWindow);
            case e_WinCtrlKind.RICHTEXTBOX:
                return new CWinCtrlRichTextbox(cWindow);
            case e_WinCtrlKind.BUTTON:
                return new CWinCtrlButton(cWindow);
            case e_WinCtrlKind.CHECKBOX:
                return new CWinCtrlCheckbox(cWindow);
            case e_WinCtrlKind.COMBOBOX:
                break;
            case e_WinCtrlKind.WINDOWCLOSEBUTTON:
                return new CWinCtrlCloseButton(cWindow);
            case e_WinCtrlKind.WINDOWCAPTION:
                break;
            case e_WinCtrlKind.WINDOWMINIMIZATION:
                break;
            case e_WinCtrlKind.HELPBUTTON:
                break;
            case e_WinCtrlKind.RADIO:
                return new CWinCtrlRadio(cWindow);
            case e_WinCtrlKind.RENDER:
                return new CWinCtrlRender(cWindow);
            case e_WinCtrlKind.CANVAS:
                return new CWinCtrlCanvas(cWindow);
            case e_WinCtrlKind.ICON:
                return new CWinCtrlIcon(cWindow);
            case e_WinCtrlKind.RENDERICON:
                return new CWinCtrlRenderIcon(cWindow);
            case e_WinCtrlKind.RECASTICON:
                return new CWinCtrlRecastIcon(cWindow);
            case e_WinCtrlKind.METER:
                return new CWinCtrlMeter(cWindow);
            case e_WinCtrlKind.SCROLLBAR:
                return new CWinCtrlScrollbar(cWindow);
            case e_WinCtrlKind.SLIDEBAR:
                break;
            case e_WinCtrlKind.TEXTURE:
                return new CWinCtrlTexture(cWindow);
            case e_WinCtrlKind.LINE:
                return new CWinCtrlLine(cWindow);
            case e_WinCtrlKind.FRAME:
                return new CWinCtrlFrame(cWindow);
            case e_WinCtrlKind.LABEL:
                return new CWinCtrlLabel(cWindow);
            case e_WinCtrlKind.BAR:
                return new CWinCtrlBar(cWindow);
            case e_WinCtrlKind.LISTBOX:
                return new CWinCtrlListbox(cWindow);
            case e_WinCtrlKind.LISTBOXEX:
                return new CWinCtrlListboxEx(cWindow);
            case e_WinCtrlKind.CONTAINER:
                return new CWinCtrlContainer(cWindow);
            }
            return null;
        }
        //==============================================================================================
        /*!Instance
            @brief	Instance
        */
        //==============================================================================================
        static CWinFactory m_instance = null;
        static public CWinFactory Instance {
            get {
                return m_instance;
            }
        }
    };
}
