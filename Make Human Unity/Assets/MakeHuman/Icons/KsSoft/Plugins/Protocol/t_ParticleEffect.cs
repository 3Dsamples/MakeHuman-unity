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
	@brief	t_ParticleEffect
*/
public class t_ParticleEffect : ISerializable {
	//==========================================================================
	/*!
		@brief	Member
	*/
	public uint	m_mId;
	public uint	m_mBillboardId;
	public uint	m_mLink;
	public uint	m_fcBone;
	public uint	m_flag;
	public short	m_num;
	public float	m_span;
	public float	m_lifeTime;
	public float	m_lifeTimeRate;
	public float	m_generateRange;
	public Vector3	m_offset;
	public Vector3	m_accel;
	public float	m_damping;
	public float	m_minSpeed;
	public float	m_maxSpeed;
	public short	m_minAngleX;
	public short	m_maxAngleX;
	public short	m_minAngleY;
	public short	m_maxAngleY;
	public float	m_startSize;
	public float	m_endSize;
	public float	m_sizeFadeTime;
	public float	m_startAlpha;
	public float	m_endAlpha;
	public float	m_alphaFadeTime;
	//==========================================================================
	/*!
		@brief	Constructor
	*/
	public t_ParticleEffect() {
		clear();
	}
	//==========================================================================
	/*!
		@brief	Accessor
	*/
	public void clear() {
		m_mId = 0;
		m_mBillboardId = 0;
		m_mLink = 0;
		m_fcBone = 0;
		m_flag = 0;
		m_num = 0;
		m_span = 0.0f;
		m_lifeTime = 0.0f;
		m_lifeTimeRate = 0.0f;
		m_generateRange = 0.0f;
		m_damping = 0.0f;
		m_minSpeed = 0.0f;
		m_maxSpeed = 0.0f;
		m_minAngleX = 0;
		m_maxAngleX = 0;
		m_minAngleY = 0;
		m_maxAngleY = 0;
		m_startSize = 0.0f;
		m_endSize = 0.0f;
		m_sizeFadeTime = 0.0f;
		m_startAlpha = 0.0f;
		m_endAlpha = 0.0f;
		m_alphaFadeTime = 0.0f;
	}
	public bool read(CReadVariable cVariable) {
		try {
			m_mId = cVariable.getU32();
			m_mBillboardId = cVariable.getU32();
			m_mLink = cVariable.getU32();
			m_fcBone = cVariable.getU32();
			m_flag = cVariable.getU32();
			m_num = cVariable.getS16();
			m_span = cVariable.getFloat();
			m_lifeTime = cVariable.getFloat();
			m_lifeTimeRate = cVariable.getFloat();
			m_generateRange = cVariable.getFloat();
			m_offset = cVariable.getVector3();
			m_accel = cVariable.getVector3();
			m_damping = cVariable.getFloat();
			m_minSpeed = cVariable.getFloat();
			m_maxSpeed = cVariable.getFloat();
			m_minAngleX = cVariable.getS16();
			m_maxAngleX = cVariable.getS16();
			m_minAngleY = cVariable.getS16();
			m_maxAngleY = cVariable.getS16();
			m_startSize = cVariable.getFloat();
			m_endSize = cVariable.getFloat();
			m_sizeFadeTime = cVariable.getFloat();
			m_startAlpha = cVariable.getFloat();
			m_endAlpha = cVariable.getFloat();
			m_alphaFadeTime = cVariable.getFloat();
		} catch (System.Exception e) {
			Debug.LogError(e);return false;
		}
		return true;
	}
	public bool write(CWriteVariable cVariable) {
		cVariable.put(m_mId);
		cVariable.put(m_mBillboardId);
		cVariable.put(m_mLink);
		cVariable.put(m_fcBone);
		cVariable.put(m_flag);
		cVariable.put(m_num);
		cVariable.put(m_span);
		cVariable.put(m_lifeTime);
		cVariable.put(m_lifeTimeRate);
		cVariable.put(m_generateRange);
		cVariable.put(m_offset);
		cVariable.put(m_accel);
		cVariable.put(m_damping);
		cVariable.put(m_minSpeed);
		cVariable.put(m_maxSpeed);
		cVariable.put(m_minAngleX);
		cVariable.put(m_maxAngleX);
		cVariable.put(m_minAngleY);
		cVariable.put(m_maxAngleY);
		cVariable.put(m_startSize);
		cVariable.put(m_endSize);
		cVariable.put(m_sizeFadeTime);
		cVariable.put(m_startAlpha);
		cVariable.put(m_endAlpha);
		cVariable.put(m_alphaFadeTime);
		return true;
	}
};

