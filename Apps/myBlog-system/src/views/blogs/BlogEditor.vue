<template>
  <div>
    <Window
      :buttons="windowConfig.buttons"
      :show="windowConfig.show"
      :showCancel="true"
      :style="{ width: '100%' }"
      @close="closeWindow"
    >
      <h1>{{ title }}</h1>

      <el-form :model="blogFormData" :rules="rules" ref="blogFormRef">
        <el-form-item prop="PostTitle">
          <div class="title-input">
            <el-input
              placeholder="请输入博客标题"
              v-model="blogFormData.postTitle"
            >
            </el-input>
          </div>
        </el-form-item>

        <el-form-item prop="PostContent">
          <EditorMarkdown
            :height="editorHeight"
            :style="{ width: '100%' }"
            v-model="blogFormData.postContent"
          ></EditorMarkdown>
        </el-form-item>
      </el-form>

      <!-- <EditorHtml  :height="editorHtmlHeight"></EditorHtml> -->

      <!-- 博客基本设置的弹窗 -->
      <Dialog
        :title="dialogConfig.title"
        :buttons="dialogConfig.buttons"
        :show="dialogConfig.show"
        width="600px"
        @close="handleDialogClose"
      >
        <el-form :model="blogFormData" :rules="rules" ref="settingsFormRef">
          <el-form-item label="博客分类" prop="type">
            <el-select
              v-model="selectedPostTypeId"
              clearable
              placeholder="请选择分类"
              @change="handleSelectChange"
            >
              <el-option
                v-for="item in categoryList"
                :key="item.id"
                :value="item.id"
                :label="item.postTypeName"
              >
              </el-option>
            </el-select>
          </el-form-item>

          <el-form-item label="博客封面" prop="cover">
            <CoverUpload
              v-model:modelValue="imageSrc"
              :local="local"
              @upload="collectUploadData"
              ref="coverUpload"
            />
          </el-form-item>

          <el-form-item label="评论设置" prop="Discuss">
            <el-radio-group v-model="blogFormData.discuss">
              <el-radio :value="1">容许</el-radio>
              <el-radio :value="0">不容许</el-radio>
            </el-radio-group>
          </el-form-item>

          <el-form-item label="博客摘要" prop="summary">
            <el-input
              placeholder="请输入摘要"
              v-model="blogFormData.summary"
              type="textarea"
              :autosize="{ minRows: 4, maxRows: 20 }"
            ></el-input>
          </el-form-item>
        </el-form>
      </Dialog>
    </Window>
  </div>
</template>

<script setup lang="ts">
import EditorMarkdown from "@/components/EditorMarkdown.vue";
import { ref, reactive, onMounted, nextTick } from "vue";
import Window from "@/components/Window.vue";
import { superAxios } from "@/common/superAxios";
import { getPostTypesAsync } from "@/httpRequest/PostTypeReuest";
import { addPostAsync, autoSaveAsync } from "@/httpRequest/PostRequest";
import { ElMessage } from "element-plus";
//两种编辑器的高度
const editorHeight = window.innerHeight - 60 - 20 - 30 - 200;

//临时存储上传的图片
const imageSrc = ref<string | undefined>(undefined);
//存储当前的后端地址
const local = ref<string>("");
//存储当前博客的所有分类信息
const categoryList = ref();
//标题
const title = ref<string>();
//用于验证博客输入的规则rules
const blogFormRef = ref<any>(null);
const settingsFormRef = ref<any>(null);

const init = async (type: string, data: any) => {
  //打开计时器，时间到会自动保存
  startTimmer();
  windowConfig.show = true;
  await nextTick();

  if (blogFormRef.value && settingsFormRef.value) {
    //重置两个表单的值
    blogFormRef.value.resetFields();
    settingsFormRef.value.resetFields();
  }

  // //初始化blogFormData
  resetBlogFormData();

  if (type === "add") {
    title.value = "新增帖子";
    dialogConfig.title = "新增帖子";
  } else {
    title.value = "修改帖子";
    dialogConfig.title = "修改帖子";

    Object.assign(blogFormData, data);
    initBaseData(data);
  }
};

let timmer = ref<any>(null);

//开始计时器
const startTimmer = () => {
  // setInterval 是 JavaScript 提供的一个函数，
  // 用于以固定的时间间隔重复执行一个函数或指定的代码片段。
  timmer.value = setInterval(async () => {
    await autoSave();
  }, 10000);
};

//清理计时器
const clearTimer = () => {
  if (timmer.value !== null) {
    clearInterval(timmer.value);
    timmer.value = null;
  }
};

//将上面的init方法暴露出去
defineExpose({ init });

//自动保存
const autoSave = async () => {
  // 没有标题和内容以及计时器未启动或弹窗打开时不保存
  if (
    (!blogFormData.postTitle && !blogFormData.postContent) ||
    !timmer.value ||
    dialogConfig.show
  ) {
    return;
  }

  const response = await autoSaveAsync({
    Id: blogFormData.id,
    PostTitle: blogFormData.postTitle,
    summary: blogFormData.summary,
    PostContent: blogFormData.postContent,
    PostTypeId: blogFormData.postTypeId,
    PostTypeName: blogFormData.postTypeName,
    Discuss: blogFormData.discuss,
    State: blogFormData.state, // 确保传递 State 值
  });

  Object.assign(blogFormData, response);
  initBaseData(response);
};

//把后端拿到的一些基本的数据去初始化
const initBaseData = (data: any) => {
  blogFormData.discuss = data.discuss === "T" ? 1 : 0;
  selectedPostTypeId.value = data.postTypeId;
  imageSrc.value = local.value + data.postCover;
};

//当前页面显示的信息配置
const windowConfig = reactive({
  title: "添加新帖信息",
  show: false,
  buttons: [
    {
      type: "danger",
      text: "确定",
      click: async (e: any) => {
        showSettings();
      },
    },
  ],
});

//上传的弹窗界面，用于博客设置的参数
const blogFormData = reactive<any>({
  id: "",
  postTitle: "",
  summary: "",
  postContent: "",
  postTypeId: "",
  postTypeName: "",
  discuss: "",
  state: 1,
  coverFile: null,
});

//初始化函数，重置所有字段
const resetBlogFormData = () => {
  Object.assign(blogFormData, {
    id: "",
    postTitle: "",
    summary: "",
    postContent: "",
    postTypeId: "",
    PostTypeName: "",
    discuss: "",
    state: 1,
    coverFile: null,
  });
};

// const setHtmlContent = (htmlContent: string) => {
//   //blogFormData.postContent = htmlContent;
// };

//文章标题规则
//在使用表单的验证时候需要严格去匹配下表单属性的名称
//1.在el-form-item中的prop属性也得是rules中的属性
//rules属性必须是el-form中的model绑定的属性，且和ref的相对应
//ref和model的作用是什么？model是我们绑定的对象
//ref则将model的值去映射到ref中，然后我们通过ref去验证映射的值
const rules = {
  postTitle: [{ required: true, message: "请输入标题" }],
  postContent: [{ required: true, message: "请输入正文" }],
  discuss: [{ required: true, message: "请选择是否允许评论" }],
  postTypeId: [{ required: true, message: "请选择文章类型" }],
  summary: [{ required: true, message: "请输入文章简介" }],
};

//显示弹窗的方法,在点击最下方的确认按钮时候触发
const showSettings = async () => {
  const valid = await blogFormRef.value.validate(); // 等待验证结果
  if (!valid) {
    return; // 如果验证不通过，不显示弹窗
  }
  dialogConfig.show = true; // 验证通过后显示弹窗
};

//对话弹窗的信息配置
const dialogConfig = reactive({
  title: "博客信息",
  show: false,
  buttons: [
    {
      type: "danger",
      text: "确定",
      click: async (e: any) => {
        await submitBlog();
      },
    },
  ],
});

const emit = defineEmits(["callback"]);

//关闭当前页面
const closeWindow = async () => {
  windowConfig.show = false;
  emit("callback");

  //清除计时器，这里是用来自动保存的
  clearTimer();
};

//新增变量，用于绑定 el-select
const selectedPostTypeId = ref("");

//设置选单中选中的博客类型和类型名称
const handleSelectChange = (value: string) => {
  console.log("value:" + value);
  const selectedCategory = categoryList.value.find(
    (item: { id: string }) => item.id === value
  );
  if (selectedCategory) {
    console.log("selectedCategory:" + selectedCategory);
    blogFormData.postTypeId = selectedCategory.id;
    blogFormData.postTypeName = selectedCategory.postTypeName;
  }
};

//关闭弹窗
const handleDialogClose = () => {
  imageSrc.value = undefined;
  dialogConfig.show = false;
};

// 收集上传的图片数据
const collectUploadData = (data: File) => {
  blogFormData.coverFile = data;
};

//提交到后端
const submitBlog = async () => {
  const valid = await settingsFormRef.value.validate();
  if (!valid) {
    return;
  }
  try {
    let res = await addPostAsync({
      Id: blogFormData.id,
      PostTitle: blogFormData.postTitle,
      summary: blogFormData.summary,
      PostContent: blogFormData.postContent,
      PostTypeId: blogFormData.postTypeId,
      PostTypeName: blogFormData.postTypeName,
      Discuss: blogFormData.discuss,
      State: blogFormData.state,
      CoverFile: blogFormData.coverFile,
    });

    console.log(res);

    // 从后端响应中提取 Message 字段
    const message = res || "博客发布成功"; // 如果没有返回 Message，则使用默认消息
    ElMessage({ message, type: "success" });
    dialogConfig.show = false;
    closeWindow();
  } catch (error: any) {
    // 在这里也从错误响应中提取 Message 字段
    const errorMessage = error?.response?.data?.Message || "博客发布失败";
    ElMessage({ message: errorMessage, type: "warning" });
  }
};

onMounted(async () => {
  local.value = await superAxios.getValue("ImageUrl");
  await loadCategoryList();
});

//获取所有的博客分类
const loadCategoryList = async () => {
  let result = await getPostTypesAsync({
    pageIndex: 1,
    pageSize: 30,
  });
  categoryList.value = result;
  console.log(result);
};
</script>

<style lang="scss" scoped>
h1 {
  margin-bottom: 10px;
}
.title-input {
  width: 100%;
  border-bottom: 1px solid #ddd;
  .el-input__wrapper {
    box-shadow: none;
  }
}
</style>
