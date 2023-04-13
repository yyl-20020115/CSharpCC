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
using System.Collections.Generic;
using System.Text;

namespace CSharpCC.Parser;


/**
 * Describes JavaCC productions.
 */

public class NormalProduction
{

    /**
     * The line and column number of the construct that corresponds
     * most closely to this node.
     */
    private int column;

    private int line;

    /**
     * The NonTerminal nodes which refer to this production.
     */
    private List<Expansion> parents = new();

    /**
     * The access modifier of this production.
     */
    private string accessMod;

    /**
     * The name of the non-terminal of this production.
     */
    private string lhs;

    /**
     * The tokens that make up the return type of this production.
     */
    private readonly List<Token> return_type_tokens = new();

    /**
     * The tokens that make up the parameters of this production.
     */
    private readonly List<Token> parameter_list_tokens = new();

    /**
     * Each entry in this list is a list of tokens that represents an
     * exception in the throws list of this production.  This list does not
     * include ParseException which is always thrown.
     */
    private List<List<Token>> throws_list = new();

    /**
     * The RHS of this production.  Not used for JavaCodeProduction.
     */
    private Expansion expansion;

    /**
     * This bool flag is true if this production can expand to empty.
     */
    private bool emptyPossible = false;

    /**
     * A list of all non-terminals that this one can expand to without
     * having to consume any tokens.  Also an index that shows how many
     * pointers exist.
     */
    private NormalProduction[] leftExpansions = new NormalProduction[10];
    public int leIndex = 0;

    /**
     * The following variable is used to maintain state information for the
     * left-recursion determination algorithm:  It is initialized to 0, and
     * set to -1 if this node has been visited in a pre-order walk, and then
     * it is set to 1 if the pre-order walk of the whole graph from this
     * node has been traversed.  i.e., -1 indicates partially processed,
     * and 1 indicates fully processed.
     */
    private int walkStatus = 0;

    /**
     * The first and last tokens from the input stream that represent this
     * production.
     */
    private Token lastToken;

    private Token firstToken;

    protected readonly string eol = Environment.NewLine;// System.getProperty("line.separator", "\n");
    protected StringBuilder DumpPrefix(int indent)
    {
        StringBuilder sb = new(128);
        for (int i = 0; i < indent; i++)
            sb.Append("  ");
        return sb;
    }

    protected string GetSimpleName()
    {
        string name = this.GetType().Name;
        return name[(name.LastIndexOf(".") + 1)..]; // strip the package name
    }

    /**
     * @return the line
     */
    //public virtual StringBuilder Dump(int indent, HashSet<Expansion> alreadyDumped)
    //{
    //    var sb = DumpPrefix(indent).Append((this).GetHashCode()).Append(' ').Append(GetSimpleName()).Append(' ').Append(GetLhs());
    //    if (!alreadyDumped.Contains(this))
    //    {
    //        alreadyDumped.Add(this);
    //        if (GetExpansion() != null)
    //        {
    //            sb.Append(eol).Append(GetExpansion().Dump(indent + 1, alreadyDumped));
    //        }
    //    }

    //    return sb;
    //}

    /**
     * @param line the line to set
     */
    public int Line { get => line; set => this.line = value; }

    /**
     * @return the column
     */
    /**
 * @param column the column to set
 */
    public int Column { get => column; set => this.column = value; }

    /**
     * @return the parents
     */
    /**
 * @param parents the parents to set
 */
    public List<Expansion> Parents { get => parents; set => this.parents = value; }

    /**
     * @return the accessMod
     */
    /**
 * @param accessMod the accessMod to set
 */
    public string AccessMod { get => accessMod; set => this.accessMod = value; }

    /**
     * @return the lhs
     */
    /**
 * @param lhs the lhs to set
 */
    public string Lhs { get => lhs; set => this.lhs = value; }

    /**
     * @return the return_type_tokens
     */
    public List<Token> ReturnTypeTokens => return_type_tokens;

    /**
     * @return the parameter_list_tokens
     */
    public List<Token> ParameterListTokens => parameter_list_tokens;

    /**
     * @return the throws_list
     */
    /**
 * @param throws_list the throws_list to set
 */
    public List<List<Token>> ThrowsList { get => throws_list; set => this.throws_list = value; }

    /**
     * @return the expansion
     */
    /**
 * @param expansion the expansion to set
 */
    public Expansion Expansion { get => expansion; set => this.expansion = value; }

    /**
     * @param emptyPossible the emptyPossible to set
     */
    public void SetEmptyPossible(bool emptyPossible)
    {
        this.emptyPossible = emptyPossible;
    }

    /**
     * @return the emptyPossible
     */
    public bool IsEmptyPossible() => emptyPossible;

    /**
     * @return the leftExpansions
     */
    /**
 * @param leftExpansions the leftExpansions to set
 */
    public NormalProduction[] LeftExpansions { get => leftExpansions; set => this.leftExpansions = value; }

    /**
     * @return the walkStatus
     */
    /**
 * @param walkStatus the walkStatus to set
 */
    public int WalkStatus { get => walkStatus; set => this.walkStatus = value; }

    /**
     * @return the firstToken
     */
    /**
 * @param firstToken the firstToken to set
 */
    public Token FirstToken { get => firstToken; set => this.firstToken = value;
        //return firstToken;
                    }

    /**
     * @return the lastToken
     */
    /**
 * @param lastToken the lastToken to set
 */
    public Token LastToken { get => lastToken; set => this.lastToken = value; }
}
