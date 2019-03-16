using System;
using System.IO;

namespace MyCompany.MyStack.MyRestApp.Tests
{
    public class TestUtils
    {
        private Lazy<string> _testDir = new Lazy<string>(() => {
            var testDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(testDir);
            return testDir;
        });

        public string TestDir => _testDir.Value;

        public void Dispose()
        {
            if (_testDir.IsValueCreated)
            {
                Directory.Delete(_testDir.Value, true);
            }
        }
    }
}
