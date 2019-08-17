from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_BillboardFrame
#/
class t_BillboardFrame:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_texid = 0	#U32
	m_color = 0	#U32
	m_frame = 0.0	#float
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
		self.m_texid = 0
		self.m_color = 0
		self.m_frame = 0.0

	def read(self,cVariable):
		self.m_texid = cVariable.getU32()
		self.m_color = cVariable.getU32()
		self.m_frame = cVariable.getFloat()
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_texid)
		cVariable.putU32(self.m_color)
		cVariable.putFloat(self.m_frame)
		return True

