using Spectre.Console;
using TextCopy;

namespace Tool.Crypt
{
    public class Program
    {
        static string copyText = "";
        const string defaultCmdTitle = "【Ctrl + C】 复制到剪贴板";
        const string OptionEncrypt = "+ 加密 +";
        const string OptionDecrypt = "- 解密 -";
        const string OptionShowKey = "[Cyan]查看密钥[/]";
        const string OptionUpdateKey = "[yellow]修改密钥[/]";
        const string OptionRemoveKey = "[red]删除密钥[/]";
        const string OptionShutDown = "关闭程序";
        const string inputKeyTip = "[yellow]请输入[bold]密钥[/][/], [grey]字符长度 32[/]:";
        const int inputKeyMinLength = 32;

        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = defaultCmdTitle;

            // 注册控制台的Ctrl+C事件处理程序
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            do
            {
                // 获取保存密钥
                string defaultKey = InitKey();
                Console.WriteLine();

                bool isShutDown = false;
                bool isUpdateKey = false;
                bool isRemoveKey = false;

                MainTask(defaultKey, out isShutDown, out isUpdateKey, out isRemoveKey);

                if (isUpdateKey)
                {
                    // 修改密钥
                    defaultKey = UpdateKey();
                }

                if (isRemoveKey)
                {
                    // 删除密钥
                    RemoveKey();
                }

                if (isShutDown)
                {
                    // 关闭程序
                    break;
                }

            } while (true);

            // 关闭程序
            Environment.Exit(0);
        }

        private static string InitKey()
        {
            string defaultKey = KeyManage.Read();
            while (string.IsNullOrEmpty(defaultKey) || defaultKey.Length != inputKeyMinLength)
            {
                defaultKey = AnsiConsole.Ask<string>(inputKeyTip);
                if (defaultKey != null && defaultKey.Length == inputKeyMinLength)
                {
                    KeyManage.Save(defaultKey);
                }
            }
            return defaultKey;
        }

        private static string UpdateKey()
        {
            string defaultKey;
            do
            {
                defaultKey = AnsiConsole.Ask<string>(inputKeyTip);
            } while (string.IsNullOrEmpty(defaultKey) || defaultKey.Length != inputKeyMinLength);
            KeyManage.Save(defaultKey);
            return defaultKey;
        }

        private static void RemoveKey()
        {
            KeyManage.Remove();
            AnsiConsole.MarkupLine("[green]密钥删除成功![/]");
            AnsiConsole.WriteLine();
        }

        /// <summary>
        /// 主要业务部分
        /// </summary>
        /// <param name="defaultKey"></param>
        /// <param name="isUpdateKey"></param>
        /// <param name="isRemoveKey"></param>
        private static void MainTask(string defaultKey, out bool isShutDown, out bool isUpdateKey, out bool isRemoveKey)
        {
            do
            {
                var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("按上下键进行选择，[green]按回车键确认[/]")
                    .PageSize(15)
                    .HighlightStyle(new Style(foreground: Color.Pink1, background: Color.DeepPink1))
                    .MoreChoicesText("[grey](上下选择操作)[/]")
                    .AddChoices(new[] {
                        OptionDecrypt, OptionEncrypt, OptionShowKey, OptionUpdateKey, OptionRemoveKey, OptionShutDown
                    }));

                switch (option)
                {
                    case OptionEncrypt:
                        var enStr = AnsiConsole.Ask<string>("输入要[bold]加密[/]的字符串, [grey]后可空格输入自定义密钥[/]:");
                        En(defaultKey, enStr);
                        break;
                    case OptionDecrypt:
                        var deStr = AnsiConsole.Ask<string>("输入要[bold]解密[/]字符串, [grey]后可空格输入自定义密钥[/]:");
                        De(defaultKey, deStr);
                        break;
                    case OptionShowKey:
                        ShowKey();
                        break;
                }

                if (option == OptionShutDown || option == OptionUpdateKey || option == OptionRemoveKey)
                {
                    isShutDown = option == OptionShutDown;
                    isUpdateKey = option == OptionUpdateKey;
                    isRemoveKey = option == OptionRemoveKey;
                    break;
                }

                Console.WriteLine();

            } while (true);
        }

        


        /// <summary>
        /// 复制加解密的文本内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            if (e.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                ClipboardService.SetText(copyText);
                var strFix = copyText.Length >= 10 ? copyText.Substring(0, 10) : copyText;
                Console.Title = $"【{strFix}...】文本已复制到剪贴板";
                Task.Delay(500).Wait();
                Console.Title = defaultCmdTitle;

                // 阻止控制台默认行为（终止程序）
                e.Cancel = true;
            }
        }

        private static void En(string defaultKey, string enStr)
        {
            Console.WriteLine();

            if (string.IsNullOrEmpty(enStr))
            {
                AnsiConsole.MarkupLine("[red]输入错误[/]");
            }

            string[] array = enStr.Split(' ').ToArray();

            if (array.Length == 1)
            {
                var result = MainCrypt.Encrypt(array[0], defaultKey);
                copyText = result;
                if (string.IsNullOrEmpty(result))
                {
                    AnsiConsole.MarkupLine("[red]加密失败[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[underline][green]{0}[/][/]", result);
                }
            }
            else if (array.Length == 2)
            {
                // 自定义密钥加密
                var result = MainCrypt.Encrypt(array[0], array[1]);
                copyText = result;
                if (string.IsNullOrEmpty(result))
                {
                    AnsiConsole.MarkupLine("[red]加密失败[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[underline][green]{0}[/][/]", result);
                }
            }
        }

        private static void De(string defaultKey, string deStr)
        {
            Console.WriteLine();

            if (string.IsNullOrEmpty(deStr))
            {
                copyText = "";
                AnsiConsole.MarkupLine("[red]输入错误[/]");
            }

            string[] array = deStr.Split(' ').ToArray();

            if (array.Length == 1)
            {
                var result = MainCrypt.Decrypt(array[0], defaultKey);
                copyText = result;
                if (string.IsNullOrEmpty(result))
                {
                    AnsiConsole.MarkupLine("[red]解密失败[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[underline][green]{0}[/][/]", result);
                }
            }
            else if (array.Length == 2)
            {
                // 自定义密钥解密
                var result = MainCrypt.Decrypt(array[0], array[1]);
                copyText = result;
                if (string.IsNullOrEmpty(result))
                {
                    AnsiConsole.MarkupLine("[red]解密失败[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[underline][green]{0}[/][/]", result);
                }
            }
        }

        private static void ShowKey()
        {
            var key = KeyManage.Read();
            copyText = key;
            AnsiConsole.MarkupLine($"[Cyan]{key}[/]");
        }
    }
}
