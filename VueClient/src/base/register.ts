import type { App } from 'vue'
import Fv from "./Form/Fv.vue";
import CText from "./Form/CText.vue";
import CList from "./Form/CList.vue";
import CDate from "./Form/CDate.vue";
import CTip from "./Form/CTip.vue";
import Routes from "./Routes.vue";
import Bar from "./Bar.vue";
import Lv from "./ListView/Lv.vue";

const ControlPlugin = {
  install(app: App) {
    // 全局注册组件
    app.component("Fv", Fv)
      .component("CText", CText)
      .component("CList", CList)
      .component("CDate", CDate)
      .component("CTip", CTip)
      .component("Routes", Routes)
      .component("Bar", Bar)
      .component("Lv", Lv);
  },
}

export default ControlPlugin;