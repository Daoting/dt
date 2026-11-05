import { createRouter, createWebHashHistory } from "vue-router";
import { useUserStore } from "./stores/user";

const routes = [
  {
    path: "/",
    component: () => import("@/Home.vue"),
    meta: { title: "收藏夹", icon: "home" },
    children: [
      {
        path: "TestSerialize",
        component: () => import("@/test/TestSerialize.vue"),
        meta: { title: "序列化", icon: "storage" },
      },
      {
        path: "FvDemo",
        component: () => import("@/test/FvDemo.vue"),
        meta: { title: "Fv 示例", icon: "style" },
      },
      {
        path: "LvDemo",
        component: () => import("@/test/LvDemo.vue"),
        meta: { title: "Lv 示例", icon: "list" },
      },
    ],
  },
  {
    path: "/login",
    name: "login",
    component: () => import("@/Login.vue"),
  },
  {
    path: "/:catchAll(.*)*",
    component: () => import("@/Error404.vue"),
  },
];

const router = createRouter({
  history: createWebHashHistory(), // Hash history 模式(带 #)，如http://xxx.com/a/b/#/about，支持相对路径，丢任意子目录都能跑
  routes,
});

router.beforeEach((to) => {
  if (to.path === "/login") {
    return;
  }

  const user = useUserStore();
  if (!user.isLogin) {
    return "/login";
  }
});

export default router;
