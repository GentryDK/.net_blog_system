// 验证邮箱格式
export const validateEmail = (rule: any, value: any, callback: any) => {
  const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
  if (!emailRegex.test(value)) {
    return callback(new Error("邮箱格式不正确"));
  }
  return callback();
};

// 验证密码格式（包含字母和数字，长度至少8位）
export const validatePassword = (rule: any, value: any, callback: any) => {
  const passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/;
  if (!passwordRegex.test(value)) {
    return callback(new Error("密码必须包含字母和数字，且长度至少8位"));
  }
  return callback();
};

// 验证重复密码
export const validateRePassword = (getPassword: () => string) => {
  return (rule: any, value: string, callback: any) => {
    const password = getPassword(); // 动态获取原始密码
    if (value !== password) {
      return callback(new Error("密码输入不一致"));
    }
    return callback();
  };
};
