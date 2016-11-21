using System;
using System.Runtime.Caching;
using System.Threading;

namespace DBEN.ETM.Common.Cache
{
    /// <summary>
    /// 表示一个内存型缓存管理器。
    /// </summary>
    public static class CacheManager
    {
        private const string _KEY_SEPARATOR = "_";
        private const string _DEFAULT_DOMAIN = "DefaultDomain";

        private static MemoryCache Cache => MemoryCache.Default;
        private static readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        /// <summary>
        /// 设置一个永久的缓存对象，只有手动删除才失效。
        /// </summary>
        public static void SetPermanent(string key, object data, string domain = null)
        {
            var policy = new CacheItemPolicy();
            Set(key, data, policy, domain);
        }

        /// <summary>
        /// 设置一个 <paramref name="minutes"/> 分钟后失效的缓存。
        /// </summary>
        public static void SetAbsolute(string key, object data, double minutes, string domain = null)
        {
            var policy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(minutes) };
            Set(key, data, policy, domain);
        }

        /// <summary>
        /// 设置一个在 <paramref name="minutes"/> 分钟后失效的缓存，在失效后调用 <paramref name="callback"/> 进行通知。
        /// </summary>
        public static void SetAbsolute(string key, object data, double minutes, CacheEntryRemovedCallback callback, string domain = null)
        {
            if(data == null)
                throw new ArgumentNullException($"参数 {nameof(data)} 不能为空。");

            var policy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(minutes), RemovedCallback = callback };
            Set(key, data, policy, domain);
        }

        /// <summary>
        /// 设置一个在 <paramref name="minutes"/> 分钟内无访问失效的缓存。
        /// </summary>
        public static void SetSliding(object key, object data, double minutes, string domain = null)
        {
            if(data == null)
                throw new ArgumentNullException($"参数 {nameof(data)} 不能为空。");

            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(minutes) };
            Set(key, data, policy, domain);
        }

        /// <summary>
        /// 使用指定的缓存策略（ <see cref="CacheItemPolicy"/> ）设置一个缓存。
        /// </summary>
        /// <param name="key">指定域的缓存键。</param>
        /// <param name="data">缓存数据。</param>
        /// <param name="policy">缓存策略。</param>
        /// <param name="domain">缓存域，如果为 null，则返回默认域。</param>
        public static void Set(object key, object data, CacheItemPolicy policy, string domain = null)
        {
            if(key == null)
                throw new ArgumentNullException($"参数 {nameof(key)} 不能为空。");
            if(data == null)
                throw new ArgumentNullException($"参数 {nameof(data)} 不能为空。");
            if(policy == null)
                throw new ArgumentNullException($"参数 {nameof(policy)} 不能为空。");

            _locker.EnterWriteLock();
            try
            {
                Cache.Add(_CombinedKey(key, domain), data, policy);
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        /// <summary>
        /// 获取一个 <typeparamref name="T"/> 类型的缓存项。
        /// </summary>
        /// <param name="key">指定域的缓存键。</param>
        /// <param name="domain">缓存域，如果为 null，则返回默认域。</param>
        /// <exception cref="NullReferenceException"></exception>
        public static T Get<T>(object key, string domain = null)
        {
            return (T)Get(key, domain);
        }

        /// <summary>
        /// 获取缓存项。
        /// </summary>
        /// <param name="key">指定域的缓存键。</param>
        /// <param name="domain">缓存域，如果为 null，则返回默认域。</param>
        /// <exception cref="NullReferenceException"></exception>
        public static object Get(object key, string domain = null)
        {
            if(key == null)
                throw new ArgumentNullException($"参数 {nameof(key)} 不能为空。");

            return Cache.Get(_CombinedKey(key, domain));
        }

        /// <summary>
        /// 获取缓存项，如果不存在就调用 <paramref name="func"/> 委托获取数据并进行缓存，并返回数据。
        /// </summary>
        /// <typeparam name="T">缓存数据类型。</typeparam>
        /// <param name="key">指定域的缓存键。</param>
        /// <param name="policy">缓存策略。</param>
        /// <param name="func">获取缓存数据的委托。</param>
        /// <param name="domain">缓存域，如果为 null，则返回默认域。</param>
        /// <returns></returns>
        public static T Get<T>(object key, CacheItemPolicy policy, Func<T> func, string domain = null) where T : class
        {
            if(key == null)
                throw new ArgumentNullException($"参数 {nameof(key)} 不能为空。");
            if(func == null)
                throw new ArgumentNullException($"参数 {nameof(func)} 不能为空。");

            var item = Get<T>(key, domain);

            if(item == null)
            {
                item = func();
                Set(key, item, policy);
            }

            return item;
        }

        /// <summary>
        /// 检查指定的 <paramref name="key"/> 是否存在。
        /// </summary>
        /// <param name="key">指定域的缓存键。</param>
        /// <param name="domain">缓存域，如果为 null，则返回默认域。</param>
        public static bool Exists(object key, string domain = null)
        {
            if(key == null)
                throw new ArgumentNullException($"参数 {nameof(key)} 不能为空。");

            return Cache[_CombinedKey(key, domain)] != null;
        }

        /// <summary>
        /// 删除一个缓存。
        /// </summary>
        /// <param name="key">指定域的缓存键。</param>
        /// <param name="domain">缓存域，如果为 null，则返回默认域。</param>
        public static void Remove(object key, string domain = null)
        {
            if(key == null)
                throw new ArgumentNullException($"参数 {nameof(key)} 不能为空。");

            Cache.Remove(_CombinedKey(key, domain));
        }

        /// <summary>
        /// 解析缓存所属域。
        /// </summary>
        public static string ParseDomain(string combinedKey)
        {
            if(string.IsNullOrWhiteSpace(combinedKey))
                throw new ArgumentNullException($"参数 {nameof(combinedKey)} 不能为空。");

            return combinedKey.Substring(0, combinedKey.IndexOf(_KEY_SEPARATOR, StringComparison.Ordinal));
        }

        /// <summary>
        /// 解决缓存键。
        /// </summary>
        public static string ParseKey(string combinedKey)
        {
            if(string.IsNullOrWhiteSpace(combinedKey))
                throw new ArgumentNullException($"参数 {nameof(combinedKey)} 不能为空。");

            return combinedKey.Substring(combinedKey.IndexOf(_KEY_SEPARATOR, StringComparison.Ordinal) + _KEY_SEPARATOR.Length);
        }

        /// <summary>
        /// 使用给定的值组合出一个缓存键。
        /// </summary>
        /// <param name="key">指定域的缓存键。</param>
        /// <param name="domain">缓存域，如果为 null，则返回默认域。</param>
        private static string _CombinedKey(object key, string domain)
        {
            return $"{(string.IsNullOrEmpty(domain) ? _DEFAULT_DOMAIN : domain)}{_KEY_SEPARATOR}{key}";
        }
    }
}
