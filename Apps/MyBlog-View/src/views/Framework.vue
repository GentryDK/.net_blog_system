<template>
    <div>
        <div class = "top">
            <div class="top-nav">
                <router-link to="/" class="logo">DK-BLOG</router-link>
                <router-link v-for="item in menus" 
                :class="['nav-item',item.path==activePath?'active':'']" 
                :to="item.path">{{ item.name }}</router-link>

                <div class="nav-item2">
                  <a :href="LoginUrl" class="nav-item2-a"  v-if="!userInfo.userName">LOGIN</a>
                    <a :href="controller" class="nav-item2-a" v-else>CONTROLLER</a>
                    <img :src="`${local}${userInfo.headUrl}`" v-if="userInfo.headUrl">
                     <img v-else src="../../public/Image/avatarIcon.png">
                </div>
            </div>
        </div>
        <div class = "body-container">
            <router-view/>
        </div>
        <div class="footer">
            <p>Remember this website's domain name!</p>
            <p>This website is for communication and learning purposes only.</p>
        </div>
    </div>
</template>

<script setup lang="ts">
import { useUserStore } from '@/stores/userStore';
import { onMounted, ref,reactive,watch} from 'vue';
import { useRouter } from 'vue-router';
import { superAxios } from "@/common/superAxios";
import { cookieHelper } from "../common/CookieHelper";
import {getUserInfoAsync} from "@/httpRequest/UserRequest";

const userStore = useUserStore();
const router = useRouter();

const local = ref<string>("");
const LoginUrl = ref<string>("");
const controller=ref<string>("");
const token = ref<string>("");

const menus = ref([
{   
    name:"BLOG",
    path:"/",
},
{   
    name:"CATEGORIZED",
    path:"/category",
},
{   
    name:"MEMBER",
    path:"/member",
}
]);

const activePath = ref();
const userInfo = reactive<any>({
  userName: "",
  headUrl: "",
});

//初始化页面时候获取数据
const init = async () => {
  var result = await getUserInfoAsync();
  if (result != null) {
    userInfo.userName = result.userName;
    userInfo.headUrl = result.headUrl;
  }
};

//退出登录状态
const logout = () => {
  cookieHelper.deleteCookie("token");
  window.location.href = LoginUrl.value;
};

watch(()=>router,(newVal,oldVal)=>{
    activePath.value = newVal.currentRoute.value.meta.activePath;  
},{immediate:true,deep:true});

const fetchCookieToken = async ()=>{
const apiRoot = await superAxios.getValue("ImageUrl");
  const webLoginUrl = await superAxios.getValue("Login", "WebUrl");
  const controllerUrl = await superAxios.getValue("Controller","WebUrl");
  const tokenValue = await superAxios.getValue("token");
  if (apiRoot) {
    local.value = apiRoot;
  }
  if (webLoginUrl) {
    LoginUrl.value = webLoginUrl;
  }
  if (controllerUrl) {
    controller.value = controllerUrl;
  }
  if (tokenValue) {
    token.value = tokenValue;
  }
}

onMounted(async()=>{
  await fetchCookieToken();
  await init();
  userStore.setUserInfo({ ...userInfo }); 
  })
</script>

<style lang="scss" scoped>
.demonstration {
  color: var(--el-text-color-secondary);
}

.carousel{
    width: 1350px;
    margin-top: 20px;
}

.el-carousel__item h3 {
 padding-top: 90px;
  color: #a3c8f2;
  opacity: 0.75;
  line-height: 200px;
  margin: 0;
  text-align: center;
  font-size: 30px;
  font-weight: bold;
  color:white;
}

.el-carousel__item:nth-child(2n) {
  background-color: #6bc0d8;
}

.el-carousel__item:nth-child(2n + 1) {
  background-color: #c2dde6;
}

.footer{
    margin-top: 250px;
    height: 300px;
    width: 100%;
    background-color:  #468af7;
    padding-top: 20px;
    box-shadow: 0 -10px 10px #468af7;
    p{
        padding-top: 5px;
        font-size: 18px;
        font-weight: bold;
        color:rgb(213, 221, 224)
    }
}
</style>