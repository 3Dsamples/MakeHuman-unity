from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_WinPropertyString
#/
class t_WinPropertyString:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_string = ""	#std::string
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
		self.m_string = ""

	def read(self,cVariable):
		self.m_string = cVariable.getString(65535)
		return True

	def write(self,cVariable):
		cVariable.putString(self.m_string,65535)
		return True

