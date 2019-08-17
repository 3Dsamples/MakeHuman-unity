from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_WindowData import	t_WindowData
#/==========================================================================
#*!
#	@brief	t_WindowDataHeader
#/
class t_WindowDataHeader:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_uMagicNo = 0	#U32
	m_uVersion = 0	#U32
	m_aWindowData = []	#t_WindowData
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
		self.m_uMagicNo = 0
		self.m_uVersion = 0
		self.m_aWindowData = []

	def read(self,cVariable):
		self.m_uMagicNo = cVariable.getU32()
		self.m_uVersion = cVariable.getU32()
		n = cVariable.getU8()
		if (n > 255) :
			cVariable.error('array size error in m_aWindowData')
			return False
		self.m_aWindowData = [None]*n
		for i in range(n):
			self.m_aWindowData[i] = t_WindowData()
			if (not self.m_aWindowData[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_uMagicNo)
		cVariable.putU32(self.m_uVersion)
		if (len(self.m_aWindowData) > 255) :
			cVariable.error('array size error in m_aWindowData')
			return False
		n = len(self.m_aWindowData)
		cVariable.putU8(n)
		for i in range(n):
			if (not self.m_aWindowData[i].write(cVariable)):
				return False
		return True

