import Vue from 'vue'
import Router from 'vue-router'
import Login from '@/components/Login'
import Logout from '@/components/Logout'
import Profile from '@/components/Profile'
import Search from '@/components/Search'
import Chat from '@/components/Chat'

Vue.use(Router)

export const routes = [
  {
    path: '/',
    redirect: '/login'
  },
  {
    path: '/login',
    name: 'Login',
    component: Login
  },
  {
    path: '/logout',
    name: 'Logout',
    component: Logout
  },
  {
    path: '/profile',
    name: 'Profile',
    component: Profile
  },
  {
    path: '/search',
    name: 'Search',
    component: Search
  },
  {
    path: '/chat',
    name: 'Chat',
    component: Chat
  },
  {
    path: '*',
    redirect: '/login'
  }
]

export default new Router({
  mode: 'history',
  base: '/',
  routes: routes
})
