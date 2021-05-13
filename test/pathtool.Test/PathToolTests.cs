using OSETesting;
using PathTool;
using System;
using System.IO;
using Xunit;
using OSECore.Text;
using OSECore.IO;

namespace pathtool.Test
{
    public class PathToolTests : IClassFixture<PathDirectoryFixture>
    {
        private readonly PathDirectoryFixture _fixture;
        public PathToolTests(PathDirectoryFixture fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public void VersionTest()
        {
            string file = "version.txt";
            string test = _fixture.GetTempFilePath(file);
            Console.SetError(new StreamWriter(test));
            Program.Main(new string[] { "--version" });
            Console.Error.Close();
            Assert.True(_fixture.CompareFiles(_fixture.GetOutputFilePath(file), test));
        }
        [Fact]
        public void HelpTest()
        {
            string file = "help.txt";
            string test = _fixture.GetTempFilePath(file);
            Console.SetError(new StreamWriter(test));
            Program.Main(new string[] { "--help" });
            Console.Error.Close();
            Assert.True(_fixture.CompareFiles(_fixture.GetOutputFilePath(file), test));
        }
        [Theory]
        [InlineData(0,"")]
        [InlineData(1,"--verbose")]
        [InlineData(2,"-v --filter .*0.*")]
        [InlineData(3, "-v --sort")]
        public void ListTest(int testNumber, string argLine)
        {
            _fixture.SetTestPath();
            string ferror = $"list{testNumber}_error.txt";
            string fout = $"list{testNumber}_output.txt";
            string perror = _fixture.GetTempFilePath(ferror);
            string pout = _fixture.GetTempFilePath(fout);
            Console.SetError(new StreamWriter(perror));
            Console.SetOut(new StreamWriter(pout));
            string[] args = argLine.SplitLine();
            Program.Main(args);
            Console.Error.Close();
            Console.Out.Close();
            Assert.True(_fixture.CompareFiles(_fixture.GetOutputFilePath(ferror), perror));
            Assert.True(_fixture.CompareFiles(_fixture.GetOutputFilePath(fout), pout));
        }
        [Theory]
        [InlineData(0, "clean -v",'Y')]
        [InlineData(1, "clean -q",'\0')]
        [InlineData(2, "clean -v",'N')]
        public void CleanTest(int testNumber, string argLine, char commit)
        {
            _fixture.SetTestPath();
            string ferror = $"clean{testNumber}_error.txt";
            string fout = $"clean{testNumber}_output.txt";
            string perror = _fixture.GetTempFilePath(ferror);
            string pout = _fixture.GetTempFilePath(fout);
            Console.SetError(new StreamWriter(perror));
            Console.SetOut(new StreamWriter(pout));
            if (commit != '\0')
            {
                string answer = new string(new char[] { commit, '\r', '\n' });
                var ins = new StringStream(answer);
                Console.SetIn(new StreamReader(ins));
            }
            string[] args = argLine.SplitLine();
            Program.Main(args);
            Console.Error.Close();
            Console.Out.Close();
            Console.In.Close();
            Assert.True(_fixture.CompareFiles(_fixture.GetOutputFilePath(ferror), perror));
            Assert.True(_fixture.CompareFiles(_fixture.GetOutputFilePath(fout), pout));
        }
        [Theory]
        [InlineData(0, "add -v j k", 'Y')]
        [InlineData(1, "add -q j", '\0')]
        [InlineData(2, "add -v j k", 'N')]
        public void AddTest(int testNumber, string argLine, char commit)
        {
            _fixture.SetTestPath();
            string ferror = $"add{testNumber}_error.txt";
            string fout = $"add{testNumber}_output.txt";
            string perror = _fixture.GetTempFilePath(ferror);
            string pout = _fixture.GetTempFilePath(fout);
            Console.SetError(new StreamWriter(perror));
            Console.SetOut(new StreamWriter(pout));
            if (commit != '\0')
            {
                string answer = new string(new char[] { commit, '\r', '\n' });
                var ins = new StringStream(answer);
                Console.SetIn(new StreamReader(ins));
            }
            string[] args = argLine.SplitLine();
            Program.Main(args);
            Console.Error.Close();
            Console.Out.Close();
            Console.In.Close();
            Assert.True(_fixture.CompareFiles(_fixture.GetOutputFilePath(ferror), perror));
            Assert.True(_fixture.CompareFiles(_fixture.GetOutputFilePath(fout), pout));
        }
        [Theory]
        [InlineData(0, "remove -v -pl 1 3", 'Y')]
        [InlineData(1, "remove -q -p 3", '\0')]
        [InlineData(2, "remove -p 1", 'N')]
        public void RemoveTest(int testNumber, string argLine, char commit)
        {
            _fixture.SetTestPath();
            string ferror = $"remove{testNumber}_error.txt";
            string fout = $"remove{testNumber}_output.txt";
            string perror = _fixture.GetTempFilePath(ferror);
            string pout = _fixture.GetTempFilePath(fout);
            Console.SetError(new StreamWriter(perror));
            Console.SetOut(new StreamWriter(pout));
            if (commit != '\0')
            {
                string answer = new string(new char[] { commit, '\r', '\n' });
                var ins = new StringStream(answer);
                Console.SetIn(new StreamReader(ins));
            }
            string[] args = argLine.SplitLine();
            Program.Main(args);
            Console.Error.Close();
            Console.Out.Close();
            Console.In.Close();
            Assert.True(_fixture.CompareFiles(_fixture.GetOutputFilePath(ferror), perror));
            Assert.True(_fixture.CompareFiles(_fixture.GetOutputFilePath(fout), pout));
        }
        [Theory]
        [InlineData(0, "move -v -ptl 5 1 3", 'Y')]
        [InlineData(1, "move -v -ptl 1 5 3", 'Y')]
        [InlineData(2, "move -q -pt 2 1", '\0')]
        [InlineData(3, "move -v -pt 2 1", 'N')]
        public void MoveTest(int testNumber, string argLine, char commit)
        {
            _fixture.SetTestPath();
            string ferror = $"move{testNumber}_error.txt";
            string fout = $"move{testNumber}_output.txt";
            string perror = _fixture.GetTempFilePath(ferror);
            string pout = _fixture.GetTempFilePath(fout);
            Console.SetError(new StreamWriter(perror));
            Console.SetOut(new StreamWriter(pout));
            if (commit != '\0')
            {
                string answer = new string(new char[] { commit, '\r', '\n' });
                var ins = new StringStream(answer);
                Console.SetIn(new StreamReader(ins));
            }
            string[] args = argLine.SplitLine();
            Program.Main(args);
            Console.Error.Close();
            Console.Out.Close();
            Console.In.Close();
            Assert.True(_fixture.CompareFiles(_fixture.GetOutputFilePath(ferror), perror));
            Assert.True(_fixture.CompareFiles(_fixture.GetOutputFilePath(fout), pout));
        }

    }
}
