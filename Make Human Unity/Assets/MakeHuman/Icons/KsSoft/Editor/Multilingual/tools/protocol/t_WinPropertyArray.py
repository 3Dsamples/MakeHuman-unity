from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_WinPropertyArray
#/
class t_WinPropertyArray:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_aValue = []	#U32
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
		self.m_aValue = []

	def read(self,cVariable):
		n = cVariable.getU8()
		if (n > 255) :
			cVariable.error('array size error in m_aValue')
			return False
		self.m_aValue = [0]*n
		for i in range(n):
			self.m_aValue[i] = cVariable.getU32()
		return True

	def write(self,cVariable):
		if (len(self.m_aValue) > 255) :
			cVariable.error('array size error in m_aValue')
			return False
		n = len(self.m_aValue)
		cVariable.putU8(n)
		for i in range(n):
			cVariable.putU32(self.m_aValue[i])
		return True

