// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)

using org.javacc.parser;

namespace org.javacc.jjtree;



/**
 * Generate the State of a tree.
 */
public static class CPPJJTreeState
{

    static readonly string JJTStateVersion = Version.majorDotMinor;

    static void generateTreeState()
    {
        Dictionary options = JJTreeOptions.getOptions();
        options.Add(Options.NONUSER_OPTION__PARSER_NAME, JJTreeGlobals.parserName);
        string filePrefix = new File(JJTreeOptions.getJJTreeOutputDirectory(), "JJT" + JJTreeGlobals.parserName + "State").getAbsolutePath();

        OutputFile outputFile = new OutputFile(new File(filePrefix + ".h"), JJTStateVersion, new String[0]);
        CPPNodeFiles.generateFile(outputFile, "/templates/cpp/JJTTreeState.h.template", options);

        outputFile = new OutputFile(new File(filePrefix + ".cc"), JJTStateVersion, new String[0]);
        CPPNodeFiles.generateFile(outputFile, "/templates/cpp/JJTTreeState.cc.template", options);

    }

}

/*end*/
