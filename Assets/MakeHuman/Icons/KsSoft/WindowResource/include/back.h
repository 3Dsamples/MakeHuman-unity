//==============================================================================================
/*!バックボタン
	@file  back.h
*/
//==============================================================================================
#pragma once

#define BACKBUTTON(_label,_id)	\
	BUTTON(Back ## _label) {	\
		ID = _id;	\
		STYLE = ANCHOR_LEFTBOTTOM;	\
		TEX_ID = 100_000_00001,"BTBK?";	\
		POSITION = 0,200;	\
		PRIORITY = 1;	\
	};	\
	TEXT(Back ## _label) {	\
		ID = _id+1;	\
		STYLE = ANCHOR_LEFTBOTTOM;	\
		FONT_KIND = NORMAL_FONT;	\
		CAPTION = 000_000_00005;	\
		POSITION = 47,209;	\
	}
