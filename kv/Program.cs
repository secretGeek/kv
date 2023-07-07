namespace kv
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using StashyLib;

    class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            var f = new FileStashy();

            string valueFromPipe = ReadPipeLine();

            if (args.Length == 0 && valueFromPipe == null)
            {
                return ListAllKeys(f);
            }

            if (args.Length == 1 && valueFromPipe == null)
            {
                if (args[0].In("h", "?", "help", "/?", "-?", "/h", "-h", "--help"))
                {
                    ShowHelp();
                    return 0;
                }

                // search by key;
                var key = args[0];
                try
                {
                    return ShowSnippet(f, key);
                }
                catch (FileNotFoundException)
                {
                    return SearchForSnippet(f, key);
                }
            }
            
            if (args.Length > 1 || (args.Length == 1 && valueFromPipe != null))
            {
                if (args[0].In("r", "-r", "/r", "--remove"))
                {
                    return DeleteKey(f, args[1]);
                }
                var key = args[0];
                return SaveKey(f, key, args, valueFromPipe);
            }
            
            System.Diagnostics.Debug.Assert(args.Length == 0 && valueFromPipe != null, "How else would you get here?");
            ShowHelp();
            return 2;
        }

        private static int SaveKey(FileStashy f, string key, string[] args, string valueFromPipe)
        {
            string data = valueFromPipe;

            if (args.Length > 1)
            {
                // all of the arguments after the first one are joined up and used as the value.
                string[] value = new string[args.Length - 1];
                for (int i = 1; i < args.Length; i++)
                {
                    value[i - 1] = args[i];
                }

                data = string.Join(" ", value);
            }

            return SaveKey(f, key, data);
        }

        private static int SaveKey(FileStashy f, string key, string data)
        {
            try
            {
                //Consider: should you be warned before overwriting a value?
                f.Save<Snippet>(new Snippet() { Value = data, Key = key }, key);
                Console.Write("Saved");
                return -5;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().ToString());
                return -4;
            }
        }

        private static int DeleteKey(FileStashy f, string key)
        {
            //To delete a key use -r
            //, e.g. kv -r a
            f.Delete<Snippet>(key);
            return -2;
        }

        private static int SearchForSnippet(FileStashy f, string key)
        {
            bool foundOne = false;

            // No exact match -- switch to a fuzzly match.
            if (!key.Contains("*") && !key.Contains("?"))
            {
                key += "*";
            }
            foreach (var s in f.ListKeys<Snippet>(key))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Key not found. ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Assume you meant: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(s);
                Console.ResetColor();
                var snippet = f.Load<Snippet>(s);
                string value = snippet.Value;
                Console.Write(value);
                Clipboard.SetDataObject(value, true);

                foundOne = true;
                break;
            }

            if (!foundOne)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Key not found. No similar keys found.");
                Console.ResetColor();
                return 5;
            }
            else
            {
                return 0;
            }
        }

        private static int ShowSnippet(FileStashy f, string key)
        {
            var snippet = f.Load<Snippet>(key);
            string value = snippet.Value;
            Console.Write(value);
            Clipboard.SetDataObject(value, true);
            return 0;
        }

        private static void ShowHelp()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(
@"
kv");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(
@" -- a key-value store integrated with the clipboard.");
            Console.ResetColor();
            Console.WriteLine(
@"inspired by: https://github.com/stevenleeg/boo");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(
@"
usage:");
            Console.ResetColor();
            Console.WriteLine(
@"kv name fred smith
    saves the value, 'fred smith' under the key, 'name'
kv name
    retrieves the value 'fred smith' straight to your clipboard.
kv
    lists all keys
kv n*
    retrieves the first key that matches the pattern n*
kv -r name
    will remove the key 'name' (and its value) from your store

Your data is here: " + FileStashy.BasePath());
        }

        private static int ListAllKeys(FileStashy f)
        {
            bool foundOne = false;
            foreach (var s in f.ListKeys<Snippet>())
            {
                if (foundOne) Console.WriteLine();
                Console.Write(s);
                foundOne = true;
            }

            if (!foundOne)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No keys found.");
                Console.ResetColor();
                ShowHelp();
            }
            return 0;
        }

        // read the whole pipeline -- or return null if there is nothing in the pipe.
        // hat tip: http://stackoverflow.com/questions/199528/c-console-receive-input-with-pipe/4074212#4074212
        private static string ReadPipeLine()
        {
            string valueFromPipe = null;
            try
            {
                bool isKeyAvailable = System.Console.KeyAvailable;
            }
            catch (InvalidOperationException)
            {
                valueFromPipe = System.Console.In.ReadToEnd();
            }
            return valueFromPipe;
        }
    }

    public class Snippet
    {
        public string Value { get; set; }
        public string Key { get; set; }
    }

    //hat tip to http://sysi.codeplex.com
    public static class Extensions
    {
        public static bool In(this string self, params string[] strings)
        {
            foreach (var s in strings)
            {
                if (s.ToLowerInvariant() == self) return true;
            }

            return false;
        }
    }
}
