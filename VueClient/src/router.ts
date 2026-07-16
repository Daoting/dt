import { createRouter, createWebHistory } from "vue-router";

const routes = [
  {
    path: "/",
    component: () => import("@/Home.vue"),
    children: [
      {
        path: "/",
        name: "序列化",
        component: () => import("@/test/TestSerialize.vue"),
        meta: { title: "序列化", icon: "home" },
      },
      {
        path: "TableBase",
        component: () => import("@/qtable/TableBase.vue"),
        meta: { title: "TableBase", icon: "home" },
      },
      {
        path: "first",
        component: () => import("@/pages/IndexPage.vue"),
        meta: { title: "first", icon: "home" },
      },
      {
        path: "second",
        component: () => import("@/pages/SecondPage.vue"),
        meta: { title: "second", icon: "home" },
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
  history: createWebHistory(), // history 模式（不带 #）
  routes,
});

export default router;
