using System;
using System.IO;
using System.Text;

namespace Capybara.Utils
{
    /// <summary>
    /// 纯同步文件读写帮助类（无 async）。
    /// 所有写入操作在目标目录不存在时会自动创建目录。
    /// </summary>
    public static class FileHelper
    {
        #region 读取文本

        /// <summary>
        /// 读取文件全部文本（UTF-8 编码）。
        /// </summary>
        public static string ReadAllText(string path)
        {
            EnsureFileExists(path);
            return File.ReadAllText(path, Encoding.UTF8);
        }

        /// <summary>
        /// 使用指定编码读取文件全部文本。
        /// </summary>
        public static string ReadAllText(string path, Encoding encoding)
        {
            EnsureFileExists(path);
            return File.ReadAllText(path, encoding);
        }

        /// <summary>
        /// 读取文件全部行（UTF-8 编码）。
        /// </summary>
        public static string[] ReadAllLines(string path)
        {
            EnsureFileExists(path);
            return File.ReadAllLines(path, Encoding.UTF8);
        }

        /// <summary>
        /// 使用指定编码读取文件全部行。
        /// </summary>
        public static string[] ReadAllLines(string path, Encoding encoding)
        {
            EnsureFileExists(path);
            return File.ReadAllLines(path, encoding);
        }

        #endregion

        #region 读取字节

        /// <summary>
        /// 读取文件全部字节。
        /// </summary>
        public static byte[] ReadAllBytes(string path)
        {
            EnsureFileExists(path);
            return File.ReadAllBytes(path);
        }

        #endregion

        #region 写入文本

        /// <summary>
        /// 将文本写入文件（UTF-8 编码），若文件已存在则覆盖。自动创建目标目录。
        /// </summary>
        public static void WriteAllText(string path, string contents)
        {
            EnsureDirectoryExists(path);
            File.WriteAllText(path, contents, Encoding.UTF8);
        }

        /// <summary>
        /// 使用指定编码将文本写入文件。
        /// </summary>
        public static void WriteAllText(string path, string contents, Encoding encoding)
        {
            EnsureDirectoryExists(path);
            File.WriteAllText(path, contents, encoding);
        }

        /// <summary>
        /// 将多行文本写入文件（UTF-8 编码），若文件已存在则覆盖。自动创建目标目录。
        /// </summary>
        public static void WriteAllLines(string path, string[] contents)
        {
            EnsureDirectoryExists(path);
            File.WriteAllLines(path, contents, Encoding.UTF8);
        }

        /// <summary>
        /// 使用指定编码将多行文本写入文件。
        /// </summary>
        public static void WriteAllLines(string path, string[] contents, Encoding encoding)
        {
            EnsureDirectoryExists(path);
            File.WriteAllLines(path, contents, encoding);
        }

        #endregion

        #region 写入字节

        /// <summary>
        /// 将字节数组写入文件，覆盖已有文件。自动创建目标目录。
        /// </summary>
        public static void WriteAllBytes(string path, byte[] bytes)
        {
            EnsureDirectoryExists(path);
            File.WriteAllBytes(path, bytes);
        }

        #endregion

        #region 追加文本

        /// <summary>
        /// 将文本追加到文件末尾（UTF-8 编码）。若文件不存在则创建，自动创建目录。
        /// </summary>
        public static void AppendAllText(string path, string contents)
        {
            EnsureDirectoryExists(path);
            File.AppendAllText(path, contents, Encoding.UTF8);
        }

        /// <summary>
        /// 使用指定编码追加文本。
        /// </summary>
        public static void AppendAllText(string path, string contents, Encoding encoding)
        {
            EnsureDirectoryExists(path);
            File.AppendAllText(path, contents, encoding);
        }

        #endregion

        #region 文件操作

        /// <summary>
        /// 复制文件。若目标目录不存在则自动创建。
        /// </summary>
        public static void Copy(string sourcePath, string destPath, bool overwrite = false)
        {
            EnsureFileExists(sourcePath);
            EnsureDirectoryExists(destPath);
            File.Copy(sourcePath, destPath, overwrite);
        }

        /// <summary>
        /// 移动文件。若目标目录不存在则自动创建。
        /// </summary>
        public static void Move(string sourcePath, string destPath)
        {
            EnsureFileExists(sourcePath);
            EnsureDirectoryExists(destPath);
            File.Move(sourcePath, destPath);
        }

        /// <summary>
        /// 删除文件。若文件不存在则不做任何操作（不抛出异常）。
        /// </summary>
        public static void Delete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// 判断文件是否存在。
        /// </summary>
        public static bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// 获取文件大小（字节）。若文件不存在返回 -1。
        /// </summary>
        public static long GetFileSize(string path)
        {
            if (!File.Exists(path))
                return -1;
            return new FileInfo(path).Length;
        }

        #endregion

        #region 流操作（同步）

        /// <summary>
        /// 打开文件流进行读取（调用方负责释放流）。
        /// </summary>
        public static FileStream OpenRead(string path)
        {
            EnsureFileExists(path);
            return File.OpenRead(path);
        }

        /// <summary>
        /// 打开或创建文件流进行写入（调用方负责释放流）。自动创建目录。
        /// </summary>
        public static FileStream OpenWrite(string path)
        {
            EnsureDirectoryExists(path);
            return File.OpenWrite(path);
        }

        /// <summary>
        /// 以指定模式打开文件流（调用方负责释放流）。
        /// 若模式涉及写入且目录不存在，则自动创建目录。
        /// </summary>
        public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            if (mode != FileMode.Open && mode != FileMode.Truncate) // 需要创建目录的模式
            {
                EnsureDirectoryExists(path);
            }
            else
            {
                EnsureFileExists(path);
            }
            return new FileStream(path, mode, access, share);
        }

        #endregion

        #region 内部辅助

        /// <summary>
        /// 确保文件存在，否则抛出 FileNotFoundException。
        /// </summary>
        private static void EnsureFileExists(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"文件未找到: {path}", path);
        }

        /// <summary>
        /// 确保文件所在目录存在，否则自动创建。
        /// </summary>
        private static void EnsureDirectoryExists(string path)
        {
            string? dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        #endregion
    }
}
