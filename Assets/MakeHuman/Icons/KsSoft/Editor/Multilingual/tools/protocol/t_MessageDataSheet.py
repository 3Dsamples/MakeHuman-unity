from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_MessageDataOne import	t_MessageDataOne
#/==========================================================================
#*!
#	@brief	t_MessageDataSheet
#/
class t_MessageDataSheet:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_type = 0	#U32
	m_aContents = []	#t_MessageDataOne
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
		self.m_type = 0
		self.m_aContents = []

	def read(self,cVariable):
		self.m_type = cVariable.getU32()
		n = cVariable.getU16()
		if (n > 65535) :
			cVariable.error('array size error in m_aContents')
			return False
		self.m_aContents = [None]*n
		for i in range(n):
			self.m_aContents[i] = t_MessageDataOne()
			if (not self.m_aContents[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_type)
		if (len(self.m_aContents) > 65535) :
			cVariable.error('array size error in m_aContents')
			return False
		n = len(self.m_aContents)
		cVariable.putU16(n)
		for i in range(n):
			if (not self.m_aContents[i].write(cVariable)):
				return False
		return True

