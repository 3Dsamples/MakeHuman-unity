from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_WindowData
#/
class t_WindowData:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_mId = 0	#U32
	m_propertyNum = 0	#U32
	m_ctrlNum = 0	#U32
	m_aData = []	#U8
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
		self.m_propertyNum = 0
		self.m_ctrlNum = 0
		self.m_aData = []

	def read(self,cVariable):
		self.m_mId = cVariable.getU32()
		self.m_propertyNum = cVariable.getU32()
		self.m_ctrlNum = cVariable.getU32()
		n = cVariable.getU16()
		if (n > 65535) :
			cVariable.error('array size error in m_aData')
			return False
		self.m_aData = [0]*n
		for i in range(n):
			self.m_aData[i] = cVariable.getU8()
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_mId)
		cVariable.putU32(self.m_propertyNum)
		cVariable.putU32(self.m_ctrlNum)
		if (len(self.m_aData) > 65535) :
			cVariable.error('array size error in m_aData')
			return False
		n = len(self.m_aData)
		cVariable.putU16(n)
		for i in range(n):
			cVariable.putU8(self.m_aData[i])
		return True

