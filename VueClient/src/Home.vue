<template>
  <q-layout view="lHh Lpr lff">
    <q-header elevated>
      <q-toolbar>
        <q-btn
          flat
          dense
          round
          icon="menu"
          aria-label="Menu"
          @click="toggleLeftDrawer"
        />
        <q-toolbar-title>搬运工</q-toolbar-title>
      </q-toolbar>
    </q-header>

    <q-drawer v-model="leftDrawerOpen" show-if-above bordered>
      <q-scroll-area class="fit">
        <q-list>
          <template v-for="(mi, index) in menus" :key="index">
            <q-item
              :to="{
                path: mi.path,
                query: mi.meta?.query,
              }"
              clickable
              v-ripple
            >
              <q-item-section avatar>
                <q-icon :name="mi.meta?.icon as string" />
              </q-item-section>
              <q-item-section>
                {{ mi.meta?.title as string }}
              </q-item-section>
            </q-item>
            <q-separator
              v-if="mi.meta?.separator === true && index < menus.length - 1"
              class="full-width"
            />
          </template>
        </q-list>
      </q-scroll-area>
    </q-drawer>

    <q-page-container>
      <!-- 确保内容可滚动，不可用h-100vh -->
      <div
        class="page-shell"
        style="height: calc(100vh - 60px); width: 100%; max-width: 100%; overflow: hidden;"
      >
        <router-view />
      </div>
    </q-page-container>
  </q-layout>
</template>

<script setup lang="ts">
import { useRouter, RouteRecordRaw } from "vue-router";

const router = useRouter();
const menus = router.options.routes[0].children as RouteRecordRaw[];
const leftDrawerOpen = ref(false);

function toggleLeftDrawer() {
  leftDrawerOpen.value = !leftDrawerOpen.value;
}

onMounted(() => {
  // 默认跳转到第一个菜单项
  if (menus.length > 0 && menus[0].path !== "") {
    router.push({
      path: menus[0].path,
    });
  }
});
</script>

<style scoped>
.page-shell {
  display: block;
  width: 100%;
  max-width: 100%;
  box-sizing: border-box;
}

.page-shell :deep(> *) {
  display: block;
  width: 100%;
  max-width: 100%;
}
</style>
