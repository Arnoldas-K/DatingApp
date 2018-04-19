<template>
  <div id="app">
    <b-container>
      <t-nav></t-nav>
      <div class="content">
      <router-view :key="$route.fullPath"></router-view>
      </div>
    </b-container>
  </div>
</template>

<script>
  export default {
    name: 'App'
  }

</script>

<style>

</style>
