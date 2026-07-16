<template>
  <Suspense>
    <router-view />
    <template #fallback>
      <div class="loading-page">
        加载中...
      </div>
    </template>
  </Suspense>
</template>