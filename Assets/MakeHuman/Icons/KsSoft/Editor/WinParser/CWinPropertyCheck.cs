//==============================================================================================
/*!CWinPropertyCheck
	@brief	CWinPropertyCheck
	@note
*/
//==============================================================================================
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using CWinParser;
using KS;

namespace CWinParser {
	public class CWinPropertyCheck {
		static Dictionary<Tokens,e_WinCtrlStyle> m_dcStyle = new Dictionary<Tokens,e_WinCtrlStyle>();
		static Dictionary<e_WinCtrlKind,Tokens[]> m_dcInvalidStyle = new Dictionary<e_WinCtrlKind,Tokens[]>();
		static Dictionary<e_WinCtrlKind,byte[]> m_dcInvalidProperty = new Dictionary<e_WinCtrlKind, byte[]>();
		static List<e_WinCtrlKind> m_lstNohitCtrl = new List<e_WinCtrlKind>();
		static CWinPropertyCheck() {
			initializeInvalidStyle();
			initializeInvalidProperty();
			initializeNohitDefaultCtrl();
		}
		internal static bool check(e_WinCtrlKind eKind, e_WinProperty eProp, Scanner scanner) {
			byte[]  aIgnore;
			if (!m_dcInvalidProperty.TryGetValue(eKind, out aIgnore)) {
				yyerror("invalid ctrl kind:" + eKind);
				return false;
			}
			if (aIgnore == null) {
				yyerror("not implement ctrl kind:" + eKind);
				return false;
			}
			int index = (int) eProp;
			if ((aIgnore[index >> 3] & (1 << (index & 7))) != 0) {
				yywarning("invalid property " + eProp + " in " + eKind);
				return false;
			}
			return true;
		}
		internal static e_WinCtrlStyle check(e_WinCtrlKind eKind, Tokens[] aTokens,Scanner scanner) {
			e_WinCtrlStyle  eResult = e_WinCtrlStyle.NONE;
			Tokens[]    aInvalidTokens;
			if (!m_dcInvalidStyle.TryGetValue(eKind, out aInvalidTokens)) {
				yyerror("invalid ctrl kind:" + eKind);
				return eResult;
			}
			foreach (Tokens tk in aTokens) {
				e_WinCtrlStyle  eStyle;
				if (!m_dcStyle.TryGetValue(tk, out eStyle)) {
					yywarning("can't use style flag:" + tk + " in " + eKind);
					continue;
				}
				if (eStyle == e_WinCtrlStyle.NOHIT) {
					if (m_lstNohitCtrl.Contains(eKind)) {
						yywarning("this value is default value:" + tk + "in " + eKind);
						break;
					}
				}
				if (eStyle == e_WinCtrlStyle.HIT) {
					if (!m_lstNohitCtrl.Contains(eKind)) {
						yywarning("this value is default value:" + tk + " in " + eKind);
						break;
					}
				}
				if (System.Array.IndexOf(aInvalidTokens, tk) >= 0) {
					yywarning("can't use style flag:" + tk + " in " + eKind);
					continue;
				}
				eResult |= eStyle;
			}
			return eResult;
		}
		static void yyerror(string err) {
			Debug.LogError(err);
		}
		static void yywarning(string war) {
			Debug.LogWarning(war);
		}
		//==============================================================================================
		/*!NOHITがデフォルトのコントロール.
			@note
		*/
		//==============================================================================================
		static void initializeNohitDefaultCtrl() {
			m_lstNohitCtrl.Add(e_WinCtrlKind.TEXT);
			m_lstNohitCtrl.Add(e_WinCtrlKind.LOGTEXT);
			m_lstNohitCtrl.Add(e_WinCtrlKind.SCROLLBAR);
			m_lstNohitCtrl.Add(e_WinCtrlKind.LINE);
			m_lstNohitCtrl.Add(e_WinCtrlKind.TEXTURE);
			m_lstNohitCtrl.Add(e_WinCtrlKind.LABEL);
			m_lstNohitCtrl.Add(e_WinCtrlKind.METER);
			m_lstNohitCtrl.Add(e_WinCtrlKind.CANVAS);
		}
		//==============================================================================================
		/*!設定しても無効なスタイル.
			@brief	initializeInvalidStyle
			@note
		*/
		//==============================================================================================
		static void initializeInvalidStyle() {
			m_dcStyle[Tokens.ANCHOR_DEFAULT] = e_WinCtrlStyle.ANCHOR_DEFAULT;
			m_dcStyle[Tokens.ANCHOR_LEFTTOP] = e_WinCtrlStyle.ANCHOR_LEFTTOP;
			m_dcStyle[Tokens.ANCHOR_LEFT] = e_WinCtrlStyle.ANCHOR_LEFTCENTER;
			m_dcStyle[Tokens.ANCHOR_LEFTBOTTOM] = e_WinCtrlStyle.ANCHOR_LEFTBOTTOM;
			m_dcStyle[Tokens.ANCHOR_TOP] = e_WinCtrlStyle.ANCHOR_TOP;
			m_dcStyle[Tokens.ANCHOR_CENTER] = e_WinCtrlStyle.ANCHOR_CENTER;
			m_dcStyle[Tokens.ANCHOR_BOTTOM] = e_WinCtrlStyle.ANCHOR_BOTTOM;
			m_dcStyle[Tokens.ANCHOR_RIGHTTOP] = e_WinCtrlStyle.ANCHOR_RIGHTTOP;
			m_dcStyle[Tokens.ANCHOR_RIGHT] = e_WinCtrlStyle.ANCHOR_RIGHTCENTER;
			m_dcStyle[Tokens.ANCHOR_RIGHTBOTTOM] = e_WinCtrlStyle.ANCHOR_RIGHTBOTTOM;
			m_dcStyle[Tokens.BASE_DEFAULT] = e_WinCtrlStyle.BASE_DEFAULT;
			m_dcStyle[Tokens.BASE_LEFTTOP] = e_WinCtrlStyle.BASE_LEFTTOP;
			m_dcStyle[Tokens.BASE_LEFT] = e_WinCtrlStyle.BASE_LEFTCENTER;
			m_dcStyle[Tokens.BASE_LEFTBOTTOM] = e_WinCtrlStyle.BASE_LEFTBOTTOM;
			m_dcStyle[Tokens.BASE_TOP] = e_WinCtrlStyle.BASE_TOP;
			m_dcStyle[Tokens.BASE_CENTER] = e_WinCtrlStyle.BASE_CENTER;
			m_dcStyle[Tokens.BASE_BOTTOM] = e_WinCtrlStyle.BASE_BOTTOM;
			m_dcStyle[Tokens.BASE_RIGHTTOP] = e_WinCtrlStyle.BASE_RIGHTTOP;
			m_dcStyle[Tokens.BASE_RIGHT] = e_WinCtrlStyle.BASE_RIGHTCENTER;
			m_dcStyle[Tokens.BASE_RIGHTBOTTOM] = e_WinCtrlStyle.BASE_RIGHTBOTTOM;
			m_dcStyle[Tokens.TEXT_NONE] = e_WinCtrlStyle.TEXT_NONE;
			m_dcStyle[Tokens.TEXT_LEFT] = e_WinCtrlStyle.TEXT_LEFT;
			m_dcStyle[Tokens.TEXT_CENTER] = e_WinCtrlStyle.TEXT_CENTER;
			m_dcStyle[Tokens.TEXT_RIGHT] = e_WinCtrlStyle.TEXT_RIGHT;
			m_dcStyle[Tokens.TEXT_NORMAL] = e_WinCtrlStyle.TEXT_NORMAL;
			m_dcStyle[Tokens.TEXT_BOLD] = e_WinCtrlStyle.TEXT_BOLD;
			m_dcStyle[Tokens.TEXT_DENT] = e_WinCtrlStyle.TEXT_DENT;
			m_dcStyle[Tokens.TEXT_SHADOW] = e_WinCtrlStyle.TEXT_SHADOW;
			m_dcStyle[Tokens.TEXT_NOHYPHENATION] = e_WinCtrlStyle.TEXT_NOHYPHENATION;
			m_dcStyle[Tokens.TEXT_AUTOSCALE] = e_WinCtrlStyle.TEXT_AUTOSCALE;
			m_dcStyle[Tokens.HIDE] = e_WinCtrlStyle.HIDE;
			m_dcStyle[Tokens.DRAG] = e_WinCtrlStyle.DRAG;
			m_dcStyle[Tokens.DISABLE] = e_WinCtrlStyle.DISABLE;
			m_dcStyle[Tokens.NOHIT] = e_WinCtrlStyle.NOHIT;
			m_dcStyle[Tokens.HIT] = e_WinCtrlStyle.HIT;
			m_dcStyle[Tokens.EDIT_BLIND] = e_WinCtrlStyle.EDIT_BLIND;
			m_dcStyle[Tokens.EDIT_TYPE_ALL] = e_WinCtrlStyle.EDIT_TYPE_ALL;
			m_dcStyle[Tokens.EDIT_TYPE_ASCIICAPABLE] = e_WinCtrlStyle.EDIT_TYPE_ASCIICAPABLE;
			m_dcStyle[Tokens.EDIT_TYPE_NUMBERANDPUNCTUATION] = e_WinCtrlStyle.EDIT_TYPE_NUMBERANDPUNCTUATION;
			m_dcStyle[Tokens.EDIT_TYPE_URL] = e_WinCtrlStyle.EDIT_TYPE_URL;
			m_dcStyle[Tokens.EDIT_TYPE_NUMBERPAD] = e_WinCtrlStyle.EDIT_TYPE_NUMBERPAD;
			m_dcStyle[Tokens.EDIT_TYPE_PHONEPAD] = e_WinCtrlStyle.EDIT_TYPE_PHONEPAD;
			m_dcStyle[Tokens.EDIT_TYPE_NAMEPHONEPAD] = e_WinCtrlStyle.EDIT_TYPE_NAMEPHONEPAD;
			m_dcStyle[Tokens.EDIT_TYPE_EMAILADDRESS] = e_WinCtrlStyle.EDIT_TYPE_EMAILADDRESS;
			m_dcStyle[Tokens.NOBOUNCES] = e_WinCtrlStyle.NOBOUNCES;
			m_dcStyle[Tokens.ITEM_STACK_V] = e_WinCtrlStyle.ITEM_STACK_V;
			m_dcStyle[Tokens.ITEM_STACK_H] = e_WinCtrlStyle.ITEM_STACK_H;
			m_dcStyle[Tokens.SCROLL_LOOP] = e_WinCtrlStyle.SCROLL_LOOP;
			m_dcStyle[Tokens.SCROLL_UNLOCK] = e_WinCtrlStyle.SCROLL_UNLOCK;
			m_dcStyle[Tokens.SCROLL_LOCK] = e_WinCtrlStyle.SCROLL_LOCK;
			m_dcStyle[Tokens.SCROLLBAR_DISPLAY_NORMAL] = e_WinCtrlStyle.SCROLLBAR_DISPLAY_NORMAL;
			m_dcStyle[Tokens.SCROLLBAR_DISPLAY_SCROLLABLE] = e_WinCtrlStyle.SCROLLBAR_DISPLAY_SCROLLABLE;
			m_dcStyle[Tokens.SCROLLBAR_DISPLAY_ALWAYS] = e_WinCtrlStyle.SCROLLBAR_DISPLAY_ALWAYS;

			Tokens[] aIgnoreTextAlign = new Tokens[] {
				Tokens.TEXT_NONE,
				Tokens.TEXT_LEFT,
				Tokens.TEXT_CENTER,
				Tokens.TEXT_RIGHT,
			};
				Tokens[] aIgnoreFontType = new Tokens[] {
				Tokens.TEXT_NORMAL,
				Tokens.TEXT_BOLD,
				Tokens.TEXT_DENT,
				Tokens.TEXT_SHADOW,
			};
				Tokens[] aIgnoreEdit = new Tokens[] {
				Tokens.EDIT_BLIND,
				Tokens.EDIT_TYPE_ALL,
				Tokens.EDIT_TYPE_ASCIICAPABLE,
				Tokens.EDIT_TYPE_NUMBERANDPUNCTUATION,
				Tokens.EDIT_TYPE_URL,
				Tokens.EDIT_TYPE_NUMBERPAD,
				Tokens.EDIT_TYPE_PHONEPAD,
				Tokens.EDIT_TYPE_NAMEPHONEPAD,
				Tokens.EDIT_TYPE_EMAILADDRESS,
			};
				Tokens[] aIgnoreScrollable = new Tokens[]{
				Tokens.NOBOUNCES,
				Tokens.ITEM_STACK_V,
				Tokens.ITEM_STACK_H,
				Tokens.SCROLL_UNLOCK,
				Tokens.SCROLL_LOCK,
				Tokens.SCROLLBAR_DISPLAY_NORMAL,
				Tokens.SCROLLBAR_DISPLAY_SCROLLABLE,
				Tokens.SCROLLBAR_DISPLAY_ALWAYS,
			};
				Tokens[] aIgnoreScrollbar = new Tokens[]{
				Tokens.NOBOUNCES,
				Tokens.SCROLL_UNLOCK,
				Tokens.SCROLL_LOCK,
			};
				Tokens[] aIgnoreListbox = new Tokens[]{
				Tokens.SCROLLBAR_DISPLAY_NORMAL,
				Tokens.SCROLLBAR_DISPLAY_SCROLLABLE,
				Tokens.SCROLLBAR_DISPLAY_ALWAYS,
			};
				Tokens[] aIgnoreListboxStackDir = new Tokens[]{
				Tokens.ITEM_STACK_V,
				Tokens.ITEM_STACK_H,
			};

			// SCROLLBAR
			List<Tokens>    lst = new List<Tokens>();
			lst.AddRange(aIgnoreTextAlign);
			lst.AddRange(aIgnoreFontType);
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollbar);
			lst.Add(Tokens.DRAG);
			lst.Add(Tokens.NOHIT);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.SCROLLBAR] = lst.ToArray();

			// LISTBOX
			lst.Clear();
			lst.AddRange(aIgnoreTextAlign);
			lst.AddRange(aIgnoreFontType);
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreListbox);
			lst.Add(Tokens.DRAG);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.LISTBOX] = lst.ToArray();

			// LISTBOXEX
			m_dcInvalidStyle[e_WinCtrlKind.LISTBOXEX] = lst.ToArray();

			//CONTAINER
			lst.Clear();
			lst.AddRange(aIgnoreTextAlign);
			lst.AddRange(aIgnoreFontType);
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreListbox);
			lst.AddRange(aIgnoreListboxStackDir);
			lst.Add(Tokens.DRAG);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.CONTAINER] = lst.ToArray();

			//TEXT
			lst.Clear();
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.TEXT] = lst.ToArray();

			//RICHTEXT
			lst.Clear();
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.DRAG);
			lst.Add(Tokens.TEXT_AUTOSCALE);
			m_dcInvalidStyle[e_WinCtrlKind.RICHTEXT] = lst.ToArray();

			//LOG
			lst.Clear();
			lst.AddRange(aIgnoreTextAlign);
			lst.AddRange(aIgnoreFontType);
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreListbox);
			lst.AddRange(aIgnoreListboxStackDir);
			lst.Add(Tokens.DRAG);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			lst.Add(Tokens.TEXT_AUTOSCALE);
			m_dcInvalidStyle[e_WinCtrlKind.LOG] = lst.ToArray();

			//LOGTEXT
			lst.Clear();
			lst.AddRange(aIgnoreTextAlign);
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_AUTOSCALE);
			m_dcInvalidStyle[e_WinCtrlKind.LOGTEXT] = lst.ToArray();

			//BUTTON
			lst.Clear();
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.BUTTON] = lst.ToArray();

			//CHECKBOX
			lst.Clear();
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.CHECKBOX] = lst.ToArray();

			//COMBOBOX
			lst.Clear();
			m_dcInvalidStyle[e_WinCtrlKind.COMBOBOX] = null;

			//RADIO
			lst.Clear();
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.RADIO] = lst.ToArray();

			//WINDOWCLOSEBUTTON
			lst.Clear();
			m_dcInvalidStyle[e_WinCtrlKind.WINDOWCLOSEBUTTON] = null;

			//WINDOWCAPTION
			lst.Clear();
			m_dcInvalidStyle[e_WinCtrlKind.WINDOWCAPTION] = null;

			//WINDOWCAPTION
			lst.Clear();
			m_dcInvalidStyle[e_WinCtrlKind.WINDOWCAPTION] = null;

			//HELPBUTTON
			lst.Clear();
			m_dcInvalidStyle[e_WinCtrlKind.HELPBUTTON] = null;

			//EDITBOX
			lst.Clear();
			lst.AddRange(aIgnoreTextAlign);
			lst.AddRange(aIgnoreListbox);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.EDITBOX] = lst.ToArray();

			//TEXTBOX
			lst.Clear();
			lst.AddRange(aIgnoreTextAlign);
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreListbox);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.TEXTBOX] = lst.ToArray();

			//RICHTEXTBOX
			lst.Clear();
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreListbox);
			m_dcInvalidStyle[e_WinCtrlKind.RICHTEXTBOX] = lst.ToArray();

			//METER
			lst.Clear();
			lst.AddRange(aIgnoreTextAlign);
			lst.AddRange(aIgnoreFontType);
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.METER] = lst.ToArray();

			//SLIDEBAR
			m_dcInvalidStyle[e_WinCtrlKind.SLIDEBAR] = null;

			//ICON
			lst.Clear();
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.ICON] = lst.ToArray();

			//IMAGE
			m_dcInvalidStyle[e_WinCtrlKind.IMAGE] = null;

			//TEXTURE
			lst.Clear();
			lst.AddRange(aIgnoreTextAlign);
			lst.AddRange(aIgnoreFontType);
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.TEXTURE] = lst.ToArray();

			//RENDER
			lst.Clear();
			lst.AddRange(aIgnoreTextAlign);
			lst.AddRange(aIgnoreFontType);
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.RENDER] = lst.ToArray();

			//Canvas
			lst.Clear();
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.CANVAS] = lst.ToArray();

			//RENDERICON
			lst.Clear();
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.RENDERICON] = lst.ToArray();

			//RECASTICON
			lst.Clear();
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.RECASTICON] = lst.ToArray();

			//LINE
			lst.Clear();
			lst.AddRange(aIgnoreTextAlign);
			lst.AddRange(aIgnoreFontType);
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.LINE] = lst.ToArray();

			//LABEL
			lst.Clear();
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.LABEL] = lst.ToArray();

			//FRAME
			lst.Clear();
			lst.AddRange(aIgnoreTextAlign);
			lst.AddRange(aIgnoreFontType);
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.FRAME] = lst.ToArray();

			//BAR
			lst.Clear();
			lst.AddRange(aIgnoreEdit);
			lst.AddRange(aIgnoreScrollable);
			lst.Add(Tokens.TEXT_NOHYPHENATION);
			m_dcInvalidStyle[e_WinCtrlKind.BAR] = lst.ToArray();
		}
		//==============================================================================================
		/*!設定しても無効なコントロールのプロパティ.
			@brief	initializeInvalidProperty
			@note
		*/
		//==============================================================================================
		static void initializeInvalidProperty() {
			Dictionary<e_WinCtrlKind,List<e_WinProperty>> dcInvalidProperty = new Dictionary<e_WinCtrlKind, List<e_WinProperty>>();

			e_WinProperty[] aIgnoreCommon = new e_WinProperty[] {
				e_WinProperty.RELATION_ID,
				e_WinProperty.HELP_ID,
				e_WinProperty.TOOLTIP,
				e_WinProperty.SLIDEMAX,
			};
			e_WinProperty[] aIgnoreCaption = new e_WinProperty[] {
				e_WinProperty.CAPTION,
				e_WinProperty.CAPTION_STR,
				e_WinProperty.CAPTION_OFFSET,
				e_WinProperty.FONT_KIND,
			};
			e_WinProperty[] aIgnoreContents = new e_WinProperty[] {
				e_WinProperty.CONTENTS,
				e_WinProperty.CONTENTS_SIZE,
				e_WinProperty.LINE_SPACE,
				e_WinProperty.LINE_FEED_OFFSET,
			};
			e_WinProperty[] aIgnoreTexture = new e_WinProperty[] {
				e_WinProperty.COLOR1,
				e_WinProperty.COLOR2,
				e_WinProperty.COLOR3,
				e_WinProperty.COLOR4,
				e_WinProperty.COLOR5,
				e_WinProperty.COLOR6,
				e_WinProperty.COLOR7,

				e_WinProperty.TEX_ID0,
				e_WinProperty.TEX_ID1,
				e_WinProperty.TEX_ID2,
				e_WinProperty.TEX_ID3,
				e_WinProperty.TEX_ID4,
				e_WinProperty.TEX_ID5,
				e_WinProperty.TEX_ID6,
				e_WinProperty.TEX_ID7,

				e_WinProperty.TEXTURE_OFFSET0,
				e_WinProperty.TEXTURE_OFFSET1,
				e_WinProperty.TEXTURE_OFFSET2,
				e_WinProperty.TEXTURE_OFFSET3,
				e_WinProperty.TEXTURE_OFFSET4,
				e_WinProperty.TEXTURE_OFFSET5,
				e_WinProperty.TEXTURE_OFFSET6,
				e_WinProperty.TEXTURE_OFFSET7,

				e_WinProperty.TEXTURE_SIZE1,
				e_WinProperty.TEXTURE_SIZE2,
				e_WinProperty.TEXTURE_SIZE3,
				e_WinProperty.TEXTURE_SIZE4,
				e_WinProperty.TEXTURE_SIZE5,
				e_WinProperty.TEXTURE_SIZE6,
				e_WinProperty.TEXTURE_SIZE7,
			};

			e_WinProperty[] aIgnoreTexture0only = new e_WinProperty[] {
				e_WinProperty.COLOR1,
				e_WinProperty.COLOR2,
				e_WinProperty.COLOR3,
				e_WinProperty.COLOR4,
				e_WinProperty.COLOR5,
				e_WinProperty.COLOR6,
				e_WinProperty.COLOR7,

				e_WinProperty.TEX_ID1,
				e_WinProperty.TEX_ID2,
				e_WinProperty.TEX_ID3,
				e_WinProperty.TEX_ID4,
				e_WinProperty.TEX_ID5,
				e_WinProperty.TEX_ID6,
				e_WinProperty.TEX_ID7,

				e_WinProperty.TEXTURE_OFFSET1,
				e_WinProperty.TEXTURE_OFFSET2,
				e_WinProperty.TEXTURE_OFFSET3,
				e_WinProperty.TEXTURE_OFFSET4,
				e_WinProperty.TEXTURE_OFFSET5,
				e_WinProperty.TEXTURE_OFFSET6,
				e_WinProperty.TEXTURE_OFFSET7,

				e_WinProperty.TEXTURE_SIZE1,
				e_WinProperty.TEXTURE_SIZE2,
				e_WinProperty.TEXTURE_SIZE3,
				e_WinProperty.TEXTURE_SIZE4,
				e_WinProperty.TEXTURE_SIZE5,
				e_WinProperty.TEXTURE_SIZE6,
				e_WinProperty.TEXTURE_SIZE7,
			};
			e_WinProperty[] aIgnoreRenderTexture = new e_WinProperty[] {
				e_WinProperty.COLOR2,
				e_WinProperty.COLOR3,
				e_WinProperty.COLOR4,
				e_WinProperty.COLOR5,
				e_WinProperty.COLOR6,
				e_WinProperty.COLOR7,

				e_WinProperty.TEX_ID1,
				e_WinProperty.TEX_ID2,
				e_WinProperty.TEX_ID3,
				e_WinProperty.TEX_ID4,
				e_WinProperty.TEX_ID5,
				e_WinProperty.TEX_ID6,
				e_WinProperty.TEX_ID7,

				e_WinProperty.TEXTURE_OFFSET1,
				e_WinProperty.TEXTURE_OFFSET2,
				e_WinProperty.TEXTURE_OFFSET3,
				e_WinProperty.TEXTURE_OFFSET4,
				e_WinProperty.TEXTURE_OFFSET5,
				e_WinProperty.TEXTURE_OFFSET6,
				e_WinProperty.TEXTURE_OFFSET7,

				e_WinProperty.TEXTURE_SIZE1,
				e_WinProperty.TEXTURE_SIZE2,
				e_WinProperty.TEXTURE_SIZE3,
				e_WinProperty.TEXTURE_SIZE4,
				e_WinProperty.TEXTURE_SIZE5,
				e_WinProperty.TEXTURE_SIZE6,
				e_WinProperty.TEXTURE_SIZE7,
			};

			// SCROLLBAR
			List<e_WinProperty> lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.SCROLLBAR] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreCaption);
			lst.AddRange(aIgnoreContents);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);

			// LISTBOX
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.LISTBOX] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreCaption);
			lst.AddRange(aIgnoreTexture);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);
			// LISTBOXEX
			dcInvalidProperty[e_WinCtrlKind.LISTBOXEX] = lst;
			//CONTAINER
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.CONTAINER] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreCaption);
			lst.AddRange(aIgnoreTexture);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);

			//TEXT
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.TEXT] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreTexture);
			lst.Add(e_WinProperty.CONTENTS);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);
			lst.Add(e_WinProperty.SIZE);
			//RICHTEXT
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.RICHTEXT] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreTexture);
			lst.Add(e_WinProperty.CONTENTS);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);
			lst.Add(e_WinProperty.SIZE);

			//LOG
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.LOG] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreCaption);
			lst.AddRange(aIgnoreTexture);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);

			//LOGTEXT
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.LOGTEXT] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreTexture);
			lst.Add(e_WinProperty.CONTENTS);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);
			lst.Add(e_WinProperty.SIZE);

			//BUTTON
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.BUTTON] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreContents);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);
			//CHECKBOX
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.CHECKBOX] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.Add(e_WinProperty.CONTENTS_SIZE);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);
			//COMBOBOX
			dcInvalidProperty[e_WinCtrlKind.COMBOBOX] = null;
			//RADIO
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.RADIO] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.Add(e_WinProperty.CONTENTS_SIZE);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);
			//WINDOWCLOSEBUTTON
			dcInvalidProperty[e_WinCtrlKind.WINDOWCLOSEBUTTON] = null;
			//WINDOWCAPTION
			dcInvalidProperty[e_WinCtrlKind.WINDOWCAPTION] = null;
			//WINDOWMINIMIZATION
			dcInvalidProperty[e_WinCtrlKind.WINDOWMINIMIZATION] = null;
			//HELPBUTTON
			dcInvalidProperty[e_WinCtrlKind.HELPBUTTON] = null;

			//EDITBOX
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.EDITBOX] = lst;
			lst = new List<e_WinProperty>();
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreTexture0only);
			lst.Add(e_WinProperty.CONTENTS);
			lst.Add(e_WinProperty.POSITION2);

			//TEXTBOX
			dcInvalidProperty[e_WinCtrlKind.TEXTBOX] = lst;
			//RICHTEXTBOX
			dcInvalidProperty[e_WinCtrlKind.RICHTEXTBOX] = lst;

			//METER
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.METER] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreCaption);
			lst.AddRange(aIgnoreContents);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);
			//SLIDEBAR
			dcInvalidProperty[e_WinCtrlKind.SLIDEBAR] = null;

			//ICON
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.ICON] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreContents);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);
			//IMAGE,
			dcInvalidProperty[e_WinCtrlKind.IMAGE] = null;
			//TEXTURE
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.TEXTURE] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreCaption);
			lst.AddRange(aIgnoreContents);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);
			//RENDER
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.RENDER] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreCaption);
			lst.AddRange(aIgnoreRenderTexture);
			lst.Add(e_WinProperty.CONTENTS);
			lst.Add(e_WinProperty.LINE_SPACE);
			lst.Add(e_WinProperty.LINE_FEED_OFFSET);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);

			//CANVAS
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.CANVAS] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreContents);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);

			//RENDERICON
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.RENDERICON] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.Add(e_WinProperty.CONTENTS);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);
			//RECASTICON
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.RECASTICON] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreContents);
			lst.Add(e_WinProperty.POSITION2);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);
			//LINE
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.LINE] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreCaption);
			lst.AddRange(aIgnoreContents);
			lst.AddRange(aIgnoreTexture0only);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);
			//LABEL
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.LABEL] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreContents);
			lst.AddRange(aIgnoreTexture0only);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);
			//FRAME
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.FRAME] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreContents);
			lst.AddRange(aIgnoreCaption);
			lst.AddRange(aIgnoreTexture0only);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);
			//BAR
			lst = new List<e_WinProperty>();
			dcInvalidProperty[e_WinCtrlKind.BAR] = lst;
			lst.AddRange(aIgnoreCommon);
			lst.AddRange(aIgnoreContents);
			lst.AddRange(aIgnoreTexture0only);
			lst.Add(e_WinProperty.EDIT);
			lst.Add(e_WinProperty.GROUP);

			// 無効プロパティフラグリストを作成する.
			foreach (e_WinCtrlKind eKind in dcInvalidProperty.Keys) {
				if (dcInvalidProperty[eKind] == null) {
					m_dcInvalidProperty[eKind] = null;
				} else {
					byte[] aFlag = new byte[((int) e_WinProperty.Max + 7) >> 3];
					m_dcInvalidProperty[eKind] = aFlag;
					foreach (e_WinProperty eProp in dcInvalidProperty[eKind]) {
						int index = (int) eProp;
						aFlag[index >> 3] |= (byte)(1 << (index & 7));
					}
				}
			}
		}
	}
}
