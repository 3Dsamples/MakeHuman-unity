//==============================================================================================
/*!パラメーター共通表示部分
	@file  parameter.h
*/
//==============================================================================================
#pragma once

#define	STATUS(_label,_id,_x,_y,_caption) \
	TEXT(StatusLabel ## _label) {	\
		ID = _id;	\
		STYLE = TEXT_DENT;	\
		POSITION = _x,_y;	\
		CAPTION = _caption;	\
		DEF_TEXT_STATUS_LABEL;	\
	};	\
	TEXT(Status ## _label) {	\
		ID = _id + 5;	\
		STYLE = BASE_RIGHTTOP;	\
		POSITION = _x + 200,_y;	\
		CAPTION = 000_000_09999;	\
		DEF_TEXT_STATUS_VALUE;	\
	}

#define STATUS_UP(_label,_id,_x,_y)	\
	TEXT(Status ## _label) {	\
		ID = _id;	\
		POSITION = _x,_y;	\
		CAPTION = 000_000_00140;	\
		DEF_TEXT_UP;	\
	}

#define	STATUS_CRITICAL(_label,_id,_x,_y,_caption,_value) \
	TEXT(StatusLabel ## _label) {	\
		ID = _id;	\
		STYLE = TEXT_DENT;	\
		POSITION = _x,_y;	\
		CAPTION = _caption;	\
		DEF_TEXT_STATUS_LABEL;	\
	};	\
	TEXT(Status ## _label) {	\
		ID = _id + 5;	\
		STYLE = BASE_RIGHTTOP;	\
		POSITION = _x + 200,_y;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_VALUE;	\
	}

#define	STATUS2(_label,_id,_x,_y,_caption,_value) \
	TEXT(StatusLabel ## _label) {	\
		ID = _id;	\
		STYLE = TEXT_DENT;	\
		POSITION = _x,_y;	\
		CAPTION = _caption;	\
		DEF_TEXT_STATUS_LABEL;	\
	};	\
	TEXT(Status ## _label) {	\
		ID = _id + 5;	\
		STYLE = BASE_RIGHTTOP;	\
		POSITION = _x + 200,_y - 25;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_VALUE;	\
	}

#define	STATUS4(_label,_id,_x,_y,_caption,_value) \
	TEXT(StatusLabel ## _label) {	\
		ID = _id;	\
		STYLE = TEXT_DENT;	\
		POSITION = _x,_y;	\
		CAPTION = _caption;	\
		DEF_TEXT_STATUS_LABEL;	\
	};	\
	TEXTURE(Status ## _label) {	\
		ID = _id + 1;	\
		POSITION = _x + 190,_y;	\
		TEX_ID = 0,"ATTR1";	\
		SIZE = 15,15;	\
	};	\
	TEXT(Status ## _label) {	\
		ID = _id + 5;	\
		STYLE = BASE_RIGHTTOP;	\
		POSITION = _x + 190,_y;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_VALUE;	\
	}

#define	STATUS_DEF(_label,_id,_x,_y,_caption,_value) \
	TEXT(StatusLabel ## _label) {	\
		ID = _id;	\
		STYLE = TEXT_DENT;	\
		POSITION = _x,_y;	\
		CAPTION = _caption;	\
		DEF_TEXT_STATUS_LABEL;	\
	};	\
	TEXTURE(Status0 ## _label) {	\
		ID = _id + 1;	\
		POSITION = _x + 185,_y;	\
		TEX_ID = 0,"ATTR1";	\
		SIZE = 15,15;	\
	};	\
	TEXT(Status0 ## _label) {	\
		ID = _id + 2;	\
		STYLE = BASE_RIGHTTOP;	\
		POSITION = _x + 185,_y;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_VALUE;	\
	};	\
	TEXTURE(Status1 ## _label) {	\
		ID = _id + 3;	\
		POSITION = _x + 185,_y- 25;	\
		TEX_ID = 0,"ATTR2";	\
		SIZE = 15,15;	\
	};	\
	TEXT(Status1 ## _label) {	\
		ID = _id + 4;	\
		STYLE = BASE_RIGHTTOP;	\
		POSITION = _x + 185,_y - 25;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_VALUE;	\
	};	\
	TEXTURE(Status2 ## _label) {	\
		ID = _id + 5;	\
		POSITION = _x + 185,_y - 50;	\
		TEX_ID = 0,"ATTR3";	\
		SIZE = 15,15;	\
	};	\
	TEXT(Status2 ## _label) {	\
		ID = _id + 6;	\
		STYLE = BASE_RIGHTTOP;	\
		POSITION = _x + 185,_y - 50;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_VALUE;	\
	}

#define	ATTRIBUTE(_label,_id,_x,_y,_partsId)	\
	TEXTURE(_label) {	\
		ID = _id;	\
		TEX_ID = _partsId;	\
		POSITION = _x,_y;	\
	};	\
	TEXT(Status ## _label) {	\
		ID = _id + 5;	\
		STYLE = BASE_RIGHTTOP;	\
		POSITION = _x + 128,_y;	\
		CAPTION = 000_000_09999;	\
		DEF_TEXT_STATUS_VALUE;	\
	}

#define	STATUS_TINY(_label,_id,_x,_y,_caption) \
	TEXT(StatusLabel ## _label) {	\
		ID = _id;	\
		STYLE = TEXT_DENT;	\
		POSITION = _x,_y;	\
		CAPTION = _caption;	\
		FONT_KIND = TINY_FONT;	\
		CAPTION_COLOR=COLOR32(130,100,0,255);	\
	};	\
	TEXT(Status ## _label) {	\
		ID = _id + 5;	\
		POSITION = _x + 32,_y;	\
		CAPTION = 000_000_09999;	\
		DEF_TEXT_STATUS_TINY_VALUE;	\
	}
#define	STATUS2_TINY(_label,_id,_x,_y,_xoffset_,_caption,_value) \
	TEXT(StatusLabel ## _label) {	\
		ID = _id;	\
		STYLE = TEXT_DENT;	\
		POSITION = _x,_y;	\
		CAPTION = _caption;	\
		FONT_KIND = TINY_FONT;	\
		CAPTION_COLOR=COLOR32(130,100,0,255);	\
	};	\
	TEXT(Status ## _label) {	\
		ID = _id + 5;	\
		STYLE = BASE_RIGHTTOP;	\
		POSITION = _x + _xoffset_,_y;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_TINY_VALUE;	\
	}
#define	STATUS3_TINY(_label,_id,_x,_y,_caption,_value) \
	TEXT(StatusLabel ## _label) {	\
		ID = _id;	\
		STYLE = TEXT_DENT;	\
		POSITION = _x,_y;	\
		CAPTION = _caption;	\
		FONT_KIND = TINY_FONT;	\
		CAPTION_COLOR=COLOR32(130,100,0,255);	\
	};	\
	TEXTURE(Status0 ## _label) {	\
		ID = _id + 1;	\
		POSITION = _x + 65,_y+2;	\
		TEX_ID = 0,"ATTR1";	\
		SIZE = 15,15;	\
	};	\
	TEXT(Status0 ## _label) {	\
		ID = _id + 2;	\
		POSITION = _x + 90,_y;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_TINY_VALUE;	\
	};	\
	TEXTURE(Status1 ## _label) {	\
		ID = _id + 3;	\
		POSITION = _x + 150,_y+2;	\
		TEX_ID = 0,"ATTR2";	\
		SIZE = 15,15;	\
	};	\
	TEXT(Status1 ## _label) {	\
		ID = _id + 4;	\
		POSITION = _x + 175,_y;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_TINY_VALUE;	\
	};	\
	TEXTURE(Status2 ## _label) {	\
		ID = _id + 5;	\
		POSITION = _x + 235,_y+2;	\
		TEX_ID = 0,"ATTR3";	\
		SIZE = 15,15;	\
	};	\
	TEXT(Status2 ## _label) {	\
		ID = _id + 6;	\
		POSITION = _x + 260,_y;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_TINY_VALUE;	\
	}
#define	STATUS_DEF_TINY(_label,_id,_x,_y,_caption,_value) \
	TEXT(StatusLabel ## _label) {	\
		ID = _id;	\
		STYLE = TEXT_DENT;	\
		POSITION = _x,_y;	\
		CAPTION = _caption;	\
		FONT_KIND = TINY_FONT;	\
		CAPTION_COLOR=COLOR32(130,100,0,255);	\
	};	\
	TEXTURE(Status0 ## _label) {	\
		ID = _id + 1;	\
		POSITION = _x + 85,_y+2;	\
		TEX_ID = 0,"ATTR1";	\
		SIZE = 15,15;	\
	};	\
	TEXT(Status0 ## _label) {	\
		ID = _id + 2;	\
		STYLE = BASE_RIGHTTOP;	\
		POSITION = _x + 85,_y;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_TINY_VALUE;	\
	};	\
	TEXTURE(Status1 ## _label) {	\
		ID = _id + 3;	\
		POSITION = _x + 85,_y- 18;	\
		TEX_ID = 0,"ATTR2";	\
		SIZE = 15,15;	\
	};	\
	TEXT(Status1 ## _label) {	\
		ID = _id + 4;	\
		STYLE = BASE_RIGHTTOP;	\
		POSITION = _x + 85,_y - 20;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_TINY_VALUE;	\
	};	\
	TEXTURE(Status2 ## _label) {	\
		ID = _id + 5;	\
		POSITION = _x + 85,_y - 38;	\
		TEX_ID = 0,"ATTR3";	\
		SIZE = 15,15;	\
	};	\
	TEXT(Status2 ## _label) {	\
		ID = _id + 6;	\
		STYLE = BASE_RIGHTTOP;	\
		POSITION = _x + 85,_y - 40;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_TINY_VALUE;	\
	}

#define	STATUS4_TINY(_label,_id,_x,_y,_caption,_value) \
	TEXT(StatusLabel ## _label) {	\
		ID = _id;	\
		STYLE = TEXT_DENT;	\
		POSITION = _x,_y;	\
		CAPTION = _caption;	\
		FONT_KIND = TINY_FONT;	\
		CAPTION_COLOR=COLOR32(130,100,0,255);	\
	};	\
	TEXTURE(Status ## _label) {	\
		ID = _id + 1;	\
		POSITION = _x + 140,_y+2;	\
		TEX_ID = 0,"ATTR1";	\
		SIZE = 15,15;	\
	};	\
	TEXT(Status ## _label) {	\
		ID = _id + 5;	\
		STYLE = BASE_RIGHTTOP;	\
		POSITION = _x + 140,_y;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_TINY_VALUE;	\
	}

#define	ATTRIBUTE_TINY(_label,_id,_x,_y,_partsId)	\
	TEXTURE(_label) {	\
		ID = _id;	\
		TEX_ID = _partsId;	\
		POSITION = _x,_y;	\
	};	\
	TEXT(Status ## _label) {	\
		ID = _id + 5;	\
		STYLE = BASE_RIGHTTOP|TEXT_DENT;	\
		POSITION = _x + 200,_y;	\
		CAPTION = 000_000_09999;	\
		FONT_KIND = TINY_FONT;	\
		CAPTION_COLOR=COLOR32(130,100,0,255);	\
	}

#define STATUS_UP_TINY(_label,_id,_x,_y)	\
	TEXT(Status ## _label) {	\
		ID = _id;	\
		STYLE = TEXT_DENT;	\
		POSITION = _x,_y;	\
		CAPTION = 000_000_00140;	\
		FONT_KIND = TINY_FONT;	\
		CAPTION_COLOR=COLOR32(130,100,0,255);	\
	}

#define	STATUS5_TINY(_label,_id,_x,_y,_caption,_value) \
	TEXT(StatusLabel ## _label) {	\
		ID = _id;	\
		STYLE = TEXT_DENT;	\
		POSITION = _x,_y;	\
		CAPTION = _caption;	\
		FONT_KIND = TINY_FONT;	\
		CAPTION_COLOR=COLOR32(130,100,0,255);	\
	};	\
	TEXT(Status ## _label) {	\
		ID = _id + 5;	\
		POSITION = _x + 32,_y;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_TINY_VALUE;	\
	}

#define	STATUS6_TINY(_label,_id,_x,_y,_caption,_value) \
	TEXT(StatusLabel ## _label) {	\
		ID = _id;	\
		STYLE = TEXT_DENT;	\
		POSITION = _x,_y;	\
		CAPTION = _caption;	\
		FONT_KIND = TINY_FONT;	\
		CAPTION_COLOR=COLOR32(130,100,0,255);	\
	};	\
	TEXT(Status ## _label) {	\
		ID = _id + 5;	\
		POSITION = _x + 32,_y;	\
		CAPTION = _value;	\
		DEF_TEXT_STATUS_TINY_VALUE;	\
	}
