<template>
  <div>
    <!-- <el-config-privider :locale="locale">

    </el-config-privider> -->

          <router-view/>
  </div>
</template>

<script setup lang="ts">
  // import zhCn from 'element-plus/lib/locale/lang/zh-cn';
  // const locale = zhCn;
</script>

<style lang="scss">
*{
  margin: 0;
  padding: 0;
  box-sizing: 0;
}
</style>
