<template>
  <div class="chat">
    <div v-if="chatId === null" class="ui huge animated divided selection list">
      <div class="item" @click="selectChat(conversation.Id, i)" v-for="(conversation, i) in conversations" v-bind:id="conversation.Id"
        :key="i">
        <img class="ui avatar image" :src="conversation.Photos[partners[i]]">
        <div class="content">
          <div class="header">{{conversation.UserNames[partners[i]]}}</div>
          {{conversation.Messages[0].From}}: {{conversation.Messages[0].Text.substring(0,10).trim()}}..
        </div>
      </div>
    </div>
    <div id="scroll" v-if="conversations !== null">
      <div class="conversations" v-if="chatId === null">
      </div>
      <!-- style="height: 90vh; overflow: auto;" -->
      <div ref="chatwindow" class="chatwindow" id="chatWindow" v-if="chatId !== null">
        <div v-if="!show" class="ui comments">
          <!-- <h3 class="ui header">Conversation with {{conversations[selectedConversation]}} </h3> -->
          <div class="comment" v-for="(message, i) in messages" :key="i">
            <a class="avatar">
              <img v-if="message.From == username" :src="conversations[selectedConversation].Photos[userid]">
              <img v-else :src="conversations[selectedConversation].Photos[(userid+1)%2]">
            </a>
            <div class="content">
              <a class="author">{{message.From}}</a>
              <div class="metadata">
                <span class="date">{{message.Time}}</span>
              </div>
              <div class="text">
                {{message.Text}}
              </div>
            </div>
          </div>
          <b-form-input type="text" v-on:keyup.enter.native="sendMessage" v-model="messageToAdd" id="messageArea" placeholder="Your message">
          </b-form-input>
        </div>
      </div>
    </div>
    <div v-else>
      <b-dd-divider></b-dd-divider>
      <b-alert v-if="!show" show variant="danger">No conversations were found. Use "Search" function to find some friends.</b-alert>
    </div>
    <b-modal class="loading-Modal" id="modal-center" v-model="show" centered cancel-disabled ok-disabled hide-header hide-footer>
      <icon name="refresh" scale=3 spin></icon>
    </b-modal>
  </div>
</template>
<style>
  .chat {
    margin-top: 60px;
  }

  .ui.comments .comment .avatar {
    height: 40px;
  }

  .ui.comments .comment img.avatar,
  .ui.comments .comment .avatar img {
    border-radius: 50%;
  }

  .user_image {
    width: 80px;
    height: 80px;
    border-radius: 50%;
    margin-bottom: 10px;
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

</style>

<script>
  import 'ms-signalr-client-jquery-3'
  import router from '@/router'
  import store from '@/store'
  import axios from 'axios'
  import jquery from 'jquery'

  export const HTTP = axios.create({
    baseURL: `http://localhost:56761/api/data/`,
    headers: {
      'Authorization': localStorage.getItem('token')
    }
  })

  export default {
    name: 'chat',
    data() {
      return {
        conversations: null,
        selectedConversation: null,
        messageToAdd: '',
        messages: [],
        partners: [], // id which represent partner for each chat
        connection: null,
        proxy: null,
        chatId: null,
        id: localStorage.getItem('id'),
        show: true,
        userid: null,
        username: null
      }
    },
    methods: {
      connectchat: function () {
        this.proxy.invoke('Connect', this.id, this.chatId) // userid,chatid
          .done(() => {
            console.log('Connect to chat -' + this.chatId)
          })
          .fail(() => {
            location.reload()
            console.log('something went wrong')
          })
      },
      sendMessage: function () {
        let that = this
        if (this.messageToAdd === '') return true;
        this.proxy.invoke('Send', this.messageToAdd, this.chatId, this.conversations[this.selectedConversation].UserNames[
            this.userid]) // this.user.name
          .done(() => {
            that.animateScroll();
            that.messageToAdd = ''
          })
          .fail(error => {
            console.log(error)
          })
      },
      retrieveMessages: function () {
        let that = this
        this.animateScroll();
        HTTP.post('conversation', {
            id: this.chatId
          })
          .then(function (response) {
            that.show = false;
            response.data.Messages.forEach(message => {
              that.messages.push({
                From: message.From,
                Text: message.Text,
                Time: message.Time
              })
            });
            that.animateScroll()
          });
      },
      selectChat: function (id, i) {
        this.show = true;
        this.selectedConversation = i
        this.chatId = id
        this.userid = this.conversations[i].Users[0] === this.id ? 0 : 1;
        this.username = this.conversations[this.selectedConversation].UserNames[this.userid]
        this.connectchat()
        this.retrieveMessages()
      },
      animateScroll: function () {
        var $ = jquery;
        $("html, body").animate({
          scrollTop: $(document).height()
        }, 1000);
      }
    },
    updated() {},
    beforeMount() {
      let that = this
      this.connection = $.hubConnection('http://localhost:56761/signalr')
      this.proxy = this.connection.createHubProxy('ChatHub')
      this.proxy.on('sendMessage', (from, message) => {
        var now = new Date(Date.now());
        var formatted = now.getHours() + ":" + now.getMinutes() + ":" + now.getSeconds();
        that.messages.push({
          From: from,
          Text: message,
          Time: formatted
        })
      })
      this.connection
        .start({
          jsonp: true
        })
        .done(() => {
          console.log('Now connected, connection ID=' + that.connection.id)
        })
        .fail(() => {
          location.reload()
          console.log('Could not connect')
        })
    },
    beforeCreate() {
      if (localStorage.getItem('id') === undefined || localStorage.getItem('id') === null) {
        HTTP.get('authenticate').then(function (response) {
          console.log(response)
          localStorage.setItem('id', response.data.m_value)
        });
      }
    },
    created() {
      console.log(this.$parent.$refs)
      let that = this
      HTTP.post('conversations', {
          'Id': this.id
        })
        .then(function (response) {
          that.show = false;
          that.conversations = response.data
          if(response.data !== null){
          that.conversations.forEach(conv => {
            var id = conv.Users[0] === that.id ? 1 : 0;
            that.partners.push(id);
          });
          }
        })
    }
  }

</script>
