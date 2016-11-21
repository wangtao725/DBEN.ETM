/*******************************************************
 * 
 * 作者：宋军瑞
 * 创建日期：20160930
 * 说明：内容说明。
 * 运行环境：.NET 4.5
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 宋军瑞 20160930 11:31
 * 
*******************************************************/

using System;
using System.IO;
using System.Text;

namespace DBEN.ETM.Common.IO
{
    /// <summary>
    /// 表示一个文件操作类。
    /// </summary>
    /// <code>
    /// using (var fileHelper = new FileHelper(fileFullName))
    /// {
    ///     fileHelper.Write(writer => {
    ///         writer.WriteLine("文本内容");
    ///     });
    /// }
    /// </code>
    public class FileHelper : IDisposable
    {
        private readonly string _fileName;
        private bool _isDisposed;
        private FileStream _stream;
        private StreamWriter _writer;

        /// <summary>
        /// 初始化 <see cref="FileHelper"/> 类的新实例。
        /// </summary>
        /// <param name="fileName"></param>
        public FileHelper(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            this._fileName = fileName;
        }

        /// <summary>
        /// 使用 <see cref="FileStream"/> 向文件写入数据。
        /// </summary>
        /// <param name="handler">执行写入操作的委托。</param>
        /// <param name="encoding">文件编码格式。</param>
        public void Write(Action<StreamWriter> handler, Encoding encoding = null)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (File.Exists(this._fileName))
                File.Delete(this._fileName);

            this._stream = File.OpenWrite(this._fileName);
            this._writer = new StreamWriter(this._stream, encoding ?? Encoding.GetEncoding("GB2312"));

            handler(this._writer);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this._writer?.Dispose();
                    this._stream?.Dispose();
                }
            }

            this._isDisposed = true;
        }
    }
}
