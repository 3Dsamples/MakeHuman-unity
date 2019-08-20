using UnityEngine;
using System.Collections;
using KS;

public class CWinQuestStory : CWinQuestStoryBase {
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	override public void onCreate() {
	}
	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	override public void onUpdate() {
	}
	//==========================================================================
	/*!onClose
		@brief	Window Callback
		@retval	true	:ウィンドウを閉じる.
				false	:ウィンドウを閉じるのを抑制する.
	*/
	public override bool onClose(int iCloseInfo) {
		return true;
	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl) {
		switch (cCtrl.id) {
		case BUTTON_BackBack:
			close ();
			CWinQuest cWinQuest = CWindowMgr.Instance.find (CWinQuest.windowId) as CWinQuest;
			if (cWinQuest == null) {
				cWinQuest = CWinQuest.create();
			}
			break;
		}
	}
}
