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

        <el-col :span="2">
          <el-button type="danger" @click="loadDataList">搜索</el-button>
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

    <!-- 时间 -->
    <template #time="{ index, row }">
      <div>创建时间:{{ formatDate(row.createDate) }}</div>
      <div>编辑时间:{{ formatDate(row.editDate) }}</div>
    </template>

    <template #op="{ row }">
      <div class="op">
        <!-- href="javascript:void(0)">修改</a> 表示点击链接时不会执行任何默认动作或导航到其他页面。主要用来创建纯粹的点击事件，比如在链接中结合 JavaScript 函数来触发一些交互效果，而不是跳转到一个 URL -->
        <a href="javascript:void(0)" @click="recover(row)">恢复</a>
        <el-divider direction="vertical" />
        <a href="javascript:void(0)" :class="'a-link'" @click="del(row)"
          >删除</a
        >
      </div>
    </template>
  </Table>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";
import {
  getRecycleBinPostAsync,
  PermanentlyDeletePostAsync,
  recoverDeletedPostAsync,
  recoverDeletedPost1Async,
} from "@/httpRequest/RecyclePostRequest";
import { formatDate } from "@/utils/DateTime";
import Confirm from "@/utils/Confirm";
import { ElMessage } from "element-plus";

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

//恢复
const recover = async (data: any) => {
  var response = await recoverDeletedPostAsync(data.id as string);

  Confirm(`你确认要恢复:${data.postTitle}`, async () => {
    try {
      var response = await recoverDeletedPost1Async(data.id as string);
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

//删除
const del = async (data: any) => {
  Confirm(`你确认要彻底删除${data.postTitle}`, async () => {
    try {
      var response = await PermanentlyDeletePostAsync(data.id as string);
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

//搜索
const searchFormData = reactive<any>({
  PostTitle: "",
});

//加载列表
const loadDataList = async () => {
  let res = await getRecycleBinPostAsync({
    pageIndex: tableData.pageIndex,
    pageSize: tableData.pageSize,
    postTitle: searchFormData.PostTitle,
  });
  tableData.list = res.postListDto;
  tableData.totalCount = res.postCount;
};

onMounted(async () => {
  await loadDataList();
});
</script>

<style lang="scss" scoped></style>
