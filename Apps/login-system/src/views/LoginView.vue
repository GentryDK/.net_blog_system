<template>
  <div class="flex min-h-full flex-col justify-center px-6 py-12 lg:px-8">
    <div class="sm:mx-auto sm:w-full sm:max-w-sm">
      <h2 class="mt-10 text-center text-2xl font-bold leading-9 tracking-tight text-gray-900">LOG YOU ACOUNT</h2>
    </div>
  
    <div class="mt-10 sm:mx-auto sm:w-full sm:max-w-sm">
      <form class="space-y-6" action="#" method="POST">
        <div>
          <label for="email" class="block text-sm font-medium leading-6 text-gray-900">Email</label>
          <div class="mt-2">
            <input id="email" name="email" type="email" v-model="email" autocomplete="email" required class="px-3 block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6">
          </div>
        </div>
  
        <div>
          <div class="flex items-center justify-between">
            <label for="password" class="block text-sm font-medium leading-6 text-gray-900">Password</label>
            <div class="text-sm">
              <RouterLink to="/about" class="font-semibold leading-6 text-indigo-600 hover:text-indigo-500">Forgot password?</RouterLink>
            </div>
          </div>
          <div class="mt-2">
            <input id="password" name="password" type="password" v-model="password" autocomplete="current-password" required class="px-3 block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6">
          </div>
        </div>
  
        <div>
          <button class="flex w-full justify-center rounded-md bg-indigo-600 px-3 py-1.5 text-sm font-semibold leading-6 text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600" @click="onLogin($event)">登录</button>
        </div>
      </form>
  
      <p class="mt-10 text-center text-sm text-gray-500">
        Don't have an account?
        <RouterLink to="/regist" class="font-semibold leading-6 text-indigo-600 hover:text-indigo-500">Click to register with email</RouterLink>
      </p>
    </div>
  </div>
  </template>
  
  <script setup lang="ts">
   import { onMounted, ref  } from 'vue';
   import {checkLogin} from '../httpRequest/loginRequest'
   import { cookieHelper } from '../common/CookieHelper'
   import { superAxios } from "@/common/superAxios";
   import { ElMessage } from "element-plus";

   const email = ref('');
   const password = ref('');
   const BlogViewUrl = ref<string>("");

   //event是获取dom事件，这里是button的表单自带一个默认事件，通过preventDefault阻止默认事件的触发
  const onLogin =async(event:any)=>{
    event.preventDefault();
    
    // Email format validation
    //test是正则表达式类型自带的函数，返回bool值
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailPattern.test(email.value)) {
      ElMessage({ message: "Email format error", type: "warning" });
      return;
    }
    try{
      var res = await checkLogin({
      email:email.value,
      password:password.value
    })
    if (res!=null){
      cookieHelper.setCookie("token",res);
      window.location.href = BlogViewUrl.value;
      }
    } catch {
      ElMessage({ message: "Login failed, incorrect account or password", type: "warning" });
    }
  }
  
  const onForgetPassword = async()=>{
  
  }

  onMounted(async()=>{
    const blogUrl = await superAxios.getValue("BlogView", "WebUrl");
    if (blogUrl) {
      BlogViewUrl.value = blogUrl;
    }
  });
  </script>
  
  <style lang="scss" scoped>
  
  </style>