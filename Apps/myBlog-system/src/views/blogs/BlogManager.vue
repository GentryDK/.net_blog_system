<template>
  <div class="top-panel">
    <el-form :model="searchFormData" label-width="50px">
      <!-- :span="5" 是指在 <el-col> 元素中设置列的跨度。它是 Element UI 中的一个属性，用来控制列在栅格系统中的宽度 -->
      <el-row>
        <el-col :span="5">
          <el-form-item label="标题" prop="userName">
            <el-input
              placeholder="请输入名称"
              v-model="searchFormData.PostTitle"
              clearable
              @keyup.enter.native="loadDataList"
            >
            </el-input>
          </el-form-item>
        </el-col>

        <el-col :span="5">
          <el-form-item label="状态" prop="status">
            <el-select
              v-model="searchFormData.status"
              clearable
              placeholder="请选择状态"
              :style="{ width: '100%' }"
            >
              <el-option :value="0" label="草稿"></el-option>
              <el-option :value="1" label="已发布"></el-option>
            </el-select>
          </el-form-item>
        </el-col>

        <!-- 这里el-select和el-option的关系和用法 -->
        <!-- elselect中选择的选项就是el-option -->
        <!-- el-option中label是显示出来的字段，value是具体选择后传递的值，传递给el-select中v-model的值 -->
        <el-col :span="5">
          <el-form-item label="分类" prop="categoryId">
            <el-select
              v-model="searchFormData.PostTypeId"
              clearable
              placeholder="请选择分类"
              :style="{ width: '100%' }"
            >
              <el-option
                :value="item.id"
                :label="item.postTypeName"
                v-for="item in categoryList"
              ></el-option>
            </el-select>
          </el-form-item>
        </el-col>

        <el-col :span="2">
          <el-button type="danger" @click="loadDataList">搜索</el-button>
        </el-col>
        <el-col :span="2">
          <el-button type="danger" @click="showEdit('add', null)"
            >创建新帖子</el-button
          >
        </el-col>
      </el-row>
    </el-form>
  </div>

  <!-- fetch是切换页码时候执行的函数 -->
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
        <Cover :cover="`${row.postCover}?t=${new Date().getTime()}`"></Cover>
      </div>
    </template>

    <!-- 文章信息 -->
    <template #blogInfo="{ index, row }">
      <div>标题:{{ row.postTitle }}</div>
      <div>分类:{{ row.postTypeName }}</div>
      <div>作者:{{ row.createUserName }}</div>
    </template>

    <!-- 评论 -->
    <template #discuss="{ index, row }">
      <div>{{ row.discuss == "T" ? "开启" : "关闭" }}</div>
    </template>

    <!-- 状态 -->
    <template #state="{ index, row }">
      <span v-if="row.state == 0" style="color: green">已发布</span>
      <span v-else style="color: red">草稿</span>
    </template>

    <!-- 时间 -->
    <template #time="{ index, row }">
      <div>创建时间:{{ formatDate(row.createDate) }}</div>
      <div>编辑时间:{{ formatDate(row.editDate) }}</div>
    </template>

    <template #op="{ row }">
      <div class="op">
        <!-- href="javascript:void(0)">修改</a> 表示点击链接时不会执行任何默认动作或导航到其他页面。主要用来创建纯粹的点击事件，比如在链接中结合 JavaScript 函数来触发一些交互效果，而不是跳转到一个 URL -->
        <a href="javascript:void(0)" @click="showEdit('update', row)">修改</a>
        <el-divider direction="vertical" />
        <a href="javascript:void(0)" @click="showReplyManager(row.id)">管理</a>
        <el-divider direction="vertical" />
        <a href="javascript:void(0)" @click="del(row)">删除</a>
        <el-divider direction="vertical" />
        <a href="javascript:void(0)" class="a-link" @click="showDetail(row)"
          >预览</a
        >
      </div>
    </template>
  </Table>
  <!-- ref 属性：用于给组件分配一个引用名称，以便在父组件中访问和操作该组件实例。 -->
  <BlogEditor ref="blogEditRef" @callback="loadDataList"></BlogEditor>
  <BlogDetail ref="blogDetailRef"></BlogDetail>
  <ReplyManager ref="replyManagerRef"></ReplyManager>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";
import { getPostTypesAsync } from "@/httpRequest/PostTypeReuest";
import { getPostsAsync, removePostAsync } from "@/httpRequest/PostRequest";
import { formatDate } from "@/utils/DateTime";
import Confirm from "@/utils/Confirm";
import BlogEditor from "./BlogEditor.vue";
import BlogDetail from "./BlogDetail.vue";
import ReplyManager from "./ReplyManager.vue";
import { ElMessage } from "element-plus";

const categoryList = ref();

//分类
const loadCategoryList = async () => {
  let result = await getPostTypesAsync({
    pageIndex: 1,
    pageSize: 30,
  });
  categoryList.value = result;
};

//列表(拥有scopedSlots这个选择的会单独成一行)
const columns = [
  {
    label: "封面",
    prop: "cover",
    width: 80,
    scopedSlots: "cover",
  },
  {
    label: "文章信息",
    prop: "blogInfo",
    scopedSlots: "blogInfo",
  },
  {
    label: "评论",
    prop: "discuss",
    scopedSlots: "discuss",
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

const tableData = reactive({
  list: [],
  totalCount: 0,
  pageSize: 15,
  pageIndex: 1,
});

const tableOptions = {
  extHeight: 50,
};

//删除
const del = async (data: any) => {
  Confirm(`你确认要删除${data.postTitle}`, async () => {
    console.log("id:" + data.id);
    await removePostAsync(data.id as string);
    // 删除完毕后重新请求数据

    ElMessage({ message: "删除成功", type: "success" });
    await loadDataList();
  });
};

//搜索
const searchFormData = reactive<any>({
  PostTitle: "",
  State: "",
  PostTypeId: "",
});

//加载列表
const loadDataList = async () => {
  let res = await getPostsAsync({
    pageIndex: tableData.pageIndex,
    pageSize: tableData.pageSize,
    PostTitle: searchFormData.PostTitle,
    State: searchFormData.State,
    PostTypeId: searchFormData.PostTypeId,
  });
  tableData.list = res.postListDto;
  tableData.totalCount = res.postCount;
};

// 这里的blogEditRef是使用BlogEditor用的，就是说通过下面的blogEditRef可以去访问BlogEditor中的方法
const blogEditRef = ref();
//修改，新增界面
const showEdit = (type: any, data: any) => {
  blogEditRef.value.init(type, data);
};

//详情页
const blogDetailRef = ref();
//调用BlogDetail中的方法
const showDetail = (postInfo: any) => {
  blogDetailRef.value.showDetail(postInfo);
};

const replyManagerRef = ref();
const showReplyManager = (postId: string) => {
  console.log("id:" + postId);
  if (replyManagerRef.value) {
    replyManagerRef.value.showReplyManager(postId);
  }
};

onMounted(async () => {
  await loadDataList();
  await loadCategoryList();
});
</script>

<style lang="scss" scoped></style>
