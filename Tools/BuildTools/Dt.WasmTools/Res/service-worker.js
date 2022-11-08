console.debug("[ServiceWorker] 初始化");

self.addEventListener('install', event => {
    console.debug('[ServiceWorker] install事件');
    var promise = caches.open('$(CACHE_KEY)').then(cache => {
        console.debug('[ServiceWorker] 缓存文件');
        return cache.addAll($(CACHE_FILES));
    });
    event.waitUntil(promise);
    // 立即触发activate事件
    return self.skipWaiting();
});

self.addEventListener('activate', event => {
    // 不可删除旧缓存，因以域名管理缓存，二级虚拟目录时造成互相删除！！！
    // skipWaiting -> 触发activate事件 -> claim -> 新Service Worker接管 -> 有旧缓存? -> 删除旧缓存 -> 刷新页面
    event.waitUntil(self.clients.claim().then(() => {
        console.debug('[ServiceWorker] 刷新页面，更新立即生效！');
        return self.clients.matchAll().then(clients => {
            clients.forEach(client => client.navigate(client.url));
        });
    }));
});

self.addEventListener('fetch', event => {
    event.respondWith(
        // 旧缓存和当前缓存的根url的键值是相同的，避免错误！
        caches.open('$(CACHE_KEY)').then(cache => {
            return cache.match(event.request.url.toLowerCase()).then(response => {
                // 本地缓存优先，查不到再请求服务端
                return response || fetch(event.request);
            });
        })
    );
});