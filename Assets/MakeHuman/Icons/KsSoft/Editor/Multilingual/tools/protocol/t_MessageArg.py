from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_MessageArg
#/
class t_MessageArg:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_eType = 0	#S8
	m_value = 0	#S32
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
		self.m_eType = 0
		self.m_value = 0

	def read(self,cVariable):
		self.m_eType = cVariable.getS8()
		self.m_value = cVariable.getS32()
		return True

	def write(self,cVariable):
		cVariable.putS8(self.m_eType)
		cVariable.putS32(self.m_value)
		return True

