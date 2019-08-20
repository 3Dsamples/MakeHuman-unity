from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_MessageDataSheet import	t_MessageDataSheet
#/==========================================================================
#*!
#	@brief	t_MessageData
#/
class t_MessageData:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_locale = ""	#std::string
	m_aSheet = []	#t_MessageDataSheet
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
		self.m_locale = ""
		self.m_aSheet = []

	def read(self,cVariable):
		self.m_locale = cVariable.getString(32)
		n = cVariable.getU8()
		if (n > 255) :
			cVariable.error('array size error in m_aSheet')
			return False
		self.m_aSheet = [None]*n
		for i in range(n):
			self.m_aSheet[i] = t_MessageDataSheet()
			if (not self.m_aSheet[i].read(cVariable)):
				return False
		return True

	def write(self,cVariable):
		cVariable.putString(self.m_locale,32)
		if (len(self.m_aSheet) > 255) :
			cVariable.error('array size error in m_aSheet')
			return False
		n = len(self.m_aSheet)
		cVariable.putU8(n)
		for i in range(n):
			if (not self.m_aSheet[i].write(cVariable)):
				return False
		return True

