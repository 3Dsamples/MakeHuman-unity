using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using KS;

public class CWinTest : CWinTestBase {
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
		@note	ウィンドウの初期化が終わった直後.
	*/
	public override void onCreate() {
		CWinCtrlLog ctrlLog = find<CWinCtrlLog>(LOG_Test);
		ctrlLog.resize (10);
		for (int i = 0;i < 10;++i) {
			ctrlLog.add ("log test " + i,WinColor.white);
		}
		CWinCtrlListbox	ctrlLB = find<CWinCtrlListbox>(LISTBOX_Test);
		ctrlLB.resize (10);
		CWinCtrlListboxEx	ctrlLBEx = find<CWinCtrlListboxEx>(LISTBOXEX_Test);
		ctrlLBEx.resize (10);
	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl) {
		Debug.Log("onClick " + cCtrl.kind); 
	}
	//==========================================================================
	/*!onHold(特定時間押されっぱなしになったとき呼ばれる).
		@brief	Window Callback
	*/
	public override void onHold(CWinCtrlBase cCtrl) {
		Debug.Log("onHold " + cCtrl.kind); 
	}
	//==========================================================================
	/*!onClickEnter(エディットボックス編集中にエンターキーが押された).
		@brief	Window Callback
	*/
	public override void onClickEnter(CWinCtrlBase cCtrl) {
		Debug.Log("onClickEnter " + cCtrl.kind); 
	}
	//==========================================================================
	/*!onClick for RichText
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl,CRichTextOne cText) {
		Debug.Log("onClick" + cCtrl.kind + " " + cText.cmd); 
	}
	//==========================================================================
	/*!onDrag
		@brief	Window Callback
	*/
	public override void onBeginDrag(CWinCtrlBase cCtrl,Vector2 pos) {
		Debug.Log("onBeginDrag " + cCtrl.kind); 
	}
	//==========================================================================
	/*!onDrag
		@brief	Window Callback
	*/
	public override void onDrag(CWinCtrlBase cCtrl,Vector2 pos,Vector2 dragVelocity) {
		Debug.Log("onDrag " + cCtrl.kind); 
	}
	//==========================================================================
	/*!onDrag
		@brief	Window Callback
	*/
	public override bool onDragRender(CWinCtrlBase cCtrl,Transform transform) {
		Debug.Log("onDragRender " + cCtrl.kind);
		return true;
	}
	//==========================================================================
	/*!onDrop
		@brief	Window Callback
	*/
	public override void onDrop(CWinCtrlBase cCtrl,CWindowBase cDragWindow,CWinCtrlBase cDragCtrl) {
		Debug.Log("onDrop " + cDragCtrl.kind + " to " + cCtrl.kind); 
	}
	//==========================================================================
	/*!onDropGround
		@brief	Window Callback
	*/
	public override void onDropGround(CWinCtrlBase cCtrl) {
		Debug.Log("onDropGround " + cCtrl.kind); 
	}
};

