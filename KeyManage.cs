namespace Tool.Crypt
{
    /// <summary>
    /// 密钥管理
    /// </summary>
    public class KeyManage
    {
        static readonly string RootFileDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tool.Crypt");
        static readonly string configFilePath = Path.Combine(RootFileDir, "config.txt");

        // 自定义Key，用于加解密保存的key
        const string DEFAULT_KEY = "E8F8AAAD9B3B43058D2788769D442D63";

        /// <summary>
        /// 保存用户密钥
        /// </summary>
        /// <param name="key">用户密钥</param>
        public static void Save(string key)
        {
            var encryKey = MainCrypt.Encrypt(key, DEFAULT_KEY);

            // 确保路径存在
            if (!Directory.Exists(RootFileDir))
            {
                Directory.CreateDirectory(RootFileDir);
            }

            // 写入配置文件
            File.WriteAllText(configFilePath, encryKey);
        }

        /// <summary>
        /// 读取用户密钥
        /// </summary>
        /// <returns></returns>
        public static string Read()
        {
            try
            {
                string configStr = File.ReadAllText(configFilePath);
                if (string.IsNullOrEmpty(configStr)) return string.Empty;
                var userKey = MainCrypt.Decrypt(configStr, DEFAULT_KEY);
                return userKey;
            }
            catch
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// 删除
        /// </summary>
        public static void Remove()
        {
            File.Delete(configFilePath);
            Directory.Delete(RootFileDir);
        }
    }
}
