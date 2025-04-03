<template>
  <div class="container">
    <div class="form-container">
      <RouterLink to="/" class="backLogin font-semibold leading-6 text-indigo-600 hover:text-indigo-500">BACK</RouterLink>
      <div style="width:70%;margin:100px auto;">
        <el-form
          ref="ruleFormRef"
          style="max-width: 600px"
          :model="registData"
          status-icon
          :rules="rules"
          label-width="auto"
          class="demo-ruleForm"
        >
          <div style="margin-bottom: 20px;">
            <h2 class="mt-10 text-center text-2xl font-bold leading-9 tracking-tight text-gray-900">Welcome to register for my blog</h2>
          </div>
          <el-form-item label="User Name" prop="userName">
            <el-input v-model="registData.userName" autocomplete="off" />
          </el-form-item>
          <el-form-item label="Email" prop="email">
            <el-input v-model="registData.email" autocomplete="off" />
          </el-form-item>
          <el-form-item class="emailCodeForm" label="Verification code" prop="emailCode">
            <el-input v-model="registData.emailCode" autocomplete="off" />
              <el-button
                 class="flex justify-center font-semibold text-white shadow-sm"
                 type="primary"
                :disabled="isSending"
                :class="{'bg-gray-400': isSending, 'bg-indigo-600': !isSending}"
                @click="submitEmail">
                {{ isSending ? `${countdown}s later to resend` : 'Send' }}
              </el-button>
          </el-form-item>
          <el-form-item label="Password" prop="pwd1">
            <el-input v-model="registData.pwd1" type="password" autocomplete="off" />
          </el-form-item>
          <el-form-item label="Confirm password" prop="pwd2">
            <el-input v-model="registData.pwd2" type="password" autocomplete="off" />
          </el-form-item>
          <el-form-item>
            <el-button class="flex w-full justify-center bg-indigo-600 font-semibold text-white shadow-sm" type="primary" @click="submitForm(ruleFormRef)">
              Submit
            </el-button>
          </el-form-item>
        </el-form>
      </div>
    </div>
    <div class="image-container">
      <img src="/images/RegristBg.jpg" alt="巨大的图片" class="rounded-lg border-4 border-blue-500"/>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';
import { useRouter } from 'vue-router';
import type { FormInstance, FormRules } from 'element-plus';
import { registUser,sendEmail} from '@/httpRequest/registRequest';

const isSending = ref(false);
const countdown = ref(60);
let timer: number | null = null;

const ruleFormRef = ref<FormInstance>();
const router = useRouter();
const registData = reactive({
  userName: "",
  email: "",
  emailCode: "",
  pwd1: "",
  pwd2: ""
});

const validateEmail = (rule: any, value: any, callback: any) => {
  const reg = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
  if (!reg.test(value)) {
    callback(new Error("Please enter a correct email address"));
  } else {
    callback();
  }
};


const validateEmailCode = (rule: any, value: any, callback: any) => {
  const reg = /^\d{4}$/;
  const res = reg.test(value);
  if (!res) {
    callback(new Error("Please enter the correct email verification code"));
  } else {
    callback();
  }
};

const validatePassword = (rule: any, value: any, callback: any) => {
  const res = value == registData.pwd1;
  if (!res) {
    callback(new Error("The two passwords do not match"));
  } else {
    callback();
  }
};

const rules = reactive<FormRules<typeof registData>>({
  userName: [{ required: true, message: "Please enter a username", trigger: "blur" }],
  email: [
    { required: true, message: "Please enter a email", trigger: "blur" },
    { validator: validateEmail, trigger: 'blur' }
  ],
  emailCode: [
    { required: true, message: "Please enter the email verification code", trigger: "blur" },
    {
      validator: validateEmailCode,
      trigger: "blur"
    }
  ],
  pwd1: [
    { required: true, message: "Please enter a password", trigger: "blur" },
  ],
  pwd2: [
    { required: true, message: "Please enter a password", trigger: "blur" },
    {
      validator: validatePassword,
      trigger: 'blur'
    }
  ]
});

const submitEmail = async () => {
  if (!ruleFormRef.value) return;
  
  // 只验证 email 字段
  await ruleFormRef.value.validateField("email").then(async () => {

    isSending.value = true;
    countdown.value = 60;

    let result = await sendEmail({
      userName: registData.userName,
      email: registData.email
    });

    timer = setInterval(() => {
      countdown.value--;
      if (countdown.value <= 0) {
        clearInterval(timer as number);
        isSending.value = false;
      }
    }, 1000);
  }).catch(() => {
    console.log("Incorrect email format, unable to send verification code");
  });
};


const submitForm = (formEl: FormInstance | undefined) => {
  if (!formEl) return;
  formEl.validate(async (valid) => {
    if (valid) {
      var res = await registUser({
        userName: registData.userName,
        password: registData.pwd1,
        email: registData.email,
        emailCode: String(registData.emailCode)
      });
      if (res == true) {
        alert("Registration successful! Automatically redirecting to the login page");
        setTimeout(() => {
          router.push('/');
        }, 1000); 
      }
    } else {
      console.log('error submit!');
    }
  });
};
</script>

<style lang="scss" scoped>
.container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
}

.form-container {
  flex: 1;
  padding: 20px;
  margin: auto;
  float: none; /* 确保没有浮动 */
  position: static; /* 确保没有绝对定位 */
  margin-left: 100px;
}

.image-container {
  flex: 1;
  display: flex;
  justify-content: center;
  align-items: center;
}

.image-container img {
  max-width: 100%;
  height: auto;
  border-radius: 15px; // 设置圆角半径为15px
  border: 2px solid rgb(4, 4, 117); // 添加蓝色边框
  margin-left: 250px;
}

.backLogin {
  margin-left: 85%;
}

.emailCodeForm {
  display: flex;
  align-items: center;
  .el-input{
    margin-right: 5px;
  }
}

.emailCodeForm .el-input {
  flex: 0 0 80%;
}

.sendBtn {
  margin-left: 20px;
}
</style>