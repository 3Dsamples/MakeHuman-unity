from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_ParticleEffect
#/
class t_ParticleEffect:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_mId = 0	#U32
	m_mBillboardId = 0	#U32
	m_mLink = 0	#U32
	m_fcBone = 0	#U32
	m_flag = 0	#U32
	m_num = 0	#S16
	m_span = 0.0	#float
	m_lifeTime = 0.0	#float
	m_lifeTimeRate = 0.0	#float
	m_generateRange = 0.0	#float
	m_offset = Vector3()
	m_accel = Vector3()
	m_damping = 0.0	#float
	m_minSpeed = 0.0	#float
	m_maxSpeed = 0.0	#float
	m_minAngleX = 0	#S16
	m_maxAngleX = 0	#S16
	m_minAngleY = 0	#S16
	m_maxAngleY = 0	#S16
	m_startSize = 0.0	#float
	m_endSize = 0.0	#float
	m_sizeFadeTime = 0.0	#float
	m_startAlpha = 0.0	#float
	m_endAlpha = 0.0	#float
	m_alphaFadeTime = 0.0	#float
	#/==========================================================================
	#*!
	#	@brief	Constructor
	#/
	def __init__(self):
		self.clear()

	#/==========================================================================
	#*!
	#	@brief	Accessor
	#/
	def clear(self):
		self.m_mId = 0
		self.m_mBillboardId = 0
		self.m_mLink = 0
		self.m_fcBone = 0
		self.m_flag = 0
		self.m_num = 0
		self.m_span = 0.0
		self.m_lifeTime = 0.0
		self.m_lifeTimeRate = 0.0
		self.m_generateRange = 0.0
		self.m_offset = Vector3()
		self.m_accel = Vector3()
		self.m_damping = 0.0
		self.m_minSpeed = 0.0
		self.m_maxSpeed = 0.0
		self.m_minAngleX = 0
		self.m_maxAngleX = 0
		self.m_minAngleY = 0
		self.m_maxAngleY = 0
		self.m_startSize = 0.0
		self.m_endSize = 0.0
		self.m_sizeFadeTime = 0.0
		self.m_startAlpha = 0.0
		self.m_endAlpha = 0.0
		self.m_alphaFadeTime = 0.0

	def read(self,cVariable):
		self.m_mId = cVariable.getU32()
		self.m_mBillboardId = cVariable.getU32()
		self.m_mLink = cVariable.getU32()
		self.m_fcBone = cVariable.getU32()
		self.m_flag = cVariable.getU32()
		self.m_num = cVariable.getS16()
		self.m_span = cVariable.getFloat()
		self.m_lifeTime = cVariable.getFloat()
		self.m_lifeTimeRate = cVariable.getFloat()
		self.m_generateRange = cVariable.getFloat()
		self.m_offset = cVariable.getVector3()
		self.m_accel = cVariable.getVector3()
		self.m_damping = cVariable.getFloat()
		self.m_minSpeed = cVariable.getFloat()
		self.m_maxSpeed = cVariable.getFloat()
		self.m_minAngleX = cVariable.getS16()
		self.m_maxAngleX = cVariable.getS16()
		self.m_minAngleY = cVariable.getS16()
		self.m_maxAngleY = cVariable.getS16()
		self.m_startSize = cVariable.getFloat()
		self.m_endSize = cVariable.getFloat()
		self.m_sizeFadeTime = cVariable.getFloat()
		self.m_startAlpha = cVariable.getFloat()
		self.m_endAlpha = cVariable.getFloat()
		self.m_alphaFadeTime = cVariable.getFloat()
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_mId)
		cVariable.putU32(self.m_mBillboardId)
		cVariable.putU32(self.m_mLink)
		cVariable.putU32(self.m_fcBone)
		cVariable.putU32(self.m_flag)
		cVariable.putS16(self.m_num)
		cVariable.putFloat(self.m_span)
		cVariable.putFloat(self.m_lifeTime)
		cVariable.putFloat(self.m_lifeTimeRate)
		cVariable.putFloat(self.m_generateRange)
		cVariable.putVector3(self.m_offset)
		cVariable.putVector3(self.m_accel)
		cVariable.putFloat(self.m_damping)
		cVariable.putFloat(self.m_minSpeed)
		cVariable.putFloat(self.m_maxSpeed)
		cVariable.putS16(self.m_minAngleX)
		cVariable.putS16(self.m_maxAngleX)
		cVariable.putS16(self.m_minAngleY)
		cVariable.putS16(self.m_maxAngleY)
		cVariable.putFloat(self.m_startSize)
		cVariable.putFloat(self.m_endSize)
		cVariable.putFloat(self.m_sizeFadeTime)
		cVariable.putFloat(self.m_startAlpha)
		cVariable.putFloat(self.m_endAlpha)
		cVariable.putFloat(self.m_alphaFadeTime)
		return True

