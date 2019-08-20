from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_BillboardFrame import	t_BillboardFrame
#/==========================================================================
#*!
#	@brief	t_Billboard
#/
class t_Billboard:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_mId = 0	#U32
	m_mMaterial = 0	#U32
	m_mLink = 0	#U32
	m_fcBone = 0	#U32
	m_origin = 0	#U32
	m_flag = 0	#U32
	m_yoffset = 0.0	#float
	m_width = 0.0	#float
	m_height = 0.0	#float
	m_rotate = 0.0	#float
	aFrame = []	#t_BillboardFrame
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
		self.m_mMaterial = 0
		self.m_mLink = 0
		self.m_fcBone = 0
		self.m_origin = 0
		self.m_flag = 0
		self.m_yoffset = 0.0
		self.m_width = 0.0
		self.m_height = 0.0
		self.m_rotate = 0.0
		self.aFrame = []

	def read(self,cVariable):
		self.m_mId = cVariable.getU32()
		self.m_mMaterial = cVariable.getU32()
		self.m_mLink = cVariable.getU32()
		self.m_fcBone = cVariable.getU32()
		self.m_origin = cVariable.getU32()
		self.m_flag = cVariable.getU32()
		self.m_yoffset = cVariable.getFloat()
		self.m_width = cVariable.getFloat()
		self.m_height = cVariable.getFloat()
		self.m_rotate = cVariable.getFloat()
		n = cVariable.getU8()
		if (n > 255) :
			cVariable.error('array size error in aFrame')
			return False
		self.aFrame = [None]*n
		for i in range(n):
			self.aFrame[i] = t_BillboardFrame()
			if (not self.aFrame[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_mId)
		cVariable.putU32(self.m_mMaterial)
		cVariable.putU32(self.m_mLink)
		cVariable.putU32(self.m_fcBone)
		cVariable.putU32(self.m_origin)
		cVariable.putU32(self.m_flag)
		cVariable.putFloat(self.m_yoffset)
		cVariable.putFloat(self.m_width)
		cVariable.putFloat(self.m_height)
		cVariable.putFloat(self.m_rotate)
		if (len(self.aFrame) > 255) :
			cVariable.error('array size error in aFrame')
			return False
		n = len(self.aFrame)
		cVariable.putU8(n)
		for i in range(n):
			if (not self.aFrame[i].write(cVariable)):
				return False
		return True

