from CVariable import CReadVariable,CWriteVariable
from Vector import *
from t_WindowData import	t_WindowData
#/==========================================================================
#*!
#	@brief	t_WindowBin
#/
class t_WindowBin:
	#/==========================================================================
	#*!
	#	@brief	Member
	#/
	m_name = ""	#std::string
	m_exportpath = ""	#std::string
	m_mAssetbundle = 0	#U32
	m_data = t_WindowData()	#t_WindowData
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
		self.m_name = ""
		self.m_exportpath = ""
		self.m_mAssetbundle = 0
		self.m_data = t_WindowData()

	def read(self,cVariable):
		self.m_name = cVariable.getString(255)
		self.m_exportpath = cVariable.getString(255)
		self.m_mAssetbundle = cVariable.getU32()
		if (not self.m_data.read(cVariable)):
			return False
		return True

	def write(self,cVariable):
		cVariable.putString(self.m_name,255)
		cVariable.putString(self.m_exportpath,255)
		cVariable.putU32(self.m_mAssetbundle)
		if (not self.m_data.write(cVariable)):
			return False
		return True

