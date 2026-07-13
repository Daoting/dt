<template>
  <q-layout view="LHh lpr lFf">
    <q-header elevated>
      <q-toolbar>
        <q-btn flat dense round icon="menu" aria-label="Menu" @click="toggleLeftDrawer" />

        <q-toolbar-title> 搬运工 </q-toolbar-title>

        <div>Quasar v{{ $q.version }}</div>
      </q-toolbar>
    </q-header>

    <q-drawer v-model="leftDrawerOpen" show-if-above bordered>
      <q-list bordered>
        <q-item v-for="route in menus" :key="route.path" clickable :to="route.path" active-class="q-item--active">
          

          <q-item-section>
            {{ route.meta?.title || route.name }}
          </q-item-section>
        </q-item>
      </q-list>
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, RouteRecordRaw } from 'vue-router'

const router = useRouter()
// 拿到所有路由配置
const routes = router.options.routes as RouteRecordRaw[]
const menus = routes[0].children

const leftDrawerOpen = ref(false)

function toggleLeftDrawer() {
  leftDrawerOpen.value = !leftDrawerOpen.value
}
</script>