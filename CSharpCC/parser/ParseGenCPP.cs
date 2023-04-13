// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)

namespace CSharpCC.Parser;



/**
 * Generate the parser.
 */
public class ParseGenCPP : ParseGen
{

    public void start()
    {

        Token t = null;

        if (CSharpCCErrors.GetErrorCount() != 0) throw new MetaParseException();

        List<string> tn = new(ToolNames)
        {
            ToolName
        };
        SwitchToStaticsFile();

        SwitchToIncludeFile();

        //standard includes
        GenCodeLine("#include \"JavaCC.h\"");
        GenCodeLine("#include \"CharStream.h\"");
        GenCodeLine("#include \"Token.h\"");
        GenCodeLine("#include \"TokenManager.h\"");


        Object o = Options.ObjectValue(Options.USEROPTION__CPP_PARSER_INCLUDE);

        if (o is string)
        {
            string include = (String)o;
            if (include.Length > 0)
            {
                if (include[0] == '<')
                    GenCodeLine("#include " + include);
                else
                    GenCodeLine("#include \"" + include + "\"");
            }
        }

        GenCodeLine("#include \"" + CuName + "Constants.h\"");

        if (JjtreeGenerated)
        {
            GenCodeLine("#include \"JJT" + CuName + "State.h\"");
        }

        GenCodeLine("#include \"ErrorHandler.h\"");

        if (JjtreeGenerated)
        {
            GenCodeLine("#include \"" + CuName + "Tree.h\"");
        }

        if (Options.StringValue(Options.USEROPTION__CPP_NAMESPACE).Length > 0)
        {
            GenCodeLine("namespace " + Options.StringValue("NAMESPACE_OPEN"));
        }

        GenCodeLine("  struct JJCalls {");
        GenCodeLine("    int        gen;");
        GenCodeLine("    int        arg;");
        GenCodeLine("    JJCalls*   next;");
        GenCodeLine("    Token*     first;");
        GenCodeLine("    ~JJCalls() { if (next) delete next; }");
        GenCodeLine("     JJCalls() { next = nullptr; arg = 0; gen = -1; first = nullptr; }");
        GenCodeLine("  };");
        GenCodeLine("");


        string superClass = Options.StringValue(Options.USEROPTION__PARSER_SUPER_CLASS);
        GenClassStart("", CuName, Array.Empty<string>(),
                      superClass == null ? Array.Empty<string>() : new String[] {
                   "public " + superClass});
        SwitchToMainFile();
        if (cu_to_insertion_point_2.Count != 0)
        {
            CSharpCCGlobals.PrintTokenSetup((cu_to_insertion_point_2[0]));
            foreach(var t3 in cu_to_insertion_point_2)
            {
                PrintToken(t=t3);
            }
        }

        SwitchToMainFile();
        /*GenCodeLine("typedef class _LookaheadSuccess { } *LookaheadSuccess; // Dummy class");
        GenCodeLine("  static LookaheadSuccess jj_ls = new _LookaheadSuccess();");*/

        GenCodeLine("");
        GenCodeLine("");

        new ParseEngine().build(this);

        SwitchToIncludeFile();
        GenCodeLine("");
        GenCodeLine("public: ");
        GenCodeLine("  void setErrorHandler(ErrorHandler *eh) {");
        GenCodeLine("    if (errorHandler) delete errorHandler;");
        GenCodeLine("    errorHandler = eh;");
        GenCodeLine("  }");
        GenCodeLine("");
        GenCodeLine("  TokenManager *token_source = nullptr;");
        GenCodeLine("  CharStream   *jj_input_stream = nullptr;");
        GenCodeLine("  /** Current token. */");
        GenCodeLine("  Token        *token = nullptr;");
        GenCodeLine("  /** Next token. */");
        GenCodeLine("  Token        *jj_nt = nullptr;");
        GenCodeLine("");
        GenCodeLine("private: ");
        GenCodeLine("  int           jj_ntk;");

        GenCodeLine("  JJCalls       jj_2_rtns[" + (jj2index + 1) + "];");
        GenCodeLine("  bool          jj_rescan;");
        GenCodeLine("  int           jj_gc;");
        GenCodeLine("  Token        *jj_scanpos, *jj_lastpos;");
        GenCodeLine("  int           jj_la;");
        GenCodeLine("  /** Whether we are looking ahead. */");
        GenCodeLine("  bool          jj_lookingAhead;");
        GenCodeLine("  bool          jj_semLA;");

        GenCodeLine("  int           jj_gen;");
        GenCodeLine("  int           jj_la1[" + (maskindex + 1) + "];");
        GenCodeLine("  ErrorHandler *errorHandler = nullptr;");
        GenCodeLine("");
        GenCodeLine("protected: ");
        GenCodeLine("  bool          hasError;");
        GenCodeLine("");
        int tokenMaskSize = (tokenCount - 1) / 32 + 1;

        if (Options.GetErrorReporting() && tokenMaskSize > 0)
        {
            SwitchToStaticsFile();
            for (int i = 0; i < tokenMaskSize; i++)
            { 
                if (maskVals.Count > 0) 
                {
                    GenCodeLine("  unsigned int jj_la1_" + i + "[] = {");
                    foreach(var tokenMask in maskVals)
                    {
                        GenCode("0x" + Convert.ToString(tokenMask[i],16) + ",");
                    }
                    GenCodeLine("};");
                }
            }
        }

        if (Options.GetDepthLimit() > 0)
        {
            GenCodeLine("  private: int jj_depth;");
            GenCodeLine("  private: bool jj_depth_error;");
            GenCodeLine("  friend class __jj_depth_inc;");
            GenCodeLine("  class __jj_depth_inc {public:");
            GenCodeLine("    " + CuName + "* parent;");
            GenCodeLine("    __jj_depth_inc(" + CuName + "* p): parent(p) { parent->jj_depth++; };");
            GenCodeLine("    ~__jj_depth_inc(){ parent->jj_depth--; }");
            GenCodeLine("  };");
        }
        if (Options.GetStackLimit() != (""))
        {
            GenCodeLine("  public: size_t jj_stack_limit;");
            GenCodeLine("  private: void* jj_stack_base;");
            GenCodeLine("  private: bool jj_stack_error;");
        }

        GenCodeLine("");

        GenCodeLine("  /** Constructor with user supplied TokenManager. */");

        SwitchToIncludeFile(); // TEMP
        GenCodeLine("  Token *head; ");
        GenCodeLine("public: ");
        GenerateMethodDefHeader(" ", CuName, CuName + "(TokenManager *tokenManager)");
        if (superClass != null)
        {
            GenCodeLine(" : " + superClass + "()");
        }
        GenCodeLine("{");
        GenCodeLine("    head = nullptr;");
        GenCodeLine("    ReInit(tokenManager);");
        if (Options.GetTokenManagerUsesParser())
            GenCodeLine("    tokenManager->setParser(this);");
        GenCodeLine("}");

        SwitchToIncludeFile();
        GenCodeLine("  virtual ~" + CuName + "();");
        SwitchToMainFile();
        GenCodeLine("" + CuName + "::~" + CuName + "()");
        GenCodeLine("{");
        GenCodeLine("  clear();");
        GenCodeLine("}");
        GenerateMethodDefHeader("void", CuName, "ReInit(TokenManager* tokenManager)");
        GenCodeLine("{");
        GenCodeLine("    clear();");
        GenCodeLine("    errorHandler = new ErrorHandler();");
        GenCodeLine("    hasError = false;");
        GenCodeLine("    token_source = tokenManager;");
        GenCodeLine("    head = token = new Token();");
        GenCodeLine("    token->kind = 0;");
        GenCodeLine("    token->next = nullptr;");
        GenCodeLine("    jj_lookingAhead = false;");
        GenCodeLine("    jj_rescan = false;");
        GenCodeLine("    jj_done = false;");
        GenCodeLine("    jj_scanpos = jj_lastpos = nullptr;");
        GenCodeLine("    jj_gc = 0;");
        GenCodeLine("    jj_kind = -1;");
        GenCodeLine("    indent = 0;");
        GenCodeLine("    trace = " + Options.GetDebugParser() + ";");
        if (Options.GetStackLimit() != (""))
        {
            GenCodeLine("    jj_stack_limit = " + Options.GetStackLimit() + ";");
            GenCodeLine("    jj_stack_error = jj_stack_check(true);");
        }

        if (Options.GetCacheTokens())
        {
            GenCodeLine("    token->next = jj_nt = token_source->getNextToken();");
        }
        else
        {
            GenCodeLine("    jj_ntk = -1;");
        }
        if (JjtreeGenerated)
        {
            GenCodeLine("    jjtree.reset();");
        }
        if (Options.GetDepthLimit() > 0)
        {
            GenCodeLine("    jj_depth = 0;");
            GenCodeLine("    jj_depth_error = false;");
        }
        if (Options.GetErrorReporting())
        {
            GenCodeLine("    jj_gen = 0;");
            if (maskindex > 0)
            {
                GenCodeLine("    for (int i = 0; i < " + maskindex + "; i++) jj_la1[i] = -1;");
            }
        }
        GenCodeLine("  }");
        GenCodeLine("");

        GenerateMethodDefHeader("void", CuName, "clear()");
        GenCodeLine("{");
        GenCodeLine("  //Since token manager was generate from outside,");
        GenCodeLine("  //parser should not take care of deleting");
        GenCodeLine("  //if (token_source) delete token_source;");
        GenCodeLine("  if (head) {");
        GenCodeLine("    Token *next, *t = head;");
        GenCodeLine("    while (t) {");
        GenCodeLine("      next = t->next;");
        GenCodeLine("      delete t;");
        GenCodeLine("      t = next;");
        GenCodeLine("    }");
        GenCodeLine("  }");
        GenCodeLine("  if (errorHandler) {");
        GenCodeLine("    delete errorHandler, errorHandler = nullptr;");
        GenCodeLine("  }");
        if (Options.GetDepthLimit() > 0)
        {
            GenCodeLine("  assert(jj_depth==0);");
        }
        GenCodeLine("}");
        GenCodeLine("");

        if (Options.GetStackLimit() != (""))
        {
            GenCodeLine("");
            SwitchToIncludeFile();
            GenCodeLine(" virtual");
            SwitchToMainFile();
            GenerateMethodDefHeader("bool ", CuName, "jj_stack_check(bool init)");
            GenCodeLine("  {");
            GenCodeLine("     if(init) {");
            GenCodeLine("       jj_stack_base = nullptr;");
            GenCodeLine("       return false;");
            GenCodeLine("     } else {");
            GenCodeLine("       volatile int q = 0;");
            GenCodeLine("       if(!jj_stack_base) {");
            GenCodeLine("         jj_stack_base = (void*)&q;");
            GenCodeLine("         return false;");
            GenCodeLine("       } else {");
            GenCodeLine("         // Stack can grow in both directions, depending on arch");
            GenCodeLine("         std::ptrdiff_t used = (char*)jj_stack_base-(char*)&q;");
            GenCodeLine("         return (std::abs(used) > jj_stack_limit);");
            GenCodeLine("       }");
            GenCodeLine("     }");
            GenCodeLine("  }");
        }


        GenerateMethodDefHeader("Token *", CuName, "jj_consume_token(int kind)", "ParseException");
        GenCodeLine("  {");
        if (Options.GetStackLimit() != (""))
        {
            GenCodeLine("    if(kind != -1 && (jj_stack_error || jj_stack_check(false))) {");
            GenCodeLine("      if (!jj_stack_error) {");
            GenCodeLine("        errorHandler->handleOtherError(\"Stack overflow while trying to parse\", this);");
            GenCodeLine("        jj_stack_error=true;");
            GenCodeLine("      }");
            GenCodeLine("      return jj_consume_token(-1);");
            GenCodeLine("    }");
        }
        if (Options.GetCacheTokens())
        {
            GenCodeLine("    Token *oldToken = token;");
            GenCodeLine("    if ((token = jj_nt)->next != nullptr) jj_nt = jj_nt->next;");
            GenCodeLine("    else jj_nt = jj_nt->next = token_source->getNextToken();");
        }
        else
        {
            GenCodeLine("    Token *oldToken;");
            GenCodeLine("    if ((oldToken = token)->next != nullptr) token = token->next;");
            GenCodeLine("    else token = token->next = token_source->getNextToken();");
            GenCodeLine("    jj_ntk = -1;");
        }
        GenCodeLine("    if (token->kind == kind) {");
        if (Options.GetErrorReporting())
        {
            GenCodeLine("      jj_gen++;");
            if (jj2index != 0)
            {
                GenCodeLine("      if (++jj_gc > 100) {");
                GenCodeLine("        jj_gc = 0;");
                GenCodeLine("        for (int i = 0; i < " + jj2index + "; i++) {");
                GenCodeLine("          JJCalls *c = &jj_2_rtns[i];");
                GenCodeLine("          while (c != nullptr) {");
                GenCodeLine("            if (c->gen < jj_gen) c->first = nullptr;");
                GenCodeLine("            c = c->next;");
                GenCodeLine("          }");
                GenCodeLine("        }");
                GenCodeLine("      }");
            }
        }
        if (Options.GetDebugParser())
        {
            GenCodeLine("      trace_token(token, \"\");");
        }
        GenCodeLine("      return token;");
        GenCodeLine("    }");
        if (Options.GetCacheTokens())
        {
            GenCodeLine("    jj_nt = token;");
        }
        GenCodeLine("    token = oldToken;");
        if (Options.GetErrorReporting())
        {
            GenCodeLine("    jj_kind = kind;");
        }
        //GenCodeLine("    throw generateParseException();");
        if (Options.GetStackLimit() != (""))
        {
            GenCodeLine("    if (!jj_stack_error) {");
        }
        GenCodeLine("    JJString image = kind >= 0 ? tokenImage[kind] : tokenImage[0];");
        GenCodeLine("    errorHandler->handleUnexpectedToken(kind, image.substr(1, image.size() - 2), getToken(1), this);");
        if (Options.GetStackLimit() != (""))
        {
            GenCodeLine("    }");
        }
        GenCodeLine("    hasError = true;");
        GenCodeLine("    return token;");
        GenCodeLine("  }");
        GenCodeLine("");

        if (jj2index != 0)
        {
            SwitchToMainFile();
            GenerateMethodDefHeader("bool ", CuName, "jj_scan_token(int kind)");
            GenCodeLine("{");
            if (Options.GetStackLimit() != (""))
            {
                GenCodeLine("    if(kind != -1 && (jj_stack_error || jj_stack_check(false))) {");
                GenCodeLine("      if (!jj_stack_error) {");
                GenCodeLine("        errorHandler->handleOtherError(\"Stack overflow while trying to parse\", this);");
                GenCodeLine("        jj_stack_error=true;");
                GenCodeLine("      }");
                GenCodeLine("      return jj_consume_token(-1);");
                GenCodeLine("    }");
            }
            GenCodeLine("    if (jj_scanpos == jj_lastpos) {");
            GenCodeLine("      jj_la--;");
            GenCodeLine("      if (jj_scanpos->next == nullptr) {");
            GenCodeLine("        jj_lastpos = jj_scanpos = jj_scanpos->next = token_source->getNextToken();");
            GenCodeLine("      } else {");
            GenCodeLine("        jj_lastpos = jj_scanpos = jj_scanpos->next;");
            GenCodeLine("      }");
            GenCodeLine("    } else {");
            GenCodeLine("      jj_scanpos = jj_scanpos->next;");
            GenCodeLine("    }");
            if (Options.GetErrorReporting())
            {
                GenCodeLine("    if (jj_rescan) {");
                GenCodeLine("      int i = 0; Token *tok = token;");
                GenCodeLine("      while (tok != nullptr && tok != jj_scanpos) { i++; tok = tok->next; }");
                GenCodeLine("      if (tok != nullptr) jj_add_error_token(kind, i);");
                if (Options.GetDebugLookahead())
                {
                    GenCodeLine("    } else {");
                    GenCodeLine("      trace_scan(jj_scanpos, kind);");
                }
                GenCodeLine("    }");
            }
            else if (Options.GetDebugLookahead())
            {
                GenCodeLine("    trace_scan(jj_scanpos, kind);");
            }
            GenCodeLine("    if (jj_scanpos->kind != kind) return true;");
            //GenCodeLine("    if (jj_la == 0 && jj_scanpos == jj_lastpos) throw jj_ls;");
            GenCodeLine("    if (jj_la == 0 && jj_scanpos == jj_lastpos) { return jj_done = true; }");
            GenCodeLine("    return false;");
            GenCodeLine("  }");
            GenCodeLine("");
        }
        GenCodeLine("");
        GenCodeLine("/** Get the next Token. */");
        GenerateMethodDefHeader("Token *", CuName, "getNextToken()");
        GenCodeLine("{");
        if (Options.GetCacheTokens())
        {
            GenCodeLine("    if ((token = jj_nt)->next != nullptr) jj_nt = jj_nt->next;");
            GenCodeLine("    else jj_nt = jj_nt->next = token_source->getNextToken();");
        }
        else
        {
            GenCodeLine("    if (token->next != nullptr) token = token->next;");
            GenCodeLine("    else token = token->next = token_source->getNextToken();");
            GenCodeLine("    jj_ntk = -1;");
        }
        if (Options.GetErrorReporting())
        {
            GenCodeLine("    jj_gen++;");
        }
        if (Options.GetDebugParser())
        {
            GenCodeLine("      trace_token(token, \" (in getNextToken)\");");
        }
        GenCodeLine("    return token;");
        GenCodeLine("  }");
        GenCodeLine("");
        GenCodeLine("/** Get the specific Token. */");
        GenerateMethodDefHeader("Token *", CuName, "getToken(int index)");
        GenCodeLine("{");
        if (lookaheadNeeded)
        {
            GenCodeLine("    Token *t = jj_lookingAhead ? jj_scanpos : token;");
        }
        else
        {
            GenCodeLine("    Token *t = token;");
        }
        GenCodeLine("    for (int i = 0; i < index; i++) {");
        GenCodeLine("      if (t->next != nullptr) t = t->next;");
        GenCodeLine("      else t = t->next = token_source->getNextToken();");
        GenCodeLine("    }");
        GenCodeLine("    return t;");
        GenCodeLine("  }");
        GenCodeLine("");
        if (!Options.GetCacheTokens())
        {
            GenerateMethodDefHeader("int", CuName, "jj_ntk_f()");
            GenCodeLine("{");

            GenCodeLine("    if ((jj_nt=token->next) == nullptr)");
            GenCodeLine("      return (jj_ntk = (token->next=token_source->getNextToken())->kind);");
            GenCodeLine("    else");
            GenCodeLine("      return (jj_ntk = jj_nt->kind);");
            GenCodeLine("  }");
            GenCodeLine("");
        }

        SwitchToIncludeFile();
        GenCodeLine("private:");
        GenCodeLine("  int jj_kind;");
        if (Options.GetErrorReporting())
        {
            GenCodeLine("  int **jj_expentries;");
            GenCodeLine("  int *jj_expentry;");
            if (jj2index != 0)
            {
                SwitchToStaticsFile();
                // For now we don't support ERROR_REPORTING in the C++ version.
                //GenCodeLine("  static int *jj_lasttokens = new int[100];");
                //GenCodeLine("  static int jj_endpos;");
                GenCodeLine("");

                GenerateMethodDefHeader("  void", CuName, "jj_add_error_token(int kind, int pos)");
                GenCodeLine("  {");
                // For now we don't support ERROR_REPORTING in the C++ version.

                //GenCodeLine("    if (pos >= 100) return;");
                //GenCodeLine("    if (pos == jj_endpos + 1) {");
                //GenCodeLine("      jj_lasttokens[jj_endpos++] = kind;");
                //GenCodeLine("    } else if (jj_endpos != 0) {");
                //GenCodeLine("      jj_expentry = new int[jj_endpos];");
                //GenCodeLine("      for (int i = 0; i < jj_endpos; i++) {");
                //GenCodeLine("        jj_expentry[i] = jj_lasttokens[i];");
                //GenCodeLine("      }");
                //GenCodeLine("      jj_entries_loop: for (Iterator it = jj_expentries.iterator(); it.hasNext();) {");
                //GenCodeLine("        int[] oldentry = (int[])(it->next());");
                //GenCodeLine("        if (oldentry.length == jj_expentry.length) {");
                //GenCodeLine("          for (int i = 0; i < jj_expentry.length; i++) {");
                //GenCodeLine("            if (oldentry[i] != jj_expentry[i]) {");
                //GenCodeLine("              continue jj_entries_loop;");
                //GenCodeLine("            }");
                //GenCodeLine("          }");
                //GenCodeLine("          jj_expentries.Add(jj_expentry);");
                //GenCodeLine("          break jj_entries_loop;");
                //GenCodeLine("        }");
                //GenCodeLine("      }");
                //GenCodeLine("      if (pos != 0) jj_lasttokens[(jj_endpos = pos) - 1] = kind;");
                //GenCodeLine("    }");
                GenCodeLine("  }");
            }
            GenCodeLine("");

            SwitchToIncludeFile();
            GenCodeLine("protected:");
            GenCodeLine("  /** Generate ParseException. */");
            GenerateMethodDefHeader("  virtual void ", CuName, "parseError()");
            GenCodeLine("   {");
            if (Options.GetErrorReporting())
            {
                GenCodeLine("      fprintf(stderr, \"Parse error at: %d:%d, after token: %s encountered: %s\\n\", token->beginLine, token->beginColumn, addUnicodeEscapes(token->image).c_str(), addUnicodeEscapes(getToken(1)->image).c_str());");
            }
            GenCodeLine("   }");
            /*generateMethodDefHeader("ParseException",  cu_name, "generateParseException()");
            GenCodeLine("   {");
            //GenCodeLine("    jj_expentries.Clear();");
            //GenCodeLine("    bool[] la1tokens = new boolean[" + tokenCount + "];");
            //GenCodeLine("    if (jj_kind >= 0) {");
            //GenCodeLine("      la1tokens[jj_kind] = true;");
            //GenCodeLine("      jj_kind = -1;");
            //GenCodeLine("    }");
            //GenCodeLine("    for (int i = 0; i < " + maskindex + "; i++) {");
            //GenCodeLine("      if (jj_la1[i] == jj_gen) {");
            //GenCodeLine("        for (int j = 0; j < 32; j++) {");
            //for (int i = 0; i < (tokenCount-1)/32 + 1; i++) {
              //GenCodeLine("          if ((jj_la1_" + i + "[i] & (1<<j)) != 0) {");
              //GenCode("            la1tokens[");
              //if (i != 0) {
                //GenCode((32*i) + "+");
              //}
              //GenCodeLine("j] = true;");
              //GenCodeLine("          }");
            //}
            //GenCodeLine("        }");
            //GenCodeLine("      }");
            //GenCodeLine("    }");
            //GenCodeLine("    for (int i = 0; i < " + tokenCount + "; i++) {");
            //GenCodeLine("      if (la1tokens[i]) {");
            //GenCodeLine("        jj_expentry = new int[1];");
            //GenCodeLine("        jj_expentry[0] = i;");
            //GenCodeLine("        jj_expentries.Add(jj_expentry);");
            //GenCodeLine("      }");
            //GenCodeLine("    }");
            //if (jj2index != 0) {
              //GenCodeLine("    jj_endpos = 0;");
              //GenCodeLine("    jj_rescan_token();");
              //GenCodeLine("    jj_add_error_token(0, 0);");
            //}
            //GenCodeLine("    int exptokseq[][1] = new int[1];");
            //GenCodeLine("    for (int i = 0; i < jj_expentries.size(); i++) {");
            //if (!Options.getGenerateGenerics())
               //GenCodeLine("      exptokseq[i] = (int[])jj_expentries[i];");
            //else
               //GenCodeLine("      exptokseq[i] = jj_expentries[i];");
            //GenCodeLine("    }");
            GenCodeLine("    return new _ParseException();");//token, nullptr, tokenImage);");
            GenCodeLine("  }"); */
        }
        else
        {
            GenCodeLine("protected:");
            GenCodeLine("  /** Generate ParseException. */");
            GenerateMethodDefHeader("virtual void ", CuName, "parseError()");
            GenCodeLine("   {");
            if (Options.GetErrorReporting())
            {
                GenCodeLine("      fprintf(stderr, \"Parse error at: %d:%d, after token: %s encountered: %s\\n\", token->beginLine, token->beginColumn, addUnicodeEscapes(token->image).c_str(), addUnicodeEscapes(getToken(1)->image).c_str());");
            }
            GenCodeLine("   }");
            /*generateMethodDefHeader("ParseException",  cu_name, "generateParseException()");
            GenCodeLine("   {");
            GenCodeLine("    Token *errortok = token->next;");
            if (Options.getKeepLineColumn())
               GenCodeLine("    int line = errortok.beginLine, column = errortok.beginColumn;");
            GenCodeLine("    JJString mess = (errortok->kind == 0) ? tokenImage[0] : errortok->image;");
            if (Options.getKeepLineColumn())
               GenCodeLine("    return new _ParseException();");// +
                   //"\"Parse error at line \" + line + \", column \" + column + \".  " +
                   //"Encountered: \" + mess);");
            else
               GenCodeLine("    return new _ParseException();");//\"Parse error at <unknown location>.  " +
                       //"Encountered: \" + mess);");
            GenCodeLine("  }");*/
        }
        GenCodeLine("");

        SwitchToIncludeFile();
        GenCodeLine("private:");
        GenCodeLine("  int  indent;	// trace indentation");
        GenCodeLine("  bool trace = " + Options.GetDebugParser() + "; // trace enabled if true");
        GenCodeLine("");
        GenCodeLine("public:");
        GenerateMethodDefHeader("  bool", CuName, "trace_enabled()");
        GenCodeLine("  {");
        GenCodeLine("    return trace;");
        GenCodeLine("  }");
        GenCodeLine("");
        if (Options.GetDebugParser())
        {
            SwitchToIncludeFile();
            GenerateMethodDefHeader("  void", CuName, "enable_tracing()");
            GenCodeLine("{");
            GenCodeLine("    trace = true;");
            GenCodeLine("}");
            GenCodeLine("");

            SwitchToIncludeFile();
            GenerateMethodDefHeader("  void", CuName, "disable_tracing()");
            GenCodeLine("{");
            GenCodeLine("    trace = false;");
            GenCodeLine("}");
            GenCodeLine("");

            SwitchToIncludeFile();
            GenerateMethodDefHeader("  void", CuName, "trace_call(const char *s)");
            GenCodeLine("  {");
            GenCodeLine("    if (trace_enabled()) {");
            GenCodeLine("      for (int i = 0; i < indent; i++) { printf(\" \"); }");
            GenCodeLine("      printf(\"Call:   %s\\n\", s);");
            GenCodeLine("    }");
            GenCodeLine("    indent = indent + 2;");
            GenCodeLine("  }");
            GenCodeLine("");

            SwitchToIncludeFile();
            GenerateMethodDefHeader("  void", CuName, "trace_return(const char *s)");
            GenCodeLine("  {");
            GenCodeLine("    indent = indent - 2;");
            GenCodeLine("    if (trace_enabled()) {");
            GenCodeLine("      for (int i = 0; i < indent; i++) { printf(\" \"); }");
            GenCodeLine("      printf(\"Return: %s\\n\", s);");
            GenCodeLine("    }");
            GenCodeLine("  }");
            GenCodeLine("");

            SwitchToIncludeFile();
            GenerateMethodDefHeader("  void", CuName, "trace_token(Token *t, const char *where)");
            GenCodeLine("  {");
            GenCodeLine("    if (trace_enabled()) {");
            GenCodeLine("      for (int i = 0; i < indent; i++) { printf(\" \"); }");
            GenCodeLine("      printf(\"Consumed token: <kind: %d(%s), \\\"%s\\\"\", t->kind, addUnicodeEscapes(tokenImage[t->kind]).c_str(), addUnicodeEscapes(t->image).c_str());");
            //GenCodeLine("      if (t->kind != 0 && !tokenImage[t->kind]==(\"\\\"\" + t->image + \"\\\"\")) {");
            //GenCodeLine("        System.out.Write(\": \\\"\" + t->image + \"\\\"\");");
            //GenCodeLine("      }");
            GenCodeLine("      printf(\" at line %d column %d> %s\\n\", t->beginLine, t->beginColumn, where);");
            GenCodeLine("    }");
            GenCodeLine("  }");
            GenCodeLine("");

            SwitchToIncludeFile();
            GenerateMethodDefHeader("  void", CuName, "trace_scan(Token *t1, int t2)");
            GenCodeLine("  {");
            GenCodeLine("    if (trace_enabled()) {");
            GenCodeLine("      for (int i = 0; i < indent; i++) { printf(\" \"); }");
            GenCodeLine("      printf(\"Visited token: <Kind: %d(%s), \\\"%s\\\"\", t1->kind, addUnicodeEscapes(tokenImage[t1->kind]).c_str(), addUnicodeEscapes(t1->image).c_str());");
            //GenCodeLine("      if (t1->kind != 0 && !tokenImage[t1->kind]==(\"\\\"\" + t1->image + \"\\\"\")) {");
            //GenCodeLine("        System.out.Write(\": \\\"\" + t1->image + \"\\\"\");");
            //GenCodeLine("      }");
            GenCodeLine("      printf(\" at line %d column %d>; Expected token: %s\\n\", t1->beginLine, t1->beginColumn, addUnicodeEscapes(tokenImage[t2]).c_str());");
            GenCodeLine("    }");
            GenCodeLine("  }");
            GenCodeLine("");
        }
        else
        {
            SwitchToIncludeFile();
            GenerateMethodDefHeader("  void", CuName, "enable_tracing()");
            GenCodeLine("  {");
            GenCodeLine("  }");
            SwitchToIncludeFile();
            GenerateMethodDefHeader("  void", CuName, "disable_tracing()");
            GenCodeLine("  {");
            GenCodeLine("  }");
            GenCodeLine("");
        }

        if (jj2index != 0 && Options.GetErrorReporting())
        {
            GenerateMethodDefHeader("  void", CuName, "jj_rescan_token()");
            GenCodeLine("{");
            GenCodeLine("    jj_rescan = true;");
            GenCodeLine("    for (int i = 0; i < " + jj2index + "; i++) {");
            //GenCodeLine("    try {");
            GenCodeLine("      JJCalls *p = &jj_2_rtns[i];");
            GenCodeLine("      do {");
            GenCodeLine("        if (p->gen > jj_gen) {");
            GenCodeLine("          jj_la = p->arg; jj_lastpos = jj_scanpos = p->first;");
            GenCodeLine("          switch (i) {");
            for (int i = 0; i < jj2index; i++)
            {
                GenCodeLine("            case " + i + ": jj_3_" + (i + 1) + "(); break;");
            }
            GenCodeLine("          }");
            GenCodeLine("        }");
            GenCodeLine("        p = p->next;");
            GenCodeLine("      } while (p != nullptr);");
            //GenCodeLine("      } catch(LookaheadSuccess ls) { }");
            GenCodeLine("    }");
            GenCodeLine("    jj_rescan = false;");
            GenCodeLine("  }");
            GenCodeLine("");

            GenerateMethodDefHeader("  void", CuName, "jj_save(int index, int xla)");
            GenCodeLine("{");
            GenCodeLine("    JJCalls *p = &jj_2_rtns[index];");
            GenCodeLine("    while (p->gen > jj_gen) {");
            GenCodeLine("      if (p->next == nullptr) { p = p->next = new JJCalls(); break; }");
            GenCodeLine("      p = p->next;");
            GenCodeLine("    }");
            GenCodeLine("    p->gen = jj_gen + xla - jj_la; p->first = token; p->arg = xla;");
            GenCodeLine("  }");
            GenCodeLine("");
        }

        if (cu_from_insertion_point_2.Count != 0)
        {
            CSharpCCGlobals.PrintTokenSetup((cu_from_insertion_point_2[0])); this.ccol = 1;
            foreach(var t3 in cu_from_insertion_point_2)
            {
                PrintToken(t=t3);
            }
            PrintTrailingComments(t);
        }
        GenCodeLine("");

        // in the include file close the class signature
        SwitchToIncludeFile();

        // copy other stuff
        Token t1 = CSharpCCGlobals.otherLanguageDeclTokenBeg;
        Token t2 = CSharpCCGlobals.otherLanguageDeclTokenEnd;
        while (t1 != t2)
        {
            PrintToken(t1);
            t1 = t1.next;
        }
        GenCodeLine("\n");
        if (JjtreeGenerated)
        {
            GenCodeLine("  JJT" + CuName + "State jjtree;");
        }
        GenCodeLine("private:");
        GenCodeLine("  bool jj_done;");

        GenCodeLine("};");

        SaveOutput(Options.GetOutputDirectory() + Path.DirectorySeparatorChar + CuName + GetFileExtension(Options.GetOutputLanguage()));
    }

    public static new void ReInit()
    {
        lookaheadNeeded = false;
    }

}
