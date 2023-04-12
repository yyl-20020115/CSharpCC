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

using System.Text;

namespace org.javacc.parser;



/**
 * Generate CharStream, TokenManager and Exceptions.
 */
public class JavaFiles:JavaCCGlobals : JavaCCParserConstants
{
  /**
   * ID of the latest version (of JavaCC) in which one of the CharStream classes
   * or the CharStream interface is modified.
   */
  static readonly string charStreamVersion = Version.majorDotMinor;

  /**
   * ID of the latest version (of JavaCC) in which the TokenManager interface is modified.
   */
  static readonly string tokenManagerVersion = Version.majorDotMinor;

  /**
   * ID of the latest version (of JavaCC) in which the Token class is modified.
   */
  static readonly string tokenVersion = Version.majorDotMinor;

  /**
   * ID of the latest version (of JavaCC) in which the ParseException class is
   * modified.
   */
  static readonly string parseExceptionVersion = Version.majorDotMinor;

  /**
   * ID of the latest version (of JavaCC) in which the TokenMgrError class is
   * modified.
   */
  static readonly string tokenMgrErrorVersion = Version.majorDotMinor;


  public interface JavaResourceTemplateLocations {
		public string getTokenManagerTemplateResourceUrl();
		public string getTokenTemplateResourceUrl();
		public string getTokenMgrErrorTemplateResourceUrl();
		public string getJavaCharStreamTemplateResourceUrl();
		public string getCharStreamTemplateResourceUrl();
		public string getSimpleCharStreamTemplateResourceUrl();
		public string getParseExceptionTemplateResourceUrl();
  }


  public static class JavaModernResourceTemplateLocationImpl : JavaResourceTemplateLocations {
		public string getTokenMgrErrorTemplateResourceUrl() {
			// Same as Java
			return "/templates/TokenMgrError.template";
		}
		public string getCharStreamTemplateResourceUrl() {
			// Same as Java
			return "/templates/CharStream.template";
		}

	  public string getTokenManagerTemplateResourceUrl() {
		// Same as Java
			return "/templates/TokenManager.template";
		}

		public string getTokenTemplateResourceUrl() {
			// Same as Java
			return "/templates/Token.template";
		}

		public string getSimpleCharStreamTemplateResourceUrl() {
			return "/templates/gwt/SimpleCharStream.template";
		}


		public string getJavaCharStreamTemplateResourceUrl() {
			return "/templates/gwt/JavaCharStream.template";
		}


		public string getParseExceptionTemplateResourceUrl() {
			return "/templates/gwt/ParseException.template";
		}
  }


  public static class JavaResourceTemplateLocationImpl : JavaResourceTemplateLocations {

	    public string getTokenTemplateResourceUrl() {
			return "/templates/Token.template";
		}
		public string getTokenManagerTemplateResourceUrl() {
			return "/templates/TokenManager.template";
		}
		public string getTokenMgrErrorTemplateResourceUrl() {
			return "/templates/TokenMgrError.template";
		}
		public string getJavaCharStreamTemplateResourceUrl() {
			return "/templates/JavaCharStream.template";
		}

		public string getCharStreamTemplateResourceUrl() {
			return "/templates/CharStream.template";
		}
		public string getSimpleCharStreamTemplateResourceUrl() {
			return "/templates/SimpleCharStream.template";
		}

		public string getParseExceptionTemplateResourceUrl() {
			return "/templates/ParseException.template";
		}

}

  public static readonly JavaResourceTemplateLocations RESOURCES_JAVA_CLASSIC = new JavaResourceTemplateLocationImpl();
  public static readonly JavaResourceTemplateLocations RESOURCES_JAVA_MODERN = new JavaModernResourceTemplateLocationImpl();


  /**
   * Replaces all backslahes with double backslashes.
   */
  static string replaceBackslash(string str)
  {
    StringBuilder b;
    int i = 0, len = str.Length;

    while (i < len && str.charAt(i++) != '\\') ;

    if (i == len)  // No backslash found.
      return str;

    char c;
    b = new StringBuilder();
    for (i = 0; i < len; i++)
      if ((c = str.charAt(i)) == '\\')
        b.Append("\\\\");
      else
        b.Append(c);

    return b.ToString();
  }

  /**
   * Read the version from the comment in the specified file.
   * This method does not try to recover from invalid comment syntax, but
   * rather returns version 0.0 (which will always be taken to mean the file
   * is out of date).
   * @param fileName eg Token.java
   * @return The version as a double, eg 4.1
   * @since 4.1
   */
  static double getVersion(string fileName)
  {
    final string commentHeader = "/* " + getIdString(toolName, fileName) + " Version ";
    File file = new File(Options.getOutputDirectory(), replaceBackslash(fileName));

    if (!file.exists()) {
      // Has not yet been created, so it must be up to date.
      try {
        string majorVersion = Version.majorDotMinor.replaceAll("[^0-9.]+.*", "");
        return Double.parseDouble(majorVersion);
      } catch (NumberFormatException e) {
        return 0.0; // Should never happen
      }
    }

    BufferedReader reader = null;
    try {
      reader = new BufferedReader(new FileReader(file));
      string str;
      double version = 0.0;

      // Although the version comment should be the first line, sometimes the
      // user might have put comments before it.
      while ( (str = reader.readLine()) != null) {
        if (str.startsWith(commentHeader)) {
          str = str.substring(commentHeader.Length);
          int pos = str.indexOf(' ');
          if (pos >= 0) str = str.substring(0, pos);
          if (str.Length > 0) {
            try {
              // str can be 4.09
              // or even 7.0.5
              // So far we keep only major.minor part
              // "4 qwerty"-> "4"
              // "4.09 qwerty" -> "4.09"
              // "7.0.5 qwerty" -> "7.0"
              str = str.replaceAll("(\\d+(\\.\\d+)?).*", "$1");
              version = Double.parseDouble(str);
            }
            catch (NumberFormatException nfe) {
              // Ignore - leave version as 0.0
            }
          }

          break;
        }
      }

      return version;
    }
    catch (IOException ioe)
    {
      return 0.0;
    }
    finally {
      if (reader != null)
      {
        try { reader.Close(); } catch (IOException e) {}
      }
    }
  }



  public static void gen_JavaCharStream(JavaResourceTemplateLocations locations) {
    try {
      final File file = new File(Options.getOutputDirectory(), "JavaCharStream.java");
      final OutputFile outputFile = new OutputFile(file, charStreamVersion, new String[] {Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC});

      if (!outputFile.needToWrite)
      {
        return;
      }

      final TextWriter ostr = outputFile.getPrintWriter();

      if (cu_to_insertion_point_1.Count != 0 &&
          ((Token)cu_to_insertion_point_1.get(0)).kind == PACKAGE
      ) {
        for (int i = 1; i < cu_to_insertion_point_1.Count; i++) {
          if (((Token)cu_to_insertion_point_1.get(i)).kind == SEMICOLON) {
            cline = ((Token)(cu_to_insertion_point_1.get(0))).beginLine;
            ccol = ((Token)(cu_to_insertion_point_1.get(0))).beginColumn;
            for (int j = 0; j <= i; j++) {
              printToken((Token)(cu_to_insertion_point_1.get(j)), ostr);
            }
            ostr.WriteLine("");
            ostr.WriteLine("");
            break;
          }
        }
      }
      string prefix = (Options.getStatic() ? "static " : "");
      Dictionary options = new Dictionary(Options.getOptions());
      options.Add("PREFIX", prefix);

      OutputFileGenerator generator = new OutputFileGenerator(
    		  locations.getJavaCharStreamTemplateResourceUrl(), options);

      generator.generate(ostr);

      ostr.Close();
    } catch (IOException e) {
      Console.Error.WriteLine("Failed to create JavaCharStream " + e);
      JavaCCErrors.semantic_error("Could not open file JavaCharStream.java for writing.");
      throw new Error();
    }
  }



  public static void gen_SimpleCharStream(JavaResourceTemplateLocations locations) {
    try {
      final File file = new File(Options.getOutputDirectory(), "SimpleCharStream.java");
      final OutputFile outputFile = new OutputFile(file, charStreamVersion, new String[] {Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC});

      if (!outputFile.needToWrite)
      {
        return;
      }

      final TextWriter ostr = outputFile.getPrintWriter();

      if (cu_to_insertion_point_1.Count != 0 &&
          ((Token)cu_to_insertion_point_1.get(0)).kind == PACKAGE
      ) {
        for (int i = 1; i < cu_to_insertion_point_1.Count; i++) {
          if (((Token)cu_to_insertion_point_1.get(i)).kind == SEMICOLON) {
            cline = ((Token)(cu_to_insertion_point_1.get(0))).beginLine;
            ccol = ((Token)(cu_to_insertion_point_1.get(0))).beginColumn;
            for (int j = 0; j <= i; j++) {
              printToken((Token)(cu_to_insertion_point_1.get(j)), ostr);
            }
            ostr.WriteLine("");
            ostr.WriteLine("");
            break;
          }
        }
      }
      string prefix = (Options.getStatic() ? "static " : "");
      Dictionary options = new Dictionary(Options.getOptions());
      options.Add("PREFIX", prefix);

      OutputFileGenerator generator = new OutputFileGenerator(
    		  locations.getSimpleCharStreamTemplateResourceUrl(), options);

      generator.generate(ostr);

      ostr.Close();
    } catch (IOException e) {
      Console.Error.WriteLine("Failed to create SimpleCharStream " + e);
      JavaCCErrors.semantic_error("Could not open file SimpleCharStream.java for writing.");
      throw new Error();
    }
  }



  public static void gen_CharStream(JavaResourceTemplateLocations locations) {
    try {
      final File file = new File(Options.getOutputDirectory(), "CharStream.java");
      final OutputFile outputFile = new OutputFile(file, charStreamVersion, new String[] {Options.USEROPTION__STATIC, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC});

      if (!outputFile.needToWrite)
      {
        return;
      }

      final TextWriter ostr = outputFile.getPrintWriter();

      if (cu_to_insertion_point_1.Count != 0 &&
          ((Token)cu_to_insertion_point_1.get(0)).kind == PACKAGE
      ) {
        for (int i = 1; i < cu_to_insertion_point_1.Count; i++) {
          if (((Token)cu_to_insertion_point_1.get(i)).kind == SEMICOLON) {
            cline = ((Token)(cu_to_insertion_point_1.get(0))).beginLine;
            ccol = ((Token)(cu_to_insertion_point_1.get(0))).beginColumn;
            for (int j = 0; j <= i; j++) {
              printToken((Token)(cu_to_insertion_point_1.get(j)), ostr);
            }
            ostr.WriteLine("");
            ostr.WriteLine("");
            break;
          }
        }
      }

      OutputFileGenerator generator = new OutputFileGenerator(
    		  locations.getCharStreamTemplateResourceUrl(), Options.getOptions());

      generator.generate(ostr);

      ostr.Close();
    } catch (IOException e) {
      Console.Error.WriteLine("Failed to create CharStream " + e);
      JavaCCErrors.semantic_error("Could not open file CharStream.java for writing.");
      throw new Error();
    }
  }



  public static void gen_JavaModernFiles() {
	  genMiscFile("Provider.java","/templates/gwt/Provider.template" );
	  genMiscFile("StringProvider.java","/templates/gwt/StringProvider.template" );

	  // This provides a bridge to standard Java readers.
	  genMiscFile("StreamProvider.java","/templates/gwt/StreamProvider.template" );
  }

  private static void genMiscFile(string fileName, string templatePath) {
	try {
	  final File file = new File(Options.getOutputDirectory(), fileName);
	  final OutputFile outputFile = new OutputFile(file, parseExceptionVersion, new String[] {/* cba -- 2013/07/22 -- previously wired to a typo version of this option -- KEEP_LINE_COL */ Options.USEROPTION__KEEP_LINE_COLUMN});

	  if (!outputFile.needToWrite)
	  {
	    return;
	  }

	  final TextWriter ostr = outputFile.getPrintWriter();

	  if (cu_to_insertion_point_1.Count != 0 &&
	      ((Token)cu_to_insertion_point_1.get(0)).kind == PACKAGE
	  ) {
	    for (int i = 1; i < cu_to_insertion_point_1.Count; i++) {
	      if (((Token)cu_to_insertion_point_1.get(i)).kind == SEMICOLON) {
	        cline = ((Token)(cu_to_insertion_point_1.get(0))).beginLine;
	        ccol = ((Token)(cu_to_insertion_point_1.get(0))).beginColumn;
	        for (int j = 0; j <= i; j++) {
	          printToken((Token)(cu_to_insertion_point_1.get(j)), ostr);
	        }
	        ostr.WriteLine("");
	        ostr.WriteLine("");
	        break;
	      }
	    }
	  }

	  OutputFileGenerator generator = new OutputFileGenerator( templatePath, Options.getOptions());

	  generator.generate(ostr);

	  ostr.Close();
	} catch (IOException e) {
	  Console.Error.WriteLine("Failed to create " + fileName + " "+ e);
	  JavaCCErrors.semantic_error("Could not open file "+fileName+" for writing.");
	  throw new Error();
	}
}


  public static void gen_ParseException(JavaResourceTemplateLocations locations) {
    try {
      final File file = new File(Options.getOutputDirectory(), "ParseException.java");
      final OutputFile outputFile = new OutputFile(file, parseExceptionVersion, new String[] {/* cba -- 2013/07/22 -- previously wired to a typo version of this option -- KEEP_LINE_COL */ Options.USEROPTION__KEEP_LINE_COLUMN});

      if (!outputFile.needToWrite)
      {
        return;
      }

      final TextWriter ostr = outputFile.getPrintWriter();

      if (cu_to_insertion_point_1.Count != 0 &&
          ((Token)cu_to_insertion_point_1.get(0)).kind == PACKAGE
      ) {
        for (int i = 1; i < cu_to_insertion_point_1.Count; i++) {
          if (((Token)cu_to_insertion_point_1.get(i)).kind == SEMICOLON) {
            cline = ((Token)(cu_to_insertion_point_1.get(0))).beginLine;
            ccol = ((Token)(cu_to_insertion_point_1.get(0))).beginColumn;
            for (int j = 0; j <= i; j++) {
              printToken((Token)(cu_to_insertion_point_1.get(j)), ostr);
            }
            ostr.WriteLine("");
            ostr.WriteLine("");
            break;
          }
        }
      }

      OutputFileGenerator generator = new OutputFileGenerator(
    		  locations.getParseExceptionTemplateResourceUrl(), Options.getOptions());

      generator.generate(ostr);

      ostr.Close();
    } catch (IOException e) {
      Console.Error.WriteLine("Failed to create ParseException " + e);
      JavaCCErrors.semantic_error("Could not open file ParseException.java for writing.");
      throw new Error();
    }
  }



  public static void gen_TokenMgrError(JavaResourceTemplateLocations locations) {


	  bool isLegacyExceptionHandling = Options.isLegacyExceptionHandling();
	string filename = isLegacyExceptionHandling ? "TokenMgrError.java" : "TokenMgrException.java";
    try {

	final File file = new File(Options.getOutputDirectory(), filename);
      final OutputFile outputFile = new OutputFile(file, tokenMgrErrorVersion, new String[0]);

      if (!outputFile.needToWrite)
      {
        return;
      }

      final TextWriter ostr = outputFile.getPrintWriter();

      if (cu_to_insertion_point_1.Count != 0 &&
          ((Token)cu_to_insertion_point_1.get(0)).kind == PACKAGE
      ) {
        for (int i = 1; i < cu_to_insertion_point_1.Count; i++) {
          if (((Token)cu_to_insertion_point_1.get(i)).kind == SEMICOLON) {
            cline = ((Token)(cu_to_insertion_point_1.get(0))).beginLine;
            ccol = ((Token)(cu_to_insertion_point_1.get(0))).beginColumn;
            for (int j = 0; j <= i; j++) {
              printToken((Token)(cu_to_insertion_point_1.get(j)), ostr);
            }
            ostr.WriteLine("");
            ostr.WriteLine("");
            break;
          }
        }
      }



      OutputFileGenerator generator = new OutputFileGenerator( locations.getTokenMgrErrorTemplateResourceUrl(), Options.getOptions());

      generator.generate(ostr);

      ostr.Close();


    } catch (IOException e) {
      Console.Error.WriteLine("Failed to create "+filename+" " + e);
      JavaCCErrors.semantic_error("Could not open file "+filename+" for writing.");
      throw new Error();
    }
  }



  public static void gen_Token(JavaResourceTemplateLocations locations) {
    try {
      final File file = new File(Options.getOutputDirectory(), "Token.java");
      final OutputFile outputFile = new OutputFile(file, tokenVersion, new String[] {Options.USEROPTION__TOKEN_EXTENDS, Options.USEROPTION__KEEP_LINE_COLUMN, Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC});

      if (!outputFile.needToWrite)
      {
        return;
      }

      final TextWriter ostr = outputFile.getPrintWriter();

      if (cu_to_insertion_point_1.Count != 0 &&
          ((Token)cu_to_insertion_point_1.get(0)).kind == PACKAGE
      ) {
        for (int i = 1; i < cu_to_insertion_point_1.Count; i++) {
          if (((Token)cu_to_insertion_point_1.get(i)).kind == SEMICOLON) {
            cline = ((Token)(cu_to_insertion_point_1.get(0))).beginLine;
            ccol = ((Token)(cu_to_insertion_point_1.get(0))).beginColumn;
            for (int j = 0; j <= i; j++) {
              printToken((Token)(cu_to_insertion_point_1.get(j)), ostr);
            }
            ostr.WriteLine("");
            ostr.WriteLine("");
            break;
          }
        }
      }

      OutputFileGenerator generator = new OutputFileGenerator(
    		  locations.getTokenTemplateResourceUrl(), Options.getOptions());

      generator.generate(ostr);

      ostr.Close();
    } catch (IOException e) {
      Console.Error.WriteLine("Failed to create Token " + e);
      JavaCCErrors.semantic_error("Could not open file Token.java for writing.");
      throw new Error();
    }
  }



  public static void gen_TokenManager(JavaResourceTemplateLocations locations) {
    try {
      final File file = new File(Options.getOutputDirectory(), "TokenManager.java");
      final OutputFile outputFile = new OutputFile(file, tokenManagerVersion, new String[] {Options.USEROPTION__SUPPORT_CLASS_VISIBILITY_PUBLIC});

      if (!outputFile.needToWrite)
      {
        return;
      }

      final TextWriter ostr = outputFile.getPrintWriter();

      if (cu_to_insertion_point_1.Count != 0 &&
          ((Token)cu_to_insertion_point_1.get(0)).kind == PACKAGE
      ) {
        for (int i = 1; i < cu_to_insertion_point_1.Count; i++) {
          if (((Token)cu_to_insertion_point_1.get(i)).kind == SEMICOLON) {
            cline = ((Token)(cu_to_insertion_point_1.get(0))).beginLine;
            ccol = ((Token)(cu_to_insertion_point_1.get(0))).beginColumn;
            for (int j = 0; j <= i; j++) {
              printToken((Token)(cu_to_insertion_point_1.get(j)), ostr);
            }
            ostr.WriteLine("");
            ostr.WriteLine("");
            break;
          }
        }
      }

      OutputFileGenerator generator = new OutputFileGenerator(
    		  locations.getTokenManagerTemplateResourceUrl(), Options.getOptions());

      generator.generate(ostr);

      ostr.Close();
    } catch (IOException e) {
      Console.Error.WriteLine("Failed to create TokenManager " + e);
      JavaCCErrors.semantic_error("Could not open file TokenManager.java for writing.");
      throw new Error();
    }
  }


  public static void reInit()
  {
  }

}
