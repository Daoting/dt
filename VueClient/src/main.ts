import { createApp } from "vue";
import { createPinia } from "pinia";
import { z } from "zod";
import { zhCN } from "zod/locales";
import App from "./App.vue";
import "./style.css";
import router from "./router.ts";
import { piniaPersistPlugin } from "./core/persist";
import ControlPlugin from "./base/register.ts";
import { loadAppConfig } from "./core/config.ts";

import { Quasar, Notify, Dialog, Loading } from "quasar";
import langZhCN from "quasar/lang/zh-CN";
import "quasar/dist/quasar.css";
import "@quasar/extras/material-icons/material-icons.css";
import "@quasar/extras/roboto-font/roboto-font.css";

try {
  z.config(zhCN());
  await loadAppConfig();

  createApp(App)
    .use(createPinia().use(piniaPersistPlugin))
    .use(Quasar, {
      plugins: { Notify, Dialog, Loading },
      lang: langZhCN,
    })
    .use(router)
    .use(ControlPlugin)
    .mount("#app");
} catch (err) {
  console.error("启动失败：", err);
  // 【失败：在#app容器渲染错误页面】
  const container = document.getElementById("app");
  if (container) {
    container.innerHTML = `
      <div style="padding:30px;font-family:system-ui;">
        <h2>启动失败</h2>
        <p><h2 style="color:#c0392b;">${err}</h2></p>
        <button onclick="location.reload()" style="padding:8px 16px;cursor:pointer;">刷新重试</button>
      </div>
    `;
  }
}
