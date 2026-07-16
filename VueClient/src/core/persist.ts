import type { PiniaPluginContext } from 'pinia'

/**
 * 自定义 Pinia 持久化插件
 * 用法和 pinia-plugin-persistedstate 完全一致
 * store 中配置 { persist: true } 即可自动持久化
 */
export function piniaPersistPlugin(context: PiniaPluginContext) {
  const { store } = context

  // 如果 store 配置了 persist: true 才开启持久化
  if (store.$options.persist !== true) return

  const storageKey = `pinia-store-${store.$id}`

  // 1. 初始化时从 localStorage 恢复
  const saved = localStorage.getItem(storageKey)
  if (saved) {
    try {
      const data = JSON.parse(saved)
      store.$patch(data)
    } catch (e) {
      console.warn(`持久化恢复失败 [${store.$id}]`, e)
    }
  }

  // 2. 状态变化时自动保存
  store.$subscribe((_, state) => {
    localStorage.setItem(storageKey, JSON.stringify(state))
  })
}