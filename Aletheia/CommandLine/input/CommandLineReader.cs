using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aletheia.CommandLine.input
{
    public enum Mode
    {
        SIL, HIL, NONE
    }

    public class CommandLineReader : ICommandLineReader
    {
        private string[] arguments;

        private Dictionary<string, CommandLineArgument> argumentsList;
        private Dictionary<string, object> args;

        public CommandLineReader(string[] arguments)
        {
            this.arguments = arguments;
            argumentsList = new Dictionary<string, CommandLineArgument>();
            args = new Dictionary<string, object>();

            parse();
        }

        public Dictionary<string, CommandLineArgument> CommandLineArguments
        {
            get { return argumentsList; }
        }

        public Dictionary<string, object> Arguments
        {
            get { return args; }
        }

        private void parse()
        {
            foreach (string arg in arguments)
            {
                string argument = arg.Trim();
                string tmpKey = argument.Split('=')[0].Trim();
                try
                {
                    if (tmpKey.Equals(PossibleCommandLineArguments.OPERATION, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.OPERATION;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.MODE, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.MODE;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.PROJECT_PATH, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.PROJECT_PATH;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.VISUAL_COVERAGE, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.VISUAL_COVERAGE;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        bool val = IConverter.changeType<bool>(value);
                        args.Add(key, val);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.DEGREE_OF_PARALLELISM, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.DEGREE_OF_PARALLELISM;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        int val = IConverter.changeType<int>(value);
                        args.Add(key, val);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.EXECUTION_TIMEOUT, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.EXECUTION_TIMEOUT;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        int val = IConverter.changeType<int>(value);
                        args.Add(key, val);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.PROJECT_NAME, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.PROJECT_NAME;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.GTEST_PATH, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.GTEST_PATH;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.PROJECT_MODE, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.PROJECT_MODE;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.SOURCE_DIRECTORY, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.SOURCE_DIRECTORY;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.OUTPUT_DIRECTORY, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.OUTPUT_DIRECTORY;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.EXPORT_PATTERN, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.EXPORT_PATTERN;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.SEPARATOR, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.SEPARATOR;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        char val = value[0];
                        args.Add(key, val);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.INPUT_DIRECTORY, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.INPUT_DIRECTORY;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.BINARY, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.BINARY;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        bool val = IConverter.changeType<bool>(value);
                        args.Add(key, val);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.CLUSTERING, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.CLUSTERING;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        bool val = IConverter.changeType<bool>(value);
                        args.Add(key, val);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.FAULT_RANKING_METRIC, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.FAULT_RANKING_METRIC;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.INPUT_PATH, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.INPUT_PATH;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.LINKAGE_METHOD, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.LINKAGE_METHOD;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.LINKAGE_METRIC, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.LINKAGE_METRIC;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.CLUSTERING_METHOD, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.CLUSTERING_METHOD;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.SIMILARITY_THRESHOLD, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.SIMILARITY_THRESHOLD;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.COMPARISON_RANGE, StringComparison.OrdinalIgnoreCase))
                    {
                        string key = PossibleCommandLineArguments.COMPARISON_RANGE;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if(tmpKey.Equals(PossibleCommandLineArguments.NUM_REPRESENTATIVE_TEST, StringComparison.OrdinalIgnoreCase))
                    {
                        String key = PossibleCommandLineArguments.NUM_REPRESENTATIVE_TEST;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);

                    }
                    else if(tmpKey.Equals(PossibleCommandLineArguments.FUNCTION_COVERAGE, StringComparison.OrdinalIgnoreCase))
                    {
                        String key = PossibleCommandLineArguments.FUNCTION_COVERAGE;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.INVOKED_FUNCTION_COVERAGE, StringComparison.OrdinalIgnoreCase))
                    {
                        String key = PossibleCommandLineArguments.INVOKED_FUNCTION_COVERAGE;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.INVOKED_FUNCTION_WITH_PARAM_COVERAGE, StringComparison.OrdinalIgnoreCase))
                    {
                        String key = PossibleCommandLineArguments.INVOKED_FUNCTION_WITH_PARAM_COVERAGE;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.COUNTING_FUNCTION_INVOKATION_COVERAGE, StringComparison.OrdinalIgnoreCase))
                    {
                        String key = PossibleCommandLineArguments.COUNTING_FUNCTION_INVOKATION_COVERAGE;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.LINE_COVERAGE, StringComparison.OrdinalIgnoreCase))
                    {
                        String key = PossibleCommandLineArguments.LINE_COVERAGE;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                    else if (tmpKey.Equals(PossibleCommandLineArguments.DEBUG, StringComparison.OrdinalIgnoreCase))
                    {
                        String key = PossibleCommandLineArguments.DEBUG;
                        string value = argument.Split('=')[1].Trim();
                        addNewArgumentToArgumentsList(key, value);
                        args.Add(key, value);
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            }
        }

        private void addNewArgumentToArgumentsList(string key, string value)
        {
            if (argumentsList.Keys.Contains(key))
            {
                throw new NotImplementedException();
            }

            argumentsList.Add(key.Trim(), new CommandLineArgument(key.Trim(), value.Trim()));
        }
    }
}

