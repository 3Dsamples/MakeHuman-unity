from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_MessageData import	t_MessageData
#/==========================================================================
#*!
#	@brief	t_MessageDataHeader
#/
class t_MessageDataHeader:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_magicNo = 4277006336	#U32
	m_aData = []	#t_MessageData
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
		self.m_magicNo = 4277006336
		self.m_aData = []

	def read(self,cVariable):
		self.m_magicNo = cVariable.getU32()
		n = cVariable.getU8()
		if (n > 255) :
			cVariable.error('array size error in m_aData')
			return False
		self.m_aData = [None]*n
		for i in range(n):
			self.m_aData[i] = t_MessageData()
			if (not self.m_aData[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_magicNo)
		if (len(self.m_aData) > 255) :
			cVariable.error('array size error in m_aData')
			return False
		n = len(self.m_aData)
		cVariable.putU8(n)
		for i in range(n):
			if (not self.m_aData[i].write(cVariable)):
				return False
		return True

