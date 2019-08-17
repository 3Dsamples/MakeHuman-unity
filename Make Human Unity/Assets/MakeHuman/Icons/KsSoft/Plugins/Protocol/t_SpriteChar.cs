//==============================================================================================
/*!
	@file  KsSoft Protocol
	
	(counter SJIS string 京)
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using KS;
//==========================================================================
/*!
	@brief	t_SpriteChar
*/
public class t_SpriteChar : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public ushort	id;
	public Vector4	uv;
	public sbyte	page;
	public sbyte	xOffset;
	public sbyte	yOffset;
	public sbyte	xAdvance;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_SpriteChar() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		id = 0;
		page = 0;
		xOffset = 0;
		yOffset = 0;
		xAdvance = 0;
	}
	public bool read(CReadVariable cVariable) {
		try {
			id = cVariable.getU16();
			uv = cVariable.getVector4();
			page = cVariable.getS8();
			xOffset = cVariable.getS8();
			yOffset = cVariable.getS8();
			xAdvance = cVariable.getS8();
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(id);
		cVariable.put(uv);
		cVariable.put(page);
		cVariable.put(xOffset);
		cVariable.put(yOffset);
		cVariable.put(xAdvance);
		return true;
	}
};

