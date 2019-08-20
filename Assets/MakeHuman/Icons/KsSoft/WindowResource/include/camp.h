//==============================================================================================
/*!拠点共通部分
	@file  camp.h
*/
//==============================================================================================
#pragma once

#if defined(DEBUG)
#define	STYLE_HIDE	0
#else
#define	STYLE_HIDE	HIDE
#endif

#define	CAMP_ALONE(_label,_id,_x,_y,_texID)	\
	BUTTON(_label) {	\
		ID = _id;	\
 		POSITION = _x,_y;	\
		TEX_ID = 0,_texID;	\
	};	\
	TEXT(_label) {   \
		ID = _id+1; \
		STYLE = BASE_TOP;    \
		POSITION = _x+120,_y + 166;    \
		FONT_KIND = NORMAL_FONT;  \
		WHITE_CAPTION_COLOR;  \
		CAPTION = NOWLOADING;   \
		PRIORITY = 1;	\
	};	\
	TEXTURE(_label ## Star0) {	\
		ID = _id+3;	\
		POSITION = _x+90,_y + 132;	\
		TEX_ID = 0,"STAR0";	\
		PRIORITY = 1;	\
	};	\
	TEXTURE(_label ## Star1) {	\
		ID = _id+4;	\
		POSITION = _x+112,_y + 132;	\
		TEX_ID = 0,"STAR0";	\
		PRIORITY = 1;	\
	};	\
	TEXTURE(_label ## Star2) {	\
		ID = _id+5;	\
		POSITION = _x+134,_y + 132;	\
		TEX_ID = 0,"STAR0";	\
		PRIORITY = 1;	\
	}

#define CampAloneContentsList(_label) BUTTON(_label),TEXT(_label),TEXTURE(_label),TEXTURE(_label ## Star0),TEXTURE(_label ## Star1),TEXTURE(_label ## Star2)

#define CAMP(_label,_id,_x,_y,_x0,_y0,_texID)	\
	CAMP_ALONE(_label,_id,_x,_y,_texID);	\
	LINE(_label) {	\
		ID = _id+6;	\
		POSITION  = _x0+120,_y0 + 100;	\
		POSITION2 = _x+120,_y + 100;	\
		TEX_ID = 0,"LINE0";	\
	}

#define CampContentsList(_label) BUTTON(_label),TEXT(_label),TEXTURE(_label),TEXTURE(_label ## Star0),TEXTURE(_label ## Star1),TEXTURE(_label ## Star2),LINE(_label)
