//==============================================================================================
/*!全アセットバンドル読み込み.
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using KS;
public class CWinAssetbundleLoading : CWinAssetbundleLoadingBase {
	Camera		m_camera;
	CWinCtrlMeter	m_ctlProgressTotal;
	CWinCtrlMeter	m_ctlProgressPart;
	CWinCtrlBase	m_ctrlMessage;
	int				m_maxLoadNum;
	int				m_loadNum;

	CWinCtrlBase	m_ctlWait;
	FiveCC			m_fcWait;
	float			m_fAnimation = 0;
	bool			m_bReady;
	//==========================================================================
	/*!Awake
		@brief	Unity Callback
	*/
	new void Awake() {
		CWindowMgr.Instance.closeWindow(e_Layer.ProgressBar|e_Layer.SystemWindow);
		base.Awake();
		GameObject	go = new GameObject("progress bar camera");
		
		m_camera = KsSoftUtility.addUICamera(go,(e_Layer) (1 << (int) e_LayerId.ProgressBar),3f);
		go.transform.position = new Vector3(0f,0f,-100f);
		layer = e_LayerId.ProgressBar;

		m_maxLoadNum = 0;
		m_loadNum = 0;
		m_bReady = false;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

	}
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	public override void onCreate() {
		m_ctlProgressTotal = find (METER_ProgressTotal) as CWinCtrlMeter;
		m_ctlProgressTotal.setMeter (1,1f,0f);
		m_ctlProgressPart = find (METER_ProgressPart) as CWinCtrlMeter;
		m_ctlProgressPart.setMeter (1,1f,0f);
		m_ctlWait = find (TEXTURE_Wait);
		m_ctrlMessage = find (TEXT_Message);
		m_fcWait = new FiveCC(m_ctlWait.partId);

		message = "Now Loading...";
	}
	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	public override void onUpdate() {
		CAssetBundleMgr	cAssetBundleMgr = CAssetBundleMgr.Instance;		
		m_loadNum = 0;
		int	iMgr = 0;

		if (cAssetBundleMgr != null) {
			if (cAssetBundleMgr.lastError < 0) {
				message = cAssetBundleMgr.errormessage + "(" + cAssetBundleMgr.lastError + "):" + KsSoftConfig.httpserver;
				return;
			}
			m_loadNum += cAssetBundleMgr.loadNum;
			iMgr++;
		}
		m_maxLoadNum = Math.Max (m_loadNum,m_maxLoadNum);

		float	k = 0f;
		if (m_maxLoadNum > 0) {
			k = (float) (m_maxLoadNum - m_loadNum)/(float) m_maxLoadNum;
		}
		m_ctlProgressTotal.setMeter(0,k);
		if (cAssetBundleMgr != null) {
			CAssetBundle[] aAB = cAssetBundleMgr.loadings;
			if (aAB == null || aAB.Length == 0) {
				m_ctlProgressPart.setMeter(0,1f);
			} else {
				m_ctlProgressPart.setMeter(0,aAB[0].progress);
			}
		}
		if (m_camera != null) {
			m_camera.orthographicSize = Screen.height/2;
		}
		// パターンアニメーション.
		m_fAnimation += Time.deltaTime * 8f;
		if (m_fAnimation >= 12f) {
			m_fAnimation %= 12f;
		}
		int	index = (int) m_fAnimation;
		m_fcWait[2] = (char) ('0' + (index / 10));
		m_fcWait[3] = (char) ('0' + (index % 10));
		m_ctlWait.partId = m_fcWait;
	}
	//==========================================================================
	/*!onClose
		@brief	Window Callback
	*/
	public override bool onClose (int iCloseInfo) {
		if (m_camera != null) {
			DestroyImmediate(m_camera.gameObject,true);
			m_camera = null;
		}
		Screen.sleepTimeout = SleepTimeout.SystemSetting;
		return true;
	}
	//==========================================================================
	/*!ロード中のカウンタをリセットする.
		@brief	reset
	*/
	public void reset() {
		m_maxLoadNum = 0;
	}
	public bool isFinish {
		get {
			if (m_bReady && m_loadNum == 0) {
				return true;
			}
			return false;
		}
	}
	public string message {
		get {
			return m_ctrlMessage.caption;
		}
		set {
			m_ctrlMessage.caption = value;
		}
	}
}
