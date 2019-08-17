//==============================================================================================
/*!Nowローディングダイアログシステムウィンドウ.
	@file	CWinWait

	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using KS;
public class CWinWait : CWinWaitBase {
	CWinCtrlBase	m_cCaption;
	CWinCtrlBase	m_cAnimation;
	CWinCtrlBase    m_cTips;
	FiveCC			m_fcAnim;
	float			m_fAnimation = 0;
	Camera			m_camera = null;
	//==========================================================================
	/*!create
		@brief	create CWinWait
	*/
	static public CWinWait create(string msg = null) {
		CWinWait	ww = CWinWaitBase.create ();
		if (msg != null) {
			ww.caption = msg;
		}
		return ww;
	}
	//==========================================================================
	/*!destroy
		@brief	destroy CWinWait
	*/
	static public void destroy() {
		CWindowMgr	cWindowMgr = CWindowMgr.Instance;
		if (cWindowMgr == null) {
			return;
		}
		CWindowBase	cWin = cWindowMgr.find(CWinWait.windowId);
		if (cWin != null && !cWin.isClose) {
			cWin.close ();
		}
	}
	//==========================================================================
	/*!Awake
		@brief	Unity Callback
	*/
	new void Awake() {
		CWindowMgr.Instance.closeWindow(e_Layer.SystemWindow);
		base.Awake();
		GameObject	go = new GameObject("wait window camera");

		m_camera = KsSoftUtility.addUICamera(go,(e_Layer) (1 << (int) e_LayerId.SystemWindow),3f);
		go.transform.position = new Vector3(0f,0f,-100f);
		layer = e_LayerId.SystemWindow;
	}
	//==========================================================================
	/*!OnDestroy
		@brief	Unity Callback
	*/
	public override bool onClose (int iCloseInfo) {
		if (m_camera != null) {
			DestroyImmediate(m_camera.gameObject,true);
			m_camera = null;
		}
		return true;
	}
	//==========================================================================
	/*!doFade(1.5sec程、ウィンドウが開くのを遅らせる).
		@brief	フェード処理を司る.
	*/
	/*
		override protected bool doFade(e_FadeState eState) {
			if (eState == e_FadeState.None) {
				return false;
			}
			if (m_fWait <= 0f) {
				return base.doFade (eState);
			}
			m_fWait -= Time.deltaTime;
			m_aColor[0].a = 0;
			return true;
		}
		*/
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	public override void onCreate() {
		m_cCaption = find (RICHTEXT_MESSAGE);
		if (!string.IsNullOrEmpty(base.caption)) {
			m_cCaption.caption = base.caption;
		}

		m_cAnimation = find (TEXTURE_Wait);
		m_cTips = find(RICHTEXT_TIPS);
		m_fcAnim = new FiveCC(m_cAnimation.partId);
	}
	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	public override void onUpdate() {
		m_fAnimation += Time.deltaTime * 8f;
		if (m_fAnimation >= 12f) {
			m_fAnimation %= 12f;
		}
		int	index = (int) m_fAnimation;
		m_fcAnim[2] = (char) ((index / 10) + '0');
		m_fcAnim[3] = (char) ((index % 10) + '0');

		m_cAnimation.partId = m_fcAnim;
		if (m_camera != null && CMainSystemBase.Instance != null && CMainSystemBase.Instance.uicamera != null) {
			m_camera.orthographicSize = CMainSystemBase.Instance.uicamera.orthographicSize;
		}
	}
	//==========================================================================
	/*!キャプション.
		@brief	caption
	*/
	override public string caption {
		get {
			if (m_cCaption == null) {
				return base.caption;
			}
			return m_cCaption.caption;
		}
		set {
			if (m_cCaption != null) {
				m_cCaption.caption = value;
			}
			base.caption = value;
		}
	}
	public string tips {
		set {
			if (value == null) {
				m_cTips.caption = "";
				return;
			}
			if (m_cTips != null) {
				m_cTips.caption = value;
			}
		}
	}
}
