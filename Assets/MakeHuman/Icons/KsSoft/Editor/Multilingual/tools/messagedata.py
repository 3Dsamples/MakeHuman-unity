#!/usr/bin/env python
# -*- coding: utf-8 -*-
import sys
import os
sys.path.append(os.path.join(os.path.abspath(os.path.dirname(__file__)), 'lib'))

from CVariable import *
from Id import *
from CError import *
from CExcel import *
from t_MessageDataOne import t_MessageDataOne
from t_MessageDataSheet import t_MessageDataSheet
from t_MessageData import t_MessageData
from t_MessageDataHeader import t_MessageDataHeader

def help():
	print("messagedata [-v version] excelfile [outputfile]")

# 引数解析
excel_file,clifile,version = CExcel.checkArgv("messagedata.xlsx","D",help)

#
# 定数
#
cError = CError()

# excel 読み込み
excel = CExcel(excel_file)

dMessageData = {}

# シートの数だけループ
for sheetname in excel.sheetnames:
	sheet = excel.getSheetByName(sheetname)
	mapId = {}
	cError.at(sheetname)
	# シート内を解析
	data = excel.convert(sheet,cError)
	if (data is None or len(data) == 0):
		continue
	# 格納されているロケールを調べる.
	if (len(dMessageData) == 0):
		for locale in list(data[0].keys()):
			if (locale == "ID"):
				continue
			tMD = t_MessageData()
			tMD.m_locale = locale
			dMessageData[locale] = tMD
	else:
		# error check(localeの設定が各シートでそろっているか調べる)
		for locale in list(data[0].keys()):
			if (locale not in data[0]):
				cError.add("locale is not find!:"  + locale)
	isFirst = True
	for locale in list(dMessageData.keys()):
		tMDS = t_MessageDataSheet()
		tMDS.m_type = CFiveCC(sheetname,cError).value
		dMessageData[locale].m_aSheet.append(tMDS)
		# 行の数だけ値を追加していく
		for one in data:
			tMDO = t_MessageDataOne()
			id = one['ID']
			cError.line(id)
			tMDO.m_id = CMulId(id,cError).value
			if (locale in one):
				tMDO.m_value = one[locale]
			else:
				tMDO.m_value = ""
			tMDS.m_aContents.append(tMDO)
			if (isFirst):
				cError.append(mapId,tMDO.m_id,tMDO)
		isFirst = False

# エラーチェック
#
if (cError.isError()):
	cError.output()
	exit(-1)

for locale in list(dMessageData.keys()):
	cVariable = CWriteVariable()
	dMessageData[locale].write(cVariable)
	cVariable.output(clifile + "." + locale + ".bin")

CExcel.quit()
