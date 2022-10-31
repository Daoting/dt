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
    console.debug('[ServiceWorker] activate事件');

    // 更新立即生效：
    // skipWaiting -> 触发activate事件 -> claim -> 新Service Worker接管 -> 有旧缓存? -> 删除旧缓存 -> 刷新页面
    event.waitUntil(self.clients.claim().then(() => {
        return caches.keys().then(keys => {
            var isUpdate = false;
            return Promise.all(keys.map(key => {
                if (key !== '$(CACHE_KEY)') {
                    isUpdate = true;
                    return caches.delete(key);
                }
            })).then(() => {
                if (isUpdate) {
                    console.debug('[ServiceWorker] 删除旧版本缓存，刷新页面');
                    return self.clients.matchAll().then(clients => {
                        clients.forEach(client => client.navigate(client.url));
                    });
                }
            });
        })
    }));
});

self.addEventListener('fetch', event => {
    event.respondWith(
        caches.match(event.request).then(response => {
            // 本地缓存优先，查不到再请求服务端
            return response || fetch(event.request);
        })
    );
});