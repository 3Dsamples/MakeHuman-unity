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
	@brief	t_Billboards
*/
public class t_Billboards : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public t_Billboard[]	aBillboard;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_Billboards() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		aBillboard = new t_Billboard[0];
	}
	public bool read(CReadVariable cVariable) {
		try {
			{
				int n = (int) cVariable.getU32();
				if (n > 65536) {
					cVariable.error("array size error in aBillboard");
					return false;
				}
				aBillboard = new t_Billboard [n];
				for(int i = 0;i < n;++i) {
					aBillboard[i] = new t_Billboard();
					if (!aBillboard[i].read(cVariable)) return false;
				}
			}
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		{	
				if (aBillboard.Length > 65536) {
					cVariable.error("array size error in aBillboard");
					return false;
				}
			int n = aBillboard.Length;
			cVariable.put((uint) n);
			for(int i = 0;i < n;++i) {
				if (!aBillboard[i].write(cVariable)) return false;

			}
		}
		return true;
	}
};

