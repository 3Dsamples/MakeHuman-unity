//==============================================================================================
/*!電卓共通部分
	@file  NumberPad.h
*/
//==============================================================================================
#pragma once

#define NumberPad(_id,_x,_y,_w)	\
	$_dx = 20; $_dy = 80;	\
	BUTTON(One) {	\
		ID = _id;	\
		POSITION = _x, _y;	\
		SIZE = _w;	\
		DEF_BUTTON_LARGE;	\
	};	\
	$_x = _x + _w + $_dx;	\
	BUTTON(Two) {	\
		ID = _id+1;	\
		POSITION = $_x, _y;	\
		SIZE = _w;	\
		DEF_BUTTON_LARGE;	\
	};	\
	$_x += _w + $_dx;	\
	BUTTON(Three) {	\
		ID = _id+2;	\
		POSITION = $_x, _y;	\
		SIZE = _w;	\
		DEF_BUTTON_LARGE;	\
	};	\
	$_x = _x; $_y = _y + $_dy;	\
	BUTTON(Four) {	\
		ID = _id+3;	\
		POSITION = $_x, $_y;	\
		SIZE = _w;	\
		DEF_BUTTON_LARGE;	\
	};	\
	$_x += _w + $_dx;	\
	BUTTON(Five) {	\
		ID = _id+4;	\
		POSITION = $_x, $_y;	\
		SIZE = _w;	\
		DEF_BUTTON_LARGE;	\
	};	\
	$_x += _w + $_dx;	\
	BUTTON(Six) {	\
		ID = _id+5;	\
		POSITION = $_x, $_y;	\
		SIZE = _w;	\
		DEF_BUTTON_LARGE;	\
	};	\
	$_x = _x; $_y += $_dy;	\
	BUTTON(Seven) {	\
		ID = _id+6;	\
		POSITION = $_x, $_y;	\
		SIZE = _w;	\
		DEF_BUTTON_LARGE;	\
	};	\
	$_x += _w + $_dx;	\
	BUTTON(Eight) {	\
		ID = _id+7;	\
		POSITION = $_x, $_y;	\
		SIZE = _w;	\
		DEF_BUTTON_LARGE;	\
	};	\
	$_x += _w + $_dx;	\
	BUTTON(Nine) {	\
		ID = _id+8;	\
		POSITION = $_x, $_y;	\
		SIZE = _w;	\
		DEF_BUTTON_LARGE;	\
	};	\
	$_x = _x; $_y += $_dy + 30;	\
	BUTTON(BackSpace) {	\
		ID = _id+110;	\
		POSITION = $_x, $_y;	\
		SIZE = _w;	\
		DEF_BUTTON_LARGE;	\
	};	\
	$_x += _w + $_dx;	\
	BUTTON(Zero) {	\
		ID = _id+9;	\
		POSITION = $_x, $_y+30;	\
		SIZE = _w;	\
		DEF_BUTTON_LARGE;	\
	};	\
	$_x += _w + $_dx;	\
	BUTTON(OK) {	\
		ID = _id+100;	\
		POSITION = $_x, $_y;	\
		SIZE = _w;	\
		CAPTION = 250_000_00030;	\
		DEF_BUTTON_LARGE;	\
	}
