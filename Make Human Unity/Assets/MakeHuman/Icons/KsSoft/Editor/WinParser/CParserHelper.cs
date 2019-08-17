//==============================================================================================
/*!CParserHelper.cs
	@file  CParserHelper.cs
*/
//==============================================================================================
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using KS;

//==============================================================================================
/*!CParserHelper
	@brief	CParserHelper
	@note	
*/
//==============================================================================================
public class CParserHelper {
	public static int preprocess(out string output,string sCmd,string sArguments,string sFile,bool bErr) {
		output = "";
		Process prc = new Process();
		string err = "";
		int res = 0;
		try {
			prc.StartInfo.FileName = sCmd;
			prc.StartInfo.Arguments = sArguments + " " + sFile;
			prc.StartInfo.CreateNoWindow = true;
			prc.StartInfo.UseShellExecute = false;
			prc.StartInfo.RedirectStandardOutput = true;
			prc.StartInfo.RedirectStandardError = true;
			// process execute
			prc.Start();
			output = prc.StandardOutput.ReadToEnd();
			err = prc.StandardError.ReadToEnd();
			// process finish
			prc.WaitForExit();
			prc.Close();
			prc.Dispose();
		}
		catch (System.Exception e) {
			if (bErr) {
				UnityEngine.Debug.LogError(e);
			}
			res = -1;
		}
		if (bErr && !string.IsNullOrEmpty(err)) {
			res = -1;
			err = err.Replace("\r\n","\n");
			UnityEngine.Debug.LogError(err);
		}
		output = output.Replace("\r\n","\n");
		return res;
	}	
};
