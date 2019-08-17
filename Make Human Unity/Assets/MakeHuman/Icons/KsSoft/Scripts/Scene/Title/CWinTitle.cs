using UnityEngine;
using System.Collections;
using KS;

public class CWinTitle : CWinTitleBase {
	CBG				m_cBG;
	Vector4			m_uvOffset;
	Color			m_color;
	int				m_angFlush;
	CWinCtrlBase	m_ctlFlush;
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	public override void onCreate() {
		fadespeed = 5f;

		m_cBG = KsSoftUtility.getGameComponent<CBG>("BG") as CBG;
		m_uvOffset = Vector4.zero;
		m_color = new Color(0.5f,0.5f,0.5f,0f);

		m_ctlFlush = find(TEXTURE_TitleFlush);
	}
	//==========================================================================
	/*!startFade
		@brief	フェード処理を司る.
	*/
	override protected void startFade(e_FadeState eState) {
		base.startFade(eState);
		m_bFadeLoaded = true;
	}
	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	public override void onUpdate() {
		m_uvOffset.y -= Time.deltaTime * 0.1f;
		if (m_uvOffset.y <= -1f) {
			m_uvOffset.y = 0f;
		}

		m_angFlush += Angle.Deg2Ang (60f * Time.deltaTime);

		WinColor	wincolor = WinColor.white;
		wincolor.a = (byte) (Mathf.Clamp01 (Angle.Sin (m_angFlush) - 0.9f) * 10f * 255f);
		m_ctlFlush.color = wincolor;

		m_color.a = Mathf.Min (1f,(Angle.Sin (m_angFlush) + 1f) * 0.5f + 0.25f);
		if (m_cBG != null) {
			m_cBG.m_material.SetColor ("_Color",m_color);
			m_cBG.m_material.SetVector("_UVOffset",m_uvOffset);
		}
	}
}
