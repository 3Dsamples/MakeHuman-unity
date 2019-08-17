using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KS;

public class CWinProfile : CWinProfileBase {
	protected const int			ACTIVE_EQUIP_SET_NUM = 3;
	protected CProfile			m_profile		= null;
	protected CWinCtrlEditbox	m_ctrlIntroduction;
	protected CWinCtrlBase		m_ctrlGuild;
	protected CWinCtrlRender	m_ctrlRender;
	protected CWinCtrlListbox	m_lbEquipList;
	protected int				m_idxEquipSet = -1;
	protected int				m_iEquipSet = 0;

	protected	GameObject[]		m_aAvatar = new GameObject[3];
	protected	Vector3[]			m_aPosAvatar = new Vector3[3];
	protected	uint[,]	aCtlID = new uint[3,2] {
		{ICON_EquipWeapon0,ICON_EquipChar0},
		{ICON_EquipWeapon1,ICON_EquipChar1},
		{ICON_EquipWeapon2,ICON_EquipChar2}
	};
	static uint[] m_aEquipFrame = new uint[] {
		CWinProfileBase.FRAME_EquipFrame0,
		CWinProfileBase.FRAME_EquipFrame1,
		CWinProfileBase.FRAME_EquipFrame2,
	};
	//==========================================================================
	/*!create
		@brief	create
	*/
	static public CWinProfile create(ulong uAUID,CWindowBase cParent = null) {
		if (uAUID == 0) {
			return null;
		}
		CWinProfile	cWin = create (cParent);
		if (cWin != null) {
			CProfile cProfile = CProfileMgr.Instance.getProfile(uAUID);
			cWin.Profile = cProfile;
			cWin.setIntroduction(cProfile.Introduction);
		}
		return cWin;
	}	
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	public override void onCreate() {
		m_lbEquipList			= find (LISTBOX_EquipList) as CWinCtrlListbox;
		m_ctrlIntroduction		= find (EDITBOX_IntroductionContents) as CWinCtrlEditbox;
		m_ctrlGuild				= find (BUTTON_Guild);
		m_ctrlRender			= find (RENDER_AVATAR) as CWinCtrlRender;

		// introduction
		m_ctrlIntroduction = find (EDITBOX_IntroductionContents) as CWinCtrlEditbox;
		m_ctrlIntroduction.editable = false;
		find (TEXT_IntroductionNote).hide = true;

		// level meter
		find (METER_Exp).hide = find (TEXT_Exp).hide = true;

		// deck info.
		find (TEXT_DeckInfo).hide = true;

		// カメラを反映させる.
		Camera	camera = m_ctrlRender.camera;
		camera.cullingMask = (int) e_Layer.Profile;
		camera.transform.position = new Vector3(0f,0.85f,0f);
		camera.transform.rotation = Quaternion.Euler(new Vector3(0f,180f,0f));
		// シェイプを準備する.	
		for (int i = 0;i < 3;++i) {
			GameObject	go = Instantiate(Resources.Load<GameObject>("Prefab/" + new MulId(1,(uint) (i + 1),1) + ".mdl"));
			go.transform.position = new Vector3(-1f + (float) i,0,-2.5f);
			KsSoftUtility.setLayer(go,e_LayerId.Profile);
			m_aAvatar[i] = go;
		}
		EquipSetIndex = 0;
	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl) {
		switch (cCtrl.id) {
		case BUTTON_Close:
			close();
			break;
		case BUTTON_Guild:
			break;
		case ICON_EquipWeapon0:
			break;
		case TEXTURE_EquipWeapon0:
			goto case ICON_EquipWeapon0;
		case ICON_EquipWeapon1:
			goto case ICON_EquipWeapon0;
		case TEXTURE_EquipWeapon1:
			goto case ICON_EquipWeapon1;
		case ICON_EquipWeapon2:
			goto case ICON_EquipWeapon0;
		case TEXTURE_EquipWeapon2:
			goto case ICON_EquipWeapon2;
		case ICON_EquipChar0:
			break;
		case TEXTURE_EquipChar0:
			goto case ICON_EquipChar0;
		case ICON_EquipChar1:
			goto case ICON_EquipChar0;
		case TEXTURE_EquipChar1:
			goto case ICON_EquipChar1;
		case ICON_EquipChar2:
			goto case ICON_EquipChar0;
		case TEXTURE_EquipChar2:
			goto case ICON_EquipChar2;
		}
	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onHold(CWinCtrlBase cCtrl) {
		switch (cCtrl.id) {
		  case ICON_EquipWeapon0:
			break;
		  case ICON_EquipWeapon1:
			break;
		  case ICON_EquipWeapon2:
			break;
		  case ICON_EquipChar0:
			break;
		  case ICON_EquipChar1:
			break;
		  case ICON_EquipChar2:
			break;
		}
	}
	//==========================================================================
	/*!アイテムプロパティを開く.
		@brief	openItemProperty
	*/
	protected void openItemProperty() {
	}
	//==========================================================================
	/*!onClose
		@brief	Window Callback
	*/
	public override bool onClose (int iCloseInfo) {
		for (int i = 0;i < m_aAvatar.Length;++i) {
			GameObject go = m_aAvatar[i];
			if (go != null) {
				GameObject.DestroyImmediate(go);
			}
		}
		return true;
	}
	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	public override void onUpdate() {
		// 自己紹介.
		m_ctrlIntroduction.caption = m_profile.Introduction;

		updateAvatarPos();

		// Iconを設定.
		m_lbEquipList.resize(3);
		for (int iEquipSet = 0;iEquipSet < 3;++iEquipSet) {
			CWinContents	cContents = m_lbEquipList.getContentsFromIndex(iEquipSet);
			for (int idxEquipSet = 0;idxEquipSet < 3;++idxEquipSet) {
				CWinCtrlIcon	cWeapon = cContents.find<CWinCtrlIcon>(aCtlID[idxEquipSet,0]);
				cWeapon.partId1 = m_profile.weaponIcons[idxEquipSet];
				cWeapon.setTextureId(1,MulId.Id (100,100,0));
				if (cWeapon.partId1 != 0) {
					cWeapon.hide = false;
				} else {
					cWeapon.hide = true;
				}

				CWinCtrlIcon	cAvatar = cContents.find<CWinCtrlIcon>(aCtlID[idxEquipSet,1]);
				cAvatar.partId1 = m_profile.avatarIcons[idxEquipSet];
				cAvatar.setTextureId(1,MulId.Id (100,100,0));
				if (cAvatar.partId1 != 0) {
					cAvatar.hide = false;
				} else {
					cAvatar.hide = true;
				}
			}
		}
	}
	//==========================================================================
	/*!Avatarの位置情報を更新する.
		@brief	updateAvatarPos
	*/
	protected void updateAvatarPos() {
		float	spd = Time.deltaTime * 2f;
		for (int i = 0;i < m_aAvatar.Length;++i) {
			GameObject	go = m_aAvatar[i];
			if (go == null) {
				continue;
			}
			Vector3 posTarget = m_aPosAvatar[i];
			Vector3	delta = posTarget - go.transform.position;

			if ( delta.sqrMagnitude < spd * spd) {
				go.transform.position = posTarget;
			} else {
				go.transform.position = go.transform.position + delta.normalized * spd; 
			}
		}
	}
	//==========================================================================
	/*!onDrag
		@brief	Window Callback
	*/
	public override void onDrag(CWinCtrlBase cCtrl,Vector2 pos,Vector2 dragVelocity) {
		if (cCtrl.id != LISTBOX_EquipList) {
			return;
		}
		// リーダーにフォーカス.
		EquipSetIndex = 0;
		//   Deck change.
		changeEquipSet(dragVelocity.x);
	}
	//==========================================================================
	/*!装備セットを切り替える.
		@brief	changeEquipSet
	*/
	protected void changeEquipSet(float dx) {
		if (m_lbEquipList.isSmoothScrolled) {
			return;
		}
		int	iSelect = m_iEquipSet;
		if (dx < 0f) {
			++iSelect;
			if (iSelect >= 3) {
				return;
			}
		} else if (dx > 0f) {
			--iSelect;
			if (iSelect < 0) {
				return;
			}
		}
		if (iSelect == m_iEquipSet) {
			return;
		}
		m_iEquipSet = iSelect;
		m_lbEquipList.setSmoothOffset(m_lbEquipList.getContentsOffset(m_iEquipSet,e_Anchor.LeftTop),0.3f);
	}
	protected int EquipSetIndex {
		get {
			return m_idxEquipSet;
		}
		set {
			CWinContents cContents = m_lbEquipList.getContentsFromIndex(m_iEquipSet);
			if (cContents != null) {
				for (int i = 0;i < m_aEquipFrame.Length;++i) {
					CWinCtrlBase	cCtrl = cContents.find (m_aEquipFrame[i]);
					if (value == i) {
						cCtrl.color = new WinColor(255,255,0,255);
					} else {
						cCtrl.color = new WinColor(128,128,128,120);
					}
				}
			}
			m_idxEquipSet = value;
			//アバター表示位置を設定.
			m_aPosAvatar[0] = new Vector3(1f,0f,-2.5f);
			m_aPosAvatar[1] = new Vector3(1f,0f,-2.5f);
			m_aPosAvatar[2] = new Vector3(-1f,0f,-2.5f);
			if (value == 2) {
				m_aPosAvatar[1] = m_aPosAvatar[2];
			}
			m_aPosAvatar[value] = new Vector3(0f,0f,-1.5f);
		}
	}
	public CProfile Profile {
		get {
			return m_profile;
		}
		set {
			if (value == null) {
				return;
			}
			m_profile = value;
		}
	}
	public ulong auid {
		get {
			if (m_profile == null) {
				return 0;
			}
			return m_profile.AUID;
		}
	}
	public void setIntroduction(string introduction) {
		m_ctrlIntroduction.caption = KsSoftUtility.stripslashes(introduction);
	}
}
