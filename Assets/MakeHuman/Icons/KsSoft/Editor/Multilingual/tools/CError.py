import sys
from Id import *

class CError:
	msgs	= []
	where	= ''
	nowline	= ''
	def __init__(self):
		msg = []

	def at(self,where):
		self.where = where

	def line(self,l):
		self.nowline = l

	def output(self):
		for msg in self.msgs:
			print(msg + '\n')
	
	def add(self,msg):
		nowline = ''
		if isinstance(self.nowline,str):
			nowline = self.nowline
		else:
		 	nowline = str(self.nowline)
		self.msgs.append(msg + ' at ' + self.where + '(' + nowline + ')')

	def addPlain(self,_msg,_where,_nowline):
		nowline = ''
		if isinstance(_nowline,str):
			nowline = _nowline
		else:
		 	nowline = str(_nowline)
		self.msgs.append(_msg + ' at ' + _where + '(' + nowline + ')')
	
	#
	# 辞書に存在するかチェックする
	# 存在しているときは値を返す
	#
	def check(self,dic,key,msg):
		if (key in dic):
			return dic[key]
		self.add('can\'t find "' + key + '" in ' + msg)
		return None
	# キーがマルチIDの場合こちらを使う.
	def checkm(self,dic,mKey,msg):
		mId = CMulId(mKey,self)
		if (mId.value in dic):
			return dic[mId.value]
		self.add('can\'t find ' + mId.toString() + ' in ' + msg)
		return None
	#
	# 辞書に追加する
	# すでに辞書内に同じキーが存在していたらエラーとする
	#
	def append(self,dic,key,value):
		if (key in dic):
			self.add('dulplicate key:' + CMulId(key,self).toString())
			return False
		dic[key] = value
		return True
	#
	# 文字列として正しいかチェックする.
	#
	def checkString(self,msg):
		value = msg
		try:
			if (value is None):
				value = ''
			if (type(value) is int or type(value) is float or type(value) is int):
				value = str(value)
		except:
			res=''
			idx = 0
			try:
				for c in list(value):
					# try to put unknown characters thru print statement:
					print(c)
					idx += 1
					res += c
			except:
				pass
			self.add("char code error:" + res + "(" + str(idx) +")")
		return msg
	
	def isError(self):
		if (len(self.msgs) == 0):
			return False
		return True
