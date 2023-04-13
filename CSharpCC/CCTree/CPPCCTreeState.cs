// Copyright 2011 Google Inc. All Rights Reserved.
// Author: sreeni@google.com (Sreeni Viswanadha)

using CSharpCC.Parser;
namespace CSharpCC.CCTree;
/**
 * Generate the State of a tree.
 */
public static class CPPCCTreeState
{

    static readonly string CCTStateVersion = Version.MajorDotMinor;

    public static void GenerateTreeState()
    {
        var options = CCTreeOptions.getOptions();
        options.Add(Options.NONUSER_OPTION__PARSER_NAME, CCTreeGlobals.ParserName);
        var filePrefix = System.IO.Path.Combine(CCTreeOptions.GetJJTreeOutputDirectory(), "JJT" + CCTreeGlobals.ParserName + "State");

        OutputFile outputFile = new((filePrefix + ".h"), CCTStateVersion, Array.Empty<string>());
        CPPNodeFiles.GenerateFile(outputFile, "/templates/cpp/JJTTreeState.h.template", options);

        outputFile = new OutputFile((filePrefix + ".cc"), CCTStateVersion, Array.Empty<string>());
        CPPNodeFiles.GenerateFile(outputFile, "/templates/cpp/JJTTreeState.cc.template", options);

    }

}
