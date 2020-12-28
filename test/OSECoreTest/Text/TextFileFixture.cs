using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OSECoreTest.Text
{
    public class TextFileFixture : FileFixture
    {
        private string _taggedBlockFilename = "TaggedBlock.txt";
        public string TaggedBlockFilePath;
        public TextFileFixture()
        {
            CreateTaggedBlockFile();
        }

        public override void Dispose()
        {
            File.Delete(TaggedBlockFilePath);
            base.Dispose();
        }

        private void CreateTaggedBlockFile()
        {
            TaggedBlockFilePath = Path.Combine(TestDir, _taggedBlockFilename);
            using (StreamWriter w = new StreamWriter(TaggedBlockFilePath))
            {
                w.WriteLine("aaa</b>");
                w.WriteLine("</a>");
            }
        }
    }
}
