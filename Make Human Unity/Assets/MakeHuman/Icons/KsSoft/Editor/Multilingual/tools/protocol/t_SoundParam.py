from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_SoundParam
#/
class t_SoundParam:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_id = 0	#U32
	m_mClip = 0	#U32
	m_volume = 0	#U8
	m_priority = 0	#U8
	m_group = 0	#U8
	m_maxPolyphony = 0	#U8
	m_minDistance = 0.0	#float
	m_maxDistance = 0.0	#float
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
		self.m_mClip = 0
		self.m_volume = 0
		self.m_priority = 0
		self.m_group = 0
		self.m_maxPolyphony = 0
		self.m_minDistance = 0.0
		self.m_maxDistance = 0.0

	def read(self,cVariable):
		self.m_id = cVariable.getU32()
		self.m_mClip = cVariable.getU32()
		self.m_volume = cVariable.getU8()
		self.m_priority = cVariable.getU8()
		self.m_group = cVariable.getU8()
		self.m_maxPolyphony = cVariable.getU8()
		self.m_minDistance = cVariable.getFloat()
		self.m_maxDistance = cVariable.getFloat()
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_id)
		cVariable.putU32(self.m_mClip)
		cVariable.putU8(self.m_volume)
		cVariable.putU8(self.m_priority)
		cVariable.putU8(self.m_group)
		cVariable.putU8(self.m_maxPolyphony)
		cVariable.putFloat(self.m_minDistance)
		cVariable.putFloat(self.m_maxDistance)
		return True

