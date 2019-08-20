//==============================================================================================
/*!タイトル:装備共通リスト
	@file  equip_list.h
	@param	$x,$y,$w
	@break	$_x,$_y,$_w,$_h
*/
//==============================================================================================
#pragma once

#define	EQUIP_SET_SIZE	565

#define	EQUIP_SET(_name,_id,_x,_y)	\
	FRAME(_name ## Frame0) {	\
		ID = _id;	\
		POSITION = _x + 190 * 0,_y;	\
		SIZE = 185,104;	\
		TEX_ID ="FRML";	\
		COLOR = COLOR32(255,255,0,255);	\
		PRIORITY = 1;	\
	};	\
	TEXTURE(_name ## Char0) {	\
		ID = _id + 5;	\
		STYLE = HIT;	\
		POSITION = _x + 5 + 190 * 0,_y + 7;	\
		TEX_ID = 100_010_00000,"ICNB1";	\
		SIZE = 90,90;	\
		PRIORITY = 1;	\
	};	\
	ICON(_name ## Char0) {	\
		ID = _id + 10;	\
		POSITION = _x + 5 + 190 * 0,_y + 7;	\
		DEF_ICON;	\
		PRIORITY = 2;	\
	};	\
	TEXTURE(_name ## Weapon0) {	\
		ID = _id + 15;	\
		STYLE = HIT;	\
		POSITION = _x + 100 + 190 * 0,_y + 17;	\
		TEX_ID = 100_010_00000,"ICNB2";	\
		SIZE = 80,80;	\
		PRIORITY = 1;	\
	};	\
	ICON(_name ## Weapon0) {	\
		ID = _id + 20;	\
		POSITION = _x + 100 + 190 * 0,_y + 17;	\
		DEF_WEAPON_ICON;	\
		PRIORITY = 2;	\
	};	\
	FRAME(_name ## Frame1) {	\
		ID = _id + 100;	\
		POSITION = _x + 190 * 1,_y;	\
		SIZE = 185,104;	\
		TEX_ID ="FRML";	\
		COLOR = COLOR32(255,255,255,240);	\
		PRIORITY = 1;	\
	};	\
	TEXTURE(_name ## Char1) {	\
		ID = _id + 105;	\
		STYLE = HIT;	\
		POSITION = _x + 5 + 190 * 1,_y + 7;	\
		TEX_ID = 100_010_00000,"ICNB1";	\
		SIZE = 90,90;	\
		PRIORITY = 1;	\
	};	\
	ICON(_name ## Char1) {	\
		ID = _id + 110;	\
		POSITION = _x + 5 + 190 * 1,_y + 7;	\
		DEF_ICON;	\
		PRIORITY = 2;	\
	};	\
	TEXTURE(_name ## Weapon1) {	\
		ID = _id + 115;	\
		STYLE = HIT;	\
		POSITION = _x + 100 + 190 * 1,_y + 17;	\
		TEX_ID = 100_010_00000,"ICNB2";	\
		SIZE = 80,80;	\
		PRIORITY = 1;	\
	};	\
	ICON(_name ## Weapon1) {	\
		ID = _id + 120;	\
		POSITION = _x + 100 + 190 * 1,_y + 17;	\
		DEF_WEAPON_ICON;	\
		PRIORITY = 2;	\
	};	\
	FRAME(_name ## Frame2) {	\
		ID = _id + 200;	\
		POSITION = _x + 190 * 2,_y;	\
		SIZE = 185,104;	\
		TEX_ID ="FRML";	\
		COLOR = COLOR32(255,255,255,240);	\
		PRIORITY = 1;	\
	};	\
	TEXTURE(_name ## Char2) {	\
		ID = _id + 205;	\
		STYLE = HIT;	\
		POSITION = _x + 5 + 190 * 2,_y + 7;	\
		TEX_ID = 100_010_00000,"ICNB1";	\
		SIZE = 90,90;	\
		PRIORITY = 1;	\
	};	\
	ICON(_name ## Char2) {	\
		ID = _id + 210;	\
		POSITION = _x + 5 + 190 * 2,_y + 7;	\
		DEF_ICON;	\
		PRIORITY = 2;	\
	};	\
	TEXTURE(_name ## Weapon2) {	\
		ID = _id + 215;	\
		STYLE = HIT;	\
		POSITION = _x + 100 + 190 * 2,_y + 17;	\
		TEX_ID = 100_010_00000,"ICNB2";	\
		SIZE = 80,80;	\
		PRIORITY = 1;	\
	};	\
	ICON(_name ## Weapon2) {	\
		ID = _id + 220;	\
		POSITION = _x + 100 + 190 * 2,_y + 17;	\
		DEF_WEAPON_ICON;	\
		PRIORITY = 2;	\
	}

#define	SMALL_EQUIP_SET(_name,_id,_x,_y,_size)	\
	TEXTURE(_name ## BG0) {	\
		ID = _id + 5;	\
		STYLE = HIT;	\
		TEX_ID = 100_010_00000,"ICNB1";	\
		POSITION = _x + 140 * 0,_y + 5;	\
		SIZE = _size,_size;	\
	};	\
	ICON(_name ## Char0) {	\
		ID = _id + 10;	\
		POSITION = _x + 140 * 0,_y + 5;	\
		TEX_ID  = 100_010_00000,"ICNB0";	\
		TEX_ID1 = 100_100_00000,"ICON";	\
		TEX_ID2 = 100_100_00000,"BGITL";	\
		TEX_ID3 = 100_100_00000,"ICN1";	\
		TEX_ID4 = 100_100_00000,"STCHP";	\
		STYLE = WIN_CTRL_STYLE_ANCHOR_LEFTTOP|WIN_CTRL_STYLE_TEXT_CENTER;	\
		SIZE = _size,_size;	\
		TEXTURE_SIZE1 = _size,_size;	\
		TEXTURE_SIZE3 = _size+30,_size+20;	\
		TEXTURE_SIZE4 = _size,_size;	\
		TEXTURE_OFFSET2 = 30,2;	\
		TEXTURE_OFFSET3 = 1,1;	\
		CAPTION = 000_000_00009;	\
		CAPTION_OFFSET = 12,-28;	\
		CAPTION_COLOR = COLOR32(255,198,0,255);	\
		FONT_KIND = TINY_FONT;	\
		PRIORITY = 2;	\
	};	\
	TEXTURE(_name ## BG1) {	\
		ID = _id + 105;	\
		STYLE = HIT;	\
		TEX_ID = 100_010_00000,"ICNB1";	\
		POSITION = _x + (_size + 5) * 1,_y + 5;	\
		SIZE = _size,_size;	\
	};	\
	ICON(_name ## Char1) {	\
		ID = _id + 110;	\
		POSITION = _x + (_size + 5) * 1,_y + 5;	\
		TEX_ID  = 100_010_00000,"ICNB0";	\
		TEX_ID1 = 100_100_00000,"ICON";	\
		TEX_ID2 = 100_100_00000,"BGITL";	\
		TEX_ID3 = 100_100_00000,"ICN1";	\
		TEX_ID4 = 100_100_00000,"STCHP";	\
		STYLE = HIDE|WIN_CTRL_STYLE_ANCHOR_LEFTTOP|WIN_CTRL_STYLE_TEXT_CENTER;	\
		SIZE = _size,_size;	\
		TEXTURE_SIZE1 = _size,_size;	\
		TEXTURE_SIZE3 = _size+30,_size+20;	\
		TEXTURE_SIZE4 = _size,_size;	\
		TEXTURE_OFFSET2 = 30,2;	\
		TEXTURE_OFFSET3 = 1,1;	\
		CAPTION = 000_000_00009;	\
		CAPTION_OFFSET = 12,-28;	\
		CAPTION_COLOR = COLOR32(255,198,0,255);	\
		FONT_KIND = TINY_FONT;	\
		PRIORITY = 2;	\
	};	\
	TEXTURE(_name ## BG2) {	\
		ID = _id + 205;	\
		STYLE = HIT;	\
		TEX_ID = 100_010_00000,"ICNB1";	\
		POSITION = _x + (_size + 5) * 2,_y + 5;	\
		SIZE = _size,_size;	\
	};	\
	ICON(_name ## Char2) {	\
		ID = _id + 210;	\
		POSITION = _x + (_size + 5) * 2,_y + 5;	\
		TEX_ID  = 100_010_00000,"ICNB0";	\
		TEX_ID1 = 100_100_00000,"ICON";	\
		TEX_ID2 = 100_100_00000,"BGITL";	\
		TEX_ID3 = 100_100_00000,"ICN1";	\
		TEX_ID4 = 100_100_00000,"STCHP";	\
		STYLE = HIDE|WIN_CTRL_STYLE_ANCHOR_LEFTTOP|WIN_CTRL_STYLE_TEXT_CENTER;	\
		SIZE = _size,_size;	\
		TEXTURE_SIZE1 = _size,_size;	\
		TEXTURE_SIZE3 = _size+30,_size+20;	\
		TEXTURE_SIZE4 = _size,_size;	\
		TEXTURE_OFFSET2 = 30,2;	\
		TEXTURE_OFFSET3 = 1,1;	\
		CAPTION = 000_000_00009;	\
		CAPTION_OFFSET = 12,-28;	\
		CAPTION_COLOR = COLOR32(255,198,0,255);	\
		FONT_KIND = TINY_FONT;	\
		PRIORITY = 2;	\
	}

#define	EQUIP_SET_LIST(_name,_id,_x,_y,_w,_h)	\
	$_x = _x; $_y = _y;	\
	$_w = _w; $_h = 100;	\
	TEXTURE(_name ## ListLeft) {	\
		ID = _id + 1010;		\
		STYLE = ANCHOR_BOTTOM|HIT;	\
		POSITION = $_x - EQUIP_SET_SIZE/2 - 16,$_y - $_h/2 + 28;	\
		SIZE = 32,48;	\
		TEX_ID = 0,"ARWL0";	\
		PRIORITY = 1;	\
	};	\
	TEXTURE(_name ## ListRight) {	\
		ID = _id + 1015;	\
		STYLE = ANCHOR_BOTTOM|HIT;	\
		POSITION = $_x + EQUIP_SET_SIZE/2 + 16,$_y - $_h/2 + 28;	\
		SIZE = 32,48;	\
		TEX_ID = 0,"ARWR0";	\
		PRIORITY = 1;	\
	};	\
	LISTBOX(_name ## List) {	\
		ID = _id + 1080;	\
		STYLE = ANCHOR_BOTTOM|ITEM_STACK_H|SCROLL_LOCK|NOBOUNCES;	\
		POSITION = $_x,$_y;	\
		SIZE = EQUIP_SET_SIZE,$_h;	\
		CONTENTS_SIZE = EQUIP_SET_SIZE,$_h;	\
		CONTENTS = {	\
			EQUIP_SET(_name,_id + 2000,0,0);	\
		}	\
		PRIORITY = 1;	\
	}	\

#define	EQUIP_SET_VIEW(_name,_y,_w,_h)	\
	RENDER(_name) {	\
		ID = 006_000_20000;	\
		STYLE = ANCHOR_TOP;	\
		POSITION = 0,_y;	\
		SIZE = 512,_h;	\
		CONTENTS_SIZE = 512,512;	\
	};	\
	EQUIP_SET(_name,006_000_00000,(_w - EQUIP_SET_SIZE)/2,(_y - _h))
