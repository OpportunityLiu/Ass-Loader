using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opportunity.AssLoader;
using Opportunity.AssLoader.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader.Test
{
    internal class TestHelper
    {
        private static readonly FileInfo[] testfiles = new DirectoryInfo("../../../../TestFiles/").GetFiles();

        public string TestFile
        {
            get;
            private set;
        }

        public IEnumerable<string> TestFiles
        {
            get
            {
                foreach (var item in testfiles)
                    using (var r = item.OpenText())
                        yield return r.ReadToEnd();
            }
        }

        public TestHelper(TestContext context)
        {
            this.Context = context;
            using (var r = testfiles[0].OpenText())
            {
                this.TestFile = r.ReadToEnd();
            }
        }

        public IEnumerable<KeyValuePair<string, TextReader>> LoadTestFiles()
        {
            foreach (var item in testfiles)
                yield return new KeyValuePair<string, TextReader>(item.Name, item.OpenText());
        }

        public KeyValuePair<string, TextReader> LoadTestFile()
        {
            return new KeyValuePair<string, TextReader>(testfiles[0].Name, new StringReader(this.TestFile));
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
            this.Context.WriteLine(format, values);
        }

        public async Task SaveResultAsync(string fileName, string result)
        {
            using (var writer = this.SaveResult(fileName))
                await writer.WriteAsync(result);
        }

        public TextWriter SaveResult(string fileName)
        {
            var directoryPath = Path.Combine("../../../../TestResults/", this.Context.FullyQualifiedTestClassName + "." + this.Context.TestName);
            var filePath = Path.Combine(directoryPath, fileName);
            Directory.CreateDirectory(directoryPath);
            return File.CreateText(filePath);
        }

        public async Task SaveResultAsync(string result)
        {
            using (var writer = this.SaveResult())
                await writer.WriteAsync(result);
        }

        public TextWriter SaveResult()
        {
            return this.SaveResult("TestResult");
        }

        public static Random Random { get; } = new Random();

        private static MemoryStream randomCache = new MemoryStream(65536);

        private static readonly BinaryReader randomReader = new BinaryReader(randomCache);

        static TestHelper()
        {
            refreshCache();
        }

        public static BinaryReader RandomReader
        {
            get
            {
                if (randomCache.Position > 65500)
                    refreshCache();
                return randomReader;
            }
        }

        private static void refreshCache()
        {
            var buffer = new byte[65536];
            Random.NextBytes(buffer);
            randomCache.Position = 0;
            randomCache.Write(buffer, 0, 65536);
            randomCache.Position = 0;
        }

        private readonly Stopwatch watch = new Stopwatch();

        public void StartTimer()
        {
            this.watch.Start();
        }

        public void EndTimer()
        {
            this.watch.Stop();
            this.WriteResult("Run time: {0} ms.", this.watch.Elapsed.TotalMilliseconds);
            this.watch.Reset();
        }
    }
}
