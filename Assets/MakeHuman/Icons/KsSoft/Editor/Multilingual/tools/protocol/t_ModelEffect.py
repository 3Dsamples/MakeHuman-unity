from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_ModelEffectFrame import	t_ModelEffectFrame
#/==========================================================================
#*!
#	@brief	t_ModelEffect
#/
class t_ModelEffect:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_mId = 0	#U32
	m_mMesh = 0	#U32
	m_fcAnim = 0	#U32
	m_mLink = 0	#U32
	m_fcBone = 0	#U32
	m_flag = 0	#U32
	m_radLight = 0.0	#float
	m_offset = Vector3()
	m_rotate = Vector3()
	m_scale = Vector3()
	m_color = 0	#U32
	m_aFrame = []	#t_ModelEffectFrame
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
		self.m_mMesh = 0
		self.m_fcAnim = 0
		self.m_mLink = 0
		self.m_fcBone = 0
		self.m_flag = 0
		self.m_radLight = 0.0
		self.m_offset = Vector3()
		self.m_rotate = Vector3()
		self.m_scale = Vector3()
		self.m_color = 0
		self.m_aFrame = []

	def read(self,cVariable):
		self.m_mId = cVariable.getU32()
		self.m_mMesh = cVariable.getU32()
		self.m_fcAnim = cVariable.getU32()
		self.m_mLink = cVariable.getU32()
		self.m_fcBone = cVariable.getU32()
		self.m_flag = cVariable.getU32()
		self.m_radLight = cVariable.getFloat()
		self.m_offset = cVariable.getVector3()
		self.m_rotate = cVariable.getVector3()
		self.m_scale = cVariable.getVector3()
		self.m_color = cVariable.getU32()
		n = cVariable.getU8()
		if (n > 255) :
			cVariable.error('array size error in m_aFrame')
			return False
		self.m_aFrame = [None]*n
		for i in range(n):
			self.m_aFrame[i] = t_ModelEffectFrame()
			if (not self.m_aFrame[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_mId)
		cVariable.putU32(self.m_mMesh)
		cVariable.putU32(self.m_fcAnim)
		cVariable.putU32(self.m_mLink)
		cVariable.putU32(self.m_fcBone)
		cVariable.putU32(self.m_flag)
		cVariable.putFloat(self.m_radLight)
		cVariable.putVector3(self.m_offset)
		cVariable.putVector3(self.m_rotate)
		cVariable.putVector3(self.m_scale)
		cVariable.putU32(self.m_color)
		if (len(self.m_aFrame) > 255) :
			cVariable.error('array size error in m_aFrame')
			return False
		n = len(self.m_aFrame)
		cVariable.putU8(n)
		for i in range(n):
			if (not self.m_aFrame[i].write(cVariable)):
				return False
		return True

