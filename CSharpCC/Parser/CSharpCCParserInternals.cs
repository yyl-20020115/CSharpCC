/* Copyright (c) 2006, Sun Microsystems, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *     * Redistributions of source code must retain the above copyright notice,
 *       this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Sun Microsystems, Inc. nor the names of its
 *       contributors may be used to endorse or promote products derived from
 *       this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */
namespace CSharpCC.Parser;

/**
 * Utilities.
 */
public abstract class CSharpCCParserInternals : CSharpCCGlobals
{

    static protected void Initialize()
    {
        int i = (0);
        LexstateS2I.Add("DEFAULT", i);
        LexstateI2S.Add(i, "DEFAULT");
        SimpleTokenTable.Add("DEFAULT", new());
    }

    static protected void Addcuname(string id)
    {
        CuName = id;
    }

    static protected void Compare(Token t, string id1, string id2)
    {
        if (id2 != (id1))
        {
            CSharpCCErrors.ParseError(t, "Name " + id2 + " must be the same as that used at PARSER_BEGIN (" + id1 + ")");
        }
    }

    static private List<Token> add_cu_token_here = CuToInsertionPoint1;
    static private Token first_cu_token;
    static private bool insertionpoint1set = false;
    static private bool insertionpoint2set = false;

    static protected void SetInsertionPoint(Token t, int no)
    {
        do
        {
            add_cu_token_here.Add(first_cu_token);
            first_cu_token = first_cu_token.next;
        } while (first_cu_token != t);
        if (no == 1)
        {
            if (insertionpoint1set)
            {
                CSharpCCErrors.ParseError(t, "Multiple declaration of parser class.");
            }
            else
            {
                insertionpoint1set = true;
                add_cu_token_here = CuToInsertionPoint2;
            }
        }
        else
        {
            add_cu_token_here = CuFromInsertionPoint2;
            insertionpoint2set = true;
        }
        first_cu_token = t;
    }

    static protected void InsertionPointErrors(Token t)
    {
        while (first_cu_token != t)
        {
            add_cu_token_here.Add(first_cu_token);
            first_cu_token = first_cu_token.next;
        }
        if (!insertionpoint1set || !insertionpoint2set)
        {
            CSharpCCErrors.ParseError(t, "Parser class has not been defined between PARSER_BEGIN and PARSER_END.");
        }
    }

    static protected void SetInitialCuToken(Token t)
    {
        first_cu_token = t;
    }

    static protected void AddProduction(NormalProduction p)
    {
        BNFProductions.Add(p);
    }

    static protected void ProductionAddexpansion(BNFProduction p, Expansion e)
    {
        e.parent = p;
        p.SetExpansion(e);
    }

    static private int nextFreeLexState = 1;

    static protected void AddRegexpr(TokenProduction p)
    {
        int ii;
        RegexpList.Add(p);
        if (Options.GetUserTokenManager())
        {
            if (p.lexStates == null || p.lexStates.Length != 1 || p.lexStates[0] != ("DEFAULT"))
            {
                CSharpCCErrors.Warning(p, "Ignoring lexical state specifications since option " +
                                        "USER_TOKEN_MANAGER has been set to true.");
            }
        }
        if (p.lexStates == null)
        {
            return;
        }
        for (int i = 0; i < p.lexStates.Length; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (p.lexStates[i] == (p.lexStates[j]))
                {
                    CSharpCCErrors.ParseError(p, "Multiple occurrence of \"" + p.lexStates[i] + "\" in lexical state list.");
                }
            }
            if (!LexstateS2I.TryGetValue(p.lexStates[i], out var t))
            {
                ii = (nextFreeLexState++);
                LexstateS2I.Add(p.lexStates[i], ii);
                LexstateI2S.Add(ii, p.lexStates[i]);
                SimpleTokenTable.Add(p.lexStates[i], new());
            }
        }
    }

    static protected void AddTokenManagerDecls(Token t, List<Token> decls)
    {
        if (TokenManagerDeclarations != null)
        {
            CSharpCCErrors.ParseError(t, "Multiple occurrence of \"TOKEN_MGR_DECLS\".");
        }
        else
        {
            TokenManagerDeclarations = decls;
            if (Options.GetUserTokenManager())
            {
                CSharpCCErrors.Warning(t, "Ignoring declarations in \"TOKEN_MGR_DECLS\" since option " +
                                        "USER_TOKEN_MANAGER has been set to true.");
            }
        }
    }

    static protected void AddInlineRegexpr(RegularExpression r)
    {
        if (r is not REndOfFile)
        {
            var p = new TokenProduction { isExplicit = false, lexStates = new String[] { "DEFAULT" }, kind = TokenProduction.TOKEN };
            RegExprSpec res = new()
            {
                rexp = r
            };
            res.rexp.tpContext = p;
            res.act = new Action();
            res.nextState = null;
            res.nsTok = null;
            p.respecs.Add(res);
            RegexpList.Add(p);
        }
    }

    public static bool IsHexchar(char ch)
    {
        if (ch >= '0' && ch <= '9') return true;
        if (ch >= 'A' && ch <= 'F') return true;
        if (ch >= 'a' && ch <= 'f') return true;
        return false;
    }

    static protected int Hexval(char ch)
    {
        if (ch >= '0' && ch <= '9') return ((int)ch) - ((int)'0');
        if (ch >= 'A' && ch <= 'F') return ((int)ch) - ((int)'A') + 10;
        return ((int)ch) - ((int)'a') + 10;
    }

    static protected string RemoveEscapesAndQuotes(Token t, string str)
    {
        string retval = "";
        int index = 1;
        char ch, ch1;
        int ordinal;
        while (index < str.Length - 1)
        {
            if (str[index] != '\\')
            {
                retval += str[index]; index++;
                continue;
            }
            index++;
            ch = str[index];
            if (ch == 'b')
            {
                retval += '\b'; index++;
                continue;
            }
            if (ch == 't')
            {
                retval += '\t'; index++;
                continue;
            }
            if (ch == 'n')
            {
                retval += '\n'; index++;
                continue;
            }
            if (ch == 'f')
            {
                retval += '\f'; index++;
                continue;
            }
            if (ch == 'r')
            {
                retval += '\r'; index++;
                continue;
            }
            if (ch == '"')
            {
                retval += '\"'; index++;
                continue;
            }
            if (ch == '\'')
            {
                retval += '\''; index++;
                continue;
            }
            if (ch == '\\')
            {
                retval += '\\'; index++;
                continue;
            }
            if (ch >= '0' && ch <= '7')
            {
                ordinal = ((int)ch) - ((int)'0'); index++;
                ch1 = str[index];
                if (ch1 >= '0' && ch1 <= '7')
                {
                    ordinal = ordinal * 8 + ((int)ch1) - ((int)'0'); index++;
                    ch1 = str[index];
                    if (ch <= '3' && ch1 >= '0' && ch1 <= '7')
                    {
                        ordinal = ordinal * 8 + ((int)ch1) - ((int)'0'); index++;
                    }
                }
                retval += (char)ordinal;
                continue;
            }
            if (ch == 'u')
            {
                index++; ch = str[index];
                if (IsHexchar(ch))
                {
                    ordinal = Hexval(ch);
                    index++; ch = str[index];
                    if (IsHexchar(ch))
                    {
                        ordinal = ordinal * 16 + Hexval(ch);
                        index++; ch = str[index];
                        if (IsHexchar(ch))
                        {
                            ordinal = ordinal * 16 + Hexval(ch);
                            index++; ch = str[index];
                            if (IsHexchar(ch))
                            {
                                ordinal = ordinal * 16 + Hexval(ch);
                                index++;
                                continue;
                            }
                        }
                    }
                }
                CSharpCCErrors.ParseError(t, "Encountered non-hex character '" + ch +
                        "' at position " + index + " of string " +
                        "- Unicode escape must have 4 hex digits after it.");
                return retval;
            }
            CSharpCCErrors.ParseError(t, "Illegal escape sequence '\\" + ch +
                    "' at position " + index + " of string.");
            return retval;
        }
        return retval;
    }

    static protected char CharacterDescriptorAssign(Token t, string s)
    {
        if (s.Length != 1)
        {
            CSharpCCErrors.ParseError(t, "String in character list may contain only one character.");
            return ' ';
        }
        else
        {
            return s[0];
        }
    }

    static protected char CharacterDescriptorAssign(Token t, string s, string left)
    {
        if (s.Length != 1)
        {
            CSharpCCErrors.ParseError(t, "String in character list may contain only one character.");
            return ' ';
        }
        else if ((int)(left[0]) > (int)(s[0]))
        {
            CSharpCCErrors.ParseError(t, "Right end of character range \'" + s +
                    "\' has a lower ordinal value than the left end of character range \'" + left + "\'.");
            return left[0];
        }
        else
        {
            return s[0];
        }
    }

    static protected void MakeTryBlock(
      Token tryLoc,
      Container result,
      Container nestedExp,
      List<List<Token>> types,
      List<Token> ids,
      List<List<Token>> catchblks,
      List<Token> finallyblk
    )
    {
        if (catchblks.Count == 0 && finallyblk == null)
        {
            CSharpCCErrors.ParseError(tryLoc, "Try block must contain at least one catch or finally block.");
            return;
        }
        var tblk = new TryBlock();
        tblk.        Line = tryLoc.beginLine;
        tblk.        Column = tryLoc.beginColumn;
        tblk.exp = (Expansion)(nestedExp.Member);
        tblk.exp.parent = tblk;
        tblk.exp.ordinal = 0;
        tblk.types = types;
        tblk.ids = ids;
        tblk.catchblks = catchblks;
        tblk.finallyblk = finallyblk;
        result.Member = tblk;
    }

    public static new void ReInit()
    {
        add_cu_token_here = CuToInsertionPoint1;
        first_cu_token = null;
        insertionpoint1set = false;
        insertionpoint2set = false;
        nextFreeLexState = 1;
    }

}
