from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_Message import	t_Message
#/==========================================================================
#*!
#	@brief	GmsvMessage
#/
class GmsvMessage:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_eChatMode = 0	#S8
	m_tMessage = t_Message()	#t_Message
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
		self.m_eChatMode = 0
		self.m_tMessage = t_Message()

	def read(self,cVariable):
		self.m_eChatMode = cVariable.getS8()
		if (not self.m_tMessage.read(cVariable)):
			return False
		return True

	def write(self,cVariable):
		cVariable.putS8(self.m_eChatMode)
		if (not self.m_tMessage.write(cVariable)):
			return False
		return True

