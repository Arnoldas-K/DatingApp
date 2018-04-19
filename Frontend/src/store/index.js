import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

const state = { isLogged: !!localStorage.getItem('token') }
/* eslint-disable */
const mutations = {
  login (state) {
    state.isLogged = true
  },
  logout (state) {
    state.isLogged = false
  }
}

export default new Vuex.Store({
  strict: process.env.NODE_ENV !== 'production',
  state,
  mutations
})
