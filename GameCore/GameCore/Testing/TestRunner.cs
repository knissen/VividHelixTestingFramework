using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using VividHelix.HotReload.GameCore.Models;
using Debug = UnityEngine.Debug;
using NUnit.Framework;
using NUnit.Common;
using NUnitLite;
using System.Text;

namespace VividHelix.HotReload.Tests
{
    public class TestRunner : GameModel
    {
        public static TestRunner Instance;

        public KeyCode runTestsKeyCode = KeyCode.RightBracket;

        private BlockingCollection<Func<object>> toExecute = new BlockingCollection<Func<object>>();
        private BlockingCollection<object> result = new BlockingCollection<object>();
        private bool runningTests;
        private bool fromCommandLine;

        public TestRunner()
        {
            Instance = this;
        }

        protected override void Init()
        {
            base.Init();

            var args = Environment.GetCommandLineArgs();

            if (args.Contains("/runTests"))
            {
                Debug.LogError($"Running tests");
                fromCommandLine = true;

                RunTestsInThread();
            }
        }

        protected override void Update()
        {
            base.Update();

            if (runningTests && toExecute.TryTake(out var func))
                result.Add(func());

#if UNITY_EDITOR
        if (Input.GetKeyDown(runTestsKeyCode))
        {
            RunTestsInThread();
        }
#endif
        }

        private void RunTestsInThread()
        {
            Debug.LogError($"Running tests in thread!!!");

            new Thread(RunTests).Start();
        }

        private void RunTests()
        {
            runningTests = true;

            try
            {
                if (fromCommandLine)
                    Thread.Sleep(1000);

                Thread.CurrentThread.Name = "Test Runner Thread";
                Thread.CurrentThread.IsBackground = true;
                var stringWriter = new StringWriter();
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                // Set default category
                var categoryParameter = "cat==WIP";

                var cmdLineCategoryParameter = Environment.GetCommandLineArgs().FirstOrDefault(x => x.StartsWith("cat"));

                if (!string.IsNullOrEmpty(cmdLineCategoryParameter))
                    categoryParameter = cmdLineCategoryParameter;

                //NNTest.RES_WIDTH = CmdLineIntParam("/width=") ?? 1280;
                //NNTest.RES_HEIGHT = CmdLineIntParam("/height=") ?? 720;

                var suffix = CmdLineStringParam("/resultSuffix=") ?? DateTime.Now.Ticks.ToString();
                var resultsFileName = $"test_results_{suffix}.xml";

                var autoRun = new AutoRun();
                var result = $"--result={resultsFileName};format=nunit3";

                autoRun.Execute(
                    new[]
                    {
                            result,
                            "--noheader",
                            "--where", categoryParameter,
                            "--teamcity"
                    },
                    new ExtendedTextWrapper(stringWriter), new StringReader(""));

                Debug.LogError($"Full output: {stringWriter}");
                Debug.LogError($"{ExtractTestRunInfo(stringWriter.ToString(), stopwatch.ElapsedMilliseconds)}");

                Thread.Sleep(3000);

#if UNITY_STANDALONE
            OnMainThread(Application.Quit);
#endif
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            runningTests = false;
        }

        private static string CmdLineStringParam(string paramPrefix)
        {
            var paramValue = Environment.GetCommandLineArgs().FirstOrDefault(x => x.StartsWith(paramPrefix));

            if (!string.IsNullOrEmpty(paramValue))
                paramValue = paramValue.Substring(paramPrefix.Length, paramValue.Length - paramPrefix.Length);

            return paramValue;
        }

        public object OnMainThread(Action toExecute)
        {
            this.toExecute.Add(() =>
            {
                toExecute();
                return true;
            });
            return result.Take();
        }

        private string ExtractTestRunInfo(string nunitOutput, long elapsedMs)
        {
            var sb = new StringBuilder($"Took {elapsedMs / 1000}s ");
            string line;
            var stringReader = new StringReader(nunitOutput);
            while ((line = stringReader.ReadLine()) != null)
            {
                if (line.Contains("Overall result:") || line.Contains("Test Count:"))
                    sb.Append(line.Trim()).Append(" ");
            }

            var result = sb.ToString();
            var passed = result.Contains("Overall result: Passed");
            var color = passed ? "00ff00" : "FFC0CB";
            var body = passed ? "" : nunitOutput;
            return $"NNTests: <color=#{color}>{result}</color>\n{body}";
        }
    }
}
