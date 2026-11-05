import { defineStore } from 'pinia'

export const useUserStore = defineStore('user', {
  state: () => {
    return {
      token: '',
      name: '',
    }
  },

  getters: {
    isLogin: (state) => !!state.token
  },

  actions: {
    setToken(token: string) {
      this.token = token
    },
    login() {
      this.token = '13604332721'
    },
    logout() {
      this.token = ''
    }
  },

  persist: true
})