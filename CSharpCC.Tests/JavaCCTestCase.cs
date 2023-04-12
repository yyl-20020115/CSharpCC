/**
 *
 */
namespace org.javacc;


/**
 * An ancestor class to enable transition to a different directory structure.
 *
 * @author timp
 * @since 2 Nov 2007
 *
 */
public abstract class CSharpCCTestCase
{

    public readonly string Name;
    /**
     * @param name the test name
     */
    public CSharpCCTestCase(string name = "CSharpCCTestCase")
    {
        this.Name= name;    
    }

    /**
     * @return the documentation output directory name string relative to the root
     */
    public string getJJDocOutputDirectory()
    {
        return "www/doc/";
        //return "src/site/resources/";
    }

    /**
     * Where the input jj files are located
     * @return the directory name string relative to the root
     */
    public string getJJInputDirectory()
    {
        return "src/main/javacc/";
        //return "src/org/javacc/parser/";
        //return "src/main/javacc/org/javacc/parser/";
    }
}
