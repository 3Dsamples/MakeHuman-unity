from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_MessageDataOne
#/
class t_MessageDataOne:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_id = 0	#U32
	m_value = ""	#std::string
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
		self.m_id = 0
		self.m_value = ""

	def read(self,cVariable):
		self.m_id = cVariable.getU32()
		self.m_value = cVariable.getString(16384)
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_id)
		cVariable.putString(self.m_value,16384)
		return True

