//==============================================================================================
/*!Tabbarウィンドウ.
	@file	CWinTabbar
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using KS;

public class CWinTabbar : CWinTabbarBase {
	CWinCtrlCheckbox[]	m_aButton = new CWinCtrlCheckbox[(int) CTown.e_mode.Num];
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	public override void onCreate() {
		m_aButton[(int) CTown.e_mode.Home]		= find<CWinCtrlCheckbox>(RADIO_Home);
		m_aButton[(int) CTown.e_mode.Quest]		= find<CWinCtrlCheckbox>(RADIO_Quest);
		m_aButton[(int) CTown.e_mode.Item]		= find<CWinCtrlCheckbox>(RADIO_Item);
		m_aButton[(int) CTown.e_mode.Gatcha]	= find<CWinCtrlCheckbox>(RADIO_Gatcha);
		m_aButton[(int) CTown.e_mode.Shop]		= find<CWinCtrlCheckbox>(RADIO_Shop);
	}

	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	public override void onUpdate() {
	}
	//==========================================================================
	/*!onYes
		@brief	Window Callback
	*/
	public override void onYes(int msgBoxWinID) {
	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl) {
		CTown	cTown = CTown.Instance;
		if (cTown == null) {
			return;
		}
		switch (cCtrl.id) {
		case RADIO_Home:
			cTown.Mode = CTown.e_mode.Home;
			break;
		case RADIO_Quest:
			cTown.Mode = CTown.e_mode.Quest;
			break;
		case RADIO_Item:
			cTown.Mode = CTown.e_mode.Item;
			break;
		case RADIO_Gatcha:
			cTown.Mode = CTown.e_mode.Gatcha;
			break;
		case RADIO_Shop:
			cTown.Mode = CTown.e_mode.Shop;
			break;
		}
	}
}
