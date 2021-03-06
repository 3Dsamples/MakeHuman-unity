// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough, QUT 2005-2014
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.5.2
// Machine:  SEIKICHI-PC
// DateTime: 2019/07/05 9:14:39
// UserName: Seikichi
// Input file <CSeParser.y - 2019/07/05 9:13:50>

// options: lines gplex

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;
using QUT.Gppg;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using KS;

namespace CSeParser
{
internal enum Tokens {error=128,EOF=129,TK_ERROR=130,OR=131,AND=132,
    EQ=133,GT=134,GE=135,LT=136,LE=137,NE=138,
    NOT=139,SHIFT_LEFT=140,SHIFT_RIGHT=141,IDENTIFIER=142,VARIABLE=143,LIT_STRING=144,
    SE_DEF=145,LIT_INTEGER=146,LIT_FLOAT=147,CONSTANT=148,VOLUME=149,PRIORITY=150,
    GROUP=151,POLYPHONY=152,DISTANCE=153,SUBSTITUTE_ADD=154,SUBSTITUTE_SUB=155,SUBSTITUTE_MUL=156,
    SUBSTITUTE_DIV=157,POW=158,INCREMENT=159,DECREMENT=160,NEG=161};

internal partial struct ValueType
#line 17 "CSeParser.y"
       {
	public char					c;
	public string				str;
	public double				value;
	public uint					uvalue;
	public Vector2				pair;
	public CSeParserProperty	prop;
	public CSeParserDef			def;
}
#line default
// Abstract base class for GPLEX scanners
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
internal abstract class ScanBase : AbstractScanner<ValueType,LexLocation> {
  private LexLocation __yylloc = new LexLocation();
  public override LexLocation yylloc { get { return __yylloc; } set { __yylloc = value; } }
  protected virtual bool yywrap() { return true; }
}

// Utility class for encapsulating token information
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
internal class ScanObj {
  public int token;
  public ValueType yylval;
  public LexLocation yylloc;
  public ScanObj( int t, ValueType val, LexLocation loc ) {
    this.token = t; this.yylval = val; this.yylloc = loc;
  }
}

[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
internal partial class Parser: ShiftReduceParser<ValueType, LexLocation>
{
  // Verbatim content from CSeParser.y - 2019/07/05 9:13:50
#line 10 "CSeParser.y"
//==============================================================================================
/*!Sound Effect Parser.
	@file  CSEParser.y
*/
//==============================================================================================
#line default
  // End verbatim content from CSeParser.y - 2019/07/05 9:13:50

#pragma warning disable 649
  private static Dictionary<int, string> aliases;
#pragma warning restore 649
  private static Rule[] rules = new Rule[51];
  private static State[] states = new State[104];
  private static string[] nonTerms = new string[] {
      "statement", "statement_list", "sedef", "substitute", "expression", "id", 
      "property", "property_list", "substitute_in_def", "variable", "top", "$accept", 
      };

  static Parser() {
    states[0] = new State(new int[]{143,50,145,53},new int[]{-11,1,-2,3,-1,103,-4,5,-10,7,-3,51});
    states[1] = new State(new int[]{129,2});
    states[2] = new State(-1);
    states[3] = new State(new int[]{143,50,145,53,129,-2},new int[]{-1,4,-4,5,-10,7,-3,51});
    states[4] = new State(-4);
    states[5] = new State(new int[]{59,6});
    states[6] = new State(-5);
    states[7] = new State(new int[]{61,8,154,42,155,44,156,46,157,48,59,-7});
    states[8] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,9});
    states[9] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-8});
    states[10] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,11});
    states[11] = new State(new int[]{124,-37,38,12,94,14,43,-37,45,-37,140,-37,141,-37,158,-37,42,-37,47,-37,37,-37,59,-37,41,-37,44,-37});
    states[12] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,13});
    states[13] = new State(-38);
    states[14] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,15});
    states[15] = new State(new int[]{124,-39,38,12,94,-39,43,-39,45,-39,140,-39,141,-39,158,-39,42,-39,47,-39,37,-39,59,-39,41,-39,44,-39});
    states[16] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,17});
    states[17] = new State(new int[]{124,10,38,12,94,14,43,-40,45,-40,140,20,141,22,158,24,42,26,47,28,37,30,59,-40,41,-40,44,-40});
    states[18] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,19});
    states[19] = new State(new int[]{124,10,38,12,94,14,43,-41,45,-41,140,20,141,22,158,24,42,26,47,28,37,30,59,-41,41,-41,44,-41});
    states[20] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,21});
    states[21] = new State(new int[]{124,10,38,12,94,14,43,-42,45,-42,140,-42,141,-42,158,24,42,26,47,28,37,30,59,-42,41,-42,44,-42});
    states[22] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,23});
    states[23] = new State(new int[]{124,10,38,12,94,14,43,-43,45,-43,140,-43,141,-43,158,24,42,26,47,28,37,30,59,-43,41,-43,44,-43});
    states[24] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,25});
    states[25] = new State(new int[]{124,10,38,12,94,14,43,-44,45,-44,140,-44,141,-44,158,-44,42,-44,47,-44,37,-44,59,-44,41,-44,44,-44});
    states[26] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,27});
    states[27] = new State(new int[]{124,10,38,12,94,14,43,-45,45,-45,140,-45,141,-45,158,-45,42,-45,47,-45,37,-45,59,-45,41,-45,44,-45});
    states[28] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,29});
    states[29] = new State(new int[]{124,10,38,12,94,14,43,-46,45,-46,140,-46,141,-46,158,-46,42,-46,47,-46,37,-46,59,-46,41,-46,44,-46});
    states[30] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,31});
    states[31] = new State(new int[]{124,10,38,12,94,14,43,-47,45,-47,140,-47,141,-47,158,-47,42,-47,47,-47,37,-47,59,-47,41,-47,44,-47});
    states[32] = new State(-34);
    states[33] = new State(-35);
    states[34] = new State(-36);
    states[35] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,36});
    states[36] = new State(-48);
    states[37] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,38});
    states[38] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-49,41,-49,44,-49});
    states[39] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,40});
    states[40] = new State(new int[]{41,41,124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30});
    states[41] = new State(-50);
    states[42] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,43});
    states[43] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-9});
    states[44] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,45});
    states[45] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-10});
    states[46] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,47});
    states[47] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-11});
    states[48] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,49});
    states[49] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-12});
    states[50] = new State(-19);
    states[51] = new State(new int[]{59,52});
    states[52] = new State(-6);
    states[53] = new State(new int[]{40,54});
    states[54] = new State(new int[]{144,96,146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-6,55,-5,97});
    states[55] = new State(new int[]{44,56,41,98});
    states[56] = new State(new int[]{144,96,146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-6,57,-5,97});
    states[57] = new State(new int[]{41,58});
    states[58] = new State(new int[]{123,59});
    states[59] = new State(new int[]{149,64,150,67,151,70,152,73,153,76,143,50},new int[]{-8,60,-7,95,-9,83,-10,84});
    states[60] = new State(new int[]{59,61});
    states[61] = new State(new int[]{125,62,149,64,150,67,151,70,152,73,153,76,143,50},new int[]{-7,63,-9,83,-10,84});
    states[62] = new State(-20);
    states[63] = new State(-23);
    states[64] = new State(new int[]{61,65});
    states[65] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,66});
    states[66] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-24});
    states[67] = new State(new int[]{61,68});
    states[68] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,69});
    states[69] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-25});
    states[70] = new State(new int[]{61,71});
    states[71] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,72});
    states[72] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-26});
    states[73] = new State(new int[]{61,74});
    states[74] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,75});
    states[75] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-27});
    states[76] = new State(new int[]{61,77});
    states[77] = new State(new int[]{44,81,146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,78});
    states[78] = new State(new int[]{44,79,124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-29});
    states[79] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,80});
    states[80] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-28});
    states[81] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,82});
    states[82] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-30});
    states[83] = new State(-31);
    states[84] = new State(new int[]{61,85,154,87,155,89,156,91,157,93,59,-13});
    states[85] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,86});
    states[86] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-14});
    states[87] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,88});
    states[88] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-15});
    states[89] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,90});
    states[90] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-16});
    states[91] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,92});
    states[92] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-17});
    states[93] = new State(new int[]{146,32,147,33,143,34,45,35,126,37,40,39},new int[]{-5,94});
    states[94] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,59,-18});
    states[95] = new State(-22);
    states[96] = new State(-32);
    states[97] = new State(new int[]{124,10,38,12,94,14,43,16,45,18,140,20,141,22,158,24,42,26,47,28,37,30,44,-33,41,-33});
    states[98] = new State(new int[]{123,99});
    states[99] = new State(new int[]{149,64,150,67,151,70,152,73,153,76,143,50},new int[]{-8,100,-7,95,-9,83,-10,84});
    states[100] = new State(new int[]{59,101});
    states[101] = new State(new int[]{125,102,149,64,150,67,151,70,152,73,153,76,143,50},new int[]{-7,63,-9,83,-10,84});
    states[102] = new State(-21);
    states[103] = new State(-3);

    for (int sNo = 0; sNo < states.Length; sNo++) states[sNo].number = sNo;

    rules[1] = new Rule(-12, new int[]{-11,129});
    rules[2] = new Rule(-11, new int[]{-2});
    rules[3] = new Rule(-2, new int[]{-1});
    rules[4] = new Rule(-2, new int[]{-2,-1});
    rules[5] = new Rule(-1, new int[]{-4,59});
    rules[6] = new Rule(-1, new int[]{-3,59});
    rules[7] = new Rule(-4, new int[]{-10});
    rules[8] = new Rule(-4, new int[]{-10,61,-5});
    rules[9] = new Rule(-4, new int[]{-10,154,-5});
    rules[10] = new Rule(-4, new int[]{-10,155,-5});
    rules[11] = new Rule(-4, new int[]{-10,156,-5});
    rules[12] = new Rule(-4, new int[]{-10,157,-5});
    rules[13] = new Rule(-9, new int[]{-10});
    rules[14] = new Rule(-9, new int[]{-10,61,-5});
    rules[15] = new Rule(-9, new int[]{-10,154,-5});
    rules[16] = new Rule(-9, new int[]{-10,155,-5});
    rules[17] = new Rule(-9, new int[]{-10,156,-5});
    rules[18] = new Rule(-9, new int[]{-10,157,-5});
    rules[19] = new Rule(-10, new int[]{143});
    rules[20] = new Rule(-3, new int[]{145,40,-6,44,-6,41,123,-8,59,125});
    rules[21] = new Rule(-3, new int[]{145,40,-6,41,123,-8,59,125});
    rules[22] = new Rule(-8, new int[]{-7});
    rules[23] = new Rule(-8, new int[]{-8,59,-7});
    rules[24] = new Rule(-7, new int[]{149,61,-5});
    rules[25] = new Rule(-7, new int[]{150,61,-5});
    rules[26] = new Rule(-7, new int[]{151,61,-5});
    rules[27] = new Rule(-7, new int[]{152,61,-5});
    rules[28] = new Rule(-7, new int[]{153,61,-5,44,-5});
    rules[29] = new Rule(-7, new int[]{153,61,-5});
    rules[30] = new Rule(-7, new int[]{153,61,44,-5});
    rules[31] = new Rule(-7, new int[]{-9});
    rules[32] = new Rule(-6, new int[]{144});
    rules[33] = new Rule(-6, new int[]{-5});
    rules[34] = new Rule(-5, new int[]{146});
    rules[35] = new Rule(-5, new int[]{147});
    rules[36] = new Rule(-5, new int[]{143});
    rules[37] = new Rule(-5, new int[]{-5,124,-5});
    rules[38] = new Rule(-5, new int[]{-5,38,-5});
    rules[39] = new Rule(-5, new int[]{-5,94,-5});
    rules[40] = new Rule(-5, new int[]{-5,43,-5});
    rules[41] = new Rule(-5, new int[]{-5,45,-5});
    rules[42] = new Rule(-5, new int[]{-5,140,-5});
    rules[43] = new Rule(-5, new int[]{-5,141,-5});
    rules[44] = new Rule(-5, new int[]{-5,158,-5});
    rules[45] = new Rule(-5, new int[]{-5,42,-5});
    rules[46] = new Rule(-5, new int[]{-5,47,-5});
    rules[47] = new Rule(-5, new int[]{-5,37,-5});
    rules[48] = new Rule(-5, new int[]{45,-5});
    rules[49] = new Rule(-5, new int[]{126,-5});
    rules[50] = new Rule(-5, new int[]{40,-5,41});
  }

  protected override void Initialize() {
    this.InitSpecialTokens((int)Tokens.error, (int)Tokens.EOF);
    this.InitStates(states);
    this.InitRules(rules);
    this.InitNonTerminals(nonTerms);
  }

  protected override void DoAction(int action)
  {
#pragma warning disable 162, 1522
    switch (action)
    {
      case 2: // top -> statement_list
#line 62 "CSeParser.y"
                     {	topDef = ValueStack[ValueStack.Depth-1].def;	}
#line default
        break;
      case 3: // statement_list -> statement
#line 65 "CSeParser.y"
                {	CurrentSemanticValue.def = ValueStack[ValueStack.Depth-1].def;	}
#line default
        break;
      case 4: // statement_list -> statement_list, statement
#line 66 "CSeParser.y"
                            {	CurrentSemanticValue.def = ValueStack[ValueStack.Depth-2].def;CurrentSemanticValue.def.next = ValueStack[ValueStack.Depth-1].def;	}
#line default
        break;
      case 5: // statement -> substitute, ';'
#line 70 "CSeParser.y"
                    {	CurrentSemanticValue.def = ValueStack[ValueStack.Depth-2].def;	}
#line default
        break;
      case 6: // statement -> sedef, ';'
#line 71 "CSeParser.y"
                {	CurrentSemanticValue.def = ValueStack[ValueStack.Depth-2].def;	}
#line default
        break;
      case 7: // substitute -> variable
#line 74 "CSeParser.y"
          {
		registVariable(ValueStack[ValueStack.Depth-1].str,0);
		CurrentSemanticValue.def = new CSeParserDef();
	}
#line default
        break;
      case 8: // substitute -> variable, '=', expression
#line 78 "CSeParser.y"
                          {
		registVariable(ValueStack[ValueStack.Depth-3].str,ValueStack[ValueStack.Depth-1].value);
		CurrentSemanticValue.def = new CSeParserDef();
	}
#line default
        break;
      case 9: // substitute -> variable, SUBSTITUTE_ADD, expression
#line 82 "CSeParser.y"
                                      {
		double	fValue = 0;
		getVariable(out fValue,ValueStack[ValueStack.Depth-3].str);
		registVariable(ValueStack[ValueStack.Depth-3].str,fValue + ValueStack[ValueStack.Depth-1].value);
		CurrentSemanticValue.def = new CSeParserDef();
	}
#line default
        break;
      case 10: // substitute -> variable, SUBSTITUTE_SUB, expression
#line 88 "CSeParser.y"
                                      {
		double	fValue = 0;
		getVariable(out fValue,ValueStack[ValueStack.Depth-3].str);
		registVariable(ValueStack[ValueStack.Depth-3].str,fValue - ValueStack[ValueStack.Depth-1].value);
		CurrentSemanticValue.def = new CSeParserDef();
	}
#line default
        break;
      case 11: // substitute -> variable, SUBSTITUTE_MUL, expression
#line 94 "CSeParser.y"
                                      {
		double	fValue = 0;
		getVariable(out fValue,ValueStack[ValueStack.Depth-3].str);
		registVariable(ValueStack[ValueStack.Depth-3].str,fValue * ValueStack[ValueStack.Depth-1].value);
		CurrentSemanticValue.def = new CSeParserDef();
	}
#line default
        break;
      case 12: // substitute -> variable, SUBSTITUTE_DIV, expression
#line 100 "CSeParser.y"
                                      {
		double	fValue = 0;
		getVariable(out fValue,ValueStack[ValueStack.Depth-3].str);
		registVariable(ValueStack[ValueStack.Depth-3].str,fValue / ValueStack[ValueStack.Depth-1].value);
		CurrentSemanticValue.def = new CSeParserDef();
	}
#line default
        break;
      case 13: // substitute_in_def -> variable
#line 108 "CSeParser.y"
          {
		registVariable(ValueStack[ValueStack.Depth-1].str,0);
		CurrentSemanticValue.prop = new CSeParserProperty();
	}
#line default
        break;
      case 14: // substitute_in_def -> variable, '=', expression
#line 112 "CSeParser.y"
                          {
		registVariable(ValueStack[ValueStack.Depth-3].str,ValueStack[ValueStack.Depth-1].value);
		CurrentSemanticValue.prop = new CSeParserProperty();
	}
#line default
        break;
      case 15: // substitute_in_def -> variable, SUBSTITUTE_ADD, expression
#line 116 "CSeParser.y"
                                      {
		double	fValue = 0;
		getVariable(out fValue,ValueStack[ValueStack.Depth-3].str);
		registVariable(ValueStack[ValueStack.Depth-3].str,fValue + ValueStack[ValueStack.Depth-1].value);
		CurrentSemanticValue.prop = new CSeParserProperty();
	}
#line default
        break;
      case 16: // substitute_in_def -> variable, SUBSTITUTE_SUB, expression
#line 122 "CSeParser.y"
                                      {
		double	fValue = 0;
		getVariable(out fValue,ValueStack[ValueStack.Depth-3].str);
		registVariable(ValueStack[ValueStack.Depth-3].str,fValue - ValueStack[ValueStack.Depth-1].value);
		CurrentSemanticValue.prop = new CSeParserProperty();
	}
#line default
        break;
      case 17: // substitute_in_def -> variable, SUBSTITUTE_MUL, expression
#line 128 "CSeParser.y"
                                      {
		double	fValue = 0;
		getVariable(out fValue,ValueStack[ValueStack.Depth-3].str);
		registVariable(ValueStack[ValueStack.Depth-3].str,fValue * ValueStack[ValueStack.Depth-1].value);
		CurrentSemanticValue.prop = new CSeParserProperty();
	}
#line default
        break;
      case 18: // substitute_in_def -> variable, SUBSTITUTE_DIV, expression
#line 134 "CSeParser.y"
                                      {
		double	fValue = 0;
		getVariable(out fValue,ValueStack[ValueStack.Depth-3].str);
		registVariable(ValueStack[ValueStack.Depth-3].str,fValue / ValueStack[ValueStack.Depth-1].value);
		CurrentSemanticValue.prop = new CSeParserProperty();
	}
#line default
        break;
      case 19: // variable -> VARIABLE
#line 142 "CSeParser.y"
               {	CurrentSemanticValue.str = ValueStack[ValueStack.Depth-1].str;	}
#line default
        break;
      case 20: // sedef -> SE_DEF, '(', id, ',', id, ')', '{', property_list, ';', '}'
#line 146 "CSeParser.y"
                                                      {	CurrentSemanticValue.def = new CSeParserDef(ValueStack[ValueStack.Depth-8].uvalue,ValueStack[ValueStack.Depth-6].uvalue,ValueStack[ValueStack.Depth-3].prop);	}
#line default
        break;
      case 21: // sedef -> SE_DEF, '(', id, ')', '{', property_list, ';', '}'
#line 147 "CSeParser.y"
                                               {	CurrentSemanticValue.def = new CSeParserDef(ValueStack[ValueStack.Depth-6].uvalue,ValueStack[ValueStack.Depth-3].prop);	}
#line default
        break;
      case 22: // property_list -> property
#line 151 "CSeParser.y"
                 {	CurrentSemanticValue.prop = ValueStack[ValueStack.Depth-1].prop;	}
#line default
        break;
      case 23: // property_list -> property_list, ';', property
#line 152 "CSeParser.y"
                              {	CurrentSemanticValue.prop = ValueStack[ValueStack.Depth-3].prop;CurrentSemanticValue.prop.next = ValueStack[ValueStack.Depth-1].prop;	}
#line default
        break;
      case 24: // property -> VOLUME, '=', expression
#line 156 "CSeParser.y"
                            {	CurrentSemanticValue.prop = new CSeParserProperty(e_SeProperty.VOLUME,(byte) Mathf.Clamp((int) (ValueStack[ValueStack.Depth-1].value * 255),0,255));	}
#line default
        break;
      case 25: // property -> PRIORITY, '=', expression
#line 157 "CSeParser.y"
                              {	CurrentSemanticValue.prop = new CSeParserProperty(e_SeProperty.PRIORITY,(byte) ValueStack[ValueStack.Depth-1].value);	}
#line default
        break;
      case 26: // property -> GROUP, '=', expression
#line 158 "CSeParser.y"
                            {	CurrentSemanticValue.prop = new CSeParserProperty(e_SeProperty.GROUP,(byte) ValueStack[ValueStack.Depth-1].value);	}
#line default
        break;
      case 27: // property -> POLYPHONY, '=', expression
#line 159 "CSeParser.y"
                               {	CurrentSemanticValue.prop = new CSeParserProperty(e_SeProperty.POLYPHONY,(byte) ValueStack[ValueStack.Depth-1].value);	}
#line default
        break;
      case 28: // property -> DISTANCE, '=', expression, ',', expression
#line 160 "CSeParser.y"
                                          {	CurrentSemanticValue.prop = new CSeParserProperty(e_SeProperty.DISTANCE,ValueStack[ValueStack.Depth-3].value,ValueStack[ValueStack.Depth-1].value);	}
#line default
        break;
      case 29: // property -> DISTANCE, '=', expression
#line 161 "CSeParser.y"
                               {	CurrentSemanticValue.prop = new CSeParserProperty(e_SeProperty.DISTANCE,ValueStack[ValueStack.Depth-1].value,-1f);	}
#line default
        break;
      case 30: // property -> DISTANCE, '=', ',', expression
#line 162 "CSeParser.y"
                                  {	CurrentSemanticValue.prop = new CSeParserProperty(e_SeProperty.DISTANCE,-1f,ValueStack[ValueStack.Depth-1].value);	}
#line default
        break;
      case 31: // property -> substitute_in_def
#line 163 "CSeParser.y"
                          {	CurrentSemanticValue.prop = ValueStack[ValueStack.Depth-1].prop;	}
#line default
        break;
      case 32: // id -> LIT_STRING
#line 167 "CSeParser.y"
                  {	if (!FiveCC.isFiveCC(ValueStack[ValueStack.Depth-1].str)) {
												string	err = "this string is not [five cc]:" + ValueStack[ValueStack.Depth-1].str;
												yyerror(err);
												YYError();
											}
											FiveCC	fiveCC = new FiveCC(ValueStack[ValueStack.Depth-1].str);
											CurrentSemanticValue.uvalue = fiveCC;
										}
#line default
        break;
      case 33: // id -> expression
#line 175 "CSeParser.y"
                   {	CurrentSemanticValue.uvalue = (uint) ValueStack[ValueStack.Depth-1].value;
										}
#line default
        break;
      case 34: // expression -> LIT_INTEGER
#line 183 "CSeParser.y"
                   {	CurrentSemanticValue.value = ValueStack[ValueStack.Depth-1].value;	}
#line default
        break;
      case 35: // expression -> LIT_FLOAT
#line 184 "CSeParser.y"
                  {	CurrentSemanticValue.value = ValueStack[ValueStack.Depth-1].value;	}
#line default
        break;
      case 36: // expression -> VARIABLE
#line 185 "CSeParser.y"
                 {	double	fValue;
											if (!getVariable(out fValue,ValueStack[ValueStack.Depth-1].str)) {	
												string	err = "can't find variable:" + ValueStack[ValueStack.Depth-1].str;
												yyerror(err);
												YYError();
											}
											CurrentSemanticValue.value = fValue;
										}
#line default
        break;
      case 37: // expression -> expression, '|', expression
#line 193 "CSeParser.y"
                              {	CurrentSemanticValue.value = (int) ValueStack[ValueStack.Depth-3].value | (int) ValueStack[ValueStack.Depth-1].value;	}
#line default
        break;
      case 38: // expression -> expression, '&', expression
#line 194 "CSeParser.y"
                              {	CurrentSemanticValue.value = (int) ValueStack[ValueStack.Depth-3].value & (int) ValueStack[ValueStack.Depth-1].value;	}
#line default
        break;
      case 39: // expression -> expression, '^', expression
#line 195 "CSeParser.y"
                              {	CurrentSemanticValue.value = (int) ValueStack[ValueStack.Depth-3].value ^ (int) ValueStack[ValueStack.Depth-1].value;	}
#line default
        break;
      case 40: // expression -> expression, '+', expression
#line 196 "CSeParser.y"
                              {	CurrentSemanticValue.value = ValueStack[ValueStack.Depth-3].value + ValueStack[ValueStack.Depth-1].value;	}
#line default
        break;
      case 41: // expression -> expression, '-', expression
#line 197 "CSeParser.y"
                              {	CurrentSemanticValue.value = ValueStack[ValueStack.Depth-3].value - ValueStack[ValueStack.Depth-1].value;	}
#line default
        break;
      case 42: // expression -> expression, SHIFT_LEFT, expression
#line 198 "CSeParser.y"
                                   {	CurrentSemanticValue.value = (int) ValueStack[ValueStack.Depth-3].value << (int) ValueStack[ValueStack.Depth-1].value;	}
#line default
        break;
      case 43: // expression -> expression, SHIFT_RIGHT, expression
#line 199 "CSeParser.y"
                                    {	CurrentSemanticValue.value = (int) ValueStack[ValueStack.Depth-3].value >> (int) ValueStack[ValueStack.Depth-1].value;	}
#line default
        break;
      case 44: // expression -> expression, POW, expression
#line 200 "CSeParser.y"
                              {	CurrentSemanticValue.value = Math.Pow(ValueStack[ValueStack.Depth-3].value,ValueStack[ValueStack.Depth-1].value);	}
#line default
        break;
      case 45: // expression -> expression, '*', expression
#line 201 "CSeParser.y"
                              {	CurrentSemanticValue.value = ValueStack[ValueStack.Depth-3].value * ValueStack[ValueStack.Depth-1].value;	}
#line default
        break;
      case 46: // expression -> expression, '/', expression
#line 202 "CSeParser.y"
                              {	CurrentSemanticValue.value = ValueStack[ValueStack.Depth-3].value / ValueStack[ValueStack.Depth-1].value;	}
#line default
        break;
      case 47: // expression -> expression, '%', expression
#line 203 "CSeParser.y"
                              {	CurrentSemanticValue.value = ValueStack[ValueStack.Depth-3].value % ValueStack[ValueStack.Depth-1].value;	}
#line default
        break;
      case 48: // expression -> '-', expression
#line 204 "CSeParser.y"
                             {	CurrentSemanticValue.value = -ValueStack[ValueStack.Depth-1].value;	}
#line default
        break;
      case 49: // expression -> '~', expression
#line 205 "CSeParser.y"
                      {	CurrentSemanticValue.value = ~(int) ValueStack[ValueStack.Depth-1].value;	}
#line default
        break;
      case 50: // expression -> '(', expression, ')'
#line 206 "CSeParser.y"
                         {	CurrentSemanticValue.value = ValueStack[ValueStack.Depth-2].value;	}
#line default
        break;
    }
#pragma warning restore 162, 1522
  }

  protected override string TerminalToString(int terminal)
  {
    if (aliases != null && aliases.ContainsKey(terminal))
        return aliases[terminal];
    else if (((Tokens)terminal).ToString() != terminal.ToString(CultureInfo.InvariantCulture))
        return ((Tokens)terminal).ToString();
    else
        return CharToString((char)terminal);
  }

#line 219 "CSeParser.y"
/* error routin */
void yyerror(string err)
{
	UnityEngine.Debug.LogError(err);
}
CSeParserDef			m_cTopDef = null;

Dictionary<string,double>	m_dicVariable = new Dictionary<string,double>();
//==============================================================================================
/*!
	@brief	Constructor
	@note	
*/
//==============================================================================================
Parser(Scanner scanner) : base(scanner) {
}
//==============================================================================================
/*!
	@brief	registVariable
	@note	
*/
//==============================================================================================
void registVariable(string sVariable,double fValue)
{
	m_dicVariable[sVariable] = fValue;
}
//==============================================================================================
/*!
	@brief	getVariable
	@note	
*/
//==============================================================================================
bool getVariable(out double rValue,string sVariable)
{
	if (m_dicVariable.TryGetValue(sVariable,out rValue)) {
		return true;
	}
	rValue = 0;
	return false;
}

public Parser() : base(null) { }
//==============================================================================================
/*!
	@brief	compile
	@note	
*/
//==============================================================================================
static public Parser compile(string file) {
	//--------------------------------
	// gcc preprocessor
	string output;
	if (CParserHelper.preprocess(out output,KsSoftConfig.Preprocessor,KsSoftConfig.SEResourcePreprocessorArguments,file,true) != 0) {
		return null;
	}
	// change code UTF8 from UTF16
	UTF8Encoding utf8enc = new UTF8Encoding();
	byte[] aOutput = utf8enc.GetBytes(output);
	
	// create memory stream
	MemoryStream ms = new MemoryStream();
	ms.Write(aOutput,0,aOutput.Length);
	ms.Seek(0,SeekOrigin.Begin);
	
	// start parse
	Parser parser = new Parser();
	parser.Scanner = new Scanner(ms);
	if (!parser.Parse()) {
		UnityEngine.Debug.LogError("se data compile error");
		ms.Close();
		return null;
	}
	return parser;
}
public CSeParserDef topDef {
	get {
		return m_cTopDef;
	}
	set {
		m_cTopDef = value;
	}
}

#line default
}
}
