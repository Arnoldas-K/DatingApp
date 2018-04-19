<template>
  <b-navbar v-if=loggedIn class="fixed-top" toggleable type="dark" variant="dark">
    <b-navbar-brand>Dating App</b-navbar-brand>
    <b-navbar-toggle target="nav_text_collapse"></b-navbar-toggle>
    <b-collapse is-nav id="nav_text_collapse">
      <b-navbar-nav>
        <b-nav-item ref="search" @click="goto('Search')">Search</b-nav-item>
        <b-nav-item ref="chat" @click="goto('Chat')">Chat</b-nav-item>
        <b-nav-item ref="profile" @click="goto('Profile')">Profile</b-nav-item>
        <b-nav-item right @click="goto('Logout')">Log Out</b-nav-item>
      </b-navbar-nav>
    </b-collapse>
  </b-navbar>
</template>

<script>
  import router from '@/router'
  import store from '@/store'

  export default {
    name: 't-nav',
    data() {
      return {
        loggedIn: store.state.isLogged
      }
    },
    created(){
    },
    methods: {
      goto: function (route) {
        if(route == 'Chat' && this.$route.path == '/Chat'){
          location.reload();
        }
        $('#nav_text_collapse > ul > li').find('a').removeClass('active')
        $(event.target).addClass('active')
        this.loggedIn = store.state.isLogged
        if (!store.state.isLogged) { // CHANGE TO NEGATIVE
          router.push('Login')
        } else {
          router.push(route)
        }
      }
    }
  }

</script>
