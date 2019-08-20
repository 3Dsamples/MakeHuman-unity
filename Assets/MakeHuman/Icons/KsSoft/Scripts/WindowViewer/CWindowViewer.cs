//==============================================================================================
/*!ウィンドウビューワ.
	@file  CWindowViewer
*/
//==============================================================================================
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using KS;

public class CWindowViewer : MonoBehaviour {
	public			Vector2	m_listPosition;
	public          SystemLanguage  m_language = SystemLanguage.Unknown;
	public          float   m_width;
	public          float   m_height;
	Vector2         m_guiPosition;
	t_WindowBin[]	m_aBin = new t_WindowBin[0];
	t_WindowBin     m_tWinTest = null;
	CWindowBase		m_cWindow = null;
	const int		szButton = 50;
	//==========================================================================
	/*!Awake
		@brief	Unity Callback
	*/
	void Awake() {
		List<t_WindowBin>	lstBin = new List<t_WindowBin>();
		string[] files = Directory.GetFiles(KsSoftConfig.WindowResourceBinaryPath);

        foreach (string f in files) {
			string file = f.Replace('\\','/');
	        if (!file.EndsWith(".wrb")) continue;
			CReadVariable	cVariable = KsSoftUtility.read(file);
			t_WindowBin	tBin = new t_WindowBin();
			tBin.read (cVariable);
			lstBin.Add(tBin);
		}
		m_aBin = lstBin.ToArray();
		Array.Sort (m_aBin,(a,b) => (a.m_data.m_mId == b.m_data.m_mId)? 0:(a.m_data.m_mId > b.m_data.m_mId)? 1:-1);
		for (int i = 0;i < m_aBin.Length;++i) {
			if (m_aBin[i].m_name == "CWinTest") {
				m_tWinTest = m_aBin[i];
			}
		}
		KsSoftConfig.language = m_language;
	}
	//==========================================================================
	/*!Start
		@brief	Unity Callback
	*/
	IEnumerator Start() {
		while (CSpriteFontMgr.Instance == null) {
			yield return 0;
		}
	}

	//==========================================================================
	/*!OnGUI
		@brief	Unity Callback
	*/
	void OnGUI() {
		CMainSystemBase cSystem = CMainSystemBase.Instance;
		if (cSystem == null || !cSystem.isInitialized) {
			return;
		}
		CWindowMgr winmgr = CWindowMgr.Instance;
		if (winmgr != null) {
			winmgr.setUIResolution(m_width, m_height);
		}
		m_guiPosition = GUI.BeginScrollView(new Rect(m_listPosition.x,m_listPosition.y, 150, 256), m_guiPosition, new Rect(0, 0, 140, m_aBin.Length * szButton));
		int	y = 0;
		foreach (t_WindowBin tBin in m_aBin) {
			string sSelectF,sSelectL;
			uint	mId = tBin.m_data.m_mId;
			if (m_cWindow != null && !m_cWindow.isClose && mId == m_cWindow.id) {
				sSelectF = ">>";
				sSelectL = "<<";
			} else {
				sSelectF = "  ";
				sSelectL = "  ";
			}
			if (GUI.Button(new Rect(0, y, 140, szButton),sSelectF + new MulId(mId).ToString() + sSelectL + "\n" + tBin.m_name)) {
				if (m_cWindow != null && !m_cWindow.isClose) {
					m_cWindow.close();
				}
				// 更新されている可能性があるので読み込みなおす.
				reload(tBin);
				m_cWindow = CWindowMgr.Instance.create<CWindowBase>(tBin.m_data);
				contentsInitialize(m_cWindow.getContents());
			}
			y += szButton;
		}
		GUI.EndScrollView();

		y += 64;
		if (GUI.Button(new Rect(0,300, 140, szButton), "Message\nvalidate")) {
			reload(m_tWinTest);
			m_cWindow = CWindowMgr.Instance.create<CWinTest>(m_tWinTest.m_data);
		}
	}
	t_WindowBin reload(t_WindowBin src) {
		// 更新されている可能性があるので読み込みなおす.
		string  path = KsSoftConfig.WindowResourceBinaryPath + src.m_name + ".wrb";

		FileStream  fs = new FileStream(path,FileMode.Open);
		byte[] buffer = new byte[fs.Length];
		fs.Read(buffer, (int)0, (int)fs.Length);
		fs.Close();
		CReadVariable   cVariable = new CReadVariable(buffer);
		src.read(cVariable);
		return src;
	}
	void contentsInitialize(CWinContents	cContents) {
		if (cContents == null) {
			return;
		}
		for (int i = 0;i < cContents.count;i++) {
			CWinCtrlBase	ctrl = cContents[i];
			if (ctrl == null) {
				continue;
			}
			switch (ctrl.kind) {
			  case e_WinCtrlKind.LISTBOX:
				ctrl.resize(16);
				for (int j = 0;j < ctrl.count;++j) {
					contentsInitialize(ctrl.getContentsFromIndex(j));
				}
				break;
			  case e_WinCtrlKind.LISTBOXEX:
				goto case e_WinCtrlKind.LISTBOX;
			  case e_WinCtrlKind.LOG:
				ctrl.resize(120);
				for (int j = 0;j < 16;j++) {
					CWinCtrlLog	cLog = ctrl as CWinCtrlLog;
					cLog.add("test" + j + ":sentence0 sentence1 sentence2 sentence3 sentence4 sentence5 sentence6 sentence7 sentence8 sentence9 sentence10 sentence11 sentence12 ",WinColor.yellow);
				}
				break;
			  case e_WinCtrlKind.RENDERICON:
				WinColor	backcolor = ctrl.color1;
				backcolor.a = 128;
				ctrl.color1 = backcolor;
				break;
			  case e_WinCtrlKind.RENDER:
				goto case e_WinCtrlKind.RENDERICON;
			}
			contentsInitialize(ctrl.getContents());
		}
	}
};
