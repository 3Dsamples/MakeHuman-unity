from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_WinProperty
#/
class t_WinProperty:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
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
		self.m_value = 0

	def read(self,cVariable):
		self.m_value = cVariable.getS32()
		return True

	def write(self,cVariable):
		cVariable.putS32(self.m_value)
		return True

