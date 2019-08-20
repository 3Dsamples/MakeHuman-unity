from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_MessageArg import	t_MessageArg
#/==========================================================================
#*!
#	@brief	t_Message
#/
class t_Message:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_mSheetId = 0	#U32
	m_mId = 0	#U32
	m_aArg = []	#t_MessageArg
	m_aString = []	#std::string
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
		self.m_mSheetId = 0
		self.m_mId = 0
		self.m_aArg = []
		self.m_aString = []

	def read(self,cVariable):
		self.m_mSheetId = cVariable.getU32()
		self.m_mId = cVariable.getU32()
		n = cVariable.getU8()
		if (n > 255) :
			cVariable.error('array size error in m_aArg')
			return False
		self.m_aArg = [None]*n
		for i in range(n):
			self.m_aArg[i] = t_MessageArg()
			if (not self.m_aArg[i].read(cVariable)):
				return False
		n = cVariable.getU8()
		if (n > 255) :
			cVariable.error('array size error in m_aString')
			return False
		self.m_aString = [0]*n
		for i in range(n):
			self.m_aString[i] = cVariable.getString(255)
		return True

	def write(self,cVariable):
		cVariable.putU32(self.m_mSheetId)
		cVariable.putU32(self.m_mId)
		if (len(self.m_aArg) > 255) :
			cVariable.error('array size error in m_aArg')
			return False
		n = len(self.m_aArg)
		cVariable.putU8(n)
		for i in range(n):
			if (not self.m_aArg[i].write(cVariable)):
				return False
		if (len(self.m_aString) > 255) :
			cVariable.error('array size error in m_aString')
			return False
		n = len(self.m_aString)
		cVariable.putU8(n)
		for i in range(n):
			cVariable.putString(self.m_aString[i],255)
		return True

