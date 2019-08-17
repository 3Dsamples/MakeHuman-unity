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
	@brief	e_EffectFlag
*/
public class e_EffectFlag {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public  const uint	Loop_Once = 0;	// 0
	public  const uint	Loop_Loop = 1;	// 1 << 0
	public  const uint	Loop_Animation = 2;	// 2 << 0
	public  const uint	Loop_Mask = 3;	// 3 << 0
	public  const uint	Bind = 16;	// 1 << 4
	public  const uint	NotBindAngle = 32;	// 1 << 5
	public  const uint	Collision = 64;	// 1 << 6
	public  const uint	Normal = 0;	// 0 << 8
	public  const uint	Billboard = 256;	// 1 << 8
	public  const uint	YBillboard = 512;	// 2 << 8
	public  const uint	Trajectory = 768;	// 3 << 8
	public  const uint	TrajectoryAxis = 1024;	// 4 << 8
	public  const uint	Gather = 1792;	// 7 << 8
	public  const uint	CameraShake = 2048;	// 8 << 8
	public  const uint	Type_Mask = 65280;	// 255 << 8
};

