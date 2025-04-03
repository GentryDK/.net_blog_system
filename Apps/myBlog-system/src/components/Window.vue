<template>
  <div class="window" :style="{ width: windowWidth + 'px' }" v-if="show">
    <div class="title">
      <span class="iconfont icon-back" @click="close"></span>
    </div>

    <!-- :style="{ height:bodyHeight+'px'}" -->
    <div class="body">
      <slot></slot>
    </div>

    <template v-if="(buttons && buttons.length > 0) || showCancel">
      <div class="footer">
        <el-button link @click="close" size="small" v-if="showCancel">
          取消
        </el-button>
        <el-button
          v-for="btn in buttons"
          :type="btn.type"
          size="small"
          @click="btn.click"
        >
          {{ btn.text }}
        </el-button>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
const windowWidth = window.innerWidth - 250;
// const bodyHeight = window.innerWidth - 30 - 380;

interface Button {
  type: string;
  text: string;
  click: () => void;
}

const props = defineProps({
  show: {
    type: Boolean,
    default: false,
  },
  showCancel: {
    type: Boolean,
    default: false,
  },
  buttons: {
    type: Array as () => Button[],
    default: () => [],
  },
});

const emit = defineEmits(["close", "closeCallback"]);
const close = () => {
  emit("close");
  emit("closeCallback");
};
</script>

<style lang="scss" scoped>
.window {
  //position: absolute; 是一种定位方式，
  //就是他是浮动在别的元素上方的，不算在盒子模型内，但是他的位置受到父元素的影响
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: calc(100vh - 70px);
  background: white;
  z-index: 50;
  display: flex;
  flex-direction: column;
  .title {
    height: 30px;
    display: flex;
    // 上下居中
    align-items: center;
    padding-left: 10px;
    margin-top: 10px;
    .icon-back {
      font-size: 20px;
      //当用户将鼠标指针悬停在设置了这个样式的元素上时，鼠标指针会变成一个手形
      cursor: pointer;
    }
  }
  .body {
    flex: 1;
    padding: 10px;
    overflow: auto; // 确保内容过多时可以滚动
  }
  .footer {
    border-top: 1px solid #ddd;
    text-align: center;
    line-height: 50px;
  }
}
</style>
