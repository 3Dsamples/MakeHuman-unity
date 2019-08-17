//==============================================================================================
/*!?IKeyFocusable.
	@file  IKeyFocusable
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KS {
    public delegate void dlWinCtrlKeyboardCommitDelegate(IWinKeyFocusable control);

    public interface IWinKeyFocusable {
        //==========================================================================
        /*!フォーカスを失ったとき呼ばれる.
            @brief	lostFocus
        */
        void lostFocus();

        //==========================================================================
        /*!文字列を設定する.
            @brief SetInputText
            @note	内部でのみ使用.
        */
        string setInputText(string inputText, ref int insertPt, ref int selectionPt);

        //==========================================================================
        /*!キーボードの情報を取得する.
            @brief GetInputText
            @note	内部でのみ使用.
        */
        string getInputText(ref t_KeyboardInfo info);

        //==========================================================================
        /*!内容を反映する.
            @brief commit
            @note	内部でのみ使用.
        */
        void commit();

        //==========================================================================
        /*!キャラクタコードが有効かどうか判定する.
            @brief	validCharCode
        */
        bool validCharCode(char c);

        //==========================================================================
        /*!コンテンツにアクセスする.
            @brief Content
        */
        string Content {
            get;
        }
        //==========================================================================
        /*!エディット可能かどうか?.
            @brief editable
        */
        bool editable {
            get;
            set;
        }

        //==========================================================================
        /*!上が押された.
            @brief GoUp
        */
        void goUp();

        //==========================================================================
        /*!下が押された.
            @brief GoDown
        */
        void goDown();
    }
}
