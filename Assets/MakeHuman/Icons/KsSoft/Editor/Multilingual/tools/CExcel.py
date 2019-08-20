#!/usr/bin/env python
# -*- coding: utf-8 -*-
import sys
import os
import os.path
import re
import glob
import win32com
import win32com.client as win32
import warnings
from Id import CColor
from CVariable import CReadVariable,CWriteVariable
from CError import *

class CExcel:
#	m_xlApp = win32.Dispatch("Excel.Application")
	m_xlApp = win32.DispatchEx("Excel.Application")
	m_fntDefault = "fn"
	#==============================================
	#!Constructor
	# @brief constructor
	#==============================================
	def __init__(self,xlsfile,abspath = False,readonly = True):
#		warnings.simplefilter("ignore")
		self.line = 0
		self.workbook = None
		self.open(xlsfile,abspath,readonly)
		self.name = xlsfile
	#/==========================================================================
	#*!
	#	@brief	Destructor
	#/
	def __del__( self ):
		self.close()
		
	@staticmethod
	def	quit():
		CExcel.m_xlApp.Quit()
		CExcel.m_xlApp = None
	#/==========================================================================
	#*!
	#	@brief	open(filename)
	#/
	def open(self,xlsfile,abspath = False,readonly = True):
		self.close()
		self.name = xlsfile
		if (abspath):
			path = xlsfile
		else:
			path = os.getcwd() + "\\" + xlsfile
		if (not isinstance(path,str)):
			path = str(path,'utf8')
		self.workbook = self.m_xlApp.Workbooks.Open(path,False,readonly)
		nSheets = self.workbook.Worksheets.Count
		self.sheetnames = []
		for i  in range(nSheets):
			ws = self.workbook.Worksheets(i + 1)
			self.sheetnames.append(ws.Name)
	#/==========================================================================
	#*!
	#	@brief	close()
	#/
	def close(self):
		if (self.m_xlApp != None and self.workbook != None):
			self.workbook.Close()
		self.workbook = None
	#==============================================
	#!getSheetNum
	# @brief シートの総数を取得
	#==============================================
	def	getSheetNum(self):
		return len(self.sheetnames)
	#==============================================
	#!getSheetName
	# @brief シートの名前を取得
	#==============================================
	def getSheetName(self,index):
		if (index >= len(self.sheetnames)):
			return None
		return self.sheetnames[index]
	#==============================================
	#!getSheet
	# @brief シートを取得
	#==============================================
	def getSheet(self,index):
		if (index >= self.getSheetNum()):
			return None
		return self.workbook.Worksheets(index + 1)
	#==============================================
	#!getSheetByName
	# @brief シートを名前を使って取得
	#==============================================
	def getSheetByName(self,sheetname):
		idx = 1
		for name in self.sheetnames:
			if (name == sheetname):
				return self.workbook.Worksheets(idx)
			idx = idx + 1
		print("can't open this seet:" + sheetname)
		return None
	#==============================================
	#!convert
	# @brief シートのデータを連想配列の配列形式に変更する
	#==============================================
	def convert(self,sheet,error,inDetail = True,config = 'D'):
		wk = sheet.UsedRange
		width = wk.Columns.Count
		height = wk.Rows.Count
		values = wk.Value
		if (values is None):
			return None
		rsel = re.compile('\[' + config + '\]')
		rrich = re.compile('@richtext')
		rcolor = re.compile('\@color')
		rcolorx2 = re.compile('\@colorx2')
		tags = []
		# 原点[ORIGIN]を探す
		ox = None
		oy = None
		row_sel = None
		for y in range(height):
			for x in range(width):
				if (values[y][x] == '[ORIGIN]'):
					ox = x
					oy = y
					# Debug,Releaseをを選択
					for x in range(ox + 1,width):
						v = values[oy + 1][x]
						if (v is not None and rsel.search(v) is not None):
							tag = values[oy][x]
							if (tag is None):
								# 行単位で選択するためのカラム
								row_sel = x
							else:
								fmt = 0
								if (inDetail):
									if (rrich.search(v) is not None):
										fmt = 1
									elif (rcolorx2.search(v) is not None):
										fmt = 3
									elif (rcolor.search(v) is not None):
										fmt = 2
								tags.append([tag,x,fmt])
					break
			if (ox is not None):
				break
		if (oy is None):
#			print "can't find keyword '[ORIGIN]'"
			return None
		# 結果を連想配列にして突っ込む
		r = []
		self.line = oy + 3
		self.tags = tags
		for y in range(oy + 2,height):
			one = {}
			skip = False
			for tag in tags:
				if (row_sel is not None and values[y][row_sel] is None):
					skip = True
					break
				x = tag[1]
				v = values[y][x]
				# フォーマット解析.
				if (tag[2] == 1):	#richtext
					if (v is not None):
						cell = wk.Cells(y + 1,x + 1)
						bold = False
						fontsize = None
						color = None
						l = len(v) + 1
						tempv = v
						v = ""
						for idx in range(1,l):
							chr = cell.GetCharacters(idx,1)
							fnt = chr.Font
							if (fnt.Size != fontsize):
								if (fontsize is None):
									fontsize = fnt.Size
								else:
									fontsize = fnt.Size
									v += "\\f[" + (CExcel.m_fntDefault + str(int(fontsize))) + "]"
							if (fnt.Bold != bold):
								bold = fnt.Bold
								if (bold):
									v += "\\b"
								else:
									v += "\\N"
							if (fnt.Color != color):
								if (color is None):
									color = fnt.Color
								else:
									color = fnt.Color
									v += "\\C[" + ('%x' % CExcel.getColor(color,None,error)) + "]"
							v += tempv[idx - 1]
				elif (tag[2] == 2):	#color
					v = CExcel.getColor(wk.Cells(y + 1,x + 1).Interior.Color,v,error)
				elif (tag[2] == 3):	#colorx2
					v = CExcel.getColorX2(wk.Cells(y + 1,x + 1).Interior.Color,v,error)
				if (type(v) is float and float(int(v)) == v):	#整数で格納可能なら整数で格納しておく.
					v = int(v)
				one[tag[0]] = v;
			if (not skip):
				r.append(one)
		return r
	#==============================================
	#!getValues
	# @brief 二次元配列の形でシート内の全てのデータを取り込み返す.
	#==============================================
	def getValues(self,sheet,sheetrange = None):
		if (sheetrange is None):
			wk = sheet.UsedRange
		else:
			wk = sheet.Range(sheetrange)
		x_cnt = wk.Columns.Count
		y_cnt = wk.Rows.Count
		values = wk.Value
		if (values is None):
			return None
		utf8values = []
		for iy in range(y_cnt):
			line = []
			utf8values.append(line)
			for ix in range(x_cnt):
				v = values[iy][ix]
				line.append(v)
		return utf8values
	#==============================================
	#!create
	# @brief シートデータを特定の値をキーとした連想配列に格納する
	#==============================================
	def create(self,mapSheet,sheetname,keytag,cError,config = 'D'):
		sheet = self.getSheetByName(sheetname)
		line = self.convert(sheet,cError,False,config)
		nLine = 0
		result = True
		for one in line:
			if (keytag not in one):
				cError.addPlain("can't find key:" + keytag,self.name + ":" + sheet,nLine)
				return False
			key = one[keytag]
			if (key is None):
				continue
			if (key in mapSheet):
				cError.addPlain("already exist key:" + key,self.name + ":" + sheet,nLine)
				result = False
			mapSheet[key] = one
			nLine += 1
		return result
	#==============================================
	#!create
	# @brief シートデータを特定の値をキーとした連想配列に格納する
	#==============================================
	def createValueMap(self,mapSheet,sheetname,keytag,valuetag,cError,config = 'D'):
		if (isinstance(sheetname,list)):
			line = sheetname
		else:
			sheet = self.getSheetByName(sheetname)
			line = self.convert(sheet,cError,False,config)
		nLine = 0
		result = True
		for one in line:
			if (keytag not in one):
				cError.addPlain("can't find key tag:" + keytag,self.name + ":" + sheetname,nLine)
				return False
			if (valuetag not in one):
				cError.addPlain("can't find value tag:" + valuetag,self.name + ":" + sheetname,nLine)
				return False
			key = one[keytag]
			if (key is None):
				continue
			if (key in mapSheet):
				cError.addPlain("already exist key:" + key,self.name + ":" + sheetname,nLine)
				result = False
			mapSheet[key] = one[valuetag]
			nLine += 1
		return result
	#==============================================
	#!create
	# @brief シートデータを特定の値をキーとした連想配列に格納する
	#==============================================
	def createMulIdMap(self,mapSheet,sheetname,keytag,valuetag,cError,config = 'D'):
		if (isinstance(sheetname,list)):
			line = sheetname
			mapSheet[':::: sheetname ::::'] = 'None'
		else:
			sheet = self.getSheetByName(sheetname)
			line = self.convert(sheet,cError,False,config)
			mapSheet[':::: sheetname ::::'] = sheetname
		mapSheet[':::: keytag ::::'] = keytag
		mapSheet[':::: valuetag ::::'] = valuetag
		nLine = 0
		result = True
		for one in line:
			if (keytag not in one):
				cError.addPlain("can't find key tag:" + keytag,self.name + ":" + sheetname,nLine)
				return False
			if (valuetag not in one):
				cError.addPlain("can't find value tag:" + valuetag,self.name + ":" + sheetname,nLine)
				return False
			val = CMulId(one[valuetag],cError).value
			mapSheet[one[valuetag]] = val
			key = one[keytag]
			if (key is None):
				continue
			if (key in mapSheet):
				cError.addPlain("already exist key:" + key,self.name + ":" + sheetname,nLine)
				result = False
			mapSheet[key] = val
			nLine += 1
		return result		
	#==============================================
	#!getFromMap
	# @brief 連想配列化したシートから特定の値を取り出す
	#        マルチIDの場合、直値として扱う.
	#==============================================
	def getFromMap(self,mapSheet,key,cError):
		if (key is None):
			if ("None" in mapSheet):
				return mapSheet["None"]
			return 0
		if (key not in mapSheet):
			if (isinstance(key,int)):
				return key
			if (CMulId.isMulId(key)):
				return CMulId(key,cError).value
			cError.add("can't find value " + str(key) + ":" + "ref[" + self.name + "," + mapSheet[':::: sheetname ::::'] + "," + mapSheet[':::: keytag ::::'] + "," + mapSheet[':::: valuetag ::::'] + "] " )
			return None
		return mapSheet[key]

	#
	# 以下のコマンドライン引数を解析する
	#   cmd [-v version] excelfile [outputfile]
	#  param:excelpath	:エクセルパス
	#  param:version	:Excelで有効にするカラムを選択
	#  param:help		:エラー発生時に呼び出すヘルプ
	#
	@staticmethod
	def checkArgv(excelpath,version,help):
		#初期値
		srcpath = None
		outpath = None
		# 引数解析
		narg = len(sys.argv)
		i = 1
		while (i < narg):
			arg = sys.argv[i]
			if (arg == '-v'):
				i += 1
				if (i >= narg):
					help()
					sys.exit()
				version = sys.argv[i]
			elif (arg == '-h'):
				help()
				sys.exit()
			else:
				if (srcpath == None):
					srcpath = arg
				elif (outpath == None):
					outpath = arg
				else:
					help()
					sys.exit()
			i += 1
		if (srcpath == None):
			srcpath = excelpath
		if (outpath == None):
			outpath,ext = os.path.splitext(srcpath)
		return [srcpath,outpath,version]
	@staticmethod
	def getColor(rgb,val,error):
		#print "src:" + str(rgb) + ":" + str(val) + str(type(val))
		if (isinstance(val,str) and ',' in val):
			argb = CColor(val,error)
			b = int(argb.b * 255)
			g = int(argb.g * 255)
			r = int(argb.r * 255)
			a = int(argb.a * 255)
		else:
			rgb = int(rgb)
			r = rgb & 255
			g = (rgb >> 8) & 255
			b = (rgb >> 16) & 255
			a = 255
			if (isinstance(val,int)):
				a = int(val)
			elif (isinstance(val,(float))):
				a = int(val * 255.0)
				if (a < 0):
					a = 0
				elif (a > 255):
					a = 255
		#print str(r) + ":" + str(g) + ":" + str(b) + ":" + str(a)
		return (a << 24)|(r << 16)|(g << 8)|b
	@staticmethod
	def getColorX2(rgb,alpha,error):
		if (isinstance(rgb,str) and ',' in rgb):
			argb = CColor(val,error)
			b = int(argb.b * 127)
			g = int(argb.g * 127)
			r = int(argb.r * 127)
			a = int(argb.a * 127)
		else:
			rgb = int(rgb)
			r = (rgb & 255) >> 1
			g = ((rgb >> 8) & 255) >> 1
			b = ((rgb >> 16) & 255) >> 1
			a = 255
			if (isinstance(val,int)):
				a = int(val)
			elif (isinstance(val,(float))):
				a = int(val * 255.0)
				if (a < 0):
					a = 0
				elif (a > 255):
					a = 255
		#print str(r) + ":" + str(g) + ":" + str(b) + ":" + str(a)
		return (a << 24)|(r << 16)|(g << 8)|b
