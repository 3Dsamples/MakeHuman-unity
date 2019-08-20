//==============================================================================================
/*!Homeウィンドウ.
	@file	CWinHome
		
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using KS;

public class CWinHome : CWinHomeBase {
	CWinCtrlRender	m_ctrlRender;
	CTown			m_cTown;
	GameObject		m_goAvatar;

	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	override public void onCreate() {
		m_ctrlRender = find<CWinCtrlRender>(RENDER_DECK);

		m_cTown		= CTown.Instance;

		// カメラを反映させる.
		Camera	camera = m_ctrlRender.camera;
		camera.cullingMask = (int) e_Layer.Home;
		camera.orthographic = true;
		camera.orthographicSize = 1.3f;
		camera.transform.position = new Vector3(0f,1.3f,0f);
		camera.transform.rotation = Quaternion.Euler(new Vector3(0f,180f,0f));
		// シェイプを準備する.
		setAvatar(1);
	}
	//==========================================================================
	/*!setAvatar
		@brief アバターを設定する.
	*/
	void setAvatar(uint idx) {
		if (m_goAvatar != null) {
			GameObject.DestroyImmediate(m_goAvatar);
		}
		m_goAvatar = Instantiate(Resources.Load<GameObject>("Prefab/" + new MulId(1,idx,1) + ".mdl"));
		KsSoftUtility.setLayer(m_goAvatar,e_LayerId.Home);
		m_goAvatar.transform.position = new Vector3(0f,0f,-2.5f);
	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl) {
		switch (cCtrl.id) {
		case BUTTON_Guild:
			m_cTown.Mode = CTown.e_mode.Guild;
			break;
		case BUTTON_Friend:
			m_cTown.Mode = CTown.e_mode.Friend;
			break;
		case BUTTON_Present:
			CMessageBox.create("Sorry...",CMessageBox.e_Type.Ok,0,this);
			break;
		case BUTTON_Info:
			CMessageBox.create("Sorry...",CMessageBox.e_Type.Ok,0,this);
			break;
		case BUTTON_Settings:
			CMessageBox.create ("Sorry...", CMessageBox.e_Type.Ok,0,this);
			break;
		case TEXTURE_Present:
			CMessageBox.create ("Sorry...", CMessageBox.e_Type.Ok,0,this);
			break;
		}
	}
	//==========================================================================
	/*!onClose
		@brief	Window Callback
	*/
	public override bool onClose (int iCloseInfo) {
		if (m_goAvatar != null) {
			GameObject.DestroyImmediate(m_goAvatar);
		}
		return true;
	}
	//==========================================================================
	/*!onDrop
		@brief	Window Callback
	*/
	public override void onDrop(CWinCtrlBase cCtrl,CWindowBase cDragWindow,CWinCtrlBase cDragCtrl) {
		if (cDragWindow.id != CWinTopPart.windowId) {
			return;
		}
		uint idx = 0;
		switch (cDragCtrl.id) {
		case CWinTopPart.RENDERICON_Party0:
			idx = 1;
			break;
		case CWinTopPart.RENDERICON_Party1:
			idx = 2;
			break;
		case CWinTopPart.RENDERICON_Party2:
			idx = 3;
			break;
		default:
			return;
		}
		setAvatar(idx);
	}
}
