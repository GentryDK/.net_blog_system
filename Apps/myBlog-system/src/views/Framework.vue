<template>
  <div class="common-layout" style="height: 100%">
    <el-container style="height: 100%">
      <el-header>
        <h2
          style="
            margin-top: -5px;
            margin-left: 25px;
            font-size: 20px;
            color: aliceblue;
          "
        >
          博客后台管理系统
        </h2>
        <div v-if="token != null">
          <span class="user-info">欢迎回来.</span>
          <el-dropdown style="color: white">
            <span class="el-dropdown-link">
              {{ userInfo.userName }}
              <el-icon class="el-icon--right">
                <arrow-down />
              </el-icon>
            </span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item>
                  <router-link to="myInfo">个人信息</router-link>
                </el-dropdown-item>
                <el-dropdown-item @click="logout">退出</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
          <div class="avatar">
            <el-avatar :size="50" :src="local + userInfo.headUrl" />
          </div>
        </div>
        <div v-else>用户请先登录！</div>
      </el-header>
      <el-container>
        <el-aside>
          <el-menu
            active-text-color="#ffd04b"
            background-color="#545c64"
            default-active="2"
            text-color="#fff"
            router
          >
            <!-- <div>
              <el-button class="post-btn">发布</el-button>
            </div> -->

            <el-menu v-for="menu in menuList" :key="menu.title">
              <!-- 根据 roleId 控制菜单项显示 -->
              <el-sub-menu
                v-if="menu.roleId <= userInfo.roleId"
                :index="menu.index"
              >
                <template #title>
                  <el-icon><location /></el-icon>
                  <span>{{ menu.title }}</span>
                </template>
                <el-menu-item-group
                  v-for="subMenu in menu.children"
                  :key="subMenu.title"
                >
                  <!-- 根据 roleId 控制子菜单项显示 -->
                  <el-menu-item
                    v-if="subMenu.roleId <= userInfo.roleId"
                    :index="subMenu.index"
                  >
                    <router-link :to="subMenu.path">{{
                      subMenu.title
                    }}</router-link>
                  </el-menu-item>
                </el-menu-item-group>
              </el-sub-menu>
            </el-menu>
          </el-menu>
        </el-aside>
        <el-main>
          <router-view />
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted } from "vue";
import { Location } from "@element-plus/icons-vue";
import { ArrowDown } from "@element-plus/icons-vue";
import { superAxios } from "@/common/superAxios";
import { getUserInfoAsync } from "@/httpRequest/UserRequest";
import { cookieHelper } from "../common/CookieHelper";

const menuList = ref([
  {
    title: "博客管理",
    index: "1",
    roleId: 2,
    children: [
      { title: "首页图片管理", index: "1-1", path: "", roleId: 3 },
      { title: "博客管理", index: "1-2", path: "blogManager", roleId: 2 },
      { title: "分类管理", index: "1-3", path: "BlogTypeManager", roleId: 3 },
      { title: "成员管理", index: "3-3", path: "BlogUser", roleId: 3 },
    ],
  },
  {
    title: "设置",
    index: "3",
    roleId: 1,
    children: [
      { title: "个人信息", index: "3-1", path: "myInfo", roleId: 1 },
      {
        title: "上传敏感词库",
        index: "3-2",
        path: "UploadSensitiveWords",
        roleId: 3,
      },
    ],
  },
  {
    title: "回收站",
    index: "4",
    roleId: 2,
    children: [
      { title: "帖子回收站", index: "4-1", path: "blogRecycleBin", roleId: 2 },
    ],
  },
]);

const userInfo = reactive<any>({
  userName: "",
  headUrl: "",
  roleId: null,
});

const local = ref<string>("");
const LoginUrl = ref<string>("");
const token = ref<string>("");

//初始化页面时候获取数据
const init = async () => {
  var result = await getUserInfoAsync();
  if (result != null) {
    userInfo.userName = result.userName;
    userInfo.headUrl = result.headUrl;
    userInfo.roleId = result.role.id;
  }
};

//退出登录状态
const logout = () => {
  cookieHelper.deleteCookie("token");
  window.location.href = LoginUrl.value;
};

onMounted(async () => {
  await init();

  const apiRoot = await superAxios.getValue("ImageUrl");
  const webLoginUrl = await superAxios.getValue("Login", "WebUrl");
  const tokenValue = await superAxios.getValue("token");
  if (apiRoot) {
    local.value = apiRoot;
  }
  if (webLoginUrl) {
    LoginUrl.value = webLoginUrl;
  }
  if (tokenValue) {
    token.value = tokenValue;
  }
});
</script>

<style lang="scss">
* {
  margin: 0;
  padding: 0;
  box-sizing: 0;
}
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

.avatar {
  margin-left: 10px;
  margin-bottom: 7px;
}

nav {
  padding: 30px;

  a {
    font-weight: bold;
    color: #2c3e50;

    &.router-link-exact-active {
      color: #42b983;
    }
  }
}
</style>

<style lang="scss" scoped>
.el-header {
  color: white;
  text-align: left;
  font-size: 20px;
  line-height: 60px;
  background-color: #545c64;
  //下边框
  border-bottom: 1px solid #dad5d5;
  display: flex;
  align-items: center;
  justify-content: space-between;
  div {
    display: flex; // 水平布局
    align-items: center; //确保垂直居中对齐
    .user-info {
      color: white;
      display: block;
      margin-right: 2px;
      margin-top: -5px;
    }
    el-dropdown {
      margin-left: 0;
      display: inline-block;
    }
  }
}
.el-container {
  padding-top: 10px;
  background: #545c64;
  height: calc(100vh - 60px);
  display: flex;
  .el-aside {
    background-color: #545c64;
    font-size: 20px;
    padding: 0px 15px;
    .post-btn {
      background: rgb(223, 228, 223);
      color: #fff;
      height: 40px;
      width: 100%;
    }
  }
  .el-main {
    position: relative;
    background: #ffffff;
    margin-top: -10px;
  }
}
</style>
