//==============================================================================================
/*!
	@file  e_WinStyle.h

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;

namespace KS {
    //==============================================================================================
    /*!
        @brief	define WINDOW STYLE
        @note
    */
    //==============================================================================================
    [Flags]
    public enum e_WinStyle {
        NONE = 0,
        // 表示位置.
        ANCHOR_DEFAULT = (0 << 0),      // ウィンドウの基準位置が画面の左上.
        ANCHOR_LEFTTOP = (1 << 0),      // ウィンドウの基準位置が画面の左上.
        ANCHOR_LEFT = (2 << 0),     // ウィンドウの基準位置が画面の左中.
        ANCHOR_LEFTBOTTOM = (3 << 0),       // ウィンドウの基準位置が画面の左下.
        ANCHOR_TOP = (4 << 0),      // ウィンドウの基準位置が画面の上部.
        ANCHOR_CENTER = (5 << 0),       // ウィンドウの基準位置が画面中央.
        ANCHOR_BOTTOM = (6 << 0),       // ウィンドウの基準位置が画面の底部.
        ANCHOR_RIGHTTOP = (7 << 0),     // ウィンドウの基準位置が画面の右上.
        ANCHOR_RIGHT = (8 << 0),        // ウィンドウの基準位置が画面の右中.
        ANCHOR_RIGHTBOTTOM = (9 << 0),      // ウィンドウの基準位置が画面の右底.
        ANCHOR_MASK = (15 << 0),
        // 優先度タイプ.
        NORMAL = (0 << 6),  // 通常のウィンドウ.
        TOP = (1 << 6), // トップウィンドウ(画面外をクリックしても閉じない).
        POPUP = (2 << 6),   // ポップアップウィンドウ(画面外をクリックしたとき閉じる).
        TOPMOST = (3 << 6), // トップモーストウィンドウ(画面外をクリックしても閉じない + 他のウィンドウに対して入力を奪う).
        PRIORITY_MASK = (3 << 6),
        NOECLIPSE = (1 << 8),   // ウィンドウが開いたときに他のウィンドウを暗くしない(TOP/POPUP時のみ有効).
                                // ウィンドウフレーム関連.
        NOCLOSE = (1 << 9), // ユーザーが閉じれない（閉じるボタン無し、ＥＳＣキー無効）.
        NOMINIMIZATION = (1 << 10), // 最小化できない.
        NOHELP = (1 << 11), // ヘルプボタンが必要ない.
        NOTITLEBAR = (1 << 12), // タイトルバー無し.
        FRAME = (0 << 13),  // フレームあり.
        NOFRAME = (1 << 13),    // フレームの表示をオフにする.
        FRAME_MASK = (3 << 13), // フレームタイプ.
                                // ウィンドウ機能制御関連.
        DISABLE = (1 << 15),    // 機能を奪う.
        DRAG = (1 << 16),   // ドラッグ可.
        NOACTIVE = (1 << 17),   // アクティブにならない.
        HIDE = (1 << 18),   // 非表示.
        NOBRINGTOTOP = (1 << 19),   // アクティブになったときに、前面に出ない.
        OPENBOTTOM = (1 << 20), // ウィンドウが開くとき、プライオリティが一番下になるように開く.
        DISABLE_FLAG = DISABLE,
    };

    //==============================================================================================
    /*!
        @brief	define CTRL STYLE
        @note
    */
    //==============================================================================================
    [Flags]
    public enum e_WinCtrlStyle {
        NONE = 0,
        // 原点を決定するスタイル.
        ANCHOR_DEFAULT = (0 << 0),      // 原点がデフォルト(左上).
        ANCHOR_LEFTTOP = (1 << 0),      // 原点が左上.
        ANCHOR_LEFTCENTER = (2 << 0),       // 原点が左中.
        ANCHOR_LEFTBOTTOM = (3 << 0),       // 原点が左下.
        ANCHOR_TOP = (4 << 0),      // 原点が上部.
        ANCHOR_CENTER = (5 << 0),       // 原点が中央.
        ANCHOR_BOTTOM = (6 << 0),       // 原点が底部.
        ANCHOR_RIGHTTOP = (7 << 0),     // 原点が右上.
        ANCHOR_RIGHTCENTER = (8 << 0),      // 原点が右中.
        ANCHOR_RIGHTBOTTOM = (9 << 0),      // 原点が右底.
        ANCHOR_MASK = (15 << 0),
        // コントロールの基準位置を決定するスタイル.
        BASE_DEFAULT = (0 << 4),        // コントロールの基準位置がデフォルト.
        BASE_LEFTTOP = (1 << 4),        // コントロールの基準位置が左上.
        BASE_LEFTCENTER = (2 << 4),     // コントロールの基準位置が左中.
        BASE_LEFTBOTTOM = (3 << 4),     // コントロールの基準位置が左下.
        BASE_TOP = (4 << 4),        // コントロールの基準位置が上部.
        BASE_CENTER = (5 << 4),     // コントロールの基準位置が中央.
        BASE_BOTTOM = (6 << 4),     // コントロールの基準位置が底部.
        BASE_RIGHTTOP = (7 << 4),       // コントロールの基準位置が右上.
        BASE_RIGHTCENTER = (8 << 4),        // コントロールの基準位置が右中.
        BASE_RIGHTBOTTOM = (9 << 4),        // コントロールの基準位置が右底.
        BASE_MASK = (15 << 4),

        // テキスト位置を設定するスタイル(ボタンで有効).
        TEXT_NONE = (0 << 8),   // テキストをデフォルト表示する.
        TEXT_LEFT = (1 << 8),   // テキストを左詰めで表示する.
        TEXT_CENTER = (2 << 8), // テキストをセンタリング表示する.
        TEXT_RIGHT = (3 << 8),  // テキストを右詰めで表示する.
        TEXT_ALIGN_MASK = (3 << 8),
        // テキストスタイルを設定する.
        TEXT_NORMAL = (0 << 10),    // テキストを通常表示.
        TEXT_BOLD = (1 << 10),  // テキストをボールド表示.
        TEXT_DENT = (2 << 10),  // テキストを凹み文字.
        TEXT_SHADOW = (3 << 10),    // テキストを影文字.
        TEXT_STYLE_MASK = (3 << 10),
        //禁則処理を行う?.
        TEXT_NOHYPHENATION = (1 << 29),
        //テキストサイズを横幅に自動調整する.
        TEXT_AUTOSCALE = (1 << 30),
        //機能を制限するスタイル.
        HIDE = (1 << 12),   // 非表示.
        DRAG = (1 << 13),   // ドラッグ.
        DISABLE = (1 << 14),    // 機能を奪う(色も暗くする).
        NOHIT = (1 << 15),  // マウスの当たり判定無し.
        HIT = (1 << 23),    // マウスの当たりあり(初期化時のみ使用).

        EDIT_BLIND = (1 << 16), // ブラインド入力スタイル.
        EDIT_TYPE_ALL = (0 << 17),
        EDIT_TYPE_ASCIICAPABLE = (1 << 17),
        EDIT_TYPE_NUMBERANDPUNCTUATION = (2 << 17),
        EDIT_TYPE_URL = (3 << 17),
        EDIT_TYPE_NUMBERPAD = (4 << 17),
        EDIT_TYPE_PHONEPAD = (5 << 17),
        EDIT_TYPE_NAMEPHONEPAD = (6 << 17),
        EDIT_TYPE_EMAILADDRESS = (7 << 17),
        EDIT_TYPE_MASK = (7 << 17),
        NOBOUNCES = (1 << 22),    // スクロール可能なコントロールにおいて、領域オーバーしてスクロールさせる処理を無効にする.
                                  // スクロール可能コントロール関連.
        SCROLL_LOOP = (1 << 28),   // スクロールがループする.
        ITEM_STACK_V = (0 << 24),   // 縦スクロール.
        ITEM_STACK_H = (1 << 24),   // 横スクロール.
        SCROLL_UNLOCK = (0 << 25),  //リストボックスのスクロールをロックしない.
        SCROLL_LOCK = (1 << 25),    //リストボックスのスクロールをロックする.
                                    // スクロールバー関連.
        SCROLLBAR_DISPLAY_NORMAL = (0 << 26),//最小限表示(デフォルト).
        SCROLLBAR_DISPLAY_SCROLLABLE = (1 << 26),//スクロール可能なとき表示.
        SCROLLBAR_DISPLAY_ALWAYS = (2 << 26),//常に表示.
        SCROLLBAR_DISPLAY_MASK = (3 << 26),
    };

}
