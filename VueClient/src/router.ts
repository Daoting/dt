import {
    createRouter,
    createWebHistory
  } from 'vue-router'
  
  const routes = [
    {
      path: '/',
      component: () => import('@/home/MainLayout.vue'),
      children: [
        {
          path: '/',
          name: '序列化',
          component: () => import('@/test/TestSerialize.vue'),
          meta: { title: '序列化', icon: 'home' },
        },
        {
          path: 'first',
          component: () => import('@/pages/IndexPage.vue'),
          meta: { title: 'first', icon: 'home' },
        },
        {
          path: 'second',
          component: () => import('@/pages/SecondPage.vue'),
          meta: { title: 'second', icon: 'home' },
        },
      ]
    },
    
    // Always leave this as last one,
    // but you can also remove it
    {
      path: '/:catchAll(.*)*',
      component: () => import('@/pages/ErrorNotFound.vue')
    }
  ]
  
  const router = createRouter({
    history: createWebHistory(), // history 模式（不带 #）
    routes
  })
  
  export default router