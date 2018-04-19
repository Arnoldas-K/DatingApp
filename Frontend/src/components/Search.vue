<template>
  <div class='searchprofile'>
    <div v-if="info !== null" class="d-flex justify-content-center">
      <div class="container">
        <div class="search">
          <div class="search_image" v-bind:style="{backgroundImage: 'url('+ info.Photo + ')'}">
            <div class="search_image_buttons">
              <button type="button" @click="like" class="btn btn-primary search_button">
                <icon name="thumbs-up" />
              </button>
              <button type="button" @click="dislike" class="btn btn-secondary search_button">
                <icon name="thumbs-down" />
              </button>
            </div>
          </div>
          <b-card class="search_info">
            <div class="card-block">
              <h4 class="card-title">{{info.Name}}, {{info.Age}}</h4>
              <div class="card-text">
                <h5 class="search_info_item">
                  <icon name="home" />Location: {{info.Location}}</h5>
                <h5 class="search_info_item">
                  <icon name="briefcase" /> Occupation: {{info.Occupation}}</h5>
                <h5 class="search_info_item">
                  <icon name="info" /> About: {{info.About}}
                </h5>
                <h5 class="search_info_item">
                  <icon name="tasks" />Interests:
                </h5>
                <div v-for="interest in info.Interests">
                  <h6 class="search_info_item">{{interest}}</h6>
                </div>

              </div>
            </div>
          </b-card>
        </div>
      </div>
    </div>
    <div v-else>
      <b-dd-divider></b-dd-divider>
      <b-alert v-if="!show" show variant="danger">No suggestions found. Please try again later.</b-alert>
    </div>
    <b-modal class="loading-Modal" id="modal-center" v-model="show" centered cancel-disabled ok-disabled hide-header hide-footer>
        <icon name="refresh" scale=3 spin></icon>
      </b-modal>
  </div>
</template>

<style>

  .container {
    margin-top: 30px;
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

  .search {
    display: flex;
    flex-wrap: wrap;
    justify-content: space-evenly;
  }

  .search_image {
    position: relative;
    min-width: 200px;
    width: 500px;
    height: 500px;
    background-repeat: no-repeat;
    background-size: cover;
    background-position: center center;
  }

  .searchprofile .btn {
    width: 60px;
    height: 60px;
    text-align: center;
    padding: 6px 0;
    font-size: 12px;
    line-height: 1.428571429;
    border-radius: 30px;
    box-shadow: 1px -1px 9px 0px black;
  }

  .search_image_buttons {
    width: 100%;
    position: absolute;
    bottom: 10px;
    display: flex;
    justify-content: space-between;
  }

  .search_image button {
    margin: 0 20px;
  }

  .search_info {
    width: 500px;
    border: none;
    -webkit-box-shadow: 0px 0px 24px -6px rgba(0, 0, 0, 0.73);
    -moz-box-shadow: 0px 0px 24px -6px rgba(0, 0, 0, 0.73);
    box-shadow: 0px 0px 24px -6px rgba(0, 0, 0, 0.73);
    border-radius: 0;
  }

  .search_info_item {
    font-weight: 200;
    color: #2d3238;
  }

</style>

<script>
  /* eslint-disable */
  import router from "@/router";
  import store from "@/store";
  import axios from "axios";

  export const HTTP = axios.create({
    baseURL: `http://localhost:56761/api/data/`,
    headers: {
      Authorization: localStorage.getItem("token")
    }
  });
  export default {
    name: "search",
    data() {
      return {
        info: null,
        show: true,
        id: localStorage.getItem("id")
      };
    },
    created() {
      this.getSuggestion();
    },
    methods: {
      getSuggestion() {
        let that = this;
        HTTP.post("suggestion", {
          Id: that.id
        }).then(function (response) {
          that.show = false;
          that.info = response.data;
        });
      },
      like() {
        let that = this;
        this.show = true;
        HTTP.post("like", {
          Id: this.id,
          Partnerid: this.info.Id
        }).then(function (response) {
          if (response.statusText == "OK") {
            that.info = null;
            that.getSuggestion();
          }
        });
      },
      dislike() {
        let that = this;
        this.show = true;
        HTTP.post("dislike", {
          Id: this.id,
          Partnerid: this.info.Id
        }).then(function (response) {
          console.log(response);
          if (response) {
            that.getSuggestion();
          }
        });
      }
    }
  };

</script>
