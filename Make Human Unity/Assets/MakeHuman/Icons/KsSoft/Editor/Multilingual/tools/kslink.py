#!/usr/bin/env python
import os
import shutil
if (os.name == "nt"):
	import ctypes
	from ctypes import Structure
	from ctypes import byref
	import ctypes.wintypes as wintypes
	from ctypes import addressof
	import subprocess
	FILE_ATTRIBUTE_DIRECTORY = 16  # (0x10)
	FILE_ATTRIBUTE_REPARSE_POINT = 1024  # (0x400)
	MAX_PATH = 260
	class FILETIME(Structure):
	    _fields_ = [("dwLowDateTime", wintypes.DWORD),
	                ("dwHighDateTime", wintypes.DWORD)]	
	class WIN32_FIND_DATAW(Structure):
	    _fields_ = [("dwFileAttributes", wintypes.DWORD),
	                ("ftCreationTime", FILETIME),
	                ("ftLastAccessTime", FILETIME),
	                ("ftLastWriteTime", FILETIME),
	                ("nFileSizeHigh", wintypes.DWORD),
	                ("nFileSizeLow", wintypes.DWORD),
	                ("dwReserved0", wintypes.DWORD),
	                ("dwReserved1", wintypes.DWORD),
	                ("cFileName", wintypes.WCHAR * MAX_PATH),
	                ("cAlternateFileName", wintypes.WCHAR * 20)]
	def isJunction(dst):
		if not os.path.isdir(dst):
			return False
		dst = str(dst)
		data = WIN32_FIND_DATAW()
		h = ctypes.windll.kernel32.FindFirstFileW(dst,byref(data))
		if (data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY and data.dwFileAttributes & FILE_ATTRIBUTE_REPARSE_POINT):
			return True
		return False
	def makeJunction(dst,src):
		subprocess.call(["cmd","/c","mklink","/j",dst,src])

def isLink(dst):
	if (os.name == "nt"):
		return isJunction(dst)
	if (os.path.islink(dst)):
		return True
	return False

def removeFolder(dst):
	if (os.path.islink(dst) or os.path.isfile(dst)):
		os.remove(dst)
		return
	if (os.name == "nt" and isJunction(dst)):
		os.rmdir(dst)
		return
	if (os.path.isdir(dst)):
		shutil.rmtree(dst)
		return

def forceLink(dst,src):
	dst = os.path.abspath(dst)
	src = os.path.abspath(src)
	if (not os.path.isdir(src)):
		return False
	removeFolder(dst)
	if (os.name == "nt"):
		makeJunction(dst,src)
	else:
		os.symlink(src, dst)
	return True
