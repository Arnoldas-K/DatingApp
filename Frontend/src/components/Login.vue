<template>
<div style="margin: 0 auto; display:table;  height: 100vh;" class="index">
  <div class="d-flex justify-content-center" style="display: table-cell!important;
  text-align: center;
  vertical-align: middle; height:300px;">
    <div class="card card-block d-table-cell align-middle">
      <!-- <b-card-header class="text-center">
        <h3>Dating application</h3>
      </b-card-header> -->
      <b-card-body>
        <b-form @submit.prevent="validate('loginform')" data-vv-scope="loginform">
          <p :class="{'control': true}">
            <b-input name="user" v-validate="'required|min:4|max:12'" :class="{'input': true, 'is-danger': errors.has('loginform.user') }"
              v-model="user" type="text" placeholder="Username" id="username" />
            <!-- <span style="max-width: 100%;" v-show="errors.has('loginform.user')" class="help is-danger text-danger">{{ errors.first('loginform.user') }}</span> -->
          </p>
          <p :class="{'control': true}">
            <b-input name="pass" v-model="pass" v-validate="'required|min:4|max:12'" :class="{'input': true, 'is-danger': errors.has('loginform.pass') }"
              type="password" placeholder="Password" id="password" />
            <!-- <span style="max-width: 300px;" v-show="errors.has('loginform.pass')" class="help is-danger text-danger">{{ errors.first('loginform.pass') }}</span> -->
          </p>
          <b-dd-divider></b-dd-divider>
          <b-button variant="success" class="btn btn-primary btn-sm btn-block" @click.prevent="validate('loginform')"> Login </b-button>
        </b-form>
        <small class="text-muted">or if you are a new user</small>
        <b-btn v-b-modal.modal-center variant="info" @click="show=true" class="btn btn-primary btn-sm btn-block"> Register </b-btn>

        <b-modal id="modal-center" v-model="show" centered title="Registration form" cancel-disabled ok-disabled hide-footer>
          <b-form @submit.prevent="validateForm('registerform')" data-vv-scope="registerform">
            <b-col>
              <label>First name</label>
              <p :class="{'control': true}">
                <b-input name="name" v-validate="'required|alpha|min:2|max:12'" :class="{'input': true, 'is-danger': errors.has('loginform.name') }"
                  v-model="form.Name" type="text" placeholder="First name" id="name" />
                <span v-show="errors.has('registerfrom.name')" class="help is-danger text-danger">{{ errors.first('registerfrom.name') }}</span>
              </p>
            </b-col>
            <b-col>
              <label>Last name</label>
              <p :class="{'control': true}">
                <b-input name="lastname" v-validate="'required|alpha|min:2|max:12'" :class="{'input': true, 'is-danger': errors.has('loginform.lastname') }"
                  v-model="form.LastName" type="text" placeholder="Your last name" id="lastname" />
                <span v-show="errors.has('registerform.lastname')" class="help is-danger text-danger">{{ errors.first('registerform.lastname') }}</span>
              </p>
            </b-col>
            <b-col>
              <label> Username </label>
              <p :class="{'control': true}">
                <b-input name="username" v-validate="'required|min:4|max:12'" v-model="form.Login" :class="{'input': true, 'is-danger': errors.has('registerform.username') }"
                  type="text" placeholder="Username"></b-input>
                <span v-show="errors.has('registerform.username')" class="help is-danger text-danger">{{ errors.first('registerform.username') }}</span>
              </p>
            </b-col>
            <b-col>
              <label> Password </label>
              <p :class="{'control': true}">
                <b-input name="password" v-validate="'required|alpha_dash|min:4|max:12'" :class="{'input': true, 'is-danger': errors.has('registerform.password') }"
                  v-model="form.Password" type="password" placeholder="Password"></b-input>
                <span v-show="errors.has('registerform.password')" class="help is-danger text-danger">{{ errors.first('registerform.password') }}</span>
              </p>
            </b-col>
            <b-col>
              <label> Age </label>
              <p :class="{'control': true}">
                <b-input name="age" v-validate="'required|min_value:18|max_value:99|numeric'" :class="{'input': true, 'is-danger': errors.has('registerform.age') }"
                  v-model="form.Age" type="number" placeholder="Your age"></b-input>
                <span v-show="errors.has('registerform.age')" class="help is-danger text-danger">{{ errors.first('registerform.age') }}</span>
              </p>
            </b-col>
            <b-col>
              <label> Gender </label>
              <p :class="{'control': true}">
                <b-form-select name="gender" v-validate="'required|in:Male,Female'" :class="{'input': true, 'is-danger': errors.has('registerform.gender') }"
                  v-model="form.Gender" type="text" :options="genders"></b-form-select>
                <span v-show="errors.has('registerform.gender')" class="help is-danger text-danger">{{ errors.first('registerform.gender') }}</span>
              </p>
            </b-col>
            <b-col>
              <label> Location </label>
              <p :class="{'control': true}">
                <b-input name="location" v-validate="'required'" :class="{'input': true, 'is-danger': errors.has('registerform.location') }"
                  v-model="form.Location" type="text" placeholder="Location"></b-input>
                <span v-show="errors.has('registerform.location')" class="help is-danger text-danger">{{ errors.first('registerform.location') }}</span>
              </p>
            </b-col>
            <b-col>
              <label> Occupation </label>
              <p :class="{'control': true}">
                <b-input name="occupation" v-validate="'required'" :class="{'input': true, 'is-danger': errors.has('registerform.occupation') }"
                  v-model="form.Occupation" type="text" placeholder="Occupation"></b-input>
                <span v-show="errors.has('registerform.occupation')" class="help is-danger text-danger">{{ errors.first('registerform.occupation') }}</span>
              </p>
            </b-col>
            <b-col>
              <label> About </label>
              <p :class="{'control': true}">
                <b-form-textarea name="about" v-validate="'required'" :class="{'input': true, 'is-danger': errors.has('registerform.about') }"
                  v-model="form.About" placeholder="A few sentences about yourself" no-resize type="text"></b-form-textarea>
                <span v-show="errors.has('registerform.about')" class="help is-danger text-danger">{{ errors.first('registerform.about') }}</span>
              </p>
            </b-col>
            <b-dd-divider></b-dd-divider>
            <small>Search preferences</small>
            <b-col>
              <label> Seeking for </label>
              <p :class="{'control': true}">
                <b-form-select name="lookfor" v-validate="'required|in:Male,Female'" :class="{'input': true, 'is-danger': errors.has('registerform.lookfor') }"
                  v-model="form.LookFor" type="text" :options="genders"></b-form-select>
                <span v-show="errors.has('registerform.lookfor')" class="help is-danger text-danger">{{ errors.first('registerform.lookfor') }}</span>
              </p>
            </b-col>
            <b-col>
              <label> Interests </label>
              <vue-tags-input v-model="tag" :tags="form.Interests" :add-only-from-autocomplete="true" :autocomplete-items="filteredItems"
                @tags-changed="newTags => form.Interests = newTags">
              </vue-tags-input>
            </b-col>
            <b-dd-divider></b-dd-divider>
            <b-btn size="md" @click.prevent="validate('registerform')" class="float-right" variant="success">
              Register
            </b-btn>
            <div slot="modal-footer">
              <b-btn size="md" @click="show=false" class="float-right" variant="warning">
                Cancel
              </b-btn>
            </div>
            <div @click.prevent="validate('registerform')" slot="modal-ok"></div>
          </b-form>
        </b-modal>
      </b-card-body>
    </div>
  </div>
  </div>
</template>

<script>
  /* eslint-disable */
  import axios from 'axios'
  import router from '@/router'
  import store from '@/store'
  import VueTagsInput from '@johmun/vue-tags-input'
  import interestsList from '../../static/interests.json'


  export const axiosauth = axios.create({
    baseURL: `http://localhost:56761/api/data/`,
    timeout: 1000,
    headers: {
      'Authorization': localStorage.getItem('token')
    }
  })

  export const axiosno = axios.create({
    baseURL: `http://localhost:56761/api/data/`
  })

  export default {
    name: 'login',
    data() {
      return {
        user: '',
        pass: '',
        // registration
        show: false,
        form: {
          Name: '',
          LastName: '',
          Login: '',
          Password: '',
          Age: '',
          Gender: null,
          Location: '',
          Occupation: '',
          About: '',
          Interests: [],
          LookFor: null,
        },
        genders: [{
            value: null,
            text: 'Select your gender'
          },
          {
            value: 'Male',
            text: 'Male'
          },
          {
            value: 'Female',
            text: 'Female'
          }
        ],
        tag: '',
        autocompleteItems: interestsList
      }
    },
    beforeCreate() {
      if (store.state.isLogged) {
        router.push('Profile')
      }
    },
    created() {
    },
    computed: {
      filteredItems() {
        return this.autocompleteItems.filter(i => new RegExp(this.tag, 'i').test(i.text));
      },
    },
    methods: {
      update(newTags) {
        this.autocompleteItems = [];
        this.tags = newTags;
      },
      mapInterests(tags) {
        return tags.map(tag => tag.text);
      },
      login() {
        let that = this
        var credentials = "username=" + this.user + "&password=" + this.pass + "&grant_type=password"
        axios.post('http://localhost:56761/token', credentials, { timeout: 3000,
            'headers': {
              'Content-Type': 'application/x-www-form-urlencoded'
            }
          })
          .then(function (response) {
            var token = response.data['token_type'] + ' ' + response.data['access_token']
            localStorage.setItem('token', token)
            store.commit('login')
            axios.get('http://localhost:56761/api/data/authenticate', {
              headers: {
                'Authorization': token
              }
            }).then(function (response) {
              localStorage.setItem('id', response.data.m_value)
              alert("Successfully logged in")
              router.push('Profile')
              location.reload()
            })
          })
          .catch(function (response) {
            console.log(response)
            alert("Incorrect username or password")
          })
      },
      register() {
        const data = { ...this.form
        };
        data.Interests = this.mapInterests(data.Interests);
        axiosno.post('register', data).then(function (response) {
          if (response.data == true) {
            alert("Successfully registered")
            location.reload()
          }
        })
      },
      validate(form) {
        console.log("OK")
        var action = form
        this.$validator.validateAll(form).then((result) => {
          if (result) {
            if (action == 'loginform') {
              this.login()
            } else if (action == 'registerform') {
              this.register()
            }
          }
        });
      }
    }
  }

</script>

<style>
#app{
background: #de6161;  /* fallback for old browsers */
background: -webkit-linear-gradient(to right, #2657eb, #de6161);  /* Chrome 10-25, Safari 5.1-6 */
background: linear-gradient(to right, #2657eb, #de6161); /* W3C, IE 10+/ Edge, Firefox 16+, Chrome 26+, Opera 12+, Safari 7+ */
}

.modal-body{
  text-align: left;
}

.card{
  background-color: rgba(255,255,255,0.3);
}

.card{
  border-radius: 0;
}

.btn{
  border-radius: 0;
}

.form-control{
  border-radius: 0;
}

.form-control.input{
  background-color: rgba(239, 239, 239, 0.6);
}
</style>
