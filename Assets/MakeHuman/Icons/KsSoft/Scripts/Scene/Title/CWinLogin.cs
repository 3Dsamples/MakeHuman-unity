//==============================================================================================
/*!ログインウィンドウ.
	@file	CWinLogin
	
	(cou nter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections;
using KS;

public class CWinLogin : CWinLoginBase {
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl) {
		switch (cCtrl.id) {
		  case BUTTON_Login:
			CMainSystem.Instance.changeScene("Town");
			//マネージ起動.
			new CProfileMgr();
			new CFriendMgr();
			new CChatMgr();
			break;
		}
	}
}
