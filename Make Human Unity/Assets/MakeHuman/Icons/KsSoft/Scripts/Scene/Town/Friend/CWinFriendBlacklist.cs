using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using KS;

public class CWinFriendBlacklist : CWinFriendBlacklistBase {
	CProfile[]	m_aBlacklist;
	// Sort state for Friend.
	enum e_SortState {
		NameAsc,
		LevelDesc,
	}
	e_SortState m_eSortState = e_SortState.NameAsc;
	// ctrl
	CWinCtrlBase	m_ctrlBlacklist;
	CWinCtrlBase	m_ctrlSearchBox;
	CWinCtrlBase	m_ctrlNotFound;
	//==========================================================================
	/*!onCreate
		@brief	Window Callback
	*/
	public override void onCreate() {
		m_ctrlBlacklist	= find (LISTBOX_Blacklist);
		m_ctrlSearchBox	= find(EDITBOX_BlacklistSearchBox);
		find (BUTTON_Sort).caption = getCaption(MulId.Id(0,0,110));
		refresh();
	}
	//==========================================================================
	/*!onUpdate
		@brief	Window Callback
	*/
	public override void onUpdate() {
		CFriend[] aFriends = CFriendMgr.Instance.getFriends();
		m_aBlacklist = new CProfile[aFriends.Length];
		for (int i = 0;i < m_aBlacklist.Length;++i) {
			m_aBlacklist[i] = aFriends[i].Profile;
		}
		m_ctrlBlacklist.resize(m_aBlacklist.Length);
		// not found
		if (m_aBlacklist.Length <= 0) {
			return;
		}
		// list
		for (int i = 0;i < m_aBlacklist.Length;i++) {
			CProfile profile = m_aBlacklist[i];
			if (profile == null) {
				continue;
			}
			CWinContents contents = m_ctrlBlacklist.getContentsFromIndex(i);
			profile.setContents(contents,ICON_BlacklistIcon,TEXT_BlacklistName,TEXTURE_BlacklistConnect,TEXTURE_BlacklistPlace,0,TEXT_BlacklistGuild,TEXTURE_BlacklistGuild,TEXT_BlacklistQuest,0,0,0);
		}
	}
	//==========================================================================
	/*!onClick
		@brief	Window Callback
	*/
	public override void onClick(CWinCtrlBase cCtrl) {
		switch (cCtrl.id) {
		  case BUTTON_BlacklistCancel:
			break;
		  case ICON_BlacklistIcon:
			{
				int index = m_ctrlBlacklist.getContentsIndex(cCtrl);
				openFriendProfile(index);
			}
			break;
		  case BUTTON_Sort:
			{
				CWinCtrlBase ctrlSort = find (BUTTON_Sort);
				if (m_eSortState == e_SortState.LevelDesc) {
					// name sort
					m_eSortState = e_SortState.NameAsc;
					ctrlSort.caption = getCaption(MulId.Id(0,0,110));
				} else if (m_eSortState == e_SortState.NameAsc) {
					// date sort
					m_eSortState = e_SortState.LevelDesc;
					ctrlSort.caption = getCaption(MulId.Id(0,0,120));
				}
				refresh();
			}
			break;
		}
	}
	public override void onHold(CWinCtrlBase cCtrl) {
		switch (cCtrl.id) {
		case ICON_BlacklistIcon:
			{
				int index = m_ctrlBlacklist.getContentsIndex(cCtrl);
				CProfile profile = m_aBlacklist[index];
				CWinProfile.create(profile.AUID, this);
			}
			break;
		}
	}
	//==========================================================================
	/*!openFriendProfile
		@brief	openFriendProfile
	*/
	private void openFriendProfile(int index) {
		CProfile profile = m_aBlacklist[index];
		CWinProfile.create(profile.AUID,this);
	}
	//==========================================================================
	/*!onClickEnter
		@brief	onClickEnter
	*/
	public override void onClickEnter(CWinCtrlBase cCtrl) {
		if (cCtrl != null && cCtrl == m_ctrlSearchBox) {
			string str = cCtrl.caption.Trim();
			if (KsSoftUtility.isInvalidSQL(str)) {
				// invalid character specified
				CMessageBox.create(MulId.Id(2,0,30), CMessageBox.e_Type.Ok, 0, this);
			}
		}
	}
	//==========================================================================
	/*!refresh
		@brief	refresh
	*/
	public void refresh() {
		CFriend[] aFriends = CFriendMgr.Instance.getFriends();
		m_aBlacklist = new CProfile[aFriends.Length];
		for (int i = 0;i < m_aBlacklist.Length;++i) {
			m_aBlacklist[i] = aFriends[i].Profile;
		}
		// sort.
		switch (m_eSortState) {
		case e_SortState.LevelDesc:
			Array.Sort (m_aBlacklist,delegate(CProfile p0, CProfile p1) { return (int)(p1.Level - p0.Level); });
			break;
		case e_SortState.NameAsc:
			Array.Sort (m_aBlacklist,delegate(CProfile p0, CProfile p1) { return string.Compare(p0.Name, p1.Name); });
			break;
		}
	}
}
