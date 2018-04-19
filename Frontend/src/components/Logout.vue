<template>
<div class="logout">
</div>
</template>
<script>
    import router from '@/router'
    import store from '@/store'

    export default {
        name: 'logout',
        mounted () {
            localStorage.removeItem('token')
            localStorage.removeItem('id')
            store.commit('logout')
            router.push('Login')
            location.reload()
        }
    }
</script>
