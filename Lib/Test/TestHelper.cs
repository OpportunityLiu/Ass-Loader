using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssLoader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using AssLoader.Collections;

namespace Test
{
    internal class TestHelper
    {
        private static readonly FileInfo[] testfiles = new DirectoryInfo("../../../TestFiles/").GetFiles();

        public string TestFile
        {
            get;
            private set;
        }

        public IEnumerable<string> TestFiles
        {
            get
            {
                foreach(var item in testfiles)
                    using(var r = item.OpenText())
                        yield return r.ReadToEnd();
            }
        }

        public TestHelper(TestContext context)
        {
            this.Context = context;
            using(var r = testfiles[0].OpenText())
            {
                this.TestFile = r.ReadToEnd();
            }
        }

        public IEnumerable<KeyValuePair<string, TextReader>> LoadTestFiles()
        {
            foreach(var item in testfiles)
                yield return new KeyValuePair<string, TextReader>(item.Name, item.OpenText());
        }

        public KeyValuePair<string, TextReader> LoadTestFile()
        {
            return new KeyValuePair<string, TextReader>(testfiles[0].Name, new StringReader(TestFile));
        }

        public TestContext Context
        {
            private set;
            get;
        }

        public static void Init()
        {
            new Subtitle<AssScriptInfo>(new AssScriptInfo());
            new Style("Default");
            new SubEvent();
        }

        public void WriteResult(string format, params object[] values)
        {
            Context.WriteLine(format, values);
        }

        public async Task SaveResultAsync(string fileName, string result)
        {
            using(var writer = SaveResult(fileName))
                await writer.WriteAsync(result);
        }

        public TextWriter SaveResult(string fileName)
        {
            var directoryPath = Path.Combine(Context.TestDir, Context.FullyQualifiedTestClassName + "." + Context.TestName);
            var filePath = Path.Combine(directoryPath, fileName);
            Context.AddResultFile(filePath);
            Directory.CreateDirectory(directoryPath);
            return File.CreateText(filePath);
        }

        public async Task SaveResultAsync(string result)
        {
            using(var writer = SaveResult())
                await writer.WriteAsync(result);
        }

        public TextWriter SaveResult()
        {
            return SaveResult("TestResult");
        }

        private static Random random = new Random();

        public static Random Random
        {
            get
            {
                return random;
            }
        }

        private static MemoryStream randomCache = new MemoryStream(65536);

        private static BinaryReader randomReader = new BinaryReader(randomCache);

        static TestHelper()
        {
            refreshCache();
        }

        public static BinaryReader RandomReader
        {
            get
            {
                if(randomCache.Position > 65500)
                    refreshCache();
                return randomReader;
            }
        }

        private static void refreshCache()
        {
            var buffer = new byte[65536];
            random.NextBytes(buffer);
            randomCache.Position = 0;
            randomCache.Write(buffer, 0, 65536);
            randomCache.Position = 0;
        }

        private Stopwatch watch = new Stopwatch();

        public void StartTimer()
        {
            watch.Start();
        }

        public void EndTimer()
        {
            watch.Stop();
            WriteResult("Run time: {0} ms.", watch.Elapsed.TotalMilliseconds);
            watch.Reset();
        }
    }
}
