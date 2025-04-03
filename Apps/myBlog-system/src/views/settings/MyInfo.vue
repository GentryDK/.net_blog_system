<template>
  <div>
    <el-form
      :model="formData"
      :rules="rules"
      ref="formDataRef"
      label-width="100px"
    >
      <!-- gutter设置列表的间距 -->
      <el-row :gutter="5">
        <el-col :span="10">
          <!-- prop是用于表单验证的 -->
          <el-form-item label="头像">
            <CoverUpload
              v-model:modelValue="imageSrc"
              :local="local"
              @upload="collectUploadData"
              ref="coverUpload"
            />
          </el-form-item>
          <el-form-item label="昵称" prop="userName">
            <el-input
              placeholder="请输入昵称"
              v-model="formData.userName"
            ></el-input>
          </el-form-item>

          <el-form-item label="邮箱" prop="email">
            <el-input
              placeholder="请输入邮箱"
              v-model="formData.email"
            ></el-input>
          </el-form-item>

          <el-form-item label="密码" prop="email">
            <a
              href="javascript:void(0)"
              class="a-link"
              @click="showUpdatePassword"
              >修改密码</a
            >
          </el-form-item>

          <el-form-item label="">
            <el-button type="danger" @click="saveUserInfo">保存</el-button>
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="简介" prop="introduction" class="">
            <EditorHtml v-model="formData.introduction"></EditorHtml>
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>

    <Dialog
      :title="dialogConfig.title"
      :buttons="dialogConfig.buttons"
      :show="dialogConfig.show"
      width="600px"
      @close="handleDialogClose"
    >
      <el-form
        :model="passwordFormData"
        :rules="rules"
        ref="passwordFormDataRef"
      >
        <!-- prop是绑定数据属性用的，就是v-model的作用 -->
        <el-form-item prop="password">
          <el-input
            placeholder="请输入密码"
            type="password"
            v-model="passwordFormData.password"
          ></el-input>
        </el-form-item>

        <el-form-item prop="rePassword">
          <el-input
            placeholder="请再次输入密码"
            type="password"
            v-model="passwordFormData.rePassword"
          ></el-input>
        </el-form-item>
      </el-form>
    </Dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted, nextTick } from "vue";
import { superAxios } from "@/common/superAxios";
import EditorHtml from "@/components/EditorHtml.vue";
import {
  updateUserAsync,
  getUserInfoAsync,
  updateUserPasswordAsync,
} from "@/httpRequest/UserRequest";
import {
  validateEmail,
  validatePassword,
  validateRePassword,
} from "@/utils/validators";
import { ElMessage } from "element-plus";

const local = ref<string>("");
const imageSrc = ref<string | undefined>(undefined);

const formDataRef = ref<any>();
const formData = reactive<any>({
  userName: "",
  headUrl: "",
  cover: "",
  email: "",
  introduction: "",
  headImgFile: null,
});

const passwordFormDataRef = ref<any>();
const passwordFormData = ref<any>({
  password: "",
  rePassword: "",
});

const rules = {
  userName: [{ required: true, message: "用户名称不能为空", trigger: "blur" }],
  email: [
    { required: true, message: "用户邮箱不能为空", trigger: "blur" },
    {
      validator: validateEmail,
      message: "邮箱格式不正确",
      trigger: "blur",
    },
  ],
  password: [
    { required: true, message: "请输入密码", trigger: "blur" },
    {
      validator: validatePassword,
      message: "密码不满足要求",
      trigger: "blur",
    },
  ],
  rePassword: [
    { required: true, message: "请输入密码", trigger: "blur" },
    {
      validator: validateRePassword(() => passwordFormData.value.password),
      message: "密码输入不一致",
      trigger: "blur",
    },
  ],
};

const collectUploadData = (data: File) => {
  formData.headImgFile = data;
};

//获取用户信息
const getUserInfo = async () => {
  var result = await getUserInfoAsync();
  if (!result) {
    return;
  }
  imageSrc.value = `${local.value + result.headUrl}?t=${new Date().getTime()}`;
  formData.userName = result.userName;
  formData.email = result.email;
  formData.introduction = result.introduction;
};

//上传
const saveUserInfo = async () => {
  formDataRef.value.validate(async (valid: any, fields?: any) => {
    if (!valid && fields) {
      const firstErrorKey = Object.keys(fields)[0];
      const errorMsg = fields[firstErrorKey]?.[0]?.message || "格式错误";
      ElMessage({ message: errorMsg, type: "warning" });
      return;
    }
    let res = await updateUserAsync({
      userName: formData.userName,
      email: formData.email,
      introduction: formData.introduction,
      headImgFile: formData.headImgFile,
    });
    if (!res) {
      return;
    }
    ElMessage({ message: "用户信息保存成功", type: "success" });
    await getUserInfo();
  });
};

//修改密码弹窗的信息配置
const dialogConfig = reactive({
  title: "修改密码",
  show: false,
  buttons: [
    {
      type: "danger",
      text: "确定",
      click: async (e: any) => {
        await sudmitPass();
      },
    },
  ],
});

//显示修改密码的弹窗
const showUpdatePassword = () => {
  dialogConfig.show = true;
  //在下一次 DOM 更新周期之后执行回调函数。这个函数来自于 Vue.js，确保在对话框显示之后执行某些操作
  nextTick(() => {
    //resetFields重置所有字段
    passwordFormDataRef.value.resetFields();
    passwordFormData.value = {};
  });
};

//关闭修改密码弹窗
const handleDialogClose = () => {
  passwordFormData.value.password = undefined;
  passwordFormData.value.rePassword = undefined;
  dialogConfig.show = false;
};

//提交密码
const sudmitPass = async () => {
  passwordFormDataRef.value.validate(async (valid: any) => {
    if (!valid) {
      return;
    }
    try {
      let response = await updateUserPasswordAsync(
        passwordFormData.value.password
      );
      if (response.status === 200) {
        dialogConfig.show = false;
        ElMessage({ message: "用户信息保存成功", type: "success" });
      }
    } catch (error) {
      console.error("Error:", error);
    }
  });
};

onMounted(async () => {
  local.value = await superAxios.getValue("ImageUrl");
  getUserInfo();
});
</script>

<style lang="scss" scoped></style>
