//==============================================================================================
/*!メッセージボックスインターフェース.
	@file  IMessageBox
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace KS {
    public interface IMessageBox {
        void iMessageBox(CMessageBox.e_Kind eKind, int iMsgBoxId);
    }
}
