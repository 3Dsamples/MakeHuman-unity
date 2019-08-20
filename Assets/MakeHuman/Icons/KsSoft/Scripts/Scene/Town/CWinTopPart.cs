//==============================================================================================
/*!TopPartウィンドウ.
	@file	CWinTopPart
	
	(counter SJIS string 京.)
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
using KS;

public class CWinTopPart : CWinTopPartBase {
	public enum e_InvitationMode {
		None,
		Party,
		Guild
	};
	struct t_PartyInfo {
		public		CWinCtrlRenderIcon	m_icon;
		public		GameObject			m_go;
		public		Animation			m_anim;
		public		AnimationState		m_animState;
		public		float				m_time;
		public		CProfile			m_profile;
	};
	t_PartyInfo[]		m_aPartyInfo = new t_PartyInfo[3];


	CWinCtrlBase		m_ctrlName;
	CWinCtrlBase		m_ctrlLevel;
	CWinCtrlButton		m_ctrlBalloon;
	CWinChat			m_cWinChat;
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	public override void onCreate() {
		m_ctrlName		= find (TEXT_Name);
		m_ctrlLevel		= find (TEXT_Level);

		for (int i = 0;i < m_aPartyInfo.Length;++i) {
			CWinCtrlRenderIcon	icon = find<CWinCtrlRenderIcon>(RENDERICON_Party0 + (uint)(10*i));
			m_aPartyInfo[i].m_icon = icon;
			m_aPartyInfo[i].m_profile = CProfileMgr.Instance.getProfile((ulong) (4 + i));

			icon.regionId = m_aPartyInfo[i].m_profile.AUID;
			// カメラを反映させる.
			Camera	camera = icon.camera;
			camera.cullingMask = (int) e_Layer.RenderIcon;
			camera.transform.position = new Vector3(0f,0.85f,0f);
			camera.transform.rotation = Quaternion.Euler(new Vector3(0f,180f,0f));
			// シェイプを準備する.	
			GameObject	go = Instantiate(Resources.Load<GameObject>("Prefab/" + new MulId(1,(uint) (i + 1),1) + ".mdl"));
			go.transform.position = new Vector3(0f,0f,-2.5f);
			KsSoftUtility.setLayer(go,e_LayerId.RenderIcon);
			m_aPartyInfo[i].m_go = go;
			m_aPartyInfo[i].m_anim = go.GetComponent<Animation>();
			m_aPartyInfo[i].m_animState = m_aPartyInfo[i].m_anim[m_aPartyInfo[i].m_anim.clip.name];
			go.SetActive(false);
		}

		m_ctrlName.caption	= CProfileMgr.Instance.own.Name;
		
		performDefaultAction();
		// M & C
		refresh();
		
		// create chat window
		m_cWinChat = CWinChat.create ();
	}
	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	public override void onUpdate() {
	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl) {
		switch (cCtrl.id) {
		case ICON_Icon:
		{
			// Profile window
			ulong uAUID = CProfileMgr.Instance.own.AUID;
			CWinProfile.create (uAUID,this);
		}
			break;
		case RENDERICON_Party0:
			break;
		case RENDERICON_Party1:
			goto case RENDERICON_Party0;
		case RENDERICON_Party2:
			goto case RENDERICON_Party0;
		case TEXTURE_BG:
			break;
		}
	}
	//==========================================================================
	/*!onBeginRenderIcon
		@brief	レンダーアイコンにレンダリングを要求された.
		@retval	true	:レンダリングに成功/onEndRederIconが後で呼ばれる.
				false	:レンダリングに失敗.
	*/
	public override bool onBeginRenderIcon(CWinCtrlRenderIcon	cCtrl) {
		int	idx = 0;
		switch (cCtrl.id) {
		case RENDERICON_Party0:
			idx = 0;
			break;
		case RENDERICON_Party1:
			idx = 1;
			break;
		case RENDERICON_Party2:
			idx = 2;
			break;
		default:
			return false;
		}
		GameObject	go = m_aPartyInfo[idx].m_go;
		go.SetActive(true);
		m_aPartyInfo[idx].m_animState.time = m_aPartyInfo[idx].m_time;
		m_aPartyInfo[idx].m_time += Time.deltaTime;
		if (m_aPartyInfo[idx].m_time >= m_aPartyInfo[idx].m_animState.length) {
			m_aPartyInfo[idx].m_time -= m_aPartyInfo[idx].m_animState.length;
		}
		m_aPartyInfo[idx].m_anim.Sample();
		return true;
	}
	//==========================================================================
	/*!onEndRenderIcon
		@brief	アイコンへのレンダリング後に呼ばれる.
		@retval	true	:次フレームも続けてレンダリングするときは、trueを返す.
				false	:次フレームはレンダリングをせず、今フレームでレンダリングした結果を使う.
	*/
	public override bool onEndRenderIcon(CWinCtrlRenderIcon	cCtrl) {
		int	idx = 0;
		switch (cCtrl.id) {
		case RENDERICON_Party0:
			idx = 0;
			break;
		case RENDERICON_Party1:
			idx = 1;
			break;
		case RENDERICON_Party2:
			idx = 2;
			break;
		default:
			return false;
		}
		m_aPartyInfo[idx].m_go.SetActive(false);
		return false;
	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override bool onClose(int iCloseInfo) {
		if (m_cWinChat) {
			m_cWinChat.close (0);
		}
		for (int i = 0;i < m_aPartyInfo.Length;++i) {
			GameObject.DestroyImmediate(m_aPartyInfo[i].m_go);
		}
		return true;
	}
	//==========================================================================
	/*!performDefaultAction
		@brief	default action
	*/
	private void performDefaultAction() {
		CWinHome.create();
	}
	public void refresh() {
		CProfile	cOwn = CProfileMgr.Instance.own;
		if (cOwn == null) {
			return;
		}
		m_ctrlLevel.caption		= "Lv. " + (cOwn.Level+1);
		// profile
		CWinCtrlIcon	cIcon = find (ICON_Icon) as CWinCtrlIcon;
		cOwn.setAvatarIcon(cIcon);
		// icon BG.
		cIcon.setTextureId(0,MulId.Id(100,100,0));
		cIcon.setPartId(0,new FiveCC("ICNBG"));
		cIcon.setTextureId(0,MulId.Id(100,100,0));
		cIcon.setPartId(1,cOwn.iconId);

		CWinCtrlText	cText = find (TEXT_Name) as CWinCtrlText;
		cText.caption = cOwn.Name;
	}
}
