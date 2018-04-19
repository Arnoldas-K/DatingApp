<template>
  <div class='userprofile'>
    <!-- <t-nav></t-nav> -->
    <!-- <div class="d-flex justify-content-center"> -->
    <div class="container">
      <div class="profile">
        <div class="profile_image" v-bind:style="{backgroundImage: 'url('+ info.Photo + ')'}">
          <label for="upload">
          <icon name="image" />
          <input type="file" id="upload" :v-model=file style="display:none">
          </label>
          <div class="profile_image_buttons">
            <button type="button" class="profile_button btn btn-primary like_button">Settings</button>
            <button type="button" @click="logoff" class="profile_button btn btn-secondary dislike_button">Log out</button>
          </div>
        </div>
        <b-card class="profile_info">
          <div class="card-block">
            <h4 class="card-title">{{info.Name}}, {{info.Age}}</h4>
            <div class="card-text">
              <h5 class="profile_info_item">
                <icon name="home" /> Location: {{info.Location}}</h5>
              <h5 class="profile_info_item">Occupation: {{info.Occupation}}</h5>

              <h5 class="profile_info_item">
                <icon name="info" /> About: {{info.About}}
              </h5>
              <h5 class="profile_info_item">
                <icon name="tasks" />Interests:
              </h5>
              <div v-for="interest in info.Interests">
                <h6 class="profile_info_item">{{interest}}</h6>
              </div>
            </div>
          </div>
          <b-form-file v-model="file" :state="Boolean(file)" placeholder="Upload a new photo"></b-form-file>
          <b-button class="profile_button" @click="upload()">Change photo</b-button>
        </b-card>
      </div>
      <b-modal class="loading-Modal" id="modal-center" v-model="show" centered cancel-disabled ok-disabled hide-header hide-footer>
        <icon name="refresh" scale=3 spin></icon>
      </b-modal>
    </div>
  </div>
</template>
<style>
  .userprofile {
    margin-top: 60px;
  }

  .profile_button .btn {
    text-align: center;
    padding: 6px 0;
    font-size: 12px;
    line-height: 1.428571429;
    box-shadow: 1px -1px 9px 0px black;
  }

  .modal-content {
    background-color: transparent;
    border-color: transparent;
  }

  .modal-body {
    color: white;
  }

  .loading-Modal {
    text-align: center;
  }

  .profile {
    display: flex;
    flex-wrap: wrap;
    justify-content: space-evenly;
  }

  .profile_image {
    position: relative;
    min-width: 200px;
    width: 500px;
    height: 500px;
    background-repeat: no-repeat;
    background-size: cover;
    background-position: center center;
  }

  .profile_image_buttons {
    width: 100%;
    position: absolute;
    bottom: 10px;
    display: flex;
    justify-content: space-between;
  }

  .profile_image button {
    margin: 0 20px;
    width: 100px;
  }

  .profile_info {
    width: 500px;
    border: none;
    -webkit-box-shadow: 0px 0px 24px -6px rgba(0, 0, 0, 0.73);
    -moz-box-shadow: 0px 0px 24px -6px rgba(0, 0, 0, 0.73);
    box-shadow: 0px 0px 24px -6px rgba(0, 0, 0, 0.73);
    border-radius: 0;
  }

  .profile_info_item {
    font-weight: 200;
    color: #2d3238;
  }

</style>

<script>
  /* eslint-disable */
  import router from '@/router'
  import store from '@/store'
  import axios from 'axios'

  export const HTTP = axios.create({
    baseURL: `http://localhost:56761/api/data/`,
    headers: {
      'Authorization': localStorage.getItem('token')
    }
  })

  export default {
    name: 'Profile',
    beforeCreate() {
      if (!store.state.isLogged) {
        router.push('Login')
      } else if (localStorage.getItem('id') === undefined || localStorage.getItem('id') === null) {
        HTTP.get('authenticate').then(function (response) {
          console.log(response)
          localStorage.setItem('id', response.data.m_value)
        }).catch(function (response) {
          router.push('Logout')
          location.reload()
        });
      }
    },
    methods: {
      logoff: function () {
        router.push('Logout')
        // localStorage.removeItem('token')
        // store.commit('logout')
        // router.push('Login')
      },
      upload: function () {
        console.log(this.file)
        const config = {
          headers: {
            'Content-Type': 'multipart/form-data'
          }
        };
        let fd = new FormData();
        fd.append('file', this.file)
        HTTP.post('http://localhost:56761/api/data/upload', fd)
          .then(function (response) {
            console.log(response)
            location.reload()
          })
      }
    },
    data() {
      return {
        info: {},
        show: true,
        file: null,
        id: localStorage.getItem('id')
      }
    },
    created() {
      let that = this
      // GET INFO FOR USER PROFILE
      HTTP.post('user', {
          'Id': this.id
        })
        .then(function (response) {
          that.show = false;
          that.info = response.data;
        })
    }
  }

</script>
