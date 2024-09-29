using Spectre.Console;
using System.Configuration;
using TextCopy;

namespace Tool.Crypt
{
    public class Program
    {
        static string copyText = "";
        const string defaultCmdTitle = "【Ctrl + C】 复制到剪贴板";

        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = defaultCmdTitle;

            // 注册控制台的Ctrl+C事件处理程序
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            // 获取 key
            string defaultKey = ConfigurationManager.AppSettings["EncryKey"] ?? "";
            if (string.IsNullOrEmpty(defaultKey))
            {
                Console.WriteLine("缺少密钥 EncryKey，请在配置文件进行设置");
                return;
            }

            Console.WriteLine();

            do
            {
                var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("上下选择，[green]回车确认[/]")
                    .PageSize(15)
                    .MoreChoicesText("[grey](上下选择操作)[/]")
                    .AddChoices(new[] {
                        "解密", "加密", "关闭程序"
                    }));

                switch (option)
                {
                    case "加密":
                        var enStr = AnsiConsole.Ask<string>("输入要[bold]加密[/]的字符串, [grey]后可空格输入自定义密钥[/]:");
                        En(defaultKey, enStr);
                        break;
                    case "解密":
                        var deStr = AnsiConsole.Ask<string>("输入要[bold]解密[/]字符串, [grey]后可空格输入自定义密钥[/]:");
                        De(defaultKey, deStr);
                        break;
                }

                if (option == "关闭程序")
                {
                    break;
                }

                Console.WriteLine();

            } while (true);

            // 关闭程序
            Environment.Exit(0);
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
    }
}
