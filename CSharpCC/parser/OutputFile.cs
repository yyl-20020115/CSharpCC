/* Copyright (c) 2007, Paul Cager.
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

using org.javacc.jjtree;
using System.Text;

namespace org.javacc.parser;

/**
 * This class handles the creation and maintenance of the boiler-plate classes,
 * such as Token.java, JavaCharStream.java etc. 
 *
 * It is responsible for:
 *
 * <ul>
 * <li>Writing the JavaCC header lines to the file.</li>
 * <li>Writing the checksum line.</li>
 * <li>Using the checksum to determine if an existing file has been changed by
 * the user (and so should be left alone).</li>
 * <li>Checking any existing file's version (if the file can not be
 * overwritten).</li>
 * <li>Checking any existing file's creation options (if the file can not be
 * overwritten).</li>
 * <li></li>
 * </ul>
 *
 * @author Paul Cager
 *
 */
public class OutputFile
{
    private static readonly string MD5_LINE_PART_1 = "/* JavaCC - OriginalChecksum=";
    private static readonly string MD5_LINE_PART_1q = "/\\* JavaCC - OriginalChecksum=";
    private static readonly string MD5_LINE_PART_2 = " (do not edit this line) */";
    private static readonly string MD5_LINE_PART_2q = " \\(do not edit this line\\) \\*/";

    TrapClosePrintWriter pw;
    readonly TextWriter dos;

    string toolName = JavaCCGlobals.ToolName;
    readonly string file;
    readonly string compatibleVersion;
    readonly String[] options;

    /**
     * Create a new OutputFile.
     *
     * @param file
     *            the file to write to.
     * @param compatibleVersion
     *      the minimum compatible JavaCC version.
     * @param options
     *      if the file already exists, and cannot be overwritten, this is
     *      a list of options (such s STATIC=false) to check for changes.
     * @throws IOException
     */
    public OutputFile(string file, string compatibleVersion, String[] options)
    {
        this.file = file;
        this.compatibleVersion = compatibleVersion;
        this.options = options;

        if (File.Exists(file))
        {
            // Generate the checksum of the file, and compare with any value 
            // stored
            // in the file.

            StreamReader reader = new StreamReader(file);
            MessageDigest digest;
            try
            {
                digest = MessageDigest.getInstance("MD5");
            }
            catch (  e)
            {
                throw (IOException)(new IOException("No MD5 implementation")
                .initCause(e));
            }
            DigestOutputStream digestStream = new DigestOutputStream(
                new NullOutputStream(), digest);
            TextWriter pw = new TextWriter(digestStream);
            string line;
            string existingMD5 = null;
            while ((line = br.readLine()) != null)
            {
                if (line.StartsWith(MD5_LINE_PART_1))
                {
                    existingMD5 = line.replaceAll(MD5_LINE_PART_1q, "").replaceAll(
                        MD5_LINE_PART_2q, "");
                }
                else
                {
                    pw.WriteLine(line);
                }
            }

            pw.Close();

            string calculatedDigest = ToHexString(digestStream
                .getMessageDigest().digest());

            if (existingMD5 == null || existingMD5 != (calculatedDigest))
            {
                // No checksum in file, or checksum differs.
                NeedToWrite = false;

                if (compatibleVersion != null)
                {
                    CheckVersion(file, compatibleVersion);
                }

                if (options != null)
                {
                    CheckOptions(file, options); 
                }
                 
            }
            else
            {
                // The file has not been altered since JavaCC created it.
                // Rebuild it.
                Console.WriteLine("File \"" + file
                    + "\" is being rebuilt.");
                NeedToWrite = true;
            }
        }
        else
        {
            // File does not exist
            Console.WriteLine("File \"" + file + "\" does not exist.  Will create one.");
            NeedToWrite = true;
        }
    }

    public OutputFile(string file)
        : this(file, null, null)
    {
       
    }

    public readonly bool NeedToWrite = true;

    /**
     * Output a warning if the file was created with an incompatible version
     * of JavaCC.
     * @param fileName
     * @param versionId
     */
    private void CheckVersion(string file, string versionId)
    { 
        string firstLine = "/* " + JavaCCGlobals.GetIdString(toolName, file) + " Version ";

        try
        {
            using var reader = new StreamReader(file);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith(firstLine))
                {
                    string version = line.Replace(".*Version ", "").Replace(" \\*/", "");
                    if (version != versionId)
                    {
                        JavaCCErrors.Warning(file
                            + ": File is obsolete.  Please rename or delete this file so"
                            + " that a new one can be generated for you.");
                        JavaCCErrors.Warning(file
                            + " file   version: " + version
                            + " javacc version: " + versionId);
                    }
                    return;
                }
            }
            // If no version line is found, do not output the warning.
        }
        catch (FileNotFoundException e1)
        {
            // This should never happen
            JavaCCErrors.SemanticError("Could not open file " + file + " for writing.");
            throw new Error();
        }
        catch (IOException e2)
        {
        }
    }

    /**
     * Read the options line from the file and compare to the options currently in
     * use. Output a warning if they are different.
     *
     * @param fileName
     * @param options
     */
    private static void CheckOptions(string file, String[] options)
    {
        try
        {
            using var reader = new StreamReader(file);

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("/* JavaCCOptions:"))
                {
                    string currentOptions = Options.GetOptionsString(options);
                    if (!line.Contains(currentOptions, StringComparison.CurrentCulture))
                    {
                        JavaCCErrors
                        .Warning(file
                            + ": Generated using incompatible options. Please rename or delete this file so"
                            + " that a new one can be generated for you.");
                    }
                    return;
                }
            }
        }
        catch (FileNotFoundException e1)
        {
            // This should never happen
            JavaCCErrors.SemanticError($"Could not open file {file} for writing.");
            throw new Error();
        }
        catch (IOException e2)
        {
        }

        // Not found so cannot check
    }

    /**
     * Return a TextWriter object that may be used to write to this file. Any
     * necessary header information is written by this method.
     *
     * @return
     * @throws IOException
     */
    public TextWriter GetPrintWriter()
    {
        if (pw == null)
        {
            pw = new TrapClosePrintWriter(this.dos, this); ;

            // Write the headers....
            string version = compatibleVersion ?? Version.VersionNumber;
            pw.WriteLine("/* "
                + JavaCCGlobals.GetIdString(toolName, file)
                + " Version " + version + " */");
            if (options != null)
            {
                pw.WriteLine("/* JavaCCOptions:" + Options.GetOptionsString(options) + " */");
            }
        }

        return pw;
    }

    /**
     * Close the OutputFile, writing any necessary trailer information
     * (such as a checksum).
     * @throws IOException
     */
    public void Close()
    {

        // Write the trailer (checksum).
        // Possibly rename the .java.tmp to .java??
        if (pw != null)
        {
            pw.WriteLine(MD5_LINE_PART_1 + GetMD5sum() + MD5_LINE_PART_2);
            pw.ClosePrintWriter();
            //    file.renameTo(dest)
        }
    }

    private string GetMD5sum()
    {
        pw.Flush();
        byte[] digest = dos.getMessageDigest().digest();
        return ToHexString(digest);
    }

    private static readonly char[] HEX_DIGITS = new char[] { '0', '1', '2', '3',
    '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

    private static string ToHexString(byte[] bytes)
    {
        var sb = new StringBuilder(32);
        for (int i = 0; i < bytes.Length; i++)
        {
            byte b = bytes[i];
            sb.Append(HEX_DIGITS[(b & 0xF0) >> 4]).Append(HEX_DIGITS[b & 0x0F]);
        }
        return sb.ToString();
    }


    public class NullOutputStream : FileStream
    {

        public void write(byte[] arg0, int arg1, int arg2)
        {
        }

        public void write(byte[] arg0)
        {
        }

        public void write(int arg0)
        {
        }
    }

    public class TrapClosePrintWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.Default;

        readonly OutputFile file;
        public TrapClosePrintWriter(TextWriter writer, OutputFile file)
            :base()
        {
            this.file = file;
        }

        public void ClosePrintWriter()
        {
            base.Close();
        }

        public override void Close()
        {
            try
            {
                this.file.Close();
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("Could not close " + file);
            }
            base.Close();
        }
    }

    /**
     * @return the toolName
     */
    public string GetToolName()
    {
        return toolName;
    }

    /** s
     * @param toolName
     *            the toolName to set
     */
    public void SetToolName(string toolName)
    {
        this.toolName = toolName;
    }

    public string GetPath()
    {
        return file;
    }
}
