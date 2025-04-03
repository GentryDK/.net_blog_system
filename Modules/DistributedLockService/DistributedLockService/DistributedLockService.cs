using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLock
{
    public class DistributedLockService
    {
        private readonly IDistributedCache _cache;
        private readonly string _lockKey = "SensitiveWordsLock"; // 锁的key

        public DistributedLockService(IDistributedCache cache)
        {
            _cache = cache;
        }

        // 尝试获取锁
        public async Task<bool> TryAcquireLockAsync()
        {
            var lockValue = Guid.NewGuid().ToString(); // 使用一个唯一的值作为锁
            var lockTimeout = TimeSpan.FromSeconds(10); // 设置锁的超时时间，避免死锁

            // 尝试设置锁，若锁已存在，则返回 false
            try
            {
                await _cache.SetStringAsync(
                    _lockKey,
                    lockValue,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = lockTimeout // 设置过期时间
                    });
            }
            catch
            {
                // 如果发生异常，表示设置锁失败
                return false;
            }

            // 如果没有异常，表示锁设置成功
            return true;
        }

        // 释放锁
        public async Task ReleaseLockAsync(string lockValue)
        {
            var currentLockValue = await _cache.GetStringAsync(_lockKey);

            // 确保只有持有锁的线程才可以释放锁
            if (currentLockValue == lockValue)
            {
                await _cache.RemoveAsync(_lockKey); // 释放锁
            }
        }
    }
}
