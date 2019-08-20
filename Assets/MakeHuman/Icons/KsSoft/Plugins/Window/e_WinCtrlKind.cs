//==============================================================================================
/*!コントロールの種類を定義.
	@file  e_WinCtrlKind.cs
	@note	この定数はデフォルトプライオリティに使われ上のものほどプライオリティが高くなる.	
	(counter SJIS string 京.)
*/
namespace KS {
    //==============================================================================================

    // IDが小さいものほど、レンダリングは、後回しされる(前面にレンダリングされる).
    public enum e_WinCtrlKind {
        NONE = -1,

        // スクロールバー.
        SCROLLBAR = 0xF000,

        // リストボックス、コンテナ系.
        LISTBOX = 0x10000,
        LISTBOXEX,
        CONTAINER,

        // テキスト系.
        TEXT = 0x11000,
        RICHTEXT,
        LOG,
        LOGTEXT,

        // ボタン系.
        BUTTON = 0x12000,
        CHECKBOX,
        COMBOBOX,
        RADIO,
        WINDOWCLOSEBUTTON,
        WINDOWCAPTION,
        WINDOWMINIMIZATION,
        HELPBUTTON,

        // エディットボックス.
        EDITBOX = 0x13000,
        TEXTBOX,
        RICHTEXTBOX,

        // メータ、スライドバー.
        METER = 0x14000,
        SLIDEBAR,

        // アイコン、テクスチャ.
        ICON = 0x15000,
        IMAGE,
        TEXTURE,
        RENDER,
        RENDERICON,
        RECASTICON,
        LINE,
        CANVAS,

        // ラベル、フレーム.
        LABEL = 0x16000,
        FRAME,
        BAR,
    };
}
