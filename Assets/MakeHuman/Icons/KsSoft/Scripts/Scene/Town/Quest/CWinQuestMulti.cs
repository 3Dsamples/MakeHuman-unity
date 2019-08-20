using UnityEngine;
using System.Collections;
using KS;

public class CWinQuestMulti : CWinQuestMultiBase {
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	override public void onCreate() {
	}
	override public bool onClose(int iCloseInfo) {
		return true;
	}
	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	override public void onUpdate() {
	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl) {
		switch (cCtrl.id) {
		case BUTTON_JoinWithKey:
			break;
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
