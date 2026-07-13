import { createApp } from "vue";
import "./style.css";
import App from "./App.vue";
import router from "./router.ts";

import { Quasar, Notify, Dialog, Loading } from "quasar";
import "quasar/dist/quasar.css";
import "@quasar/extras/material-icons/material-icons.css";
import '@quasar/extras/roboto-font/roboto-font.css'


createApp(App)
  .use(Quasar, {
    plugins: { Notify, Dialog, Loading },
    config: {
      notify: { position: "top" }, // 可选：全局默认配置
    },
  })
  .use(router)
  .mount("#app");
