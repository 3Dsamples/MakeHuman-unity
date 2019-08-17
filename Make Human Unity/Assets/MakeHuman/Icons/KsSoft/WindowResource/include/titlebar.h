//==============================================================================================
/*!タイトルバー
	@file  titlebar.h
*/
//==============================================================================================
#pragma once

#define TITLEBAR(_label,_id,_y,_caption)	\
	TEXTURE(TitleBarBG ## _label) {	\
		ID = _id;	\
		STYLE = ANCHOR_LEFTTOP;	\
		POSITION = 0,_y;	\
		TEX_ID = 100_000_00001,"TTBAR";	\
	};	\
	TEXT(Title ## _label) {	\
		ID = _id+1;	\
		STYLE = ANCHOR_LEFTTOP;	\
		POSITION = 30,_y + 5;	\
		FONT_KIND = LARGE_FONT;	\
		CAPTION = _caption;	\
 	}
