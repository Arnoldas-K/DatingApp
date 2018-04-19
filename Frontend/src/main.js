// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import vuex from 'vuex'
import VeeValidate from 'vee-validate'
import router from './router'
import store from './store'
import NavBar from './Components/Navigation'

// Boostrap
import BootstrapVue from 'bootstrap-vue'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import './assets/comment.css';
import './assets/image.css';
import './assets/item.css';
import './assets/list.css';

import { Nav, Form, Card, Image, Button } from 'bootstrap-vue/es/components';

// Icons
import 'vue-awesome/icons'
import Icon from 'vue-awesome/components/Icon'
import VueTagsInput from '@johmun/vue-tags-input'

Vue.use(BootstrapVue)
Vue.use(VeeValidate)
Vue.use(vuex)

Vue.use(Image);
Vue.use(Card);
Vue.use(Form);
Vue.use(Button);
Vue.use(Nav);

Vue.component('icon', Icon)
Vue.component(VueTagsInput.name, VueTagsInput)
Vue.component(NavBar.name, NavBar)
Vue.config.productionTip = false


/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  store,
  components: { App },
  template: '<App/>'
})
