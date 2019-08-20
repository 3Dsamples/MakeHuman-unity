using UnityEngine;
using System.Collections;
using KS;

public class CWinQuest : CWinQuestBase {
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	override public void onCreate() {
		// refresh
		refresh();
	}
	override public bool onClose(int iCloseInfo) {
		return true;
	}
	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	override public void onUpdate() {
		if (isClose) {
			return;
		}
	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl) {
		switch (cCtrl.id) {
		case BUTTON_Solo:
			close ();
			CWinQuestStory.create();
			break;
		case BUTTON_Multi:
			close ();
			CWinQuestMulti.create();
			break;
		case BUTTON_Event:
			close ();
			// TODO:
			break;
		}
	}
	//==========================================================================
	/*!refresh
		@brief	refresh
	*/
	public void refresh () {
	}
}
