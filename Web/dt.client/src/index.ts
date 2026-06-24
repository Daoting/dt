import MyButton from './components/MyButton.vue'

export { MyButton }

export default {
  install(app) {
    app.component('MyButton', MyButton)
  }
}