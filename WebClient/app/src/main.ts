import { createApp } from 'vue'
import './style.css'
import App from './App.vue'

import { Quasar, Notify, Dialog, Loading } from 'quasar'
import 'quasar/dist/quasar.css'

// 图标
import '@quasar/extras/material-icons/material-icons.css'

const app = createApp(App)
app.use(Quasar, {
  plugins: { Notify, Dialog, Loading }, // 👈 传对象，不是字符串数组
  config: {
    notify: { position: 'top' } // 可选：全局默认配置
  }
})
app.mount('#app')