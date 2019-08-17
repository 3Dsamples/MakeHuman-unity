from CVariable import CReadVariable,CWriteVariable
from Vector import *
#/==========================================================================
#*!
#	@brief	t_EffectAttribute
#/
class t_EffectAttribute:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_mId = 0	#U32
	m_mNoneId = 0	#U32
	m_mFireId = 0	#U32
	m_mWaterId = 0	#U32
	m_mEarthId = 0	#U32
	m_mLightId = 0	#U32
	m_mDarkId = 0	#U32
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
		self.m_mId = 0
		self.m_mNoneId = 0
		self.m_mFireId = 0
		self.m_mWaterId = 0
		self.m_mEarthId = 0
		self.m_mLightId = 0
		self.m_mDarkId = 0

	def read(self,cVariable):
		self.m_mId = cVariable.getU32()
		self.m_mNoneId = cVariable.getU32()
		self.m_mFireId = cVariable.getU32()
		self.m_mWaterId = cVariable.getU32()
		self.m_mEarthId = cVariable.getU32()
		self.m_mLightId = cVariable.getU32()
		self.m_mDarkId = cVariable.getU32()
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_mId)
		cVariable.putU32(self.m_mNoneId)
		cVariable.putU32(self.m_mFireId)
		cVariable.putU32(self.m_mWaterId)
		cVariable.putU32(self.m_mEarthId)
		cVariable.putU32(self.m_mLightId)
		cVariable.putU32(self.m_mDarkId)
		return True

