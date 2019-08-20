from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_WinCtrlData
#/
class t_WinCtrlData:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_kind = 0	#U32
	m_propertyNum = 0	#U32
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
		self.m_kind = 0
		self.m_propertyNum = 0

	def read(self,cVariable):
		self.m_kind = cVariable.getU32()
		self.m_propertyNum = cVariable.getU32()
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_kind)
		cVariable.putU32(self.m_propertyNum)
		return True

