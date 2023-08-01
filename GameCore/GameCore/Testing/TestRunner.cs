// Copyright 2023 Vivid Helix
// This file is part of ViviHelixTestFramework.
// ViviHelixTestFramework is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// ViviHelixTestFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License along with ViviHelixTestFramework. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using VividHelix.HotReload.GameCore.Models;
using Debug = UnityEngine.Debug;
using NUnit.Common;
using NUnitLite;
using System.Text;
using TMPro;

namespace VividHelix.HotReload.Tests
{
    public class TestRunner : GameModel
    {
        public static TestRunner Instance;

        public KeyCode runTestsKeyCode = KeyCode.T;

        private BlockingCollection<Func<object>> toExecute = new BlockingCollection<Func<object>>();
        private BlockingCollection<object> result = new BlockingCollection<object>();
        private bool runningTests;
        private bool fromCommandLine;

        private TextMeshProUGUI infoDisplay;

        public TestRunner()
        {
            Instance = this;
        }

        protected override void Init()
        {
            base.Init();

            var viewMarker = Resources.FindObjectsOfTypeAll<ViewMarker>().FirstOrDefault(x => x.name == "InfoDisplay");

            if (viewMarker == null)
            {
                Debug.LogError($"Live view marker not found for gameobject: InfoDisplay");
            }
            else
            {
                infoDisplay = viewMarker.GetComponent<TextMeshProUGUI>();
            }

            var args = Environment.GetCommandLineArgs();

            if (infoDisplay != null)
            {
                infoDisplay.text = "Command line args: ";

                for (int i = 1; i < args.Length; i++)
                {
                    infoDisplay.text += args[i] + " ";
                }
            }

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

            if (Input.GetKeyDown(runTestsKeyCode))
            {
                if(infoDisplay != null)
                    infoDisplay.text = "Running Tests. Check root folder for XML output.";

                RunTestsInThread();
            }
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
                var categoryParameter = "cat==CoreTests";

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
