from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_SoundEffect
#/
class t_SoundEffect:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_id = 0	#U32
	m_mAssetBundle = 0	#U32
	m_idClip = 0	#U32
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
		self.m_mAssetBundle = 0
		self.m_idClip = 0

	def read(self,cVariable):
		self.m_id = cVariable.getU32()
		self.m_mAssetBundle = cVariable.getU32()
		self.m_idClip = cVariable.getU32()
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_id)
		cVariable.putU32(self.m_mAssetBundle)
		cVariable.putU32(self.m_idClip)
		return True

