import { createApp } from "vue";
import { createPinia } from "pinia";
import "./style.css";
import App from "./App.vue";
import router from "./router.ts";
import { piniaPersistPlugin } from "./core/persist";

import { Quasar, Notify, Dialog, Loading } from "quasar";
import langZhCN from "quasar/lang/zh-CN";
import "quasar/dist/quasar.css";
import "@quasar/extras/material-icons/material-icons.css";
import "@quasar/extras/roboto-font/roboto-font.css";

createApp(App)
  .use(createPinia().use(piniaPersistPlugin))
  .use(Quasar, {
    plugins: { Notify, Dialog, Loading },
    lang: langZhCN,
  })
  .use(router)
  .mount("#app");
