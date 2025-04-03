<template>
  <div class="top-panel">
    <el-form :model="searchFormData" label-width="50px">
      <!-- :span="5" 是指在 <el-col> 元素中设置列的跨度。它是 Element UI 中的一个属性，用来控制列在栅格系统中的宽度 -->
      <el-row>
        <el-col :span="6">
          <el-form-item label="昵称" prop="UserName">
            <el-input
              placeholder="请输入成员昵称"
              v-model="searchFormData.userName"
              clearable
              @keyup.enter.native="loadDataList"
            >
            </el-input>
          </el-form-item>
        </el-col>

        <el-col :span="2">
          <el-button type="danger" @click="loadDataList">搜索</el-button>
        </el-col>
        <el-col :span="2"> </el-col>
      </el-row>
    </el-form>
  </div>

  <!-- 这里table是我们自定义的表单组件，这里的值是在子组件中通过defineProps去定义的-->
  <Table
    :dataSource="tableData"
    :columns="columns"
    :showPagination="true"
    :fetch="loadDataList"
    :options="tableOptions"
  >
    <!-- 封面 -->
    <template #cover="{ row }">
      <div class="cover">
        <Cover :cover="`${row.headUrl}?t=${new Date().getTime()}`"></Cover>
      </div>
    </template>

    <!-- 文章信息 -->
    <template #userInfo="{ index, row }">
      <div>昵称:{{ row.userName }}</div>
      <div>邮箱:{{ row.email }}</div>
    </template>

    <!-- 评论 -->
    <template #role="{ index, row }">
      <div>{{ row.role.roleName }}</div>
    </template>

    <!-- 状态 -->
    <template #state="{ index, row }">
      <span v-if="row.status === 0" style="color: green">启用</span>
      <span v-else style="color: red">封禁</span>
    </template>

    <!-- 时间 -->
    <template #time="{ index, row }">
      <div>创建时间:{{ formatDate(row.creationTime) }}</div>
    </template>

    <template #op="{ row }">
      <div class="op">
        <!-- href="javascript:void(0)">修改</a> 表示点击链接时不会执行任何默认动作或导航到其他页面。主要用来创建纯粹的点击事件，比如在链接中结合 JavaScript 函数来触发一些交互效果，而不是跳转到一个 URL -->
        <a
          href="javascript:void(0)"
          :class="[row.role.id > 2 ? 'not-allow' : '']"
          @click="showEdit(row)"
          >修改</a
        >
        <el-divider direction="vertical" />
        <a
          v-if="row.status === 0"
          href="javascript:void(0)"
          :class="[row.role.id > 2 ? 'not-allow' : '']"
          @click="ban('禁用', row, '1')"
          >禁用</a
        >
        <a
          v-else="row.status == 1"
          href="javascript:void(0)"
          :class="[row.role.id > 2 ? 'not-allow' : '']"
          @click="ban('启用', row, '0')"
          >启用</a
        >
        <el-divider direction="vertical" />
        <a
          href="javascript:void(0)"
          :class="[row.role.id > 2 ? 'not-allow' : 'a-link']"
          @click="del(row)"
          >删除</a
        >
      </div>
    </template>
  </Table>

  <Dialog
    :title="dialogConfig.title"
    :buttons="dialogConfig.buttons"
    :show="dialogConfig.show"
    @close="handleDialogClose"
  >
    <el-form
      :model="userInfo"
      label-width="auto"
      :rules="rules"
      ref="userInfoRef"
    >
      <el-form-item label="名称" prop="userName">
        <el-input placeholder="请输入名称" v-model="userInfo.userName">
        </el-input>
      </el-form-item>
      <el-form-item label="邮箱" prop="email">
        <el-input placeholder="请输入邮箱" v-model="userInfo.email"> </el-input>
      </el-form-item>
      <el-form-item label="密码" prop="password">
        <el-input placeholder="请输入密码" v-model="userInfo.password">
        </el-input>
      </el-form-item>
      <el-form-item label="重复密码" prop="rePassword">
        <el-input placeholder="请再次输入密码" v-model="userInfo.rePassword">
        </el-input>
      </el-form-item>
      <el-form-item label="权限">
        <el-radio-group v-model.number="userInfo.roleId">
          <el-radio :label="2">管理员</el-radio>
          <el-radio :label="1">用户</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item label="头像" prop="cover">
        <CoverUpload
          v-model:modelValue="imageSrc"
          :local="local"
          @upload="collectUploadData"
          ref="coverUpload"
        />
      </el-form-item>
    </el-form>
  </Dialog>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, nextTick } from "vue";
import { formatDate } from "@/utils/DateTime";
import {
  getUsersAsync,
  AdminUpdateUserAsync,
  BanUserAsync,
} from "@/httpRequest/UserRequest";
import { validateEmail, validatePassword } from "@/utils/validators";
import { superAxios } from "@/common/superAxios";
import Confirm from "@/utils/Confirm";
import { ElMessage } from "element-plus";

const local = ref<string>("");
const imageSrc = ref<string | undefined>(undefined);

const searchFormData = reactive({
  userName: "",
});

//新增的用户数据
const userInfo = reactive<any>({
  userId: "",
  userName: "",
  email: "",
  password: "",
  rePassword: "",
  roleId: null,
  headFile: null,
});

//dialog表单的引用,通过这个可以去操作userInfo在form中的数据,比如验证是否正确
//操作表单的数据是userInfo中的数据。
// 这里之所以需要通过userInfoRef进行表单验证，
// 而不是直接验证userInfo，是因为Element UI提供了一些内置的方法和功能来简化表单处理，
// 这些功能依赖于对表单实例的引用
const userInfoRef = ref<any>();

const rules = {
  email: [
    {
      required: false,
      validator: validateEmail,
      message: "邮箱格式不正确",
      trigger: "blur",
    },
  ],
  password: [
    {
      required: false, // 密码非必填
      validator: (rule: any, value: string, callback: any) => {
        if (value && !validatePassword(rule, value, callback)) {
          callback(new Error("密码必须包含字母和数字，且长度至少8位"));
        } else {
          callback(); // 通过验证
        }
      },
      trigger: "blur",
    },
  ],
  rePassword: [
    {
      required: false, // 重复密码非必填
      validator: (rule: any, value: string, callback: any) => {
        // 如果输入了密码，才进行重复密码验证
        if (value && value !== userInfo.password) {
          callback(new Error("密码输入不一致"));
        } else {
          callback();
        }
      },
      trigger: "blur",
    },
  ],
};

const tableData = reactive({
  list: [],
  totalCount: 0,
  pageSize: 15,
  pageIndex: 1,
});

const tableOptions = {
  extHeight: 50,
};

//新增成员弹窗的信息配置
const dialogConfig = reactive({
  title: "修改成员信息",
  show: false,
  buttons: [
    {
      type: "danger",
      text: "确定",
      click: async (e: any) => {
        submitAllData();
      },
    },
  ],
});

//关闭弹窗时候清空
const handleDialogClose = () => {
  userInfo.password = undefined;
  userInfo.rePassword = undefined;
  imageSrc.value = undefined;
  dialogConfig.show = false;
};

// 收集上传的图片数据
const collectUploadData = (data: File) => {
  userInfo.headFile = data;
};

//修改，新增界面
const showEdit = (data: any) => {
  if (data.role.id > 2) return;
  dialogConfig.show = true;
  nextTick(() => {
    // resetFields 是 element-plus自带的参数，可以重置页面
    userInfoRef.value.resetFields();
    if (data != null) {
      userInfo.userId = data.userId;
      userInfo.userName = data.userName;
      userInfo.email = data.email;
      userInfo.roleId = data.role.id;
      imageSrc.value = local.value + data.headUrl;
    } else {
      imageSrc.value = undefined; // 每次打开弹窗时初始化
    }
  });
};

//删除
const del = async (data: any) => {
  if (data.role.id > 2) return;
  // Confirm(`你确认要删除用户:${data.userName}`, async () => {
  //   //TODO 去后端删除
  //   await loadDataList();
  // });

  Confirm("暂未实装此功能", async () => {
    //TODO 去后端删除
    await loadDataList();
  });
};

//封禁
const ban = async (type: string, data: any, status: string) => {
  if (data.role.id > 2) return;
  Confirm(`你确认要${type}用户:${data.userName}`, async () => {
    try {
      var response = await BanUserAsync({
        UserId: data.userId,
        Status: status,
      });
      if (response.status === 200) {
        ElMessage({ message: "用户操作成功", type: "success" });
      }
      await loadDataList();
    } catch (error: any) {
      const errorMessage = error?.response?.data?.Message || "用户操作失败";
      ElMessage({ message: errorMessage, type: "error" });
    }
  });
};

//统一将弹窗的数据提交到后端(新增和修改都是同一个接口,具体操作在后端区别执行)
const submitAllData = async () => {
  userInfoRef.value.validate(async (valid: any) => {
    if (!valid) {
      return;
    }
    try {
      console.log(typeof userInfo.userId, userInfo.userId);
      console.log(typeof userInfo.userName, userInfo.userName);
      console.log(typeof userInfo.email, userInfo.email);
      console.log(typeof userInfo.password, userInfo.password);
      console.log(typeof userInfo.roleId, userInfo.roleId);
      let response = await AdminUpdateUserAsync(
        {
          UserId: userInfo.userId,
          UserName: userInfo.userName,
          Email: userInfo.email,
          Password: userInfo.password,
          RoleId: userInfo.roleId,
        },
        userInfo.headFile
      );
      if (response.status === 200) {
        dialogConfig.show = false;
        ElMessage({ message: "用户信息保存成功", type: "success" });
        window.location.reload();
      }
    } catch (error) {
      ElMessage({ message: "用户信息保存失败", type: "error" });
      console.error("Error:", error);
    }
  });
};

//重新获取页面的数据
const loadDataList = async () => {
  let res = await getUsersAsync({
    pageIndex: tableData.pageIndex,
    pageSize: tableData.pageSize,
    userName: searchFormData.userName,
  });
  tableData.list = res.userInfoDtos;
  tableData.totalCount = res.count;
};

const columns = [
  {
    label: "头像",
    prop: "cover",
    width: 80,
    scopedSlots: "cover",
  },
  {
    label: "用户信息",
    prop: "userInfo",
    scopedSlots: "userInfo",
  },
  {
    label: "角色",
    prop: "role",
    scopedSlots: "role",
  },
  {
    label: "状态",
    prop: "state",
    scopedSlots: "state",
  },
  {
    label: "时间",
    prop: "time",
    width: 200,
    scopedSlots: "time",
  },
  {
    label: "操作",
    prop: "op",
    width: 200,
    scopedSlots: "op",
  },
];

onMounted(async () => {
  local.value = await superAxios.getValue("ImageUrl");
});
</script>

<style lang="scss" scoped></style>
