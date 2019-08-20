from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_WinPropertyTexId
#/
class t_WinPropertyTexId:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_texId = 0	#U32
	m_partId = 0	#U32
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
		self.m_texId = 0
		self.m_partId = 0

	def read(self,cVariable):
		self.m_texId = cVariable.getU32()
		self.m_partId = cVariable.getU32()
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_texId)
		cVariable.putU32(self.m_partId)
		return True

