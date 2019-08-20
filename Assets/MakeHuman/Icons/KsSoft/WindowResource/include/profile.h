//==============================================================================================
/*!プロフィール関連共通表示部分
	@file  profile.h
	@param	$x,$y
	@break	$_x,$_y
*/
//==============================================================================================
#pragma once

#define	AVATAR_ICON_DEF	\
		STYLE = TEXT_CENTER;	\
		TEX_ID  = 100_100_00000,"ICNB0";	\
		TEX_ID1 = 100_100_00000;	\
		TEX_ID2 = 100_100_00000,"BGITL";	\
		TEX_ID3 = 100_100_00000,"ICN1";	\
		TEX_ID4 = 100_100_00000,"STCHP";	\
		TEXTURE_SIZE1 = 100, 100;	\
		TEXTURE_OFFSET2 = 50,6;	\
		TEXTURE_OFFSET3 = 3,2;	\
		TEXTURE_SIZE3 = 124,124;	\
		TEXTURE_SIZE4 = 102,102;	\
		CAPTION = 000_000_00009;	\
		CAPTION_OFFSET = 24,-34;	\
		CAPTION_COLOR = COLOR32(255,198,0,255);	\
		FONT_KIND = TINY_FONT;	\
		SIZE = 100, 100

#define	Profile(_name,_id,_x,_y)	\
	ICON(_name ## Icon) {	\
		ID = _id;	\
		POSITION = _x,_y;	\
		AVATAR_ICON_DEF;	\
        PRIORITY = 1;   \
	};	\
	$_x = _x + 100 + 10;	\
	$_y = _y + 7;	\
	TEXTURE(_name ## Connect) {	\
		ID = _id + 3;	\
		DEF_ICON_CONNECT;	\
		POSITION = $_x + 10,$_y + 4;	\
	};	\
	TEXT(_name ## Name) {	\
		ID = _id + 2;	\
		FONT_KIND = SMALL_FONT;	\
		WHITE_CAPTION_COLOR;	\
		CAPTION = 000_000_00010;	\
		POSITION = $_x + 30,$_y - 1;	\
	};	\
	$_y += 27;	\
	TEXTURE(_name ## Party) {	\
		ID = _id + 6;	\
		STYLE = HIDE;	\
		DEF_ICON_PARTY;	\
		POSITION = $_x+8,$_y + 4;	\
	};	\
	TEXTURE(_name ## Place) {	\
		ID = _id + 4;	\
		DEF_ICON_TOWN;	\
		POSITION = $_x + 38,$_y + 4;	\
		SIZE = 27,27;	\
	};	\
	TEXT(_name ## Quest) {	\
		ID = _id + 5;	\
		FONT_KIND = SMALL_FONT;	\
		CAPTION = 000_000_0010;	\
		POSITION = $_x + 68,$_y + 10;	\
	};	\
	$_y += 32;	\
	TEXTURE(_name ## Guild) {	\
		ID = _id + 7;	\
		DEF_ICON_GUILD;	\
		POSITION = $_x+38,$_y + 4;	\
	};	\
	TEXT(_name ## Guild) {	\
		ID = _id + 8;	\
		POSITION = $_x + 68,$_y + 10;	\
		FONT_KIND = SMALL_FONT;	\
		CAPTION = NOWLOADING;	\
	}


#define	ProfileTiny(_name,_id,_x,_y)	\
	ICON(_name ## Icon) {	\
		ID = _id;	\
		POSITION = _x,_y;	\
		AVATAR_ICON_DEF;	\
        PRIORITY = 1;   \
	};	\
	TEXTURE(_name ## Connect) {	\
		ID = _id + 3;	\
		DEF_ICON_CONNECT;	\
		POSITION = _x,_y + 8;	\
	};	\
	$_x = _x + 100 + 10;	\
	$_y = _y + 7;	\
	TEXT(_name ## Name) {	\
		ID = _id + 2;	\
		STYLE = WIN_CTRL_STYLE_TEXT_LEFT;	\
		DEF_TEXT_TITLE;	\
		CAPTION = 000_000_00010;	\
		POSITION = $_x + 35,$_y + 5;	\
	};	\
	$_y += 27;	\
	TEXTURE(_name ## Party) {	\
		ID = _id + 6;	\
		STYLE = HIDE;	\
		DEF_ICON_PARTY;	\
		POSITION = $_x-3,$_y + 4;	\
	};	\
	$_y += 32;	\
	TEXTURE(_name ## Guild) {	\
		ID = _id + 7;	\
		DEF_ICON_GUILD;	\
		POSITION = $_x-3,$_y;	\
	};	\
	TEXT(_name ## Guild) {	\
		ID = _id + 8;	\
		STYLE = WIN_CTRL_STYLE_TEXT_LEFT;	\
		DEF_TEXT_INFO;	\
		POSITION = $_x + 27,$_y + 6;	\
		CAPTION = NOWLOADING;	\
	}
